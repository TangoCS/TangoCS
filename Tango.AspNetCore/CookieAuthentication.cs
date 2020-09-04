using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Tango.UI;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tango.AspNetCore
{
	public static partial class SolutionConfig
	{
		public static void ConfigureCookieAuthentication(this IServiceCollection services, Action<CookieAuthenticationOptions> options = null)
		{
			Action<CookieAuthenticationOptions> allOptions = o => {
				o.LoginPath = new PathString("/account/login");
				o.AccessDeniedPath = new PathString("/forbidden/");
			};
			if (options != null)
				allOptions += options;

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(allOptions);
		}

		public static void UseCustomCookieAuthentication(this IApplicationBuilder app)
		{
			app.UseAuthentication();

			app.Map("/logout", b => {
				b.Run(async context => {
					var manager = context.RequestServices.GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;
					await manager.SignOut();
					context.Response.Redirect("/");
				});
			});

			app.Map("/forbidden", b => {
				b.Run(async context => {
					await context.Response.WriteAsync("forbidden");
				});
			});
		}
	}

	public class AspNetCoreAuthenticationManager : IAuthenticationManager
	{
		HttpContext _context;
		public AspNetCoreAuthenticationManager(HttpContext context)
		{
			_context = context;
		}

		public async Task Challenge()
		{
			await _context.ChallengeAsync();
		}

		public async Task SignIn(IIdentity user)
		{
			var principal = new ClaimsPrincipal(user);
			var scheme = CookieAuthenticationDefaults.AuthenticationScheme;

			await _context.SignInAsync(scheme, principal);
		}

		public async Task SignOut()
		{
			await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
