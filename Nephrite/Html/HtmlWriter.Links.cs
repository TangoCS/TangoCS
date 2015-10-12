using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Html
{
	public static class HtmlWriterLinksExtensions
	{
		public static void A(this IHtmlWriter w, string linkTitle, string url, string image = null)
		{
			w.A(a => a.Href(url), () => {
				if (!image.IsEmpty()) w.Img(a => a.Src(IconSet.RootPath + image).Alt(linkTitle).Class("linkicon"));
				w.Write(linkTitle);
			});
		}
	}
}
