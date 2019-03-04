using System;
using System.Net;
using System.Threading.Tasks;
using Tango.Logger;

namespace Tango.UI
{
	public static class RunResource
	{
		public static async Task Ajax(ActionContext ctx) => await Run(ctx, RunAjax, OnAjaxError);
		public static async Task Page<T>(ActionContext ctx) where T : ViewRootElement, new() => await Run(ctx, RunPage<T>, OnError);

		public static async Task Run(ActionContext ctx, Func<ActionContext, ActionResult> run, Func<ActionContext, Exception, ActionResult> onError)
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

				r = onError(ctx, e);
			}

			await r.ExecuteResultAsync(ctx);
		}

		static ActionResult OnError(ActionContext ctx, Exception e)
		{
			return new HtmlResult(e.ToString().Replace(Environment.NewLine, "<br/>"), "");
		}

		static ActionResult OnAjaxError(ActionContext ctx, Exception e)
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

		static ActionResult RunAjax(ActionContext ctx)
		{
			var cache = ctx.RequestServices.GetService(typeof(ITypeActivatorCache)) as ITypeActivatorCache;
			var key = ctx.RootReceiver.IsEmpty() ? ctx.Service + "." + ctx.Action : ctx.RootReceiver;
			(var type, var invoker) = cache.Get(key) ?? (null, null);
			return invoker?.Invoke(ctx, type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
		}
	}
}
