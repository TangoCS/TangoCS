using System;
using Tango.Html;

namespace Tango.UI.Controls
{
	public static class ButtonsExtension
	{
		public static void BackButton(this LayoutWriter c, Action<ButtonTagAttributes> attrs = null, string title = "", string url = null)
		{
			if (url.IsEmpty()) url = c.Context.GetArg("returnurl");
			if (title.IsEmpty()) title = c.Resources.Get("Common.Back");
			//c.Button(a => a.OnClick($"document.location='{url}';return false;").Set(attrs), title);
			c.Button(a => a.Class("btn-default").OnClick($"window.location.href='{url}';return false;").Set(attrs), title);
			
		}

		public static void SubmitButton(this LayoutWriter c)
		{
			c.SubmitButton(null, c.Resources.Get("Common.OK"));
		}
	}
}