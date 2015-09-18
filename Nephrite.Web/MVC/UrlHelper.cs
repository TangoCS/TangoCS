using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nephrite.MVC;

namespace Nephrite.Web
{
	public class UrlHelper
	{
		public static AbstractQueryString Current()
		{
			var request = HttpContext.Current.Request;
			return new Url(
				request.Url.PathAndQuery,
				request.RequestContext.RouteData.Values);
		}
	}

	public static class AbstractQueryStringExtensions
	{
		public static void Go(this AbstractQueryString url)
		{
			HttpContext.Current.Response.Redirect(url);
		}

		public static void RedirectBack(this AbstractQueryString url)
		{
			HttpContext.Current.Response.Redirect(url.ReturnUrl);
		}

		public static void Redirect(this AbstractQueryString url)
		{
			HttpContext.Current.Response.Redirect(url);
		}
	}
}