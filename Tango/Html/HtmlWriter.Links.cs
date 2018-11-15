using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Html
{
	public static class HtmlWriterLinksExtensions
	{
		public static void A(this HtmlWriter w, string linkTitle, string url, string image = null)
		{
			w.A(a => a.Href(url), () => {
				if (!image.IsEmpty()) w.Img(a => a.Src(IconSet.RootPath + image).Alt(linkTitle).Class("linkicon"));
				w.Write(linkTitle);
			});
		}
	}
}
