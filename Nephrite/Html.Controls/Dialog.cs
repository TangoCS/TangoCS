﻿using System;
using Nephrite.MVC;
using Newtonsoft.Json;

namespace Nephrite.Html.Controls
{
	public class DialogOptions
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string ParentName { get; set; }
	}

	public static class DialogExtensions
	{
		public static ActionLink ActionOpenDialogLink(this HtmlHelper html, DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			return new OpenDialogLink().Link(options, title, customATagAttributes);
		}

		public static void Dialog(this HtmlWriter w, string id, string title, Action body, Action footer)
		{
			w.Div(a => a.ID(id).Class("modal-dialog").Role("dialog").Style("display: none"), () => 
			{
				w.Div(a => a.Class("modal-header"), () =>
				{
					w.Button(null, a => a.Class("close").Aria("label", "Close").OnClick("dialog.hide(this)"), () => {
						w.Span(a => a.Aria("hidden", "true"), "x");
					});
					w.H3(a => a.Class("modal-title"), title);
				});
				w.Div(a => a.Class("modal-body"), body);
				w.Div(a => a.Class("modal-footer"), footer);
			});
		}

		public static void Dialog_Footer_OKCancelButtons(this HtmlWriter w)
		{
			w.Button(null, "ОК", a => a.Class("ms-ButtonHeightWidth").OnClick("dialog.submit(this)"));
			w.Write("&nbsp;");
			w.Button(null, "Отмена", a => a.Class("ms-ButtonHeightWidth").OnClick("dialog.hide(this)"));
		}
    }

	public class OpenDialogLink : ActionLink
	{
		Action<ATagAttributes> _aTagAttributes = null;

		//public OpenDialogLink(IUrlHelper urlHelper, IAccessControl accessControl) : base(urlHelper, accessControl) { }

		public OpenDialogLink Link(DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		{
			if (!title.IsEmpty()) _title = title;

			_aTagAttributes = a => {
				options.Url = Url;
				a.Href("#");
				a.OnClick("dialog.show(this, " + JsonConvert.SerializeObject(options, Json.CamelCase) + ")");
				if (customATagAttributes != null) customATagAttributes(a);
			};
			return this;
		}

		public override string ToString()
		{
			HtmlWriter w = new HtmlWriter();
			w.A(_aTagAttributes, _title);
			return w.ToString();
		}
	}
}