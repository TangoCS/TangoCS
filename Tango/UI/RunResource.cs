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
		public static async Task RunXml(ActionContext ctx) => await Run(ctx, c => c.RunAction(), (e, r, id) => OnErrorXml(e));

		public static async Task Run(ActionContext ctx, Func<ActionContext, ActionResult> run, Func<Exception, IErrorResult, int, ActionResult> onError)
		{
			ActionResult r = null;
			try
			{
				r = ctx.RunResourceFilter() ?? run(ctx);
			}
			catch (Exception e)
            {
                int errorId = 0;
				if (ctx.RequestServices.GetService(typeof(IErrorLogger)) is IErrorLogger errLogger)
                    errorId = errLogger.Log(e);

                if (ctx.RequestServices.GetService(typeof(IErrorResult)) is IErrorResult result)
					r = onError(e, result, errorId);
			}

			await r.ExecuteResultAsync(ctx);
		}

		static ActionResult OnError(Exception e, IErrorResult result, int errorId)
		{
			return new HtmlResult(result.OnError(e, errorId), "");
		}

		public static XDocument ErrorMessage(int code, string text)
		{
			return new XDocument(new XElement("error", new XElement("errorcode", code), new XElement("errortext", text)));
		}

		public static ActionResult OnErrorXml(Exception e)
		{
			var text = e.ToString().Replace(Environment.NewLine, "<br/>");
			return new ContentResult { Content = ErrorMessage(-1, text).ToString(), ContentType = "text/xml", StatusCode = HttpStatusCode.InternalServerError };
		}

		static ActionResult OnAjaxError(Exception e, IErrorResult result, int errorId)
		{
			var api = new ApiResult();
			api.ApiResponse.Data.Add("error", result.OnError(e, errorId));
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
