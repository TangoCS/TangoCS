using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Html
{
	public interface IUrlResolver
	{
		UrlResolverResult Resolve(IReadOnlyDictionary<string, string> parameters);
	}

	public struct UrlResolverResult
	{
		public StringBuilder Result;
		public bool Resolved;
	}

	public class RouteUrlResolver : IUrlResolver
	{
		protected string _routeTemplate = "";
		protected string _notMatchedParmsBeginWith = "?";

		public RouteUrlResolver(string routeTemplate)
		{
			_routeTemplate = routeTemplate;
		}

		public RouteUrlResolver(string routeTemplate, string notMatchedParmsBeginWith) : this(routeTemplate)
		{
			_notMatchedParmsBeginWith = notMatchedParmsBeginWith;
		}

		public virtual UrlResolverResult Resolve(IReadOnlyDictionary<string, string> parameters)
		{
			return new UrlResolverResult { Resolved = true, Result = RouteUtils.Resolve(_routeTemplate, parameters, false, _notMatchedParmsBeginWith) };
		}
	}
}
