using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Tango.UI
{
	public static class InteractionHelper
	{
		public static MethodInfo GetEventMethod(IInteractionFlowElement recipient, string e)
		{
			var t = recipient.GetType();
			var name = e.ToLower();
			return FindMethod(t, name, recipient.Context.RequestMethod);
		}

		public static ActionResult RunEvent(IInteractionFlowElement recipient, string e)
		{
			var m = GetEventMethod(recipient, e);
			if (m == null)
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };

			if (recipient is IWithCheckAccess secured && !secured.CheckAccess(m))
				return secured.OnNoAccess();

			var filtersCollection = recipient.Context.GetService<FilterCollection>();
			var ps = m.GetParameters();

			if (ps.Length == 1 && m.ReturnType == typeof(void))
			{
				IJsonResponse resp = null;
				ActionResult res = null;

				var p = ps[0].ParameterType;
				if (p == typeof(ArrayResponse))
				{
					var arrRes = new ArrayResult();
					resp = arrRes.ApiResponse;
					res = arrRes;
				}
				else if (p == typeof(ApiResponse))
				{
					var apiRes = new ApiResult();
					resp = apiRes.ApiResponse;
					res = apiRes;
				}
				else if (p == typeof(ObjectResponse))
				{
					var arrObj = new ObjectResult();
					resp = arrObj.ApiResponse;
					res = arrObj;
				}

				var filterContext = new ActionFilterContext(recipient, m, res);
				RunBeforeActionFilters(filtersCollection.BeforeActionFilters, filterContext);
				if (filterContext.CancelResult != null) return filterContext.CancelResult;

				m.Invoke(recipient, new object[] { resp });

				RunAfterActionFilters(filtersCollection.AfterActionFilters, filterContext);
				if (filterContext.CancelResult != null) return filterContext.CancelResult;

				return res;
			}
			else if (m.ReturnType == typeof(ActionResult))
			{
				var p = ProcessParameters(recipient.Context, ps);

				ActionResult res = null;
				res = m.Invoke(recipient, p) as ActionResult;

				var filterContext = new ActionFilterContext(recipient, m, res);
				RunAfterActionFilters(filtersCollection.AfterActionFilters, filterContext);
				if (filterContext.CancelResult != null) return filterContext.CancelResult;

				return res;
			}

			throw new Exception($"{m.DeclaringType.Name}.{m.Name} method is not a valid action");
		}

		static object[] ProcessParameters(ActionContext context, ParameterInfo[] ps)
		{
			object[] p = new object[ps.Length];

			for (int i = 0; i < ps.Length; i++)
			{
				var name = ps[i].Name.ToLower();

				if (ps[i].ParameterType == typeof(Guid))
					p[i] = context.GetGuidArg(name);
				else if (ps[i].ParameterType == typeof(DateTime))
					p[i] = context.GetDateTimeArg(name);
				else
				{
					string val = WebUtility.UrlDecode(context.GetArg(name));
					if (!val.IsEmpty())
						p[i] = Convert.ChangeType(val, ps[i].ParameterType);
				}
			}

			return p;
		}

		static ActionResult RunBeforeActionFilters(IReadOnlyList<IBeforeActionFilter> collection, ActionFilterContext context)
		{
			foreach (var f in collection)
			{
				f.BeforeAction(context);
				if (context.CancelResult != null)
					return context.CancelResult;
			}
			return null;
		}

		static ActionResult RunAfterActionFilters(IReadOnlyList<IAfterActionFilter> collection, ActionFilterContext context)
		{
			foreach (var f in collection)
			{
				f.AfterAction(context);
				if (context.CancelResult != null)
					return context.CancelResult;
			}
			return null;
		}

		static MethodInfo FindMethod(Type type, string methodName, string httpMethod)
		{
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			MethodInfo method = null;
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
