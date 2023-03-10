using System;
using Tango.Html;
using Tango.Logger;
using Tango.Logger.Std;

namespace Tango.UI.Std
{
	public class ContainerFilter : IActionInvokingFilter, IBeforeActionFilter
	{
		ViewContainer containerObj = null;
		bool selfContainerView = false;

		public void OnActionInvoking(ActionInvokingFilterContext context)
		{
			var ctx = context.ActionContext;
			var element = context.Element;

			if (ctx.AddContainer || !ctx.ContainerPrefix.IsEmpty())
			{
				if (!ctx.ContainerType.IsEmpty())
				{
					var t = new ContainersCache().Get(ctx.ContainerType);
					if (t != null)
						containerObj = Activator.CreateInstance(t) as ViewContainer;
				}
				else if (element is IContainerItem)
				{
					containerObj = (element as IContainerItem).GetContainer();
					selfContainerView = true;
				}
			}
		}

		public void BeforeAction(ActionFilterContext context)
		{
			if (!(context.Result is ApiResult result)) return;

			var ctx = context.ActionContext;
			var element = context.Element;
			var response = result.ApiResponse;

			if (element is IViewElement view)
			{
				if (containerObj != null)
				{
					containerObj.Context = ctx;
					containerObj.ID = selfContainerView ? view.ClientID : ctx.ContainerPrefix;
					containerObj.InjectProperties(ctx.RequestServices);
					containerObj.OnInit();
					containerObj.ProcessResponse(response, ctx.AddContainer, ctx.ContainerPrefix);
					if (containerObj.IsModal)
						view.IsModal = true;
				}
				//else if (context.Method.Name != "OnLoad")
				response.WithWritersFor(view);

			}
		}
	}

	public class FirstLoadFilter : IBeforeActionFilter
	{
		public void BeforeAction(ActionFilterContext context)
		{
			if (!(context.Result is ApiResult result)) return;

			if (!(context.Element is ViewPagePart view)) return;

			if (context.ActionContext.IsFirstLoad)
				view.OnFirstLoad(result.ApiResponse);
		}
	}

	public class LogFilter : IAfterActionFilter, IResourceFilter
	{
		public void AfterAction(ActionFilterContext context)
		{
			if (!(context.Result is ApiResult result)) return;
			if (context.ActionContext.GetArg(Constants.ShowLogsName) != "1") return;

			var response = result.ApiResponse;
			response.WithRootNames(() => {
				response.AddAdjacentWidget("container", "log", AdjacentHTMLPosition.BeforeEnd, w => {
					w.Pre(a => a.ID("log"), () => {
						var loggerProvider = context.ActionContext.GetService<IRequestLoggerProvider>();
						foreach (var l in loggerProvider.GetLoggers())
						{
							w.Write(l.Key);
							w.Write("\r\n");
							w.Write(l.Value.ToString());
						}
					});
				});
			});
		}

		public void OnResourceExecuting(ResourceExecutingContext context)
		{
			if (context.ActionContext.GetArg(Constants.ShowLogsName) != "1") return;

			var lp = context.ActionContext.GetService<IRequestLoggerProvider>();
			var list = context.ActionContext.GetService<RequestLoggerList>();
			if (list != null)
				foreach (var l in list)
					lp?.RegisterLogger<RequestLogger>(l);
		}
	}
}
