using System;
using System.Collections.Generic;
using System.Net;

namespace Tango.UI.Std
{
	public class StdTypeActivatorCache : DefaultTypeActivatorCache
	{
		public StdTypeActivatorCache()
		{
			typeInfos.Add(new InvokeableTypeInfo
			{
				Filter = t => t.IsSubclassOf(typeof(ViewPage)) && !t.IsAbstract,
				Keys = t => new List<string> { t.Name },
				Invoker = new CsPageInvoker()
			});
		}
	}

	public class CsPageInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext ctx, Type t)
		{
			var page = Activator.CreateInstance(t) as ViewPage;
			page.Context = ctx;
			page.InjectProperties(ctx.RequestServices);
			page.OnInit();

			ActionResult result = null;

			if (ctx.Service.IsEmpty() && page.DefaultView != null)
			{
				ctx.Service = page.DefaultView.Service;
				ctx.Action = page.DefaultView.Action;
			}

			if (!ctx.Service.IsEmpty())
			{
				var cache = ctx.GetService<ITypeActivatorCache>();
				(var type, var invoker) = cache?.Get(ctx.Service + "." + ctx.Action) ?? (null, null);
				result = invoker?.Invoke(ctx, type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}
			else
				result = new ApiResult();

			if (result is ApiResult ajax)
			{
				var pageParts = new ApiResponse();
				page.OnLoadContent(pageParts);
				ajax.ApiResponse.Insert(pageParts);
			}

			return result;
		}
	}
}
