using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Html
{
	public interface IUrlResolver
	{
		UrlResolverResult Resolve(string template, IReadOnlyDictionary<string, string> parameters, DynamicDictionary globalParameters);
	}

	public struct UrlResolverResult
	{
		public StringBuilder Result;
		public bool Resolved;
	}

	public class RouteUrlResolver : IUrlResolver
	{
		public virtual UrlResolverResult Resolve(string template, IReadOnlyDictionary<string, string> parameters, DynamicDictionary globalParameters)
		{
			return new UrlResolverResult { 
				Resolved = true, 
				Result = RouteUtils.Resolve(template, parameters, globalParameters) 
			};
		}
	}
}
