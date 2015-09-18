using System;
using Nephrite.Layout;
using Nephrite.MVC;

namespace Nephrite.Html.Controls
{
	public static class ButtonsExtension
	{
		public static string BackButton(this HtmlHelper c, string title = "", string url = null)
		{
			if (url.IsEmpty()) url = c.Query.ReturnUrl;
			if (title.IsEmpty()) title = c.TextResource.Get("Common.Back", "Назад");
			return String.Format("<button class='btn' onClick={0}>{1}</button>", 
				String.Format("document.location='{0}';return false;", url).InQuot(),
				title);
		}

		public static string SubmitButton(this HtmlHelper c, string title = "")
		{
			if (title.IsEmpty()) title = c.TextResource.Get("Common.OK", "OK");
			return String.Format("<button class='btn' type='submit'>{0}</button>", title);
		}
	}
}