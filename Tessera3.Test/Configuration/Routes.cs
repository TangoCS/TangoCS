using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Nephrite.Meta.Forms;

namespace Solution.Configuration
{
	public class Routes
	{
		public const string DefaultMasterPage = "InternalPage";

		public static void Register(RouteCollection routes)
		{
			using (routes.GetWriteLock())
			{
				routes.Clear();
			}

			var hRazor = new RazorRouteHandler();
			var hApi = new ApiRouteHandler();
			var hWeb = new WebFormsRouteHandler();

			routes.Ignore("{file}.txt");
			routes.Ignore("{file}.htm");
			routes.Ignore("{file}.html");
			routes.Ignore("{file}.js");
			routes.Ignore("{file}.css");

			routes.Add(new Route("api/{mode}/{action}", hApi));
			routes.Add(new Route("web/{mode}/{action}", hWeb));
			routes.Add(new Route("web", hWeb));
			routes.Add(new Route("{mode}/{action}", hRazor));
			routes.Add(new Route("{mode}/{action}/{oid}", hRazor));
			routes.Add(new Route("{mode}/{oid}", hRazor));
			routes.Add(new Route("", hRazor));
			
			//routes.MapPageRoute("{mode}/{oid}", DefaultMasterPage, x =>
			//{
			//	x.SetRenderer("content", () => WebFormRenderer.RenderOperation());
			//	x.NumericMatch("oid");
			//});
			//routes.MapPageRoute("{mode}/{action}", DefaultMasterPage, x => x.SetRenderer("content", () => WebFormRenderer.RenderOperation()));
			//routes.MapPageRoute("{mode}/{action}/{oid}", DefaultMasterPage, x =>
			//{
			//	x.SetRenderer("content", () => WebFormRenderer.RenderOperation());
			//	x.NumericMatch("oid");
			//});
			//routes.MapPageRoute("{mode}/{oid}", DefaultMasterPage, x =>
			//{
			//	x.SetRenderer("content", () => WebFormRenderer.RenderOperation());
			//	x.GuidMatch("oid");
			//});
			//routes.MapPageRoute("{mode}/{action}/{oid}", DefaultMasterPage, x =>
			//{
			//	x.SetRenderer("content", () => WebFormRenderer.RenderOperation());
			//	x.GuidMatch("oid");
			//});
		}
	}

}