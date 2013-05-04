using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
	public partial class Toolbar : UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected List<ToolbarItem> items = new List<ToolbarItem>();
		protected List<ToolbarItem> rightItems = new List<ToolbarItem>();

		public string TableCssClass { get; set; }



		public void AddItem<T>(string image, string title, Expression<Action<T>> action) where T : BaseController, new()
		{
			AddItem<T>(image, title, null, action);
		}
		public void AddItem<T>(string image, string title, string onClick, Expression<Action<T>> action) where T : BaseController, new()
		{
			string url = HtmlHelperBase.Instance.ActionUrl<T>(action);

			if (url != "#")
			{
				items.Add(new ToolbarItemMethod
				{
					Image = image,
					Title = title,
					OnClick = onClick,
					Url = url
				});
			}
		}
		public void AddItem(string image, string title, string actionUrl)
		{
			AddItem(image, title, null, actionUrl, false);
		}
		public void AddItem(string image, string title, string actionUrl, bool newWindow)
		{
			AddItem(image, title, null, actionUrl, newWindow);
		}
		public void AddItem(string image, string title, string onClick, string actionUrl)
		{
			AddItem(image, title, onClick, actionUrl, false);
		}
		public void AddItem(string image, string title, string onClick, string actionUrl, bool newWindow)
		{
			items.Add(new ToolbarItemMethod
			{
				Image = image,
				Title = title,
				OnClick = onClick,
				Url = actionUrl,
				TargetBlank = newWindow
			});
		}
		public void AddItemJS(string image, string title, string actionJsFunction)
		{
			items.Add(new ToolbarItemMethod
			{
				Image = image,
				Title = title,
				OnClick = actionJsFunction,
				Url = "#",
				TargetBlank = false
			});
		}
		public void AddItemSeparator()
		{
			if (items.Count > 0)
            {
                if (items[items.Count - 1] is ToolbarItemSeparator)
                    return;

                if (items[items.Count - 1] is ToolbarItemPopupMenu)
                {
                    var m = ((ToolbarItemPopupMenu)items[items.Count - 1]).Control;
                    if (m is PopupMenu && (((PopupMenu)m).Items == null ||
                        ((PopupMenu)m).Items.Count == 0)) return;
                }
            }

			items.Add(new ToolbarItemSeparator());
		}
        
        public void AddItemControl(Control control)
        {
            items.Add(new ToolbarItemPopupMenu { Control = control });
        }

		public ToolbarPopupMenuCompact AddPopupMenuCompact()
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
            Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-1.4.2.min.js");
            ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			items.Add(m);

			UserControl u = new UserControl();
			ToolbarPopupMenuCompact control = (ToolbarPopupMenuCompact)u.LoadControl(Settings.BaseControlsPath + "ToolbarPopupMenuCompact.ascx");
			m.Control = control;
			return control;
		}
		public ToolbarPopupMenuLarge AddPopupMenuLarge(string title)
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
            Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-1.4.2.min.js");
			
			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			items.Add(m);

			UserControl u = new UserControl();
			ToolbarPopupMenuLarge control = (ToolbarPopupMenuLarge)u.LoadControl(Settings.BaseControlsPath + "ToolbarPopupMenuLarge.ascx");
			m.Control = control;

			control.Title = title;
			
			return control;
		}

		public ToolbarPopupMenuCompact AddRightPopupMenuCompact()
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
            Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-1.4.2.min.js");
			
			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			rightItems.Add(m);

			UserControl u = new UserControl();
			ToolbarPopupMenuCompact control = (ToolbarPopupMenuCompact)u.LoadControl(Settings.BaseControlsPath + "ToolbarPopupMenuCompact.ascx");
			m.Control = control;
			return control;
		}
		public void AddRightItemText(string title)
		{
			rightItems.Add(new ToolbarItemText { Title = title });
		}
		public void AddRightItemSeparator()
		{
			rightItems.Add(new ToolbarItemSeparator());
		}

		protected abstract class ToolbarItem
		{
			
		}

		protected class ToolbarItemMethod : ToolbarItem
		{
			public string Image { get; set; }
			public string Url { get; set; }
			public string OnClick { get; set; }
			public string Title { get; set; }
			public bool TargetBlank { get; set; }

			 
		}
		protected class ToolbarItemSeparator : ToolbarItem
		{
			
		}
		protected class ToolbarItemPopupMenu : ToolbarItem
		{
			public Control Control { get; set; }
		}
		protected class ToolbarItemText : ToolbarItem
		{
			public string Title { get; set; }
		}

	}

	
}
