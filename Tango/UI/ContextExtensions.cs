using System;
using System.Linq;
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
			return actionUrl.UseResolver(actionUrl.Context.CreateDefaultUrlResolver());
		}

		public static ActionLink ToCurrent(this ActionLink a)
		{
			a = a.RunAction(a.Context.Service, a.Context.Action).UseDefaultResolver()
				.WithArgs(a.Context.AllArgs.Where(arg => !a.Context.FormData.ContainsKey(arg.Key)));
			foreach(var r in a.Context.ReturnUrl)
				a.WithArg(Constants.ReturnUrl + (r.Key == 1 ? "" : $"_{r.Key}"), r.Value);
			return a;
		}

		public static ActionLink ToReturnUrl(this ActionLink a, int code)
		{
			var target = a.Context.ReturnTarget.Get(code);
			if (target == null) return a;
			return a.RunAction(target.Service, target.Action).UseDefaultResolver().WithArgs(target.Args);
		}

		public static ActionLink BaseUrl(this ActionContext context)
		{
			return new ActionLink(context).ToCurrent();
		}

		public static ActionLink CallbackToCurrent(this ActionContext context, Action<ApiResponse> serverEvent)
		{
			return new ActionLink(context).RunAction(context.Service, context.Action).UseDefaultResolver()
						.WithArgs(context.AllArgs).WithArg(Constants.ContainerNew, "0")
						.WithArg(Constants.EventName, serverEvent.Method.Name);
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
