using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Tango.UI;

namespace Tango.AspNetCore
{
    public class AspNetCoreActionContext : ActionContext
    {
        HttpContext HttpContext { get; set; }
        RouteData RouteData { get; set; }
        string[] skipcookies => new[] { ".AspNetCore.Cookies", "x-csrf-token" };

        public AspNetCoreActionContext(HttpContext ctx) : base(ctx.RequestServices)
		{
			HttpContext = ctx;
			RouteData = ctx.GetRouteData();

			if (Guid.TryParse(ctx.Request.Headers["X-Request-Guid"], out Guid rid))
				RequestID = rid;

			RequestMethod = ctx.Request.Method;
			IsLocalRequest = ctx.IsLocal();

			if (ctx.Request.ContentType != null)
			{
				if (ctx.Request.ContentType.ToLower().In("application/json; charset=utf-8", "text/plain;charset=utf-8"))
				{
					var jsonString = string.Empty;
					if (ctx.Request.Body.CanSeek)
						ctx.Request.Body.Position = 0;
					using (var inputStream = new StreamReader(ctx.Request.Body))
					{
						jsonString = inputStream.ReadToEndAsync().Result;
					}

					var postData = JsonConvert.DeserializeObject<DynamicDictionary>(jsonString, new DynamicDictionaryConverter());
					if (postData != null)
						FormData = postData;
				}
				else if (ctx.Request.ContentType.ToLower().StartsWith("multipart/form-data"))
				{
					foreach (var f in ctx.Request.Form)
						if (f.Value == "on")
							FormData.Add(f.Key, true);
						else if (f.Key == Constants.IEFormFix)
							continue;
						//else if (f.Key.StartsWith(Constants.PersistentArgsFormPrefix))
						//	FormData.Add(f.Key.Replace(Constants.PersistentArgsFormPrefix, ""), f.Value.ToString());
						else
							FormData.Add(f.Key, f.Value.ToString());

					foreach (var file in ctx.Request.Form.Files)
					{
						var cd = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
						var name = cd.Name.Value.Trim('"');
						var fileName = cd.FileName.Value.Trim('"');					

						if (!string.IsNullOrEmpty(fileName) && file.Length > 0 && file.Length < 2147483648)
						{
							var fi = new PostedFileInfo { FileName = fileName };
							using (var fs = file.OpenReadStream())
							{
								fi.FileBytes = new byte[fs.Length];
								fs.Read(fi.FileBytes, 0, (int)fs.Length);
							}
							FormData.Add(name, fi);
						}
						else
							FormData.Add(name, null);
					}
				}
			}

			var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var q in ctx.GetRouteData().Values)
				d[q.Key] = q.Value.ToString();

			ParseRouteParms(d);

			foreach (var q in ctx.Request.Query)
				d[q.Key] = q.Value.ToString();

            foreach (var q in ctx.Request.Cookies)
            {
                if (!skipcookies.Contains(q.Key))
					PersistentArgs[q.Key] = q.Value.ToString();
            }

            ParseQueryParms(d);

			ProcessFormData();

			foreach(var ret in ReturnUrl)
				ReturnTarget[ret.Key] = ParseReturnUrl(ret.Value);
		}
		
		protected override ActionTarget ParseReturnUrl(string returnUrl)
		{
			var parms = "";
			var values = new RouteValueDictionary();

			var i = returnUrl.IndexOf("?");
			if (i > 0)
			{
				parms = returnUrl.Substring(i);
				returnUrl = returnUrl.Substring(0, i);
			}
			if (!returnUrl.StartsWith("/")) returnUrl = "/" + returnUrl;

			var routes = RouteData.Routers.OfType<RouteCollection>().First();

			for (int j = 0; j < routes.Count; j++)
			{
				var route = routes[j] as Route;
				var matcher = new TemplateMatcher(route.ParsedTemplate, null);
				if (matcher.TryMatch(returnUrl, values))
					break;
			}

			var target = new ActionTarget();
			if (values.TryGetValue(Constants.ServiceName, out var service))
			{
				target.Service = service.ToString();
				values.Remove(Constants.ServiceName);
			}
			if (values.TryGetValue(Constants.ActionName, out var action))
			{
				target.Action = action.ToString();
				values.Remove(Constants.ActionName);
			}
			foreach (var value in values)
				target.Args.Add(value.Key, value.Value.ToString());

			var parsedParms = Url.ParseQuery(parms);
			foreach (var parm in parsedParms)
				target.Args.Add(parm.Key, parm.Value.Join(","));

			return target;
		}

		public override IServiceScope CreateServiceScope()
		{
			return new ServiceScopeProxy(RequestServices);
		}		
	}

	public class ServiceScopeProxy : IServiceScope
	{
		Microsoft.Extensions.DependencyInjection.IServiceScope _scope;

		public ServiceScopeProxy(IServiceProvider provider)
		{
			_scope = provider.CreateScope();
		}

		public IServiceProvider ServiceProvider => _scope.ServiceProvider;

		public void Dispose()
		{
			_scope.Dispose();
		}
	}

	public static class HttpContextExtensions
	{
		public const string NullIPv6 = "::1";

		public static bool IsLocal(this ConnectionInfo conn)
		{
			if (!conn.RemoteIpAddress.IsSet())
				return true;

			// we have a remote address set up
			// is local is same as remote, then we are local
			if (conn.LocalIpAddress.IsSet())
				return conn.RemoteIpAddress.Equals(conn.LocalIpAddress);

			// else we are remote if the remote IP address is not a loopback address
			return conn.RemoteIpAddress.IsLoopback();
		}

		public static bool IsLocal(this HttpContext ctx)
		{
			return ctx.Connection.IsLocal();
		}

		public static bool IsLocal(this HttpRequest req)
		{
			return req.HttpContext.IsLocal();
		}

		public static bool IsSet(this IPAddress address)
		{
			return address != null && address.ToString() != NullIPv6;
		}

		public static bool IsLoopback(this IPAddress address)
		{
			return IPAddress.IsLoopback(address);
		}
	}

}
