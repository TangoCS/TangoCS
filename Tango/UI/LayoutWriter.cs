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

		LayoutWriter(ActionContext context, StringBuilder sb) : base(sb) 
		{
			Context = context;
		}
		
		public LayoutWriter(ActionContext context, string idPrefix) : base(idPrefix) 
		{
			Context = context;
		}

		LayoutWriter(ActionContext context, string idPrefix, StringBuilder sb) : base(idPrefix, sb) 
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

	public class FieldsBlockCollapsibleOptions
	{
		public Action<TagAttributes> Attributes { get; set; }
		/// <summary>
		/// Признак свернутого состояния
		/// </summary>
		public bool IsCollapsed { get; set; }
		/// <summary>
		/// Позволяет вставлять таблицу
		/// </summary>
		public bool IncludeTable { get; set; }
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
				content?.Invoke();
				// Workaround to avoid corrupted XHR2 request body in IE10 / IE11
				w.Hidden(Constants.IEFormFix, null);
			});
			w.AddClientAction("ajaxUtils", "initForm", f => new { ID = f(name), SubmitOnEnter = submitOnEnter });
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

		public static void FieldsBlock(this LayoutWriter w, Action content)
		{
			w.FormTable(a => a.ID(), content);
		}

		public static void FieldsBlock(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.FormTable(a => a.ID().Set(attributes), content);
		}

		public static void FieldsBlockStd(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("widthstd"), () => w.FormTable(a => a.ID().Set(attributes), content));
		}

		public static void FieldsBlock100Percent(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("width100"), () => w.FormTable(a => a.ID().Set(attributes), content));
		}

		public static void FieldsBlockCollapsible(this LayoutWriter w, string title, Action content, FieldsBlockCollapsibleOptions collapsibleOptions = null)
		{
			var id = Guid.NewGuid().ToString();
			var js = "domActions.toggleClass({id: '" + w.GetID(id) + "', clsName: 'collapsed' })";

			w.Div(a =>
			{
				a.ID(id).Class("fieldsblock");
				if (collapsibleOptions != null && collapsibleOptions.IsCollapsed)
					a.Class("collapsed");
			}, () => {
				w.Div(a => a.Class("fieldsblock-header").OnClick(js), () => {
					w.Div(a => a.Class("fieldsblock-btn"), () => w.Icon("right"));
					w.Div(a => a.Class("fieldsblock-title"), title);
				});
				if(collapsibleOptions != null && collapsibleOptions.IncludeTable)
					w.Div(a => a.Set(collapsibleOptions.Attributes), content);
				else
				{
					w.Div(() => w.FormTable(a =>
					{
						a.ID();
						if (collapsibleOptions != null)
							a.Set(collapsibleOptions.Attributes);
					}, content));
				}
			});
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

		public static void ButtonsBarRight(this LayoutWriter w, Action<TagAttributes> attributes, Action content)
		{
			w.Div(a => a.Class("right").Set(attributes), content);
		}

		public static void ButtonsBarLeft(this LayoutWriter w, Action content)
        {
            w.Div(a => a.Class("left"), content);
        }

		public static void Icon(this HtmlWriter w, string name, string tip = null, string color = null)
		{
			w.I(a => {
				a.Icon(name).Title(tip);
				if (color != null)
					a.Style("color:" + color);
			});
		}

		public static T Icon<T>(this TagAttributes<T> a, string name)
			where T : TagAttributes<T>
		{
			return a.Class("icon icon-" + name?.ToLower());
		}
		public static void IconFlag<T>(this TagAttributes<T> a, string name, bool issquare = false)
			where T : TagAttributes<T>
		{
			if (!issquare)
				 a.Class("flag-icon flag-icon-" + name?.ToLower());
			else
				a.Class("flag-icon flag-icon-" + name?.ToLower() + " flag-icon-squared");
		}
	}
}
