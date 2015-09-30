using System;
using System.Collections.Generic;
using Nephrite.AccessControl;
using Nephrite.Data;
using Nephrite.Http;
using Nephrite.Identity;

namespace Nephrite.MVC
{
	public class Controller
	{
		public ActionContext ActionContext { get; set; }

		public IDataContext DataContext { get; private set; }
		public IIdentityManager<int> IdentityManager { get; private set; }
		public IAccessControl AccessControl { get; private set; }

		public IHttpContext HttpContext { get { return ActionContext.HttpContext; } }
		public IHttpRequest Request
		{
			get
            {
                return HttpContext.Request;
            }
		}
		public IHttpResponse Response
		{
			get
            {
                return HttpContext.Response;
            }
		}
		public Url Url { get { return ActionContext.Url; } }
		public Subject<int> User 
		{ 
			get 
			{ 
				return IdentityManager.CurrentSubject; 
			} 
		}

		public Controller()
		{
			DataContext = DI.GetService<IDataContext>();
			IdentityManager = DI.GetService<IIdentityManager<int>>();
			AccessControl = DI.GetService<IAccessControl>();
        }

		public string Name
		{
			get { return GetType().Name.Replace("Controller", ""); }
		}

		[NonAction]
		public ActionResult View(string viewName, object viewData)
		{
			return new ViewResult(Name, viewName, viewData);
		}

		[NonAction]
		public ActionResult RedirectBack()
		{
			return new RedirectBackResult();
		}

		[NonAction]
		public ActionResult Redirect(string url)
		{
			return new RedirectResult(url);
		}

		[NonAction]
		public ActionResult Message(string message)
		{
			return new MessageResult(message);
		}

		[NonAction]
		public ActionResult Message(string title, string message)
		{
			return new MessageResult(title, message);
		}
	}
}
