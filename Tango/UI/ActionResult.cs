using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tango.UI
{
	public abstract class ActionResult
	{
		public abstract Task ExecuteResultAsync(ActionContext context);
	}

	public class HttpResult : ActionResult
	{
		[JsonIgnore]
		public string ContentType { get; set; }
		[JsonIgnore]
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
		[JsonIgnore]
		public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
		[JsonIgnore]
		public Dictionary<string, string> Cookies { get; private set; } = new Dictionary<string, string>();
		[JsonIgnore]
		public Func<ActionContext, byte[]> ContentFunc { get; protected set; }

		public override Task ExecuteResultAsync(ActionContext context)
		{
			var executor = context.RequestServices.GetService(typeof(IHttpResultExecutor)) as IHttpResultExecutor;
			return executor.Execute(context, this);
		}
	}

	public class HtmlResult : HttpResult
	{
		public string Html { get; set; }

		public HtmlResult(string html, string csrfToken)
		{
			Html = html;
			Cookies.Add("x-csrf-token", csrfToken);
			Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
			ContentFunc = ctx => Encoding.UTF8.GetBytes(Html);
		}
	}

	public class MessageResult : HttpResult
	{
		public MessageResult(string title, string message)
		{

		}
		public MessageResult(string message)
		{

		}
	}

	public class AjaxResult : HttpResult
	{
		public IJsonResponse ApiResponse { get; private set; }
		public AjaxResult(IJsonResponse data)
		{
			ApiResponse = data;
			Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
			ContentType = "application/json";
			ContentFunc = ctx => Encoding.UTF8.GetBytes(ApiResponse.Serialize(ctx));
		}
	}

	public class RedirectResult : HttpResult
	{
		public string Url { get; set; }

		public RedirectResult(string url)
		{
			Url = url;
			ContentType = "application/json";
			ContentFunc = ctx => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Json.StdSettings));
		}
	}

	public class RedirectBackResult : RedirectResult
	{
		public RedirectBackResult(string url = null) : base(url) { }
		public override Task ExecuteResultAsync(ActionContext context)
		{
			Url = Url ?? context.ReturnUrl ?? context.AllArgs.Parse<string>(Constants.ReturnUrl);
			return base.ExecuteResultAsync(context);
		}
	}

	public class SignInResult : RedirectBackResult
	{
		IIdentity _user;
		public SignInResult(IIdentity user)
		{
			_user = user;
		}

		public override Task ExecuteResultAsync(ActionContext context)
		{
			var executor = context.RequestServices.GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;
			executor.SignIn(_user);

			return base.ExecuteResultAsync(context);
		}
	}

	public class ChallengeResult : ActionResult
	{
		public override Task ExecuteResultAsync(ActionContext context)
		{
			var executor = context.RequestServices.GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;
			return executor.Challenge();
		}
	}

	public class SignOutResult : ActionResult
	{
		public override Task ExecuteResultAsync(ActionContext context)
		{
			var executor = context.RequestServices.GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;
			return executor.SignOut();
		}
	}

	public class FileResult : HttpResult
	{
		public FileResult(string fileName, byte[] bytes)
		{
			ContentType = "application/octet-stream";
			fileName = fileName.Replace(" ", "_");
			Headers.Add("content-disposition", "attachment; filename=\"" + Uri.EscapeDataString(fileName) + "\"");
			ContentFunc = ctx => { return bytes; };
		}
	}

	public class FileContentResult : HttpResult
	{
		public FileContentResult(byte[] fileContents, string contentType)
		{
			ContentType = contentType;
			ContentFunc = ctx => fileContents;
		}
	}

	public interface IAuthenticationManager
	{
		Task SignIn(IIdentity user);
		Task SignOut();
		Task Challenge();
	}

	public interface IHttpResultExecutor
	{
		Task Execute(ActionContext actionContext, HttpResult result);
	}
}
