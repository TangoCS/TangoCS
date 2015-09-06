using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nephrite.AccessControl;
using Nephrite.Data;
using Nephrite.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nephrite.MVC
{
	/// <summary>
	/// Пока у нас есть поддержка webforms, возващаем bool, чтобы можно было прекращать Response
	/// </summary>
	public interface IActionInvoker
	{
		bool Invoke();
	}

	public class ControllerActionInvoker : IActionInvoker
	{
		ActionContext _actionContext;
		//IDataContext _dataContext;
		//IIdentityManager<int> _identityManager;
		//IAccessControl _accessControl;

		public ControllerActionInvoker(
			ActionContext actionContext)
		{
			_actionContext = actionContext;
			//_dataContext = dataContext;
			//_identityManager = identityManager;
			//_accessControl = accessControl;
		}

		public ActionContext ActionContext
		{
			get { return _actionContext; }
		}

        public bool Invoke()
		{
			ActionResult res = null;

			var t = _actionContext.RouteData.Values[MvcOptions.ControllerName].ToString() + "Controller";
			var m = _actionContext.RouteData.Values[MvcOptions.ActionName].ToString().ToLower();
			Type controllerType =  ControllersCache.Get(t);
			if (controllerType == null)
			{
				res = new MessageResult(string.Format("Controller class {0} not found", t));
				res.ExecuteResult(_actionContext);
				return res.EndResponse;
			}

			Controller controller = DI.RequestServices.GetService(controllerType) as Controller;
			//Activator.CreateInstance(controllerType) as Controller;

			//controller.ActionContext = _actionContext;
			//controller.DataContext = _dataContext;
			//controller.IdentityManager = _identityManager;
			//controller.AccessControl = _accessControl;

			var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo method = null;
			var httpMethod = _actionContext.HttpContext.Request.Method;
			for (int i = 0; i < methods.Length; i++)
			{
				var curMethod = methods[i];				
                if (curMethod.Name.ToLower() == m)
				{
					if (httpMethod == "POST" && Attribute.IsDefined(curMethod, typeof(HttpPostAttribute)))
					{
						method = curMethod; break;
					}
					if (httpMethod == "GET" && Attribute.IsDefined(curMethod, typeof(HttpGetAttribute)))
					{
						method = curMethod; break;
					}
					if (httpMethod == "GET" && !Attribute.IsDefined(curMethod, typeof(HttpPostAttribute)))
					{
						method = curMethod; continue;
					}
					if (httpMethod == "POST" && !Attribute.IsDefined(curMethod, typeof(HttpGetAttribute)))
					{
						method = curMethod; continue;
					}
				}
			}
			if (method == null)
			{
				res = new MessageResult(string.Format("The method {0} was not found in the controller {1}", m, t));
				res.ExecuteResult(_actionContext);
				return res.EndResponse;
			}
			//var method = controllerType.GetMethod(m, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			var engine = method.GetCustomAttribute<ViewEngineAttribute>();
			if (engine != null)
			{
				_actionContext.Renderer = Activator.CreateInstance(engine.ViewEngineType) as IViewRenderer;
			}

			var anon = method.GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon == null)
			{
				string securableObjectKey = controller.Name + ".";

				var so = method.GetCustomAttribute<SecurableObjectAttribute>();
				if (so != null)
				{
					securableObjectKey += so.Name;
				}
				else
				{
					securableObjectKey += method.Name;
				}

				if (!controller.AccessControl.Check(securableObjectKey))
				{
					res = new MessageResult("Недостаточно полномочий для выполнения операции.");
					res.ExecuteResult(_actionContext);
					return res.EndResponse;
				}
			}

			ParameterInfo[] mp = method.GetParameters();
			object[] p = new object[mp.Length];

			bool isJson = _actionContext.HttpContext.Request.ContentType.ToLower() == "application/json; charset=utf-8";
			dynamic jsonObj = null;
            if (isJson)
			{
				var jsonString = String.Empty;
				_actionContext.HttpContext.Request.Body.Position = 0;
				using (var inputStream = new StreamReader(_actionContext.HttpContext.Request.Body))
				{
					jsonString = inputStream.ReadToEnd();
				}
				var converter = new ExpandoObjectConverter();
				jsonObj = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, converter);
            }

			for (int i = 0; i < mp.Length; i++)
			{
				if (isJson && mp[i].GetCustomAttributes(typeof(DynamicAttribute), true).Length > 0)
				{
					p[i] = jsonObj;
					continue;
                }

				string parmName = mp[i].Name.ToLower() == "id" ? "oid" : mp[i].Name.ToLower();
				string val = WebUtility.UrlDecode(_actionContext.Url.GetString(parmName));

				if (!isJson && val.IsEmpty())
				{
					val = _actionContext.HttpContext.Request.Form.Get(mp[i].Name);
				}

				if (!val.IsEmpty())
				{
					if (mp[i].ParameterType == typeof(Guid))
						p[i] = val.ToGuid();
                    else
						p[i] = Convert.ChangeType(val, mp[i].ParameterType);
				}
			}


			res = method.Invoke(controller, p) as ActionResult;
			res.ExecuteResult(_actionContext);
			return res.EndResponse;
		}
	}
}
