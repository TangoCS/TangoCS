using System.Reflection;

namespace Tango.UI
{
	public abstract class Controller : InteractionFlowElement, IWithCheckAccess
	{
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

		public abstract bool CheckAccess(MethodInfo method);
	}
}
