using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.DependencyInjection;
using Nephrite.Multilanguage;
using Nephrite.Layout;

namespace Nephrite.Html.Controls
{
	public static class BackButtonExtension
	{
		public static void BackButton(this HtmlWriter c, string title = null, string url = null)
		{
			var textResource = DI.RequestServices.GetService<ITextResource>();
			//if (url.IsEmpty()) url = Query.GetReturnUrl();
			if (title.IsEmpty()) title = textResource.Get("Common.Buttons.Back", "Назад");
			c.Button(null, title, (a) => {
				a.OnClick = String.Format("document.location='{0}';return false;", url);
				a.Class = AppLayout.Current.Button.CssClass;
			});
		}
	}
}