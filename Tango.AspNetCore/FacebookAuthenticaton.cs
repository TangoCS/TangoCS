using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tango.Identity;
using Tango.Logger;
using Tango.UI;

namespace Tango.AspNetCore
{
	public static partial class SolutionConfig
	{
		public static void UseFacebookAuthentication(this IApplicationBuilder app, Action<FacebookAuthenticationOptions> configureOptions)
		{
			var opt = new FacebookAuthenticationOptions();
			configureOptions(opt);

			var clientID = opt.FacebookAppID;
			var secretKey = opt.FacebookAppSecret;
			var callback = WebUtility.UrlEncode(opt.CallbackUrl);

			app.Map("/facebooklogin", b => {
				b.Run(async context => {
					var facebookAppId = opt.FacebookAppID;
					var redirectUri = "https://" + context.Request.Host.Value + opt.CallbackUrl;
					var facebookAuthUrl = $"https://www.facebook.com/v12.0/dialog/oauth?client_id={facebookAppId}&redirect_uri={redirectUri}&response_type=code&scope=email";
					context.Response.Redirect(facebookAuthUrl);
					await Task.CompletedTask;
				});
			});
			app.Map(opt.CallbackUrl, b => {
				b.Run(async context => {
					AspNetCoreActionContext ac = null;
					ActionResult result = null;

					try
					{
						ac = new AspNetCoreActionContext(context);

						FacebookUserProfile profile = null;
						var code = context.Request.Query["code"];
						var facebookAppId = opt.FacebookAppID;
						var facebookAppSecret = opt.FacebookAppSecret;
						var redirectUri = "https://" + context.Request.Host.Value + opt.CallbackUrl;

						var tokenEndpoint = $"https://graph.facebook.com/v12.0/oauth/access_token?client_id={facebookAppId}&redirect_uri={redirectUri}&client_secret={facebookAppSecret}&code={code}";

						using (var client = new HttpClient())
						{
							var tokenResponse = await client.GetStringAsync(tokenEndpoint);
							var tokenData = JsonConvert.DeserializeObject<FacebookTokenResponse>(tokenResponse);
							var accessToken = tokenData.AccessToken;

							var userInfoEndpoint = $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";
							var userResponse = await client.GetStringAsync(userInfoEndpoint);
							profile = JsonConvert.DeserializeObject<FacebookUserProfile>(userResponse);
						}

						var helper = context.RequestServices.GetService<IFacebookAuthenticationHelper>();
						helper.InjectProperties(context.RequestServices);

						result = await helper.DoLogin(profile, "/");
					}
					catch (Exception ex)
					{
						var res = context.RequestServices.GetService(typeof(IErrorResult)) as IErrorResult;
						var message = res?.OnError(ex) ?? ex.ToString().Replace(Environment.NewLine, "<br/>");
						result = new HtmlResult(message, "");

						var err = context.RequestServices.GetService(typeof(IErrorLogger)) as IErrorLogger;
						err.Log(ex);
					}

					if (ac != null)
						await result.ExecuteResultAsync(ac);
				});
			});
		}
	}

	public class FacebookAuthenticationOptions
	{
		public string FacebookAppID { get; set; }
		public string FacebookAppSecret { get; set; }
		public string CallbackUrl { get; set; } = "/facebooklogin-oidc";
	}

	internal class FacebookTokenResponse
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
	}

	
}
