using System;
using Nephrite.Html.Layout;

namespace Nephrite.Html.Controls
{
	public static class ButtonsExtension
	{
		public static void BackButton(this LayoutWriter c, string title = "", string url = null)
		{
			if (url.IsEmpty()) url = c.Context.ActionArgs.ReturnUrl;
			if (title.IsEmpty()) title = c.TextResource.Get("Common.Back", "Назад");
			c.Write(String.Format("<button class='btn' type='button' onClick={0}>{1}</button>", 
				String.Format("document.location='{0}';return false;", url).InQuot(),
				title));
		}

		public static void SubmitButton(this LayoutWriter c)
		{
			c.SubmitButton(a => a.Class("btn"), c.TextResource.Get("Common.OK", "OK"));
		}
	}
}