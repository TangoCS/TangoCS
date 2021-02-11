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
				Filter = t => t.IsSubclassOf(typeof(AbstractViewPage)) && !t.IsAbstract,
				Keys = t => new List<string> { t.Name },
				Invoker = new CsPageInvoker()
			});
		}
	}

	public class CsPageInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext ctx, Type t)
		{
			var cache = ctx.GetService<ITypeActivatorCache>();
			AbstractViewPage oldPage = null;

			var page = Activator.CreateInstance(t) as AbstractViewPage;
			page.Context = ctx;
			page.InjectProperties(ctx.RequestServices);

			if (ctx.Service.IsEmpty() && page.DefaultView != null)
			{
				ctx.Service = page.DefaultView.Service;
				ctx.Action = page.DefaultView.Action;
			}

			(Type type, IActionInvoker invoker) view = (null, null);

			if (!ctx.Service.IsEmpty())
				view = cache.Get(ctx.Service + "." + ctx.Action) ?? (null, null);

			if (ctx.RootReceiver != t.Name.ToLower() && !(view.type?.IsSubclassOf(typeof(Controller)) ?? false))
			{
				if (ctx.RootReceiver != null)
				{
					ctx.IsFirstLoad = true;
					var tOldPage = cache.Get(ctx.RootReceiver) ?? (null, null);
					if (tOldPage.Type == null)
						return new HttpResult { StatusCode = HttpStatusCode.NotFound };
					oldPage = Activator.CreateInstance(tOldPage.Type) as AbstractViewPage;
					oldPage.Context = ctx;
					oldPage.InjectProperties(ctx.RequestServices);
				}
			}

			page.OnInit();

			ActionResult result;
			if (!ctx.Service.IsEmpty())
				result = view.invoker?.Invoke(ctx, view.type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
			else
				result = new ApiResult();

			if (result is ApiResult ajax && ctx.IsFirstLoad)
			{
				var pageParts = new ApiResponse();

				if (oldPage != null)
				{
					oldPage.OnInit();
					oldPage.OnUnloadContent(pageParts);
					pageParts.SetElementAttribute("head", "data-page", page.GetType().Name.ToLower());
				}

				page.OnLoadContent(pageParts);

				ajax.ApiResponse.Insert(pageParts);
			}

			return result;
		}
	}
}
