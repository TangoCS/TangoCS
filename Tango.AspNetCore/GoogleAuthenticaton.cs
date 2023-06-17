using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tango.Identity;
using Tango.UI;

namespace Tango.AspNetCore
{
	public static partial class SolutionConfig
	{
		public static void UseGoogleAuthentication(this IApplicationBuilder app, Action<GoogleAuthenticationOptions> configureOptions)
		{
			var opt = new GoogleAuthenticationOptions();
			configureOptions(opt);

			var clientID = opt.GoogleClientID;
			var secretKey = opt.GoogleClientSecret;
			var callback = WebUtility.UrlEncode(opt.CallbackUrl);

			app.Map("/googlelogin", b => {
				b.Run(async context => {
					if (!context.Request.Query.TryGetValue("returnurl", out var ret))
						ret = "/";

					ret = WebUtility.UrlEncode(ret);

					context.Response.Redirect($"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientID}&response_type=code&scope=openid%20email%20profile&redirect_uri=http:%2F%2Flocalhost:5000{callback}&state={ret}");
					await Task.CompletedTask;
				});
			});
			app.Map(opt.CallbackUrl, b => {
				b.Run(async context => {
					var code = context.Request.Query["code"];
					var state = context.Request.Query["state"];

					GoogleToken token = null;
					GoogleUserProfile profile = null;

					using (var httpClient = new HttpClient { BaseAddress = new Uri("https://www.googleapis.com") })
					{
						var requestUrl = $"oauth2/v4/token?code={code}&client_id={clientID}&client_secret={secretKey}&redirect_uri=http:%2F%2Flocalhost:5000{callback}&grant_type=authorization_code";

						var dict = new Dictionary<string, string> {
							{ "Content-Type", "application/x-www-form-urlencoded" }
						};
						var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
						var response = await httpClient.SendAsync(req);
						var s = await response.Content.ReadAsStringAsync();
						token = JsonConvert.DeserializeObject<GoogleToken>(s);
					}

					using (var httpClient = new HttpClient { BaseAddress = new Uri("https://www.googleapis.com") })
					{
						string url = $"oauth2/v1/userinfo?alt=json&access_token={token.AccessToken}";
						var response = await httpClient.GetAsync(url);
						var s = await response.Content.ReadAsStringAsync();
						profile = JsonConvert.DeserializeObject<GoogleUserProfile>(s);
					}

					ActionResult result = null;

					try
					{
						var helper = context.RequestServices.GetService<IGoogleAuthenticationHelper>();
						helper.InjectProperties(context.RequestServices);
						result = await helper.DoLogin(profile, state);
					}
					catch (Exception ex)
					{
						var res = context.RequestServices.GetService(typeof(IErrorResult)) as IErrorResult;
						var message = res?.OnError(ex) ?? ex.ToString().Replace(Environment.NewLine, "<br/>");
						result = new HtmlResult(message, "");
					}

					var ac = new AspNetCoreActionContext(context);
					await result.ExecuteResultAsync(ac);
				});
			});
		}
	}

	public class GoogleAuthenticationOptions
	{
		public string GoogleClientID { get; set; }
		public string GoogleClientSecret { get; set; }
		public string CallbackUrl { get; set; } = "/googlelogin-oidc";
	}

	internal class GoogleToken
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public long ExpiresIn { get; set; }

		[JsonProperty("id_token")]
		public string IdToken { get; set; }
	}
}
