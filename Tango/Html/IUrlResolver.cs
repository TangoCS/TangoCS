using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Html
{
	public interface IUrlResolver
	{
		StringBuilder Resolve(IReadOnlyDictionary<string, string> parameters);
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

		public virtual StringBuilder Resolve(IReadOnlyDictionary<string, string> parameters)
		{
			return RouteUtils.Resolve(_routeTemplate, parameters, false, _notMatchedParmsBeginWith);
		}
	}
}
