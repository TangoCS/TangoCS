using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using System.IO;
using System.Linq.Expressions;
using Nephrite.Meta;
using System.Web.UI.WebControls;
using Nephrite.Html;
using Nephrite.Layout;
using Nephrite.AccessControl;
using Nephrite.Multilanguage;

namespace Nephrite.Web.Controls
{

	public class Toolbar : BaseControl
	{
		[Inject]
		public IAccessControl AccessControl { get; set; }

		[Inject]
		public ITextResource TextResource { get; set; }

		protected List<ToolbarItem> items = new List<ToolbarItem>();
		protected List<ToolbarItem> rightItems = new List<ToolbarItem>();

		[Obsolete("Use ToolbarPosition and ToolbarMode properties")]
		public string TableCssClass { get; set; }
		public ToolbarPosition? Position { get; set; }
		public ToolbarMode? Mode { get; set; }
		public ToolbarItemsAlign ItemsAlign { get; set; }

		public ILayoutToolbar Layout { get; set; }
		public ILink TitleLink { get; set; }
		
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Layout = AppLayout.Current.Toolbar;
			ItemsAlign = ToolbarItemsAlign.Left;
		}

		//public void AddItem<T>(string image, string title, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	AddItem<T>(image, title, null, action);
		//}
		//public void AddItem<T>(string image, string title, string onClick, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	string url = HtmlHelperBase.Instance.ActionUrl<T>(action);

		//	if (url != "#")
		//	{
		//		items.Add(new ToolbarItemMethod
		//		{
		//			Image = image,
		//			Title = title,
		//			OnClick = onClick,
		//			Url = url
		//		});
		//	}
		//}
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

		//public void AddItemImage<T>(string image, string title, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	AddItemImage<T>(image, title, null, action);
		//}
		//public void AddItemImage<T>(string image, string title, string onClick, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	string url = HtmlHelperBase.Instance.ActionUrl<T>(action);

		//	if (url != "#")
		//	{
		//		items.Add(new ToolbarImageMethod
		//		{
		//			Image = image,
		//			Title = title,
		//			OnClick = onClick,
		//			Url = url
		//		});
		//	}
		//}
		public void AddItemImage(string image, string title, string actionUrl)
		{
			AddItemImage(image, title, null, actionUrl, false);
		}
		public void AddItemImage(string image, string title, string actionUrl, bool newWindow)
		{
			AddItemImage(image, title, null, actionUrl, newWindow);
		}
		public void AddItemImage(string image, string title, string onClick, string actionUrl)
		{
			AddItemImage(image, title, onClick, actionUrl, false);
		}
		public void AddItemImage(string image, string title, string onClick, string actionUrl, bool newWindow)
		{
			items.Add(new ToolbarImageMethod
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
			
			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			items.Add(m);

			ToolbarPopupMenuCompact control = new ToolbarPopupMenuCompact();
			m.Control = control;
			return control;
		}
		public ToolbarPopupMenuLarge AddPopupMenuLarge(string title)
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
			

			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			items.Add(m);

			ToolbarPopupMenuLarge control = new ToolbarPopupMenuLarge();
			m.Control = control;

			control.Title = title;

			return control;
		}

		public ToolbarPopupMenuCompact AddRightPopupMenuCompact()
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
			
			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			rightItems.Add(m);

			ToolbarPopupMenuCompact control = new ToolbarPopupMenuCompact();
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

		//public void AddRightItem<T>(string image, string title, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	AddRightItem<T>(image, title, null, action);
		//}
		//public void AddRightItem<T>(string image, string title, string onClick, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	string url = HtmlHelperBase.Instance.ActionUrl<T>(action);

		//	if (url != "#")
		//	{
		//		rightItems.Add(new ToolbarItemMethod
		//		{
		//			Image = image,
		//			Title = title,
		//			OnClick = onClick,
		//			Url = url
		//		});
		//	}
		//}
		public void AddRightItem(string image, string title, string actionUrl)
		{
			AddRightItem(image, title, null, actionUrl, false);
		}
		public void AddRightItem(string image, string title, string actionUrl, bool newWindow)
		{
			AddRightItem(image, title, null, actionUrl, newWindow);
		}
		public void AddRightItem(string image, string title, string onClick, string actionUrl)
		{
			AddRightItem(image, title, onClick, actionUrl, false);
		}
		public void AddRightItem(string image, string title, string onClick, string actionUrl, bool newWindow)
		{
			rightItems.Add(new ToolbarItemMethod
			{
				Image = image,
				Title = title,
				OnClick = onClick,
				Url = actionUrl,
				TargetBlank = newWindow
			});
		}

		//public void AddRightItemImage<T>(string image, string title, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	AddRightItemImage<T>(image, title, null, action);
		//}
		//public void AddRightItemImage<T>(string image, string title, string onClick, Expression<Action<T>> action) where T : BaseController, new()
		//{
		//	string url = HtmlHelperBase.Instance.ActionUrl<T>(action);

		//	if (url != "#")
		//	{
		//		rightItems.Add(new ToolbarImageMethod
		//		{
		//			Image = image,
		//			Title = title,
		//			OnClick = onClick,
		//			Url = url
		//		});
		//	}
		//}
		public void AddRightItemImage(string image, string title, string actionUrl)
		{
			AddRightItemImage(image, title, null, actionUrl, false);
		}
		public void AddRightItemImage(string image, string title, string actionUrl, bool newWindow)
		{
			AddRightItemImage(image, title, null, actionUrl, newWindow);
		}
		public void AddRightItemImage(string image, string title, string onClick, string actionUrl)
		{
			AddRightItemImage(image, title, onClick, actionUrl, false);
		}
		public void AddRightItemImage(string image, string title, string onClick, string actionUrl, bool newWindow)
		{
			rightItems.Add(new ToolbarImageMethod
			{
				Image = image,
				Title = title,
				OnClick = onClick,
				Url = actionUrl,
				TargetBlank = newWindow
			});
		}

		public ToolbarPopupMenuLarge AddRightPopupMenuLarge(string title)
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
			
			ToolbarItemPopupMenu m = new ToolbarItemPopupMenu();
			rightItems.Add(m);

			ToolbarPopupMenuLarge control = new ToolbarPopupMenuLarge();
			m.Control = control;

			control.Title = title;

			return control;
		}

		protected abstract class ToolbarItem
		{
			public abstract void Render(HtmlTextWriter writer, ILayoutToolbar layout);
		}

		protected class ToolbarItemMethod : ToolbarItem
		{
			public string Image { get; set; }
			public string Url { get; set; }
			public string OnClick { get; set; }
			public string Title { get; set; }
			public bool TargetBlank { get; set; }
			public string AccessKey { get; set; }

			public override void Render(HtmlTextWriter writer, ILayoutToolbar layout)
			{
				writer.Write(layout.ToolbarLink(Title, Url, Image, OnClick, TargetBlank));
			}
		}
		protected class ToolbarImageMethod : ToolbarItem
		{
			public string Image { get; set; }
			public string Url { get; set; }
			public string OnClick { get; set; }
			public string Title { get; set; }
			public bool TargetBlank { get; set; }

			public override void Render(HtmlTextWriter writer, ILayoutToolbar layout)
			{
				writer.Write(layout.ToolbarImageLink(Title, Url, Image, OnClick, TargetBlank));
			}
		}

		protected class ToolbarItemSeparator : ToolbarItem
		{
			public override void Render(HtmlTextWriter writer, ILayoutToolbar layout)
			{
				writer.Write(layout.ToolbarSeparator());
			}
		}
		protected class ToolbarItemPopupMenu : ToolbarItem
		{
			public Control Control { get; set; }

			public override void Render(HtmlTextWriter writer, ILayoutToolbar layout)
			{
				HtmlTextWriter hw = new HtmlTextWriter(new StringWriter());
				Control.RenderControl(hw);
				writer.Write(layout.ToolbarItem(hw.InnerWriter.ToString()));
			}
		}
		protected class ToolbarItemText : ToolbarItem
		{
			public string Title { get; set; }
			public override void Render(HtmlTextWriter writer, ILayoutToolbar layout)
			{
				writer.Write(layout.ToolbarItem(Title));
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);

			if (items.Count > 0 && items[0] is ToolbarItemSeparator) items.RemoveAt(0);
			if (items.Count > 0 && items[items.Count - 1] is ToolbarItemSeparator) items.RemoveAt(items.Count - 1);

			if (items.Count > 0 || rightItems.Count > 0)
			{
				writer.Write(Layout.ToolbarBegin(Position ?? ToolbarPosition.StaticTop, Mode ?? ToolbarMode.FormsToolbar, ItemsAlign, TitleLink));

				foreach (ToolbarItem item in items)
					item.Render(writer, Layout);

				writer.Write(Layout.ToolbarWhiteSpace());

				foreach (ToolbarItem item in rightItems)
					item.Render(writer, Layout);

				writer.Write(Layout.ToolbarEnd());
			}
		}
	}
}
