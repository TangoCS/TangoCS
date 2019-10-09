using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tango.AspNetCore
{
	public class RequestEnvironmentProxy : IRequestEnvironment
	{
		HttpContext _ctx;
		public RequestEnvironmentProxy(IHttpContextAccessor contextAccessor)
		{
			_ctx = contextAccessor.HttpContext;
		}

		public IDictionary<string, IReadOnlyList<string>> Headers =>
			_ctx.Request.Headers.ToDictionary(o => o.Key, o => o.Value as IReadOnlyList<string>);

		public string Method => _ctx.Request.Method;
		public string Protocol => _ctx.Request.Protocol;
		public string Scheme => _ctx.Request.Scheme;
		public string Host => _ctx.Request.Host.Value;
		public string Path => _ctx.Request.Path.Value;
		public string QueryString => _ctx.Request.QueryString.Value;

		public string Referrer => _ctx.Request.Headers["Referer"];
		public string UserAgent => _ctx.Request.Headers["User-Agent"];
		public IPAddress IP => _ctx.Connection.RemoteIpAddress;
	}
}
