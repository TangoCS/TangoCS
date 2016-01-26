using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nephrite.UI
{
	public abstract class ActionResult
	{
		//[JsonIgnore]
		//public bool EndResponse { get; set; }
		public virtual Task ExecuteResultAsync(ActionContext context)
		{
			ExecuteResult(context);
			return Task.FromResult(true);
		}

		public virtual void ExecuteResult(ActionContext context)
		{
		}
	}

	//public class ViewResult : ActionResult
	//{
	//	public string ViewFolder { get; set; }
	//	public string ViewName { get; set; }
	//	public object ViewData { get; set; }

	//	public ViewResult(string folder, string viewName, object viewData)
	//	{
	//		ViewFolder = folder;
	//		ViewName = viewName;
	//		ViewData = viewData;
	//	}

	//	public override void ExecuteResult(ActionContext context)
	//	{
	//		context.Renderer.RenderView(ViewFolder, ViewName, ViewData);
	//	}
	//}



	public class HtmlResult : ActionResult
	{
		public string Html { get; set; }

		public HtmlResult(string html)
		{
			Html = html;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Response.Write(Html);
		}
	}

	public class MessageResult : ActionResult
	{
		public MessageResult(string title, string message)
		{

		}
		public MessageResult(string message)
		{

		}
		public override void ExecuteResult(ActionContext context)
		{
			//context.Response.Write(Html);
		}
	}

	//public class RedirectResult : ActionResult
	//{
	//	public string Url { get; set; }

	//	public RedirectResult(string url)
	//	{
	//		Url = url;
	//	}

	//	public override void ExecuteResult(ActionContext context)
	//	{
	//		context.HttpContext.Response.Redirect(Url);
	//	}
	//}

	//public class RedirectToLoginResult : ActionResult
	//{
	//	public override void ExecuteResult(ActionContext context)
	//	{
	//		var AppSettings = DI.GetService<IPersistentSettings>();

	//		if (AppSettings.Get("loginurl").IsEmpty())
	//		{
	//			var msg = new MessageResult("Недостаточно полномочий для доступа к информации");
	//			msg.ExecuteResult(context);
	//		}
	//		else
	//		{
	//			AbstractQueryString u = new Url(AppSettings.Get("loginurl"));
	//			u = u.AddParameter(MvcOptions.ReturnUrl, context.Url.CreateReturnUrl());
	//			context.HttpContext.Response.Redirect(u);
	//		}
	//	}
	//}

	public class AjaxResult : ActionResult
	{
		public IJsonResponse ApiResponse { get; private set; }
		public AjaxResult(IJsonResponse data)
		{
			ApiResponse = data;
			//EndResponse = true;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Response.ContentType = "application/json";
			context.Response.Write(ApiResponse.Serialize());
        }
	}

	public class RedirectResult : ActionResult
	{
		public string Url { get; set; }

		public RedirectResult(string url)
		{
			//EndResponse = true;
			Url = url;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Response.ContentType = "application/json";
			context.Response.Write(JsonConvert.SerializeObject(this, Json.CamelCase));
		}
	}

	public class RedirectBackResult : ActionResult
	{
		public string Url { get; set; }
		public override void ExecuteResult(ActionContext context)
		{
			Url = context.GetArg(TemplatingConstants.ReturnUrl);
			context.Response.ContentType = "application/json";
			context.Response.Write(JsonConvert.SerializeObject(this, Json.CamelCase));			
		}
	}

	public class UserLoginResult : ActionResult
	{
		IIdentity _user;
		public UserLoginResult(IIdentity user)
		{
			_user = user;
		}

		public override Task ExecuteResultAsync(ActionContext context)
		{
			new RedirectBackResult().ExecuteResult(context);
			var executor = context.RequestServices.GetService(typeof(IUserSignInExecutor)) as IUserSignInExecutor;
			return executor.Execute(_user);
		}
	}

	public interface IUserSignInExecutor
	{
		Task Execute(IIdentity user);
	}
}
