using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using Tango.AccessControl;
using Tango.Html;
using Tango.Identity.Std;
using Tango.Logger;

namespace Tango.UI.Std
{
	public abstract class ViewPagePart : ViewRootElement, IWithCheckAccess, IContainerItem
	{
		[Inject]
		public IRequestLoggerProvider LoggerProvider { get; set; }

		public override bool UsePropertyInjection => true;
		public virtual ViewContainer GetContainer() => new DefaultContainer();

		public override ActionResult Execute()
		{
			OnInit();
			AfterInit();

			var r = Context.EventReceiver;
			var recipient = r == null || r == ID?.ToLower() || r == ClientID?.ToLower() ? this : Context.EventReceivers.First(o => o.ClientID == r);
			var e = Context.Event.IsEmpty() ? "onload" : Context.Event;

			return InteractionHelper.RunEvent(recipient, e);
		}

		public virtual void OnFirstLoad(ApiResponse response) { }
		public abstract void OnLoad(ApiResponse response);

		public virtual bool CheckAccess(MethodInfo method)
		{
			var anon = method.GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon != null) return true;

			var ac = Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
			if (ac == null) return false;

			var so = method.GetCustomAttribute<SecurableObjectAttribute>();
			var soname = so != null ? so.Name : Context.Action;

			return ac.Check(Context.Service + "." + soname);
		}

		public ActionResult OnNoAccess()
		{
			if (Context.RequestServices.GetService(typeof(IIdentity)) is IIdentity identity && identity.IsAuthenticated)
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };
			else
				return new ChallengeResult();
		}
	}

	public abstract class ViewPage : ViewRootElement
	{
		[Inject]
		public IIdentity User { get; set; }

		public override ActionResult Execute()
		{
			if (!CheckAccess()) return new ChallengeResult();

			OnInit();
			AfterInit();

			var w = new HtmlWriter();

			byte[] token = new byte[32];
			using (var csprng = new RNGCryptoServiceProvider())
			{
				csprng.GetBytes(token);
			}
			var tokenString = token.ByteArrayToHex();

			void headAttr(TagAttributes a)
			{
				a.Data("x-csrf-token", tokenString).Data("page", GetType().Name.ToLower());
			}

			w.DocType();
			w.Html(() => {
				w.Head(headAttr, () => {
					w.HeadTitle(a => a.ID("title"));
					w.HeadMeta(a => a.HttpEquiv("content-type").Content("text/html; charset=utf-8"));
					var r = DefaultView?.Resolve(Context);
					w.HeadMeta(a => a.ID(Constants.MetaHome).Data("href", "/").Data("alias", r?.Result.ToString()));
					w.HeadMeta(a => a.ID(Constants.MetaCurrent));
					HeadContent(w);
				});
				w.Body(() => {
					Body(w);
				});
			});

			return new HtmlResult(w.ToString(), tokenString);
		}

		public virtual void OnLoadContent(ApiResponse response)
		{
		}

		bool CheckAccess()
		{
			var anon = GetType().GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon != null) return true;

			return User != null && User.IsAuthenticated;
		}


		public virtual ActionTarget DefaultView => null;

		protected abstract void Body(HtmlWriter w);
		protected abstract void HeadContent(HtmlWriter w);
	}

	[OnAction]
	public class Home_index : ViewPagePart
	{
		public override void OnLoad(ApiResponse response)
		{
		}
	}

}