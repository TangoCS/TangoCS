using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nephrite.MVC;

namespace Nephrite.Web
{
	public class UrlHelper : IUrlHelper
	{
		public string GeneratePathAndQueryFromRoute(IDictionary<string, object> values)
		{
			return GeneratePathAndQueryFromRoute(null, values);
		}

		public string GeneratePathAndQueryFromRoute(string routeName, IDictionary<string, object> values)
		{
			if (routeName != null)
			{
				var route = RouteTable.Routes[routeName];
				if (route == null) return null;
				return CreateUrl((route as Route).Url, values);	
			}
			else
			{
				var d = new RouteValueDictionary(values);
				var r = HttpContext.Current.Request.RequestContext;
				VirtualPathData virtualPathData = RouteTable.Routes.GetVirtualPath(r, d);
				if (virtualPathData == null) return null;
				return CreateUrl((virtualPathData.Route as Route).Url, values);
			}					
		}

		public string CreateUrl(string url, IDictionary<string, object> parms)
		{
			string[] parts = url.Split(new char[] { '/' });
			StringBuilder res = new StringBuilder();

			foreach (string part in parts)
			{
				if (part.StartsWith("{"))
				{
					string key = part.Substring(1, part.Length - 2);
					if (parms.ContainsKey(key))
					{
						res.Append("/").Append(parms[key]);
						parms.Remove(key);
					}
				}
				else
					res.Append("/").Append(part);
			}

			if (parms.Count > 0) res.Append("?");
			bool first = true;
			foreach (var parm in parms)
			{
				if (!first) res.Append("&");
				res.Append(parm.Key);
				if (parm.Value != null) res.Append("=").Append(parm.Value);
				first = false;
			}

			return res.ToString();
		}

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