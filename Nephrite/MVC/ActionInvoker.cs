using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nephrite.AccessControl;
using Nephrite.Data;
using Nephrite.Identity;

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
			var m = _actionContext.RouteData.Values[MvcOptions.ActionName].ToString();
			Type controllerType = ControllersCache.Get(t);
			Controller controller = Activator.CreateInstance(controllerType) as Controller;

			controller.ActionContext = _actionContext;
			controller.DataContext = _dataContext;
			controller.IdentityManager = _identityManager;
			controller.AccessControl = _accessControl;

			var method = controllerType.GetMethod(m, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			ParameterInfo[] mp = method.GetParameters();
			object[] p = new object[mp.Length];
			for (int i = 0; i < mp.Length; i++)
			{
				string val = mp[i].Name.ToLower() == "id" ? 
					_actionContext.Url.GetString("o" + mp[i].Name) : 
					_actionContext.Url.GetString(mp[i].Name);
				val = WebUtility.UrlDecode(val);
				try
				{
					if (mp[i].ParameterType == typeof(Guid))
						p[i] = val.ToGuid();
					else
						p[i] = Convert.ChangeType(val, mp[i].ParameterType);
				}
				catch
				{
					switch (mp[i].ParameterType.Name)
					{
						case "DateTime":
							p[i] = DateTime.Today;
							break;
						case "Int32":
							p[i] = 0;
							break;
						case "Boolean":
							p[i] = false;
							break;
						default:
							throw;
					}
				}
			}

			res = method.Invoke(controller, p) as ActionResult;
			res.ExecuteResult(_actionContext);
		}
	}
}
