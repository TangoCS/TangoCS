using System;
using Nephrite.Html.Layout;
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
		//public static ActionLink ActionOpenDialogLink(this IHtmlWriter w, DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
		//{
		//	return new OpenDialogLink().Link(options, title, customATagAttributes);
		//}
		public static ATagAttributes OpenDialogOnClick(this ATagAttributes a, string serverEvent, string id, string callBack = null)
		{
			var open = callBack.IsEmpty() ?
				string.Format("dialog.open(this, '{0}', '{1}')", serverEvent, id) :
				string.Format("dialog.open(this, '{0}', '{1}', {2})", serverEvent, id, callBack);
			return a.OnClick(open);
		}


		public static void Dialog(this LayoutWriter w, string id, string title, Action body, Action footer)
		{
			w.Div(a => a.ID("dialog").Class("modal-dialog").Role("dialog").Style("display: none"), () => {
				w.Form(a => a.ID("form"), () => {
					w.Div(a => a.Class("modal-header"), () => {
						w.Button(a => a.Class("close").Aria("label", "Close").OnClick($"dialog.hide('{id}')"), () => {
							w.Span(a => a.Aria("hidden", "true"), "x");
						});
						w.H3(a => a.ID("title").Class("modal-title"), title);
					});
					w.Div(a => a.ID("body").Class("modal-body"), body);
					w.Div(a => a.ID("footer").Class("modal-footer"), footer);
				});
			});
		}

		//public static void Dialog_Footer_OKCancelButtons(this IHtmlWriter w)
		//{
		//	w.Button(a => a.Class("ms-ButtonHeightWidth").OnClick("dialog.submit(this)"), "OK");
		//	w.Write("&nbsp;");
		//	w.Button(a => a.Class("ms-ButtonHeightWidth").OnClick("dialog.hide(this)"), "Отмена");
		//}
    }

	//public class OpenDialogLink : ActionLink
	//{
	//	Action<ATagAttributes> _aTagAttributes = null;

	//	public OpenDialogLink Link(DialogOptions options, string title = null, Action<ATagAttributes> customATagAttributes = null)
	//	{
	//		if (!title.IsEmpty()) _title = title;

	//		_aTagAttributes = a => {
	//			options.Url = Url;
	//			a.Href("#");
	//			a.OnClick("dialog.show(this, " + JsonConvert.SerializeObject(options, Json.CamelCase) + ")");
	//			if (customATagAttributes != null) customATagAttributes(a);
	//		};
	//		return this;
	//	}

	//	public override string ToString()
	//	{
	//		HtmlWriter w = new HtmlWriter();
	//		w.A(_aTagAttributes, _title);
	//		return w.ToString();
	//	}
	//}
}
