using System;
using Tango.Html;

namespace Tango.UI
{
	public static class ButtonsExtensions
	{
		public static void SubmitButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.SubmitButton(a => a.Class("btn btn-primary").Set(attributes), () => w.Write(text));
		}

		public static void SubmitButtonConfirm(this HtmlWriter w, Action<ButtonTagAttributes> attributes, string text, string message)
		{
			w.SubmitButton(a => a.Class("btn btn-primary").Data("confirm", message).Set(attributes), () => w.Write(text));
		}

		public static void SubmitButtonConfirm(this HtmlWriter w, string text, string message)
		{
			w.SubmitButtonConfirm(null, text, message);
		}

		public static void Button(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "OK")
		{
			w.Button(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

		public static void ResetButton(this HtmlWriter w, Action<ButtonTagAttributes> attributes = null, string text = "Reset")
		{
			w.ResetButton(a => a.Class("btn").Set(attributes), () => w.Write(text));
		}

		//public static void SubmitButton(this LayoutWriter w)
		//{
		//	w.SubmitButton(a => a.DataResult(1).OnClick("return ajaxUtils.processResult(this)"), w.Resources.Get("Common.OK"));
		//}

		public static void SubmitDeleteButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null)
		{
			w.SubmitButton(a => a.Class("btn btn-primary").Set(attrs), () => {
				w.Icon("delete");
				w.Write("&nbsp;");
				w.Write(w.Resources.Get("Common.Delete"));
			});
		}

		public static void SubmitContinueButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null)
		{
			w.SubmitButton(attrs, w.Resources.Get("Common.Continue"));
		}
	}
}
