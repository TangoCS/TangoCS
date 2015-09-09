using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
using Newtonsoft.Json;
using Nephrite.Http;

namespace Nephrite.MVC
{
	public abstract class ActionResult
	{
		[JsonIgnore]
		public bool EndResponse { get; set; }
		public abstract void ExecuteResult(ActionContext context);
	}

	public class ViewResult : ActionResult
	{
		public string ViewFolder { get; set; }
		public string ViewName { get; set; }
		public object ViewData { get; set; }

		public ViewResult(string folder, string viewName, object viewData)
		{
			ViewFolder = folder;
			ViewName = viewName;
			ViewData = viewData;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Renderer.RenderView(ViewFolder, ViewName, ViewData);
		}
	}

	public class MessageResult : ActionResult
	{
		public string Title { get; set; }
		public string Message { get; set; }

		public MessageResult(string message)
		{
			Title = "Внимание!";
			Message = message;
		}

		public MessageResult(string title, string message)
		{
			Title = title;
			Message = message;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Renderer.RenderMessage(Title, Message);
		}
	}

	public class HtmlResult : ActionResult
	{
		public string Title { get; set; }
		public string Html { get; set; }

		public HtmlResult(string title, string html)
		{
			Title = title;
			Html = html;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.Renderer.RenderHtml(Title, Html);
		}
	}

	public class RedirectResult : ActionResult
	{
		public string Url { get; set; }

		public RedirectResult(string url)
		{
			Url = url;
		}

		public override void ExecuteResult(ActionContext context)
		{
			context.HttpContext.Response.Redirect(Url);
		}
	}

	public class RedirectBackResult : ActionResult
	{
		public override void ExecuteResult(ActionContext context)
		{
			context.HttpContext.Response.Redirect(context.Url.ReturnUrl);
		}
	}

	public class RedirectToLoginResult : ActionResult
	{
		public override void ExecuteResult(ActionContext context)
		{
			var AppSettings = context.HttpContext.RequestServices.GetService<IPersistentSettings>();

			if (AppSettings.Get("loginurl").IsEmpty())
			{
				var msg = new MessageResult("Недостаточно полномочий для доступа к информации");
				msg.ExecuteResult(context);
			}
			else
			{
				AbstractQueryString u = new Url(AppSettings.Get("loginurl"));
				u = u.AddParameter(MvcOptions.ReturnUrl, context.Url.CreateReturnUrl());
				context.HttpContext.Response.Redirect(u);
			}
		}
	}

	public class AjaxResult : ActionResult
	{
        public Dictionary<string, object> Data { get; set; }
		public Dictionary<string, object> Html { get; set; }

		public AjaxResult()
		{
			EndResponse = true;
			Data = new Dictionary<string, object>();
			Html = new Dictionary<string, object>();
			Data.Add("html", Html);
        }

		public override void ExecuteResult(ActionContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "application/json";
			context.HttpContext.Response.Write(JsonConvert.SerializeObject(Data, Json.CamelCase));
        }
	}

	public class AjaxRedirectResult : ActionResult
	{
		public string Url { get; set; }

		public AjaxRedirectResult(string url)
		{
			EndResponse = true;
			Url = url;
		}

		public override void ExecuteResult(ActionContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "application/json";
			context.HttpContext.Response.Write(JsonConvert.SerializeObject(this, Json.CamelCase));
		}
	}
}
