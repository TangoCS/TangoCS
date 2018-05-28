using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI
{
	public class LayoutWriter : HtmlWriter
	{	
		public ActionContext Context { get; }
		public IResourceManager Resources => Context.Resources;

		public List<ClientAction> ClientActions { get; private set; } = new List<ClientAction>();
		public HashSet<string> Includes { get; private set; } = new HashSet<string>();

		public LayoutWriter(ActionContext context, StringBuilder sb) : base(sb) 
		{
			Context = context;
		}
		
		public LayoutWriter(ActionContext context, string idPrefix) : base(idPrefix) 
		{
			Context = context;
		}
		
		public LayoutWriter(ActionContext context, string idPrefix, StringBuilder sb) : base(idPrefix, sb) 
		{
			Context = context;
		}

		public LayoutWriter(ActionContext context)
        {
			Context = context;
		}

		public LayoutWriter Clone(string newIdPrefix)
		{
			return new LayoutWriter(Context, newIdPrefix, GetStringBuilder()) {
				ClientActions = ClientActions,
				Includes = Includes
			};
		}
	}

	public static class LayoutWriterMainExtensions
	{
		public static LayoutWriter Clone(this LayoutWriter w, IViewElement el)
		{
			return w.Clone(el.ClientID);
		}

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

		public static void FormTable(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Table(a => a.Class("formtable").Set(attributes), content);
		}

		public static void FormTable100Percent(this LayoutWriter w, Action content)
		{
			w.FormTable(a => a.Class("width100"), content);
		}

		public static void FieldsBlockStd(this LayoutWriter w, Action content)
		{
			w.FieldsBlockStd(null, content);
		}

		public static void FieldsBlock100Percent(this LayoutWriter w, Action content)
		{
			w.FieldsBlock100Percent(null, content);
		}

		public static void FieldsBlockStd(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.FormTable(a => a.ID().Class("widthstd").Set(attributes), content);
		}

		public static void FieldsBlock100Percent(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.FormTable(a => a.ID().Class("width100").Set(attributes), content);
		}



		public static void GroupTitle(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("tabletitle").Set(attributes), content);
		}

		public static void FormMargin(this LayoutWriter w, Action inner)
		{
			w.Div(a => a.Style("padding:8px"), inner);
		}

		public static void ButtonsBar(this LayoutWriter w, Action content)
		{
			w.ButtonsBar(null, content);
		}

		public static void ButtonsBar(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.ID("buttonsbar").Class("buttonsbar").Set(attributes), content);
		}

		public static void ButtonsBarRight(this LayoutWriter w, Action content)
		{
			w.Div(a => a.Class("right"), content);
		}

        public static void ButtonsBarLeft(this LayoutWriter w, Action content)
        {
            w.Div(a => a.Class("left"), content);
        }
    }
}
