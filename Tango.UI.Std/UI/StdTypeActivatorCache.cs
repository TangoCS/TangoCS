using System;
using System.Collections.Generic;
using System.Net;
using Tango.Html;

namespace Tango.UI.Std
{
	public class StdTypeActivatorCache : DefaultTypeActivatorCache
	{
		public StdTypeActivatorCache()
		{
			typeInfos.Add(new InvokeableTypeInfo
			{
				Filter = t => t.IsSubclassOf(typeof(AbstractViewPage)) && !t.IsAbstract,
				Keys = (p, t) => new List<ActionInfo> { new ActionInfo { Service = "_page", Action = t.Name } },
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

			TypeActivatorInfo view = null;

			if (!ctx.Service.IsEmpty())
				view = cache.Get(ctx.Service + "." + ctx.Action);

			if (ctx.RootReceiver != t.Name.ToLower() && !(view?.Type?.IsSubclassOf(typeof(Controller)) ?? false))
			{
				if (ctx.RootReceiver != null)
				{
					ctx.IsFirstLoad = true;
					var tOldPage = cache.Get("_page." + ctx.RootReceiver);
					if (tOldPage == null)
						return new HttpResult { StatusCode = HttpStatusCode.NotFound };
					oldPage = Activator.CreateInstance(tOldPage.Type) as AbstractViewPage;
					oldPage.Context = ctx;
					oldPage.InjectProperties(ctx.RequestServices);
				}
			}

			page.OnInit();

			ActionResult result;
			if (!ctx.Service.IsEmpty())
			{
				if (view?.Args != null)
				{
					foreach (var kv in view.Args)
					{
						if (!ctx.AllArgs.ContainsKey(kv.Key))
						{
							ctx.AllArgs.Add(kv.Key, kv.Value);
							ctx.FormData.Add(kv.Key, kv.Value);
						}
					}
				}
				result = view?.Invoker?.Invoke(ctx, view.Type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
			}
			else
			{
				var res = new ApiResult();
				result = res;
				res.ApiResponse.ReplaceWidget("container", w => w.Article(a => a.ID("container")));
			}

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
