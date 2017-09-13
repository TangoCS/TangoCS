using System;

namespace Tango.UI
{
	public class CsFormInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext actionContext, Type t)
		{
			var template = Activator.CreateInstance(t) as ViewRootElement;
			template.Context = actionContext;
			return template.InjectProperties(actionContext.RequestServices).Execute();
		}
	}
}
