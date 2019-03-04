using System;

namespace Tango.UI
{
	public class CsFormInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext actionContext, Type t)
		{
			var template = Activator.CreateInstance(t) as ViewRootElement;
			template.Context = actionContext;
			template.InjectProperties(actionContext.RequestServices);
			return template.RunActionInvokingFilter() ?? template.Execute();
		}
	}
}
