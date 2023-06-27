using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
	public interface IViewPagePart : IViewElement, IWithCheckAccess, IContainerItem
	{
		void OnLoad(ApiResponse response);
		void SetArgGroup(string groupName, ApiResponse response);
	}

	public abstract class ViewPagePart : ViewRootElement, IViewPagePart
	{
		[Inject]
		public IRequestLoggerProvider LoggerProvider { get; set; }

		//public override bool UsePropertyInjection => true;
		public virtual ViewContainer GetContainer() => new DefaultContainer();

		public override ActionResult Execute()
		{
			OnInit();
			//AfterInit();

			var r = Context.EventReceiver;
			var recipient = r == null || r == ID?.ToLower() || r == ClientID?.ToLower() ? this : Context.EventReceivers.First(o => o.ClientID == r);
			var e = Context.Event.IsEmpty() ? "onload" : Context.Event;

			var el = recipient;
			do
			{
				el.IsLazyLoad = false;
				el = el.ParentElement;
			}
			while (el != null);

			this.RunOnEvent();

			return recipient.RunEvent(e);
		}

		public virtual void OnFirstLoad(ApiResponse response) { }

		protected void RegisterRealtimeConnection(ApiResponse response, string key)
		{
			response.AddClientAction("tangohub", "init", new { service = Context.Service, action = Context.Action, key, prefix = ClientID });
		}

		public abstract void OnLoad(ApiResponse response);

		public virtual bool CheckAccess(MethodInfo method)
		{
			var anon = method.DeclaringType.GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon != null) return true;

			var ac = Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;
			if (ac == null) return false;

			var so = method.DeclaringType.GetCustomAttribute<SecurableObjectAttribute>();
			var soname = so != null ? so.Name : (Context.Service + "." + Context.Action);

			return ac.Check(soname);
		}

		public ActionResult OnNoAccess()
		{
			if (Context.IsCurrentUserAuthenticated())
				return new HttpResult { StatusCode = HttpStatusCode.Forbidden };
			else
				return new ChallengeResult();
		}

		Guid? formID;
		public virtual Guid GetFormID()
		{
			if (!formID.HasValue)
			{
				var formidAttr = GetType().GetCustomAttribute(typeof(FormIDAttribute)) as FormIDAttribute;
				if (formidAttr != null)
					formID = formidAttr.Guid;
				else
				{
					using (MD5 hasher = MD5.Create())
						formID = new Guid(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Context.Service + "." + Context.Action)));
				}
			}
			return formID.Value;
		}

		public void SetArgGroup(string groupName, ApiResponse response)
		{
			var names = new List<string>();

			if (ElementArgNames != null)
				names.AddRange(ElementArgNames);

			void addChidrenNames(IViewElement parentEl)
			{
				foreach (var child in parentEl.ChildElements)
				{
					if (child is IViewPagePart vpp)
					{
						if (vpp.ElementArgNames != null)
							names.AddRange(vpp.ElementArgNames);
						addChidrenNames(vpp);
					}
				}
			}

			addChidrenNames(this);

			response.SetArgGroup(groupName, names);
		}
	}

	public abstract class AbstractViewPage : ViewRootElement
	{
		[Inject]
		public IIdentity User { get; set; }

		protected abstract void RenderContent(HtmlWriter w);

		public override ActionResult Execute()
		{
			if (!CheckAccess()) return new ChallengeResult();

			OnInit();

			HtmlWriter w = new HtmlWriter();
			RenderContent(w);

			return new HtmlResult(w.ToString());
		}

		public virtual void OnLoadContent(ApiResponse response)
		{
		}

		public virtual void OnUnloadContent(ApiResponse response)
		{
		}

		bool CheckAccess()
		{
			var anon = GetType().GetCustomAttribute<AllowAnonymousAttribute>();
			if (anon != null) return true;

			return User != null && User.IsAuthenticated;
		}


		public virtual ActionTarget DefaultView => null;
	}

	public abstract class ViewPage : AbstractViewPage
	{
		[Inject]
		public ITypeActivatorCache Cache { get; set; }

		protected override void RenderContent(HtmlWriter w)
		{
			//byte[] token = new byte[32];
			//RandomNumberGenerator.Fill(token);

			//var tokenString = token.ByteArrayToHex();

			//void headAttr(TagAttributes a)
			//{
			//	a.ID("head").Data("x-csrf-token", tokenString).Data("page", GetType().Name.ToLower());
			//}

			w.DocType();
			w.Html(() => {
				w.Head(a => a.ID("head").Data("page", GetType().Name.ToLower()), () => {
					w.HeadTitle(a => a.ID("title"));
					w.HeadMeta(a => a.HttpEquiv("content-type").Content("text/html; charset=utf-8"));
					var r = DefaultView?.Resolve(Context);
					w.HeadMeta(a => a.ID(Constants.MetaHome).Data("href", "/").Data("alias", r?.Result.ToString()));
					w.HeadMeta(a => a.ID(Constants.MetaCurrent));
					w.HeadMeta(a => a.Name("viewport").Content("width=device-width"));
					HeadContent(w);
				});
				w.Body(a => a.ID("body"), () => {
					Body(w);
				});
			});

			if (w.AllowModify)
				RenderView(w);
		}

		void RenderView(HtmlWriter w)
		{
			Context.IsFirstLoad = true;
			Context.AddContainer = true;

			if (Context.Service.IsEmpty() && DefaultView != null)
			{
				Context.Service = DefaultView.Service;
				Context.Action = DefaultView.Action;
			}

			(Type type, IActionInvoker invoker) view = (null, null);

			if (!Context.Service.IsEmpty())
				view = Cache.Get(Context.Service + "." + Context.Action) ?? (null, null);

			ActionResult result;

			if (!Context.Service.IsEmpty())
				result = view.invoker?.Invoke(Context, view.type) ?? new HttpResult { StatusCode = HttpStatusCode.NotFound };
			else
			{
				var res = new ApiResult();
				result = res;
				res.ApiResponse.ReplaceWidget("container", w1 => w1.Article(a => a.ID("container")));
			}

			if (result is ApiResult ajax)
			{
				var pageParts = new ApiResponse();
				OnLoadContent(pageParts);
				ajax.ApiResponse.Insert(pageParts);
				ajax.ApiResponse.ApplyTo(Context, w);

				var sw = new HtmlWriter();
				sw.Script(null, () => {
					sw.Write("document.addEventListener('DOMContentLoaded', onLoad);\n");
					sw.Write("function onLoad() {\n");
					sw.Write("const data = " + JsonConvert.SerializeObject(ajax.ApiResponse.Data, Json.StdSettings) + ";\n");
					sw.Write("const ctrls = ajaxUtils.processControls(document, data.ctrl);\n");
					sw.Write("ajaxUtils.postProcessControls(ctrls);\n");
					sw.Write("ajaxUtils.state.loc.arggroups = data.arggroups;\n");

					if (ajax.ApiResponse.ClientActions.Count > 0)
					{
						foreach (var ca in ajax.ApiResponse.ClientActions)
						{
							var call = ca.Service;

							foreach (var ch in ca.CallChain)
							{
								var method = ch.Method == "apply" ? "" : $".{ch.Method}";
								if (ch.Args != null)
								{
									var args = JsonConvert.SerializeObject(ch.Args, Json.StdSettings);
									call += $"{method}({args})";
								}
								else
									call += $"{method}()";
							};

							call += ";";

							sw.Write(call);
							sw.Write("\n");
						}
					}
					sw.Write("if (data.error) {\n");
					sw.Write("ajaxUtils.showError(localization.resources.title.systemError, data.error, 'err');\n");
					sw.Write("}\n");
					sw.Write("}\n");
				});
				w.AddAdjacentWidget("body", AdjacentHTMLPosition.BeforeEnd, sw);


			}
		}
		
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