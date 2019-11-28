using System;

namespace Tango.UI
{
	public class CsFormInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext actionContext, Type t)
		{
			return ViewRootElement.Invoke(actionContext, t);
		}
	}
}
