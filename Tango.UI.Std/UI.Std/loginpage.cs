using Tango;
using Tango.Data;
using Tango.Html;
using Tango.Identity;
using Tango.Localization;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	[AllowAnonymous]
	public class loginpage : ViewPage
	{
		[Inject]
		public IPersistentSettings Settings { get; set; }

		[Inject]
		protected IIdentityOptions Options { get; set; }

		[Inject]
		protected IVersionProvider VersionProvider { get; set; }

		[Inject]
		protected ILanguage Language { get; set; }

		protected override void HeadContent(HtmlWriter w)
		{
			w.Script(GlobalSettings.JSPath + "browsercheck.js");
			w.Script(GlobalSettings.JSPath + "bundle/vendors.bundle.js");
			w.Script(GlobalSettings.JSPath + "bundle/tango.bundle.js");
			w.Script(GlobalSettings.JSPath + $"tango/tango.{Language.Current.Code}.js?");

			w.HeadLinkCss("/css/login.css");
			w.HeadLinkCss("/css/core.css");
			w.HeadLinkCss("/css/home2.css");
			w.HeadLinkCss("/css/app.css");
			w.HeadLinkCss("/css/components.css");
			w.HeadLinkCss("/css/modaleffects.css");
			w.HeadLinkCss(GlobalSettings.JSPath + "contextmenu/contextmenu.css");
			var theme = Settings.Get("theme");
			if (!theme.IsEmpty()) w.HeadLinkCss("/themes/" + theme.ToLower() + ".css");
		}

		protected override void Body(HtmlWriter w)
		{
			var title = "";
			if (!Resources.TryGet("loginpagetitle", out title))
				title = Resources.Get("systemname");
			
			w.Div(a => a.ID("topmessagecontainer"), () => w.Span(a => a.ID("topmessage"), Resources.Get("Common.Wait")));
			w.Main(a => a.ID("main"), () => {
				w.Header(a => a.ID("header").Class("login-header"), () => {
					w.Div(a => a.Class("header-logo"), () => w.A(a => a.Class("logo").Href("/")));
					w.Div(a => a.Class("header-title"), () => w.H1(title));

					w.Ul(a => a.Class("header-buttons right"), () => {
						if (Settings.GetBool("canchangedb"))
						{
							w.Li(a => a.ID("header-db"), () => {
							});
						}
						if (VersionProvider != null)
						{
							w.Li(() => w.Span(() => {
								var v = VersionProvider.Version;
								w.Write($"v. {v.Major}.{v.Minor}.{v.Build}");
							}));
						}
					});
				});
				w.Article(a => a.ID("container"), () => {
					w.Div(a => a.Class("login-main"), () => {
						w.Div(a => a.ID("content").Class("login-card"), () => {
						});
					});
				});
			});

			w.ErrorPlaceholder(Resources);
			w.ModalOverlay();
		}

		public override void OnLoadContent(ApiResponse response)
		{
			if (!Context.IsFirstLoad) return;
			if (!Settings.GetBool("canchangedb")) return;

			response.ReplaceWidget("header-db", w => {
				w.Li(a => a.ID("header-db"), () => {
					w.Span(() => {
						w.Icon("database");
						var conn = Context.PersistentArgs.Get("conn") ?? Context.GetArg("conn");
						w.Write(conn ?? ConnectionManager.DefaultConnection);
					});

					w.DropDownForElement("header-db", () => {
						w.Div(() => {
							foreach (var cs in ConnectionManager.ConnectionStrings)
							{
								w.ActionLink(al => al.To<ConnectionController>("changeconnection").WithTitle(cs.Key).WithArg("newconn", cs.Key));
							}
						});
					});
				});
			});
		}
	}
}