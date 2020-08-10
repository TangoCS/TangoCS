using System;
using Tango.Html;

namespace Tango.UI
{
	public static class PopupExtensions
	{
		public static void PopupForElement(this LayoutWriter w, string id, Action<TagAttributes> attrs = null, Action content = null, PopupOptions options = null)
		{
			w.Div(a => a.ID("popup-" + id).Set(attrs).Class("iw-contextMenu").DataRef(id), content);
			w.BindPopup(id, options);
		}

		public static void BindPopup(this LayoutWriter w, string elementid, PopupOptions options = null)
		{
			w.BindPopup(elementid, "popup-" + elementid, options);
		}

		public static void BindPopup(this LayoutWriter w, string elementid, string popupid, PopupOptions options = null)
		{
			if (options == null) options = PopupOptions.ShowOnClick;
			w.AddClientAction(options.ProxyName, "init", f => new {
				triggerid = f(elementid),
				popupid = f(popupid),
				triggeron = options.TriggerOn.ToString().ToLower(),
				displaysaround = options.DisplaysAround.ToString().ToLower(),
				position = options.Position.ToString().ToLower(),
				closeonclick = options.CloseOnClick,
				closeonscroll = options.CloseOnScroll
			});
		}
	}

	public class PopupOptions
	{
		public static PopupOptions ShowOnClick = new PopupOptions();
		public static PopupOptions ShowOnHover = new PopupOptions { TriggerOn = PopupTriggersOn.Hover };

		public string ProxyName { get; set; } = "contextmenuproxy";
		public PopupTriggersOn TriggerOn { get; set; } = PopupTriggersOn.Click;
		public PopupDispaysAround DisplaysAround { get; set; } = PopupDispaysAround.Trigger;
		public PopupPosition Position { get; set; } = PopupPosition.Bottom;
		public bool CloseOnClick { get; set; } = true;
		public bool CloseOnScroll { get; set; } = true;
	}

	public enum PopupTriggersOn
	{
		Click, Hover, MouseMove, DblClick, ContextMenu
	}
	public enum PopupDispaysAround
	{
		Cursor, Trigger
	}

	public enum PopupPosition
	{
		Left, Top, Right, Bottom
	}
}
