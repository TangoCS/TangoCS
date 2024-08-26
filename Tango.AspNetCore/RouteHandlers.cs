using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
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

		public static async Task PageHandler<T>(this HttpContext c, string service = null, string action = null)
			where T : ViewRootElement, new()
		{
			var d = c.GetRouteData();
			if (service != null) d.DataTokens.Add("service", service);
			if (action != null)	d.DataTokens.Add("action", action);
			await c.PageHandler<T>();
		}

		public static async Task FileHandler(this HttpContext c)
		{
			var ctx = new AspNetCoreActionContext(c);
			await RunFileResource.File(ctx);
		}

		public static async Task XmlHandler(this HttpContext c)
		{
			var ctx = new AspNetCoreActionContext(c);
			await RunResource.RunXml(ctx);
		}

		public static async Task ActionHandler(this HttpContext c)
		{
			var ctx = new AspNetCoreActionContext(c);
			await RunResource.Run(ctx, x => x.RunAction(), (x, e) => {
				return new HtmlResult(e.Message) { StatusCode = System.Net.HttpStatusCode.NotFound };
			});
		}

		public static async Task ActionHandler(this HttpContext c, string service, string action)
		{
			var d = c.GetRouteData();
			d.DataTokens.Add("service", service);
			d.DataTokens.Add("action", action);

			await c.ActionHandler();
		}
	}

	/// <summary>
	/// Provides extension methods for adding new handlers to a <see cref="IRouteBuilder"/>.
	/// </summary>
	public static class RequestDelegateRouteBuilderExtensions
	{
		/// <summary>
		/// Adds a route to the <see cref="IRouteBuilder"/> for the given <paramref name="template"/>, and
		/// <paramref name="handler"/>.
		/// </summary>
		/// <param name="builder">The <see cref="IRouteBuilder"/>.</param>
		/// <param name="template">The route template.</param>
		/// <param name="handler">The <see cref="RequestDelegate"/> route handler.</param>
		/// <returns>A reference to the <paramref name="builder"/> after this operation has completed.</returns>
		public static IRouteBuilder MapRoute(this IRouteBuilder builder, string name, string template, RequestDelegate handler)
		{
			var route = new Route(
				new RouteHandler(handler),
				name,
				template,
				defaults: null,
				constraints: null,
				dataTokens: null,
				inlineConstraintResolver: builder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>());

			builder.Routes.Add(route);
			return builder;
		}
	}
}
