using System.Net;
using System.Reflection;
using System.Security.Principal;

namespace Tango.UI
{
	public abstract class Controller : InteractionFlowElement, IWithCheckAccess
	{
		[NonAction]
		public ActionResult RedirectBack(int code = 0)
		{
			return new RedirectBackResult(code);
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
		public virtual ActionResult OK()
		{
			return new HttpResult { StatusCode = HttpStatusCode.OK };
		}

		[NonAction]
		public ActionResult MessageResult(string message)
		{
			return new MessageResult(message);
		}

		public abstract bool CheckAccess(MethodInfo method);

		public ActionResult OnNoAccess()
		{
			if (Context.RequestServices.GetService(typeof(IIdentity)) is IIdentity identity && identity.IsAuthenticated)
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };
			else
				return new ChallengeResult();
		}
	}
}
