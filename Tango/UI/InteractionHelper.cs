using System;
using System.Net;
using System.Reflection;

namespace Tango.UI
{
	public static class InteractionHelper
	{
		public static ActionResult RunEvent(IInteractionFlowElement recipient, string e, Action<ApiResponse> firstLoad, Action<ApiResponse> renderLog)
		{	
			var t = recipient.GetType();
			var m = FindMethod(t, e.ToLower(), recipient.Context.RequestMethod);
			if (m == null)
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };

			if (recipient is IWithCheckAccess && !(recipient as IWithCheckAccess).CheckAccess(m))
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };

			var ps = m.GetParameters();

			if (ps.Length == 1 && m.ReturnType == typeof(void))
			{
				IJsonResponse resp = null;
				var p = ps[0].ParameterType;
				if (p == typeof(ArrayResponse))
					resp = new ArrayResponse();
				else if (p == typeof(ApiResponse))
				{
					var apiResp = new ApiResponse(recipient);
					firstLoad?.Invoke(apiResp);
					resp = apiResp;
				}
				else if (p == typeof(ObjectResponse))
					resp = new ObjectResponse();

				m.Invoke(recipient, new object[] { resp });

				if (resp is ApiResponse && renderLog != null)
					renderLog(resp as ApiResponse);

				return new AjaxResult(resp);
			}
			else if (m.ReturnType == typeof(ActionResult))
			{
				object[] p = new object[ps.Length];

				for (int i = 0; i < ps.Length; i++)
				{
					string parmName = ps[i].Name.ToLower() == "id" ? "oid" : ps[i].Name.ToLower();
					string val = WebUtility.UrlDecode(recipient.Context.GetArg(parmName));

					if (!val.IsEmpty())
					{
						if (ps[i].ParameterType == typeof(Guid))
							p[i] = val.ToGuid();
						else
						{
							p[i] = Convert.ChangeType(val, ps[i].ParameterType);
						}
					}
				}

				return m.Invoke(recipient, p) as ActionResult;
			}

			throw new Exception($"{t.Name}.{e} method is not a valid action");
		}

		public static MethodInfo FindMethod(Type type, string methodName, string httpMethod)
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
