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
				w.AjaxForm("form", true, a => a.Action("/account/login"), () => {
					w.TextBox("userName", null, a => a.Placeholder(Resources.Get("Account.UserName")));
					w.Password("password", null, a => a.Placeholder(Resources.Get("Account.Password")));
					//w.Hidden("returnurl", Context.GetArg("returnurl"));
					if (Options.AllowRememberMe)
					{
						w.CheckBox("cbRememberMe");
						w.Label("cbRememberMe", Resources.Get("Account.RememberMe"));
					}
					w.P(() => w.SubmitButton(a => a.Class("btn"), Resources.Get("Account.SignIn")));
					w.Span(a => a.Class("err").ID("err"), "");
				});

				if (Options.AllowRegister)
					w.P(() => w.A(a => a.Href("/account/register"), Resources.Get("Account.Register")));
				if (Options.AllowPasswordReset)
					w.P(() => w.A(a => a.Href("/account/passwordreset"), Resources.Get("Account.PasswordReset")));
			});
			return res;
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
			//var userName = Context.FormData.Parse<string>("username");
			//var password = Context.FormData.Parse<string>("password");

			//if (userName.IsEmpty())
			//	return Message("Incorrect user name or password");

			//var s = DataContext.Connection.Query(@"select subjectid, systemname, passwordhash, isactive, isdeleted 
			//	from spm_subject where lower(systemname) = @p1", new { p1 = userName.ToLower() }).FirstOrDefault();

			//if (s == null || s.passwordhash == null)
			//	return Message("Incorrect user name or password");

			//string hash = s.passwordhash is string ? s.passwordhash : BitConverter.ToString(s.passwordhash).Replace("-", "");
			//if (!PasswordHasher.ValidatePassword(password, hash))
			//	return Message("Incorrect user name or password");

			//int subjid = s.subjectid;
			//if (DataContext.SPM_SubjectRole().Where(o => o.SubjectID == subjid).Count() == 0)
			//	return Message("User does not have any roles");

			//if (!s.isactive)
			//	return Message("User account has been blocked or you have not yet activated");

			//if (s.isdeleted)
			//	return Message("User is deleted");

			//return SignIn(subjid.ToString(), s.systemname);
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