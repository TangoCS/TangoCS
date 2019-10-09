using System;
using Tango.Html;

namespace Tango.UI
{
	public static class DropDownButtonExtensions
	{
		public static void DropDownButton(this LayoutWriter w, string id, string title,
			Action<ApiResponse> serverEvent, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			w.DropDownButton(id, title, content: null, icon: icon, btnAttrs: btnAttrs, popupAttrs: a => a.DataEvent(serverEvent).Set(popupAttrs), options: options);
		}

		public static void DropDownButton(this LayoutWriter w, string id, string title, 
			Action content = null, string icon = null, 
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			w.Div(a => a.ID(id).Class("actionbtn").Set(btnAttrs), () => {
				if (!icon.IsEmpty()) w.Icon(icon);
				w.Write(title);
				w.Icon("dropdownarrow");
			});
			w.DropDownForElement(id, popupAttrs, content, options);
		}

		public static void DropDownImage(this LayoutWriter w, string id, string icon, Action content = null, string comment = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			w.Div(a => a.ID(id).Class("dropdownimage").Title(comment).Set(btnAttrs), () => {
				w.Icon(icon);
			});
			w.DropDownForElement(id, popupAttrs, content, options);
		}

		public static void DropDownImage(this LayoutWriter w, string id, string icon, Action<ApiResponse> serverEvent, string comment = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			w.DropDownImage(id, icon, comment: comment, btnAttrs: btnAttrs, popupAttrs: a => a.DataEvent(serverEvent).Set(popupAttrs), options: options);
		}

		public static void DropDownForElement(this LayoutWriter w, string id, Action content, PopupOptions options = null)
		{
			w.DropDownForElement(id, null, content, options);
		}

		public static void DropDownForElement(this LayoutWriter w, string id, Action<TagAttributes> attrs, Action content = null, PopupOptions options = null)
		{
			w.PopupForElement(id, a => a.Class("dropdownmenu").Set(attrs), content, options);		
		}

		public static void PopupMenuSeparator(this LayoutWriter w)
		{
			w.Div(a => a.Class("separator"));
		}
	}
}
