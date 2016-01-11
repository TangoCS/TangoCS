using System.Reflection;

namespace Nephrite.Templating
{
	public abstract class Controller : InteractionFlowElement, IWithPropertyInjection
	{
		public void Init(ActionContext context)
		{
			Context = context;
			this.InjectProperties(Context.RequestServices);
		}

		[NonAction]
		public ActionResult RedirectBack()
		{
			return new RedirectBackResult();
		}

		[NonAction]
		public ActionResult Redirect(string url)
		{
			return new RedirectResult(url);
		}

		[NonAction]
		public virtual ActionResult AccessDeniedResult()
		{
			return MessageResult("Недостаточно полномочий для выполнения операции.");
		}

		[NonAction]
		public ActionResult MessageResult(string message)
		{
			return new MessageResult(message);
		}

		[NonAction]
		public virtual bool CheckAccess(MethodInfo mi)
		{
			return true;
		}
	}
}
