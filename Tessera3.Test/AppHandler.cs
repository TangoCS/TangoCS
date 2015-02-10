using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Routing;
using Nephrite;
using Nephrite.Html;
using RazorEngine;
using RazorEngine.Templating;
//using Tessera3.Views;

namespace Solution
{
	public class RouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			var r = requestContext.RouteData;

			return null;
		}

		//public void ProcessRequest(HttpContext context)
		//{
		//	Stopwatch sw = new Stopwatch();
		//	sw.Start();

		//	var r = context.Response;

		//	var routes = context.Request.RequestContext.RouteData;

		//	using (var appContext = new DefaultAppContext())
		//	{
		//		var templatePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views"), "HomePage.cshtml");
		//		var templateKey = "homepage" + File.GetLastWriteTime(templatePath).ToString();

		//		if (Engine.Razor.IsTemplateCached(templateKey, typeof(IAppContext)))
		//			r.Write(Engine.Razor.Run(templateKey, typeof(IAppContext), appContext));
		//		else
		//		{
		//			r.Write(Engine.Razor.RunCompile(File.ReadAllText(templatePath), templateKey, typeof(IAppContext), appContext));
		//		}
		//	}

		//	sw.Stop();
		//	r.Write(sw.Elapsed);
		//	r.End();
		//}
	}
}
