using System;
using System.Collections.Generic;
using System.IO;
using Nephrite.Html;
using Nephrite.Multilanguage;

namespace Nephrite.UI
{
	public class LayoutWriter : StringWriter, IHtmlWriter
	{
		public string IDPrefix { get; set; }

		public ITextResource TextResource { get; private set; }
		public ActionContext Context { get; private set; }

		public List<ClientAction> ClientActions { get; set; }
		public HashSet<string> Includes { get; set; }

		public LayoutWriter(ActionContext context, ITextResource textResource)
        {
			TextResource = textResource;
			Context = context;
			ClientActions = new List<ClientAction>();
			Includes = new HashSet<string>();
		}

		public void Write(object v)
		{
			throw new NotImplementedException();
		}
	}

	public static class LayoutWriterMainExtensions
	{
		public static void AjaxForm(this LayoutWriter w, string name, Action content)
		{
			w.AjaxForm(name, null, content);
		}

		public static void AjaxForm(this LayoutWriter w, string name, Action<FormTagAttributes> attributes, Action content)
		{
			w.Form(a => a.ID(name).Set(attributes), content);
			w.AddClientAction("ajaxUtils", "initForm", new { ID = w.GetID(name) });
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
