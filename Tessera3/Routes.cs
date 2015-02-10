using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Nephrite.Meta.Forms;

namespace Solution
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

			var h = new RouteHandler();
			
			routes.Add(new Route("{mode}/{action}", h));
			routes.Add(new Route("{mode}/{action}/{oid}", h));
			routes.Add(new Route("{mode}/{oid}", h));
			routes.Add(new Route("", h));

			routes.MapPageRoute("{mode}/{oid}", DefaultMasterPage, x =>
			{
				x.SetRenderer("content", () => WebFormRenderer.RenderOperation());
				x.NumericMatch("oid");
			});
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