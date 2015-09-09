using System;
using System.Collections.Generic;
using Microsoft.Framework.DependencyInjection;
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
		public Url Url { get { return ActionContext.Url; } }
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
		public Subject<int> User 
		{ 
			get 
			{ 
				return IdentityManager.CurrentSubject; 
			} 
		}

		public Controller()
		{
			DataContext = DI.RequestServices.GetService<IDataContext>();
			IdentityManager = DI.RequestServices.GetService<IIdentityManager<int>>();
			AccessControl = DI.RequestServices.GetService<IAccessControl>();
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

	public static class ControllersCache
	{
		static Dictionary<string, Type> _collection = new Dictionary<string, Type>();
		public static void Add<T>() where T : Controller
		{
			_collection.Add(typeof(T).Name.ToLower(), typeof(T));
		}
		public static void Add<T>(string name) where T : Controller
		{
			_collection.Add(name.ToLower(), typeof(T));
		}
		public static Type Get(string name)
		{
			if (!_collection.ContainsKey(name.ToLower())) return null;
			return _collection[name.ToLower()];
		}

		public static void AddController<T>(this IServiceCollection sc) where T : Controller
		{
			Add<T>();
			sc.AddScoped<T>();
		}
	}
}
