using System;

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
			var controller = Activator.CreateInstance(t) as Controller;
			controller.Context = actionContext;
			controller.InjectProperties(actionContext.RequestServices);
			return InteractionHelper.RunEvent(controller, actionContext.Action, null);
		}		
	}
}
