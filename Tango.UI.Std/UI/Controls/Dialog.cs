using System;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class DialogFormContainer : ViewContainer
	{
		protected virtual string ContainerWidth => "";
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
			w.DialogControl(DialogExtensions.DialogContainerAttrs(w.Context, Type, w.IDPrefix, ContainerWidth), () => {
					w.AjaxForm("form", a => a.DataResultPostponed(1), () => {
						w.DialogControlBody(null, () => { }, null, null, () => { });
						w.Hidden(Constants.ReturnUrl, Context.ReturnUrl.Get(1));
					});
				});
			});
		}
	}
	public class WideDialogFormContainer : DialogFormContainer
	{
		protected override string ContainerWidth => "width: 70%;";
	}
	public class NoCloseIconDialogFormContainer : DialogFormContainer
	{
		public override void Render(ApiResponse response)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				w.DialogControl(DialogExtensions.DialogContainerAttrs(w.Context, Type, w.IDPrefix), () => {
					w.AjaxForm("form", a => a.DataResultPostponed(1), () => {
						w.DialogControlBody(null, () => { }, null, null, () => { }, false);
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
		internal static Action<TagAttributes> DialogContainerAttrs(ActionContext ctx, string containerType, string prefix, params string[] styles)
		{
			void attrs(TagAttributes a)
			{				
				a.Style(styles.Join(" ").ToString());

				a.DataHref(ctx.BaseUrl().Url).DataContainer(containerType, prefix);
				var parent = ctx.GetArg("c-parent");
				if (!parent.IsEmpty())
					a.Data("c-parent", parent);
			}
			return attrs;
		}

		internal static void DialogControl(this LayoutWriter w, Action<TagAttributes> attrs, Action content)
		{
			w.Div(a => a.ID("dialog").Class("modal-dialog").Role("dialog").Aria("modal", "true").DataCtrl("dialog").DataResultHandler().Set(attrs), () => content());
		}		
		internal static void DialogControlBody(this LayoutWriter w, Action title, Action toolbar, Action body, Action bottomToolbar, Action footer, bool showCloseIcon = true)
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
		public static void AddYesNoDialogWidget(this ApiResponse response, string title, Action<LayoutWriter> content, string IDPrefix = null, bool warningMode = false, Action<ButtonTagAttributes> btnAttrs = null, Func<ActionResult> action = null)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				if (IDPrefix != null)
					w.PushPrefix(IDPrefix);
				w.DialogControl(DialogContainerAttrs(w.Context, "", IDPrefix), () => {
					w.AjaxForm("form", a => a.DataResult(1), () => {
						w.DialogControlBody(() => w.Write(title), null, () => content(w), null, () => {
							w.ButtonsBarRight(() => {
								if (!warningMode)
									w.SubmitButton(a => {
										if (!w.Context.ResponseType.IsEmpty())
											a.Data("responsetype", w.Context.ResponseType);
										a.Set(btnAttrs);
										if(action != null) a.DataEvent(action);
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
		public static void AddYesNoDialogWidget2(this ApiResponse response, string title, Action<LayoutWriter> content, Func<ActionResult> action, string IDPrefix = null, Action<ButtonTagAttributes> btnAttrs = null, string dataKey = null, string dataValue = null, string value = null)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				if (IDPrefix != null)
					w.PushPrefix(IDPrefix);
				w.DialogControl(DialogContainerAttrs(w.Context, "", IDPrefix), () => {
					w.AjaxForm("form", a => a.DataResult(1), () => {
						w.DialogControlBody(() => w.Write(title), null, () => content(w), null, () => {
							w.ButtonsBarRight(() => {
								w.SubmitButton(a => a.Set(btnAttrs).DataEvent(action).Data(dataKey, dataValue).Value(value), "Продолжить");
								w.Button(a => a.Aria("label", "Close").DataResult(0).OnClick("ajaxUtils.processResult(this)"), "Нет");
							});
						});
					});
				});
				if (IDPrefix != null)
					w.PopPrefix();
			});
		}
		public static void AddOKDialogWidget(this ApiResponse response, string title, Action<LayoutWriter> content, string IDPrefix = null)
		{
			response.AddAdjacentWidget(null, "dialog", AdjacentHTMLPosition.AfterBegin, w => {
				if (IDPrefix != null)
					w.PushPrefix(IDPrefix);
				w.DialogControl(DialogContainerAttrs(w.Context, "", IDPrefix), () => {
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

		public static ActionLink AsDialog<T>(this ActionLink link, string dialogPrefix = null)
		{
			return link.InContainer(typeof(T), dialogPrefix).KeepTheSameUrl();
		}
		public static ActionLink AsDialog(this ActionLink link, string dialogPrefix = null)
		{
			return link.InContainer(typeof(DialogFormContainer), dialogPrefix).KeepTheSameUrl();
		}

		public static ActionLink AsNoCloseIconDialog(this ActionLink link, string dialogPrefix = null)
		{
			return link.InContainer(typeof(NoCloseIconDialogFormContainer), dialogPrefix).KeepTheSameUrl();
		}

		public static ActionLink AsConsoleDialog(this ActionLink link, string dialogPrefix = null)
		{
			return link.InContainer(typeof(ConsoleContainer), dialogPrefix).KeepTheSameUrl();
		}

		public static ActionLink AsDialog(this ActionLink link, Action<ApiResponse> serverEvent, string dialogPrefix = null)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			return link.InContainer(typeof(DialogFormContainer), dialogPrefix).KeepTheSameUrl().RunEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static ActionLink AsDialogPost(this ActionLink link, Action<ApiResponse> serverEvent, string dialogPrefix = null)
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			return link.InContainer(typeof(DialogFormContainer), dialogPrefix).KeepTheSameUrl().PostEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static TagAttributes<T> AsDialog<T>(this TagAttributes<T> a, Action<ApiResponse> serverEvent, string dialogPrefix = null)
			where T : TagAttributes<T>
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			var res = a.DataContainer(typeof(DialogFormContainer), dialogPrefix);

			return res.OnClickRunEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static TagAttributes<T> AsDialogPost<T>(this TagAttributes<T> a, Action<ApiResponse> serverEvent, string dialogPrefix = null)
			where T : TagAttributes<T>
		{
			var el = serverEvent.Target as ViewElement;
			if (el == null) throw new InvalidCastException("Invalid class type for serverEvent.Target; must be of type ViewElement");

			if (dialogPrefix == null)
				dialogPrefix = el.ClientID;

			var res = a.DataContainer(typeof(DialogFormContainer), dialogPrefix);

			return res.OnClickPostEvent(serverEvent.Method.Name, el.ClientID);
		}

		public static void ErrorPlaceholder(this HtmlWriter w, bool reloadPageOnError = false)
		{
			w.Div(a => a.ID("container_err").Class("modal-dialog").Role("dialog").DataCtrl("dialog").DataResultHandler().Data("reuse", 1), () => {
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
