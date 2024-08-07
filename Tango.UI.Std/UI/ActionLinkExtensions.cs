﻿using System;
using Tango.Html;

namespace Tango.UI
{
	public static class ActionLinkExtensions
	{
		public static void BackLink(this LayoutWriter w, Action<ATagAttributes> attrs = null, string title = null)
		{
			var url = w.Context.ReturnUrl.ContainsKey(0) ?
				w.Context.ReturnUrl[0] :
				w.Context.ReturnUrl.ContainsKey(1) ?
				w.Context.ReturnUrl[1] : null;
			if (url.IsEmpty()) return;

			if (title.IsEmpty()) title = w.Resources.Get("Common.Back");
			
			w.A(a => a.Class("actionbtn").Href(url).DataResult(0).OnClickRunHref().Data(Constants.ContainerNew, 1).Set(attrs), () => {
				w.Icon("back");
				w.Write(title);
			});
		}

		static string GetReturnUrl(ActionContext ctx)
		{
			return ctx.ReturnUrl.ContainsKey(0) ?
				ctx.ReturnUrl[0] :
				ctx.ReturnUrl.ContainsKey(1) ?
				ctx.ReturnUrl[1] : null;
		}

		public static void BackButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null, string title = null)
		{
			var url = GetReturnUrl(w.Context);
			if (url == null) return;

			if (title.IsEmpty()) title = w.Resources.Get("Common.Back");
			w.Button(a => a.Class("btn-primary").Data("href", url).DataResult(0).OnClickRunHref().Data(Constants.ContainerNew, 1).Set(attrs), title);
		}

		public static void BackButton(this LayoutWriter w, IViewElement form)
		{
			var title = w.Resources.Get(form.IsModal ? "Common.Close" : "Common.Back");
			string url = null;
			if (!form.IsModal)
			{
				url = GetReturnUrl(w.Context);
				if (url == null) return;
			}
			w.Button(a => {
				a.Class("btn-primary").DataResult(0).OnClickRunHref().Data(Constants.ContainerNew, 1);
				if (url != null) a.Data("href", url);
			}, title);
		}

		static ATagAttributes SetTarget(this ATagAttributes a, ActionLink link)
		{
			var url = link.Url;

			if (link.ChangeUrl || link.IsTargetBlank)
				a.Href(url);
			else
				a.Data("href", url);

			if (link.IsTargetBlank)
			{
				a.Target(Target._blank);
				return a;
			}

			if (url.IsEmpty())
				a.DataParm(link.Args);

			if(!link.Description.IsEmpty())
				a.Title(link.Description);

			foreach (var data in link.Data)
				a.Data(data.Key, data.Value);

			if (!link.CallbackUrl.IsEmpty())
				a.DataParm(Constants.ReturnUrl, link.CallbackUrl);

			if (!link.Event.IsEmpty())
				a.DataEvent(link.Event, link.EventReceiver);

			if (!link.Container.Type.IsEmpty())
			{
				a.Data(Constants.ContainerType, link.Container.Type);
				a.Data(Constants.ContainerPrefix, link.Container.Prefix.IsEmpty() ? $"{link.Service}_{link.Action}" : link.Container.Prefix);
				a.Data(Constants.ContainerNew, "1");
			}
			if (link.Container.Parms != null)
				foreach (var p in link.Container.Parms)
					a.DataParm("c-" + p.Key, p.Value);

			foreach (var r in link.References)
				a.DataRef(r);

			if (link.RequestMethod == "GET")
				if (link.ChangeUrl)
					a.OnClickRunHref();
				else
					a.OnClickRunEvent();
			else if (link.RequestMethod == "POST")
				a.OnClickPostEvent();
			else
				a.OnClick($"{link.RequestMethod}(this)");
			return a;
		}

		public static void ActionLink(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null, Action content = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			var title = link.Title.IsEmpty() ? w.Resources.Get("Common.Empty") : link.Title;
			if (!link.Enabled)
			{
				if (!link.HideDisabled)
				{
					if (content == null) w.Write(title); else content();
				}
			}
			else
			{
				w.A(a => a.Set(attrs).SetTarget(link), () => {
					if (content == null) w.Write(title); else content();
				});
			}
		}

		public static void ActionLink(this LayoutWriter w, ActionLink link, Action<ATagAttributes> attrs = null)
		{
			var title = link.Title.IsEmpty() ? w.Resources.Get("Common.Empty") : link.Title;

			if (!link.Enabled)
			{
				if (!link.HideDisabled)
					w.Write(title);
			}
			else
				w.A(a => a.Set(attrs).SetTarget(link), title);
		}

		public static void ActionImage(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Set(attrs).SetTarget(link), () => w.Icon(link.ImageSrc, link.Title));
		}

        public static void ActionImageLink(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			ActionLink link = new ActionLink(w.Context);
			link.WithHideDisabled(true);
			urlAttributes(link);
			if (link.Enabled)
			{
				w.A(a => a.Set(attrs).Href(link).SetTarget(link), () => {
					w.Icon(link.ImageSrc, link.ImageTip, link.ImageColor);
					w.Write(link.Title);
				});
			}
			else if (!link.HideDisabled)
			{
				w.Icon(link.ImageSrc, link.ImageTip, link.ImageColor);
				w.Write(link.Title);
			}
		}

		public static void ActionTextButton(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Class("actionbtn").Set(attrs).SetTarget(link), link.Title);
		}

		public static void ActionImageButton(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Class("actionimg").Set(attrs).SetTarget(link).Title(link.Title), () => w.Icon(link.ImageSrc, tip: link.ImageTip ?? link.Title, color: link.ImageColor));
		}

		public static void ActionButton(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<LayoutWriter> content, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Class("actionbtn").Set(attrs).SetTarget(link), () => content(w));
		}

		public static void ActionImageTextButton(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Class("actionbtn").Set(attrs).SetTarget(link), () => {
					w.Icon(link.ImageSrc, tip: link.ImageTip, color: link.ImageColor);
					w.Write(link.Title);
				});
		}

		public static void AjaxActionLink(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.AjaxActionLink(attrs, () => w.Write(title));
		}

		public static void AjaxActionSpan(this LayoutWriter w, Action<TagAttributes> attrs, string title)
		{
			w.AjaxActionSpan(attrs, () => w.Write(title));
		}

		public static void PostBackLink(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.PostBackLink(attrs, () => w.Write(title));
		}

		public static void PostBackSpan(this LayoutWriter w, Action<TagAttributes> attrs, string title)
		{
			w.PostBackSpan(attrs, () => w.Write(title));
		}

		public static void AjaxActionLink(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Set(attrs).OnClick("ajaxUtils.runHrefWithApiResponse(this); return false;"), content);
		}

		public static void AjaxActionSpan(this LayoutWriter w, Action<TagAttributes> attrs, Action content)
		{
			w.Span(a => a.Set(attrs).OnClick("ajaxUtils.runHrefWithApiResponse(this); return false;"), content);
		}

		public static void PostBackLink(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Set(attrs).OnClick($"ajaxUtils.postEventFromElementWithApiResponse(this)"), content);
		}

		public static void PostBackSpan(this LayoutWriter w, Action<TagAttributes> attrs, Action content)
		{
			w.Span(a => a.Set(attrs).OnClick($"ajaxUtils.postEventFromElementWithApiResponse(this)"), content);
		}

		public static void PostBackButton(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Class("actionbtn").Set(attrs).OnClick($"ajaxUtils.postEventFromElementWithApiResponse(this)"), content);
		}

		public static void PostBackButton(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.PostBackButton(attrs, () => w.Write(title));
		}

		public static void CopyToClipboardButton(this LayoutWriter w, Action<ATagAttributes> attrs, string contentElementId, Action content)
		{
			//var el = w.GetID(contentElementId);
			w.A(a => a.Class("actionbtn").Set(attrs).OnClick($"commonUtils.copyToClipboard('{contentElementId}');"), content);
		}

		public static void ExternalImageLink(this LayoutWriter w, string href)
		{
			w.ExternalImageLink(null, href);
		}

		public static void ExternalImageLink(this LayoutWriter w, Action<ATagAttributes> attrs, string href, string image = "external-link")
		{
			if (href.IsEmpty()) return;
			if (!href.StartsWith("http")) href = "http://" + href;
			w.A(a => a.Href(href).Target(Target._blank).Set(attrs), () => w.Icon(image));
		}

		public static void ExternalLink(this LayoutWriter w, string href, string title = null)
		{
			w.ExternalLink(null, href, title ?? href);
		}

		public static void ExternalLink(this LayoutWriter w, Action<ATagAttributes> attrs, string href, string title)
		{
			if (href.IsEmpty()) return;
			if (!href.StartsWith("http")) href = "http://" + href;
			w.A(a => a.Href(href).Target(Target._blank).Set(attrs), title);
		}
	}
}
 