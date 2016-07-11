using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Tango.UI
{
	public interface IActionInvoker
	{
		ActionResult Invoke(ActionContext actionContext, Type t);
	}

	public class ControllerActionInvoker : IActionInvoker
	{
        public ActionResult Invoke(ActionContext actionContext, Type t)
		{
			ActionResult res = null;

			var m = actionContext.Action.ToLower();

			Controller controller = null;
            var method = FindMethod(actionContext, t, m);
			if (method == null)
			{
				var std = t.GetCustomAttributes<HasStandardActionAttribute>().FirstOrDefault(o => o.Name == m);
                if (std != null)
				{
					t = std.StdControllerType;
					method = FindMethod(actionContext, t, std.StdControllerAction ?? m);
					//if (std.ViewEngineType != null)
					//	_actionContext.RendererType = std.ViewEngineType;
				}	
			}

			controller = Activator.CreateInstance(t) as Controller;
			controller.Init(actionContext);
			

			if (method == null)
				return controller.MessageResult(string.Format("The method {0} was not found in the controller {1}", m, t.Name));

			//var engine = method.GetCustomAttribute<ViewEngineAttribute>();
			//if (engine != null)
			//{
			//	_actionContext.RendererType = engine.ViewEngineType;
			//}

			if (!controller.CheckAccess(method)) return controller.AccessDeniedResult();

			ParameterInfo[] mp = method.GetParameters();
			object[] p = new object[mp.Length];

			for (int i = 0; i < mp.Length; i++)
			{
				string parmName = mp[i].Name.ToLower() == "id" ? "oid" : mp[i].Name.ToLower();
				string val = WebUtility.UrlDecode(actionContext.GetArg(parmName));

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
			return res;
		}

		MethodInfo FindMethod(ActionContext actionContext, Type controllerType, string methodName)
		{
			var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo method = null;
			var httpMethod = actionContext.RequestMethod;
			for (int i = 0; i < methods.Length; i++)
			{
				var curMethod = methods[i];
				if (Attribute.IsDefined(curMethod, typeof(NonActionAttribute))) continue;
                if (curMethod.Name.ToLower() == methodName)
				{
					bool hasPost = Attribute.IsDefined(curMethod, typeof(HttpPostAttribute));
					bool hasGet = Attribute.IsDefined(curMethod, typeof(HttpGetAttribute));
					if (!hasGet && !hasPost)
					{
						method = curMethod; break;
					}
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
	}
}
