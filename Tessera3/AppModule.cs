using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using Nephrite.Web.Html;
using Tessera3.Views;

namespace Tessera3Sample
{
	public class AppModule : IHttpModule
	{
		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PostAuthenticateRequest += (o, e) =>
			{
				var r = context.Response;
				HtmlPage page = new HtmlPage();

				HomePage p = new HomePage();
				p.Page = page;
				p.Render();

				r.Write(p.ToString());
				r.End();
			};
		}
	}
}
