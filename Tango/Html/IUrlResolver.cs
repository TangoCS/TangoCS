using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Html
{
	public interface IUrlResolver
	{
		UrlResolverResult Resolve(IReadOnlyDictionary<string, string> parameters, DynamicDictionary globalParameters);
	}

	public struct UrlResolverResult
	{
		public StringBuilder Result;
		public bool Resolved;
	}

	public class RouteUrlResolver : IUrlResolver
	{
		protected string _routeTemplate;

		public RouteUrlResolver(string routeTemplate)
		{
			_routeTemplate = routeTemplate;
		}

		public virtual UrlResolverResult Resolve(IReadOnlyDictionary<string, string> parameters, DynamicDictionary globalParameters)
		{
			return new UrlResolverResult { Resolved = true, Result = RouteUtils.Resolve(_routeTemplate, parameters, globalParameters) };
		}
	}
}
