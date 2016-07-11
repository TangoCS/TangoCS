using System;
using System.Collections.Generic;
using System.IO;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI
{
	public class LayoutWriter : HtmlWriter
	{	
		public ActionContext Context { get; private set; }
		public ITextResource TextResource => Context.TextResource;

		public List<ClientAction> ClientActions { get; set; }
		public HashSet<string> Includes { get; set; }

		public LayoutWriter(ActionContext context)
        {
			Context = context;
			ClientActions = new List<ClientAction>();
			Includes = new HashSet<string>();
		}
	}

	public static class LayoutWriterMainExtensions
	{
		public static void AjaxForm(this LayoutWriter w, string name, Action content)
		{
			w.AjaxForm(name, false, null, content);
		}

        public static void AjaxForm(this LayoutWriter w, string name, bool submitOnEnter, Action content)
        {
            w.AjaxForm(name, submitOnEnter, null, content);
        }

        public static void AjaxForm(this LayoutWriter w, string name, Action<FormTagAttributes> attributes, Action content)
        {
            w.AjaxForm(name, false, attributes, content);
        }

        public static void AjaxForm(this LayoutWriter w, string name, bool submitOnEnter, Action<FormTagAttributes> attributes, Action content)
		{
			w.Form(a => a.ID(name).Set(attributes), () => {
                content();
                // Workaround to avoid corrupted XHR2 request body in IE10 / IE11
                w.Hidden("__dontcare", null);
            });
			w.AddClientAction("ajaxUtils", "initForm", new { ID = w.GetID(name), SubmitOnEnter = submitOnEnter });
		}

		public static void ListTable(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Table(a => a.Class("ms-listviewtable").Set(attributes), content);
		}

		public static void ListHeader(this LayoutWriter w, Action<TagAttributes> attributes, Action columns)
		{
			w.Tr(a => a.Class("ms-viewheadertr").Set(attributes), columns);
		}

		public static void ColumnHeader(this LayoutWriter w, Action<ThTagAttributes> attributes, Action content)
		{
			w.Th(a => a.Class("ms-vh2").Set(attributes),
				() => w.Div(a => a.Class("ms-vb"), content)
			);
		}

		public static void ListRow(this LayoutWriter w, Action<TagAttributes> attributes, Action cells)
		{
			w.Tr(attributes, cells);
		}

		public static void Cell(this LayoutWriter w, Action<TdTagAttributes> attributes, Action content)
		{
			w.Td(a => a.Class("ms-vb2").Set(attributes), content);
		}

		public static void FormTable(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Table(a => a.Class("ms-formtable").Set(attributes), content);
		}

		public static void GroupTitle(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("tabletitle").Set(attributes), content);
		}

		public static void FormMargin(this LayoutWriter w, Action inner)
		{
			w.Div(a => a.Style("padding:8px"), inner);
		}

		public static void ButtonsBar(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Table(a => a.Class("ms-formtoolbar").Set(attributes), content);
		}

		public static void ButtonsBarWhiteSpace(this LayoutWriter w)
		{
			w.Td(a => a.Style("width:100%"));
		}

		public static void ButtonsBarItem(this LayoutWriter w, Action content)
		{
			w.Td(a => a.Style("vertical-align:middle"), content);
		}
	}	
}
