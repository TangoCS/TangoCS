using System;
using Nephrite.Multilanguage;

namespace Nephrite.UI
{
	public class CsFormInvoker : IActionInvoker
	{
		public ActionResult Invoke(ActionContext actionContext, Type t)
		{
			var template = Activator.CreateInstance(t) as ViewContainer;
			var tr = actionContext.RequestServices.GetService(typeof(ITextResource)) as ITextResource;
			template.Init(null, actionContext, tr);
			return template.InjectProperties(actionContext.RequestServices).Execute();
		}
	}
}
