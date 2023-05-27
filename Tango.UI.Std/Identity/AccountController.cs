using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Tango.AccessControl.Std;
using Tango.Html;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.Identity.Std
{
	[HasPredicates]
	public class AccountController : BaseController
	{
		[Inject]
		public IIdentityStore IdentityStore { get; set; }

		[Inject]
		public IPasswordHasher PasswordHasher { get; set; }

		[Inject]
		protected IIdentityOptions Options { get; set; }

		[OnAction("spm_subject", "activate")]
		public ActionResult Activate(int oid)
		{
			IdentityStore.Activate(oid);
			return RedirectBack();
		}

		[OnAction("spm_subject", "deactivate")]
		public ActionResult Deactivate(int oid)
		{
			IdentityStore.Deactivate(oid);
			return RedirectBack();
		}
		
		//[Predicate]
		//[BindPredicate("spm_subject.activate")]
		//public static bool CanActivate(SPM_Subject s) => !s.IsActive;

		//[Predicate]
		//[BindPredicate("spm_subject.deactivate")]
		//public static bool CanDeactivate(SPM_Subject s) => s.IsActive;

		[AllowAnonymous]
		[HttpGet]
		public virtual ActionResult Login()
		{
			var res = new ApiResult();
			res.ApiResponse.AddWidget("content", w => {
				w.H2(Resources.Get("Account.SignInTitle"));
				w.AjaxForm("form", true, a => a.Action("/account/dologin"), () => {
					w.TextBox("userName", null, a => a.Placeholder(Resources.Get("Account.UserName.Placeholder")));
					w.Password("password", null, a => a.Placeholder(Resources.Get("Account.Password.Placeholder")));
					//w.Hidden("returnurl", Context.GetArg("returnurl"));
					if (Options.AllowRememberMe)
					{
						w.CheckBox("cbRememberMe");
						w.Label("cbRememberMe", Resources.Get("Account.RememberMe"));
					}
					w.P(() => {
						w.SubmitButton(a => a.Class("btn"), Resources.Get("Account.SignIn"));
					});
					w.Span(a => a.Class("err").ID("err"), "");
				});

				if (Options.AllowRegister)
					w.P(() => {
						w.Write(Resources.Get("Account.DontHaveAccountQ") + " ");
						w.A(a => a.Href("/account/register"), Resources.Get("Account.Register"));
					});
				if (Options.AllowPasswordReset)
					w.P(a => a.Class("login-register-link"), () => w.A(a => a.Href("/account/passwordreset"), Resources.Get("Account.PasswordReset")));
			});
			return res;
		}

		[AllowAnonymous]
		[HttpGet]
		public virtual ActionResult PasswordReset()
		{
			var res = new ApiResult();
			res.ApiResponse.AddWidget("content", w => {
				w.H2(Resources.Get("Account.PasswordResetTitle"));
				w.P(Resources.Get("Account.PasswordResetHint"));
				w.AjaxForm("form", true, a => a.Action("/account/passwordreset"), () => {
					w.Label("emailAddress", Resources.Get("Account.Email"));
					w.TextBox("emailAddress", null, a => a.Autocomplete(false).Placeholder(Resources.Get("Account.Email.Placeholder")));
					w.SubmitButton(text: Resources.Get("Account.SignIn"));
					w.A(a => a.Href("/account/login?ReturnUrl=%2F"), Resources.Get("Common.Back"));
					w.Span(a => a.Class("err").ID("err"), "");
				});
			});
			return res;
		}

		[AllowAnonymous]
		[HttpPost]
		public virtual ActionResult PasswordReset(string emailAddress)
		{
			return RedirectBack();
		}

		[AllowAnonymous]
		[HttpPost]
		public virtual ActionResult Login(string returnUrl)
		{
			var userName = Context.FormData.Parse<string>("username");
			var password = Context.FormData.Parse<string>("password");

			if (userName.IsEmpty() || password.IsEmpty())
				return Message("Неправильное имя пользователя или пароль");

			var s = IdentityStore.UserFromName(userName);

			if (s == null || s.PasswordHash == null || !PasswordHasher.ValidatePassword(password, s.PasswordHash))
				return Message("Неправильное имя пользователя или пароль");

			if (s.LockoutEnabled)
				return Message("Пользователь заблокирован");

			return SignIn(s.Id.ToString(), s.UserName);
		}

		protected ActionResult Message(string text)
		{
			var res = new ApiResult();
			res.ApiResponse.AddWidget("err", text);
			return res;
		}

		protected ActionResult SignIn(string userId, string userName)
		{
			return new SignInResult(new ClaimsIdentity(
				new GenericIdentity(userName, "Password"),
				new List<Claim>()
				{
					new Claim(ClaimTypes.NameIdentifier, userId)
				})
			);
		}
	}
}