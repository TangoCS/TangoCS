using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Html
{
	public interface IUrlResolver
	{
		StringBuilder Resolve(IReadOnlyDictionary<string, string> parameters, bool isHashPart = false);
	}

	public class RouteUrlResolver : IUrlResolver
	{
		protected string _routeTemplate = "";

		public RouteUrlResolver(string routeTemplate)
		{
			_routeTemplate = routeTemplate;
		}

		public virtual StringBuilder Resolve(IReadOnlyDictionary<string, string> parameters, bool isHashPart = false)
		{
			return RouteUtils.Resolve(_routeTemplate, parameters, notMatchedParmsBeginWith: isHashPart ? "/" : "?");
		}
	}
}
