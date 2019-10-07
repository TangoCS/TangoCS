using Microsoft.AspNetCore.Http;
using Tango.UI;
using System.Threading.Tasks;

namespace Tango.AspNetCore
{
	public class HttpResultExecutor : IHttpResultExecutor
	{
		HttpContext context;

		public HttpResultExecutor(IHttpContextAccessor contextAccessor)
		{
			context = contextAccessor.HttpContext;
		}

		public Task Execute(ActionContext actionContext, HttpResult result)
		{
			if (result.Cookies.Count > 0)
				foreach (var cookie in result.Cookies)
					context.Response.Cookies.Append(cookie.Key, cookie.Value);

			if (result.Headers.Count > 0)
				foreach (var header in result.Headers)
					context.Response.Headers.Add(header.Key, header.Value);

			if (!result.ContentType.IsEmpty())
				context.Response.ContentType = result.ContentType;

			context.Response.StatusCode = (int)result.StatusCode;

			if (actionContext.RequestID != null)
				context.Response.Headers["X-Request-Guid"] = actionContext.RequestID.ToString();

			if (result.ContentFunc != null)
			{
				var data = result.ContentFunc(actionContext);
				return context.Response.Body.WriteAsync(data, 0, data.Length);
			}

			return Task.FromResult(0);
		}
	}
}
