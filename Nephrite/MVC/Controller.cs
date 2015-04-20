using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.AccessControl;
using Nephrite.Data;
using Nephrite.Http;
using Nephrite.Identity;

namespace Nephrite.MVC
{
	public class Controller
	{
		public ActionContext ActionContext { get; set; }
		public IDataContext DataContext { get; set; }
		public IIdentityManager<int> IdentityManager { get; set; }
		public IAccessControl AccessControl { get; set; }

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

		public string Name
		{
			get { return GetType().Name.Replace("Controller", ""); }
		}

		protected StandardOperation<T> StandardOperation<T>() where T : new()
		{
			return new StandardOperation<T>(Name, DataContext, AccessControl);
		}

		protected ActionResult View(string viewName, object viewData)
		{
			return new ViewResult(Name, viewName, viewData);
		}
		protected ActionResult RedirectBack()
		{
			return new RedirectBackResult();
		}
		protected ActionResult Redirect(string url)
		{
			return new RedirectResult(url);
		}
		protected ActionResult Message(string message)
		{
			return new MessageResult(message);
		}
		protected ActionResult Message(string title, string message)
		{
			return new MessageResult(title, message);
		}
	}

	public class ControllersCache
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
	}
}
