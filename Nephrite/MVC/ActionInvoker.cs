using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Nephrite.AccessControl;
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

		public ControllerActionInvoker(ActionContext actionContext)
		{
			_actionContext = actionContext;
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
			Type controllerType = _actionContext.TypeActivatorCache.Get(t);
			if (controllerType == null)
			{
				return MessageResult(string.Format("Controller class {0} not found", t));
			}

			Controller controller = null;
            var method = FindMethod(controllerType, m);
			if (method == null)
			{
				var std = controllerType.GetCustomAttributes<HasStandardMvcActionAttribute>().FirstOrDefault(o => o.Name == m);
                if (std != null)
				{
					controllerType = std.StdControllerType;
					method = FindMethod(controllerType, std.StdControllerAction ?? m);
					if (std.ViewEngineType != null)
						_actionContext.RendererType = std.ViewEngineType;
					controller = Activator.CreateInstance(controllerType) as Controller;
				}	
			}
			else
				controller = Activator.CreateInstance(controllerType) as Controller;

			if (method == null)
				return MessageResult(string.Format("The method {0} was not found in the controller {1}", m, t));

			controller.ActionContext = _actionContext;

			var engine = method.GetCustomAttribute<ViewEngineAttribute>();
			if (engine != null)
			{
				_actionContext.RendererType = engine.ViewEngineType;
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
					return AccessDeniedResult();
				}
			}

			ParameterInfo[] mp = method.GetParameters();
			object[] p = new object[mp.Length];

			bool isJson = _actionContext.HttpContext.Request.ContentType.ToLower() == "application/json; charset=utf-8";
			dynamic jsonObj = null;
            if (isJson)
			{
				var jsonString = string.Empty;
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
					{
						p[i] = Convert.ChangeType(val, mp[i].ParameterType);
					}
				}
			}

			res = method.Invoke(controller, p) as ActionResult;
			res.ExecuteResult(_actionContext);
			return res.EndResponse;
		}

		MethodInfo FindMethod(Type controllerType, string methodName)
		{
			var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo method = null;
			var httpMethod = _actionContext.HttpContext.Request.Method;
			for (int i = 0; i < methods.Length; i++)
			{
				var curMethod = methods[i];
				if (Attribute.IsDefined(curMethod, typeof(NonActionAttribute))) continue;
                if (curMethod.Name.ToLower() == methodName)
				{
					bool hasPost = Attribute.IsDefined(curMethod, typeof(HttpPostAttribute));
					bool hasGet = Attribute.IsDefined(curMethod, typeof(HttpGetAttribute));

					if (httpMethod == "POST" && hasPost)
					{
						method = curMethod; break;
					}
					if (httpMethod == "GET" && hasGet)
					{
						method = curMethod; break;
					}
					if (httpMethod == "GET" && !hasPost)
					{
						method = curMethod; continue;
					}
					if (httpMethod == "POST" && !hasGet)
					{
						method = curMethod; continue;
					}
				}
			}
			return method;
		}


		bool AccessDeniedResult()
		{
			return MessageResult("Недостаточно полномочий для выполнения операции.");
		}

		bool MessageResult(string message)
		{
			var res = new MessageResult(message);
			res.ExecuteResult(_actionContext);
			return res.EndResponse;
		}
	}
}
