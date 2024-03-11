using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Tango.Localization;

namespace Tango.AspNetCore
{
	public class LangRouteConstraint : IRouteConstraint
	{
		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			var candidate = values[routeKey]?.ToString();
			return Language.Options.SupportedLanguages.Any(x => x.Code == candidate);
		}
	}
}
