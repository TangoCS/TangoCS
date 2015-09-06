using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.DependencyInjection;
using Nephrite.Multilanguage;
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
			return String.Format("<button class={0} onClick={1}>{2}</button>", 
				AppLayout.Current.Button.CssClass.InQuot(),
				String.Format("document.location='{0}';return false;", url).InQuot(),
				title);
		}

		public static string SubmitButton(this HtmlHelper c, string title = "OK")
		{
			if (title.IsEmpty()) title = c.TextResource.Get("Common.OK");
			return String.Format("<button class={0} type='submit'>{1}</button>",
				AppLayout.Current.Button.CssClass.InQuot(),
				title);
		}
	}
}