using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Tango.FileStorage;
using Tango.UI;

namespace Tango.AspNetCore
{
	public static partial class Handlers
	{
		public static async Task PageHandler<T>(this HttpContext c)
			where T : ViewRootElement, new()
		{
			var isAjax = c.Request.Headers.ContainsKey("x-request-guid") || c.GetRouteData().DataTokens.ContainsKey("data");
			var ctx = new AspNetCoreActionContext(c);

			if (isAjax)
				await RunResource.Ajax<T>(ctx);
			else
				await RunResource.Page<T>(ctx);
		}

		public static async Task PageHandler<T>(this HttpContext c, string service, string action)
			where T : ViewRootElement, new()
		{
			var d = c.GetRouteData();
			d.Values.Add("service", service);
			d.Values.Add("action", action);

			await c.PageHandler<T>();
		}

		public static async Task FileHandler(this HttpContext c)
		{
			var ctx = new AspNetCoreActionContext(c);
			await RunFileResource.File(ctx);
		}
	}
}
