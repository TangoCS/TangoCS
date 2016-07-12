using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tango.UI
{
	public abstract class ActionResult
	{
		public abstract Task ExecuteResultAsync(ActionContext context);
	}

	public abstract class HttpResult : ActionResult
	{
		[JsonIgnore]
		public string ContentType { get; set; }
		[JsonIgnore]
		public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
		[JsonIgnore]
		public Dictionary<string, string> Cookies { get; private set; } = new Dictionary<string, string>();
		[JsonIgnore]
		public Func<string> ContentFunc { get; protected set; }

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
			Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
			Headers.Add("Expires", "0"); // Proxies.
			ContentFunc = () => Html;
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
			Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
			Headers.Add("Expires", "0"); // Proxies.
			ContentType = "application/json";
			ContentFunc = () => ApiResponse.Serialize();
		}
	}

	public class RedirectResult : HttpResult
	{
		public string Url { get; set; }

		public RedirectResult(string url)
		{
			Url = url;
			ContentType = "application/json";
			ContentFunc = () => JsonConvert.SerializeObject(this, Json.CamelCase);
		}
	}

	public class RedirectBackResult : RedirectResult
	{
		public RedirectBackResult(string url = null) : base(url) { }
		public override Task ExecuteResultAsync(ActionContext context)
		{
			Url = Url ?? context.GetArg(Constants.ReturnUrl);
			return base.ExecuteResultAsync(context);
		}
	}

	public class UserLoginResult : RedirectBackResult
	{
		IIdentity _user;
		public UserLoginResult(IIdentity user)
		{
			_user = user;
		}

		public override Task ExecuteResultAsync(ActionContext context)
		{
			var executor = context.RequestServices.GetService(typeof(IUserSignInExecutor)) as IUserSignInExecutor;
			executor.Execute(_user);

			return base.ExecuteResultAsync(context);
		}
	}

	public interface IUserSignInExecutor
	{
		Task Execute(IIdentity user);
	}

	public interface IHttpResultExecutor
	{
		Task Execute(ActionContext actionContext, HttpResult result);
	}
}
