using System;
using System.Collections.Generic;
using System.Net;
using Tango.Html;

namespace Tango.UI
{
	public static class ContextExtensions
	{
		public static IUrlResolver CreateDefaultUrlResolver(this ActionContext context)
		{
			return new RouteUrlResolver(context.Routes["default"]);
		}

		public static ActionLink UseDefaultResolver(this ActionLink actionUrl)
		{
			return actionUrl.UseResolver(new RouteUrlResolver(actionUrl.Context.Routes["default"]));
		}

		public static ActionLink ToCurrent(this ActionLink a)
		{
			a = a.To(a.Context.Service, a.Context.Action).UseDefaultResolver()
				.WithArgs(a.Context.AllArgs);
			foreach(var r in a.Context.ReturnUrl)
				a.WithArg(Constants.ReturnUrl + (r.Key == 1 ? "" : $"_{r.Key}"), r.Value);
			return a;
		}

		public static ActionLink ToReturnUrl(this ActionLink a, int code)
		{
			var target = a.Context.ReturnTarget[code];
			return a.To(target.Service, target.Action).UseDefaultResolver().WithArgs(target.Args);
		}

		public static ActionLink BaseUrl(this ActionContext context)
		{
			return new ActionLink(context).ToCurrent();
		}

		public static string CreateReturnUrl(this ActionContext context, Action<ActionLink> linkAttrs)
		{
			var l = new ActionLink(context).UseDefaultResolver();
			linkAttrs?.Invoke(l);
			return l.Url;
		}

		public static string CreateReturnUrl(this ActionContext context, int code, IDictionary<string, object> args = null)
		{
			var	baseUrl = context.BaseUrl();
			if (args != null) baseUrl.WithArgs(args);

			var returnurl = baseUrl.Url;
			if (returnurl.Length > 1800)
			{
				Stack<string> urlStack = new Stack<string>();
				AbstractQueryString url = new Url(context.ReturnUrl[code]);

				while (url.GetString(Constants.ReturnUrl) != "")
				{
					urlStack.Push(url.RemoveParameter(Constants.ReturnUrl));
					url = new Url(WebUtility.UrlDecode(url.GetString(Constants.ReturnUrl)));
				}
				url = new Url(urlStack.Pop());
				while (urlStack.Count > 0)
					url = new Url(urlStack.Pop()).AddParameter(Constants.ReturnUrl, WebUtility.UrlEncode(url));
				returnurl = url;
			}
			return returnurl;
		}
	}
}
