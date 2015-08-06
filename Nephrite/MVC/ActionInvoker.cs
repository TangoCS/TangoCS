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
	public interface IActionInvoker
	{
		void Invoke();
	}

	public class ControllerActionInvoker : IActionInvoker
	{
		ActionContext _actionContext;
		IDataContext _dataContext;
		IIdentityManager<int> _identityManager;
		IAccessControl _accessControl;

		public ControllerActionInvoker(
			ActionContext actionContext,
			IDataContext dataContext,
			IIdentityManager<int> identityManager,
			IAccessControl accessControl)
		{
			_actionContext = actionContext;
			_dataContext = dataContext;
			_identityManager = identityManager;
			_accessControl = accessControl;
		}

		public void Invoke()
		{
			ActionResult res = null;

			var t = _actionContext.RouteData.Values[MvcOptions.ControllerName].ToString() + "Controller";
			var m = _actionContext.RouteData.Values[MvcOptions.ActionName].ToString().ToLower();
			Type controllerType = ControllersCache.Get(t);
			if (controllerType == null)
			{
				res = new MessageResult(string.Format("Controller class {0} not found", t));
				res.ExecuteResult(_actionContext);
				return;
			}

			Controller controller = Activator.CreateInstance(controllerType) as Controller;

			controller.ActionContext = _actionContext;
			controller.DataContext = _dataContext;
			controller.IdentityManager = _identityManager;
			controller.AccessControl = _accessControl;

			var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo method = null;
			var httpMethod = _actionContext.HttpContext.Request.Method;
			bool methodFound = false;
			for (int i = 0; i < methods.Length; i++)
			{
				method = methods[i];				
                if (method.Name.ToLower() == m)
				{
					if (httpMethod == "GET" && !Attribute.IsDefined(method, typeof(HttpPostAttribute)))
					{
						methodFound = true; break;
					}
					if (httpMethod == "POST" && !Attribute.IsDefined(method, typeof(HttpGetAttribute)))
					{
						methodFound = true; break;
					}
				}
			}
			if (!methodFound)
			{
				res = new MessageResult(string.Format("The method {0} was not found in the controller {1}", m, t));
				res.ExecuteResult(_actionContext);
				return;
			}
			//var method = controllerType.GetMethod(m, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			object[] anon = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
			if (anon == null || anon.Length == 0)
			{
				string securableObjectKey = controller.Name + ".";

				object[] so = method.GetCustomAttributes(typeof(SecurableObjectAttribute), true);
				if (so != null && so.Length == 1)
				{
					securableObjectKey += (so[0] as SecurableObjectAttribute).Name;
				}
				else
				{
					securableObjectKey += method.Name;
				}

				if (!_accessControl.Check(securableObjectKey))
				{
					res = new MessageResult("Недостаточно полномочий для выполнения операции.");
					res.ExecuteResult(_actionContext);
					return;
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
		}
	}
}
