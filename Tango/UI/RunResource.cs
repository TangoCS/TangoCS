using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tango.Logger;

namespace Tango.UI
{
	public static class RunResource
	{
		public static async Task Ajax<T>(ActionContext ctx) where T : ViewRootElement, new() => await Run(ctx, RunAjax<T>, OnAjaxError);
		public static async Task Page<T>(ActionContext ctx) where T : ViewRootElement, new() => await Run(ctx, RunPage<T>, OnError);
		public static async Task RunXml(ActionContext ctx) => await Run(ctx, c => c.RunAction(), OnErrorXml);

		public static async Task Run(ActionContext ctx, Func<ActionContext, ActionResult> run, Func<Exception, ActionResult> onError)
		{
			ActionResult r = null;
			try
			{
				r = ctx.RunResourceFilter() ?? run(ctx);
			}
			catch (Exception e)
			{
				if (ctx.RequestServices.GetService(typeof(IErrorLogger)) is IErrorLogger errLogger)
					errLogger.Log(e);

				r = onError(e);
			}

			await r.ExecuteResultAsync(ctx);
		}

		static ActionResult OnError(Exception e)
		{
			return new HtmlResult(e.ToString().Replace(Environment.NewLine, "<br/>"), "");
		}

		public static XDocument ErrorMessage(int code, string text)
		{
			return new XDocument(new XElement("error", new XElement("errorcode", code), new XElement("errortext", text)));
		}

		public static ActionResult OnErrorXml(Exception e)
		{
			var text = e.ToString().Replace(Environment.NewLine, "<br/>");
			return new ContentResult { Content = ErrorMessage(-1, text).ToString(), ContentType = "text/xml" };
		}

		static ActionResult OnAjaxError(Exception e)
		{
			var api = new ApiResult();
			api.ApiResponse.Data.Add("error", e.ToString().Replace(Environment.NewLine, "<br/>"));
			return api;
		}

		static ActionResult RunPage<T>(ActionContext ctx)
			where T : ViewRootElement, new()
		{
			var p = new T() { Context = ctx }.InjectProperties(ctx.RequestServices);
			return p.Execute();
		}

		static ActionResult RunAjax<T>(ActionContext ctx)
			where T : ViewRootElement, new()
		{
			var cache = ctx.RequestServices.GetService(typeof(ITypeActivatorCache)) as ITypeActivatorCache;
			var key = typeof(T).Name.ToLower();
			(var type, var invoker) = cache.Get(key) ?? (null, null);
			return invoker?.Invoke(ctx, type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
		}
	}
}
