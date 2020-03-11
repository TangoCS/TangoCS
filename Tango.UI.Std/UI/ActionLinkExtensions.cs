using System;
using Tango.Html;

namespace Tango.UI
{
	public static class ActionLinkExtensions
	{
		//public static void SubmitButton(this LayoutWriter w)
		//{
		//	w.SubmitButton(a => a.DataResult(1).OnClick("return ajaxUtils.processResult(this)"), w.Resources.Get("Common.OK"));
		//}

		public static void SubmitDeleteButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null)
		{
			w.SubmitButton(attrs, w.Resources.Get("Common.Delete"));
		}

		public static void SubmitContinueButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null)
		{
			w.SubmitButton(attrs, w.Resources.Get("Common.Continue"));
		}

		public static void BackLink(this LayoutWriter w, Action<ATagAttributes> attrs = null, string title = null)
		{
			var url = w.Context.ReturnUrl.ContainsKey(0) ?
				w.Context.ReturnUrl[0] :
				w.Context.ReturnUrl.ContainsKey(1) ?
				w.Context.ReturnUrl[1] : null;
			if (url == null) return;

			if (title.IsEmpty()) title = w.Resources.Get("Common.Back");
			
			w.A(a => a.Class("actionbtn").Href(url).OnClickRunHref().Set(attrs), () => {
				w.Icon("back");
				w.Write(title);
			});
		}

		public static void BackButton(this LayoutWriter w, Action<ButtonTagAttributes> attrs = null, string title = null)
		{
			var url = w.Context.ReturnUrl.ContainsKey(0) ?
				w.Context.ReturnUrl[0] :
				w.Context.ReturnUrl.ContainsKey(1) ?
				w.Context.ReturnUrl[1] : null;
			if (url == null) return;

			if (title.IsEmpty()) title = w.Resources.Get("Common.Back");
			w.Button(a => a.Class("btn-primary").Data("href", url).DataResult(0).OnClickRunHref().Set(attrs), title);
		}

		static ATagAttributes SetTarget(this ATagAttributes a, ActionLink link)
		{
			if (link.ChangeUrl || link.IsTargetBlank)
				a.Href(link);
			else
				a.Data("href", link);

			if (link.IsTargetBlank)
			{
				a.Target(Target._blank);
				return a;
			}

			if (link.Service.IsEmpty())
				a.DataParm(link.Args);

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

			foreach (var r in link.References)
				a.DataRef(r);

			if (link.RequestMethod == "GET")
				if (link.ChangeUrl)
					a.OnClickRunHref();
				else
					a.OnClickRunEvent();
			else if (link.RequestMethod == "POST")
				a.OnClickPostEvent();
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
				w.A(a => a.Set(attrs).SetTarget(link), () => w.Icon(link.Image, link.Title));
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
					w.Icon(link.Image);
					w.Write(link.Title);
				});
			}
			else if (!link.HideDisabled)
			{
				w.Icon(link.Image);
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
				w.A(a => a.Class("actionimg").Set(attrs).SetTarget(link), () => w.Icon(link.Image));
		}

		public static void ActionImageTextButton(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null)
		{
			if (urlAttributes == null) return;
			var link = new ActionLink(w.Context);
			urlAttributes(link);
			if (link.Enabled)
				w.A(a => a.Class("actionbtn").Set(attrs).SetTarget(link), () => {
					w.Icon(link.Image);
					w.Write(link.Title);
				});
		}

		public static void AjaxActionLink(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.AjaxActionLink(attrs, () => w.Write(title));
		}

		public static void PostBackLink(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.PostBackLink(attrs, () => w.Write(title));
		}

		public static void AjaxActionLink(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Set(attrs).OnClickRunHref(), content);
		}

		public static void PostBackLink(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Set(attrs).OnClick($"ajaxUtils.postEventFromElementWithApiResponse(this)"), content);
		}

		public static void PostBackButton(this LayoutWriter w, Action<ATagAttributes> attrs, Action content)
		{
			w.A(a => a.Class("actionbtn").Set(attrs).OnClick($"ajaxUtils.postEventFromElementWithApiResponse(this)"), content);
		}

		public static void PostBackButton(this LayoutWriter w, Action<ATagAttributes> attrs, string title)
		{
			w.PostBackButton(attrs, () => w.Write(title));
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

		public static void ExternalLink(this LayoutWriter w, string href)
		{
			w.ExternalLink(null, href);
		}

		public static void ExternalLink(this LayoutWriter w, Action<ATagAttributes> attrs, string href)
		{
			if (href.IsEmpty()) return;
			var title = href;
			if (!href.StartsWith("http")) href = "http://" + href;
			w.A(a => a.Href(href).Target(Target._blank).Set(attrs), title);
		}
	}
}
 