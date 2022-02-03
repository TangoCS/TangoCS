using System;
using System.Collections.Generic;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class DialogFormContainer : ViewContainer
	{
		public DialogFormContainer()
		{
			Mapping.Add("contentbody", "body");
			Mapping.Add("contenttitle", "title");
			Mapping.Add("buttonsbar", "footer");
			Mapping.Add("form", "body");
			Mapping.Add("title", "title");
			Mapping.Add("contenttoolbar", "toolbar");
		}
		public override void Render(ApiResponse response)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				var options = DialogOptions.FromParms(w.Context);
				w.DialogControl(a => a.DialogContainerAttrs(w.Context, Type, w.IDPrefix, options), () => {
					w.AjaxForm("form", a => a.DataResultPostponed(1), () => {
						w.DialogControlBody(null, () => { }, null, null, () => { }, options.ShowCloseIcon);
						w.Hidden(Constants.ReturnUrl, Context.ReturnUrl.Get(1));
					});
				});
			});
		}
	}

	public class DialogContainer : ViewContainer
	{
		public string Class { get; set; }

		public DialogContainer()
		{
			Mapping.Add("contentbody", $"body");
			Mapping.Add("contenttitle", $"title");
			Mapping.Add("title", $"title");
		}
		public override void Render(ApiResponse response)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				w.DialogControl(a => a.Class(Class), () => w.DialogControlBody(null, null, null, null, null));
			});
		}
	}

	public class ConsoleContainer : DialogContainer
	{
		public ConsoleContainer()
		{
			Class = "console";
		}

		public override void Render(ApiResponse response)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				w.DialogControl(a => a.Class(Class).Data("reuse", "1"), () => w.DialogControlBody(null, null, null, null, null));
			});
		}
	}

	public static class DialogExtensions
	{
		public static TagAttributes DialogContainerAttrs(this TagAttributes a, ActionContext ctx, string type, string prefix, DialogOptions options)
		{
			if (options == null)
				options = new DialogOptions();
			if (options.ModalBodyPadding)
				a.Class("modalbodypadding");
			if (options.Height == DialogHeight.Height100)
				a.Class("height100");
			a.Style("width:" + options.Width);
			a.Data("c-changeloc", "false");
			if (options.ShowOnRender)
				a.Data("showonrender");
			a.DataHref(ctx.BaseUrl().Url);
			a.DataNewContainer(type, prefix);
			var parent = ctx.GetArg("c-parent");
			if (!parent.IsEmpty())
				a.Data("c-parent", parent);
			return a;
		}

		public static void DialogControl(this LayoutWriter w, Action<TagAttributes> attrs, Action content)
		{
			w.Div(a => a.ID("dialog").Class("modal-dialog").Role("dialog").Aria("modal", "true").DataCtrl("dialog").DataResultHandler().Set(attrs), () => content());
		}

		public static void DialogControlBody(this LayoutWriter w, Action title, Action toolbar, Action body, Action bottomToolbar, Action footer, bool showCloseIcon = true)
		{
			w.Div(a => a.Class("modal-container"), () => {
				w.Div(a => a.Class("modal-header"), () => {
					w.H3(a => a.ID("title").Class("modal-title"), title);
					if (showCloseIcon)
					{
						w.Button(a => a.Class("close").Aria("label", "Close").DataResult(0).OnClick("ajaxUtils.processResult(this)"), () => {
							w.Span(a => a.Aria("hidden", "true"), () => w.Icon("close"));
						});
					}
				});
				if (toolbar != null)
					w.Div(a => a.ID("toolbar").Class("modal-toolbar"), toolbar);
				w.Div(a => a.ID("body").Class("modal-body"), body);
				if (bottomToolbar != null)
					w.Div(a => a.ID("bottomtoolbar").Class("modal-bottomtoolbar"), bottomToolbar);
				if (footer != null)
					w.Div(a => a.ID("footer").Class("modal-footer"), footer);
			});
		}

		///TODO. Сделать автоматическое прокидываение всех полей из контекста.
		public static void AddYesNoDialogWidget(this ApiResponse response, string title, Action<LayoutWriter> content,
			string IDPrefix = null, DialogOptions options = null, 
			bool warningMode = false,
			Action<ButtonTagAttributes> btnAttrs = null,
			Func<ActionResult> action = null)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				if (IDPrefix != null)
					w.PushPrefix(IDPrefix);

				w.DialogControl(a => a.DialogContainerAttrs(w.Context, "", IDPrefix, options), () => {
					w.AjaxForm("form", a => a.DataResult(1), () => {
						w.DialogControlBody(() => w.Write(title), null, () => content(w), null, () => {
							w.ButtonsBarRight(() => {
								if (!warningMode)
									w.SubmitButton(a => {
										if (!w.Context.ResponseType.IsEmpty())
											a.Data("responsetype", w.Context.ResponseType);
										a.Set(btnAttrs);
										if (action != null) a.DataEvent(action);
									}, "Да");
								w.Button(a => a.Aria("label", "Close").DataResult(0).OnClick("ajaxUtils.processResult(this)"), warningMode ? "Назад" : "Нет");
							});
						});
					});
				});
				if (IDPrefix != null)
					w.PopPrefix();
			});
		}
		

		public static void AddOKDialogWidget(this ApiResponse response, string title, Action<LayoutWriter> content, 
			string IDPrefix = null, DialogOptions options = null)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				if (IDPrefix != null)
					w.PushPrefix(IDPrefix);
				w.DialogControl(a => a.DialogContainerAttrs(w.Context, "", IDPrefix, options), () => {
					w.AjaxForm("form", a => a.DataResult(1), () => {
						w.DialogControlBody(() => w.Write(title), null, () => content(w), null, () => {
							w.Div(a => a.Style("width:100%; text-align:center"), () => {
								w.Button(a => a.Aria("label", "Close").DataResult(0).OnClick("ajaxUtils.processResult(this)"), "ОК");
							});
						});
					});
				});
				if (IDPrefix != null)
					w.PopPrefix();
			});
		}

		public static ActionLink AsDialog<T>(this ActionLink link, string dialogPrefix = null, DialogOptions options = null)
		{
			return link.InContainer(typeof(T), dialogPrefix, options?.ToParms()).KeepTheSameUrl();
		}
		public static ActionLink AsDialog(this ActionLink link, string dialogPrefix = null, DialogOptions options = null)
		{
			return link.InContainer(typeof(DialogFormContainer), dialogPrefix, options?.ToParms()).KeepTheSameUrl();
		}

		public static ActionLink AsConsoleDialog(this ActionLink link, string dialogPrefix = null, DialogOptions options = null)
		{
			return link.InContainer(typeof(ConsoleContainer), dialogPrefix, options?.ToParms()).KeepTheSameUrl();
		}

		public static ActionLink AsDialog(this ActionLink link, Action<ApiResponse> serverEvent, string dialogPrefix = null, DialogOptions options = null)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			return link.InContainer(typeof(DialogFormContainer), dialogPrefix, options?.ToParms()).KeepTheSameUrl().RunEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static ActionLink AsDialogPost(this ActionLink link, Action<ApiResponse> serverEvent, string dialogPrefix = null, DialogOptions options = null)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			return link.InContainer(typeof(DialogFormContainer), dialogPrefix, options?.ToParms()).KeepTheSameUrl().PostEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static TagAttributes<T> AsDialog<T>(this TagAttributes<T> a, Action<ApiResponse> serverEvent, string dialogPrefix = null)
			where T : TagAttributes<T>
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			var res = a.DataNewContainer(typeof(DialogFormContainer), dialogPrefix);

			return res.OnClickRunEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static TagAttributes<T> AsDialogPost<T>(this TagAttributes<T> a, Action<ApiResponse> serverEvent, string dialogPrefix = null)
			where T : TagAttributes<T>
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			var res = a.DataNewContainer(typeof(DialogFormContainer), dialogPrefix);

			return res.OnClickPostEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static void ErrorPlaceholder(this HtmlWriter w, bool reloadPageOnError = false)
		{
			w.Div(a => a.ID("container_err").Class("modal-dialog").Role("dialog").DataCtrl("dialog").DataResultHandler().Data("showonrender").Data("reuse", 1), () => {
				w.Div(a => a.Class("modal-container"), () => {
					w.H3(a => a.ID("container_err_title"));
					w.Div(a => a.ID("container_err_body"));
					w.Div(() => {
						w.Button(a => a.Aria("label", "Close").DataResult(0).OnClick(reloadPageOnError ? "location.reload()" : "ajaxUtils.processResult(this)"));
					});
				});
			});
		}

		public static void ModalOverlay(this HtmlWriter w)
		{
			w.Div(a => a.Class("md-overlay"));
		}

		public static TagAttributes<T> CallbackToCurrent<T>(this TagAttributes<T> a, ActionContext context)
			where T : TagAttributes<T>
		{
			return a.DataParm(Constants.ReturnUrl, context.BaseUrl().Url);
		}
	}

	public class DialogOptions
	{
		public Unit Width { get; set; } = new Unit(600, UnitType.Pixel);
		public DialogHeight Height { get; set; } = DialogHeight.Auto;
		public bool ModalBodyPadding { get; set; } = true;
		public bool ShowCloseIcon { get; set; } = true;
		public bool ShowOnRender { get; set; } = true;

		public Dictionary<string, string> ToParms()
		{
			return new Dictionary<string, string> {
				["Width"] = Width.ToString(),
				["Height"] = ((int)Height).ToString(),
				["ModalBodyPadding"] = ModalBodyPadding.ToString(),
				["ShowCloseIcon"] = ShowCloseIcon.ToString()
			};
		}

		public static DialogOptions FromParms(ActionContext context)
		{
			return new DialogOptions
			{
				ModalBodyPadding = context.GetBoolArg("c-modalbodypadding", true),
				ShowCloseIcon = context.GetBoolArg("c-showcloseicon", true),
				Width = new Unit(context.GetArg("c-width", "600px")),
				Height = (DialogHeight)context.GetIntArg("c-height", 0),
			};
		}
	}

	public enum DialogHeight
	{
		Auto = 0,
		Height100 = 100
	}

	public enum ModalEffect
	{
		FadeInAndScale = 1,
		SlideInRight,
		SlideInBottom,
		Newspaper,
		Fall,
		SideFall,
		StickyUp,
		FlipHorizontal,
		FlipVertical,
		Sign,
		SuperScaled,
		JustMe,
		Slit,
		RotateBottom,
		RotateInLeft,
		Blur, // no IE support
		LetMeIn,
		MakeWay,
		SlipFromTop
	}
}
