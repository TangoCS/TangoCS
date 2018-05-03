using System;
using Tango.Html;

namespace Tango.UI
{
	public static class ButtonsExtension
	{
		public static void BackButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null, string title = "", string url = null)
		{
			if (url.IsEmpty()) url = w.Context.GetArg("returnurl");
			if (title.IsEmpty()) title = w.Resources.Get("Common.Back");
			w.Button(a => a.Class("btn-default").OnClick($"window.location.href='{url}';return false;").Set(attrs), title);			
		}

		public static void SubmitButton(this LayoutWriter w)
		{
			w.SubmitButton(null, w.Resources.Get("Common.OK"));
		}
	}
}