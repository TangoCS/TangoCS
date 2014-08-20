using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using Nephrite.Web.Html;

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
				r.Clear();

				HtmlControl h = new HtmlControl();
				h.DocType();
				h.Html(() =>
				{
					
					h.Head();
					h.Body(() => h.Write("Test"));
				});

				r.Write(h.ToString());
				r.End();
			};
		}
	}
}
