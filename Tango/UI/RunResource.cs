﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tango.UI
{
	public static class RunResource
	{
		public static async Task Ajax<T>(ActionContext ctx) where T : ViewRootElement, new() => await Run(ctx, RunAjax<T>, OnAjaxError);
		public static async Task Page<T>(ActionContext ctx) where T : ViewRootElement, new() => await Run(ctx, RunPage<T>, OnError);
		public static async Task RunXml(ActionContext ctx) => await Run(ctx, c => c.RunAction(), OnErrorXml);

		public static async Task Run(ActionContext ctx, Func<ActionContext, ActionResult> run, Func<ActionContext, Exception, ActionResult> onError)
		{
			ActionResult r = null;
			try
			{
				r = ctx.RunResourceFilter() ?? run(ctx);
			}
			catch (Exception e)
            {
				r = onError(ctx, e);
			}

			await r.ExecuteResultAsync(ctx);
		}

		static ActionResult OnError(ActionContext ctx, Exception e)
		{
			var res = ctx.RequestServices.GetService(typeof(IErrorResult)) as IErrorResult;
			var message = res?.OnError(e) ?? e.ToString().Replace(Environment.NewLine, "<br/>");
			return new HtmlResult(message, "");
		}

		public static XDocument ErrorMessage(int code, string text)
		{
			return new XDocument(new XElement("error", new XElement("errorcode", code), new XElement("errortext", text)));
		}

		public static ActionResult OnErrorXml(ActionContext ctx, Exception e)
		{
			var res = ctx.RequestServices.GetService(typeof(IErrorResultXml)) as IErrorResultXml;
			var message = res?.OnError(e) ?? e.ToString().Replace(Environment.NewLine, "<br/>");

			return new ContentResult { Content = ErrorMessage(-1, message).ToString(), ContentType = "text/xml", StatusCode = HttpStatusCode.InternalServerError };
		}

		static ActionResult OnAjaxError(ActionContext ctx, Exception e)
		{
			var res = ctx.RequestServices.GetService(typeof(IErrorResult)) as IErrorResult;
			var message = res?.OnError(e) ?? e.ToString().Replace(Environment.NewLine, "<br/>");

			var api = new ApiResult();
			api.ApiResponse.Data.Add("error", message);
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
			return ctx.RunAction(key: "_page." + typeof(T).Name.ToLower());
		}
	}
}
