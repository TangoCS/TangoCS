﻿using System;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class MenuBuilder
	{
		public class MenuItem
		{
			public Func<bool> Visible { get; set; } = () => true;
			public Func<bool> Enabled { get; set; } = () => true;
			public Action<LayoutWriter> Render { get; set; }
		}

		public class MenuItemSeparator : MenuItem { }

		public ActionContext Context { get; }
		public TreeNode<MenuItem> Menu { get; } = new TreeNode<MenuItem>();

		public MenuBuilder(ActionContext context)
		{
			Context = context;
		}

		public void Item(Action<LayoutWriter> content) => Menu.AddChild(new MenuItem { Render = w => content(w) });
		public void ItemSeparator() => Menu.AddChild(new MenuItemSeparator());
		public void ItemHeader(string text) => Item(w => w.H3(text));
		public void ItemBack() => Item(w => w.BackLink());

		public void ItemActionText(Action<ActionLink> urlAttributes) => Item(w => w.ActionTextButton(urlAttributes));
		public void ItemActionImage(Action<ActionLink> urlAttributes) => Item(w => w.ActionImageButton(urlAttributes));
		public void ItemActionImageText(Action<ActionLink> urlAttributes) => Item(w => w.ActionImageTextButton(urlAttributes));

		public void ItemDropDownButton(string id, string title,
			Action<ApiResponse> serverEvent, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			Item(w => w.DropDownButton(id, title, (Action)null, icon, btnAttrs, a => a.DataEvent(serverEvent).Set(popupAttrs), options));
		}

		public void ItemDropDownButton(string id, string title,
			Action content = null, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			Item(w => w.DropDownButton(id, title, content, icon, btnAttrs, popupAttrs, options));
		}

		public void QuickSearch(IViewElement list, Paging paging)
		{
			Item(w => w.TextBox("qsearch", w.Context.GetArg("qsearch"), a =>
				a.Class("filterInput")
				.Autocomplete(false)
				.DataParm(paging.ClientID, 1)
				.DataEvent("OnQuickSearch", list.ClientID)
				.OnKeyUpRunHrefDelayed()
				.Placeholder(w.Resources.Get("Common.Search"))
                .Data(list.DataCollection))                
			);
		}

		public void ItemFilter(ListFilter filter)
		{
			void render(LayoutWriter w)
			{
				filter.LoadPersistent();

				void button() => w.ActionImageTextButton(a => a.CallbackToCurrent().AsDialog(filter.OpenFilterDialog)
					.WithImage("filter")
					.WithTitle(r => r.Get("Common.Filter")));

				if (filter.PersistentFilter.ID > 0)
					w.B(button);
				else
					button();
			}
			Item(render);
		}

		public void ItemViews(ListFilter filter)
		{
			Item(w => {
				filter.LoadPersistent();
				var text = filter.PersistentFilter.ID == 0 ? w.Resources.Get("Common.AllItems") :
					(filter.PersistentFilter.Name.IsEmpty() ? w.Resources.Get("System.Filter.Custom") : filter.PersistentFilter.Name);
				w.DropDownButton("tableviews", text, filter.GetViewsMenu, "view", popupAttrs: a => a.DataContainer("popup", w.IDPrefix));
			});
		}


		public void Render(LayoutWriter w)
		{
			bool separator = false;
			bool allowSeparator = false;
			foreach (var item in Menu.Children)
			{
				if (item.Data is MenuItemSeparator)
					separator = allowSeparator;
				else
				{
					if (separator)
					{
						separator = false;
						w.Li(() => w.Div(a => a.Class("menutoolbarseparator")));
					}
					allowSeparator = true;
					w.Li(() => item.Data.Render(w));
				}
			}
		}
	}

	public static class ToolbarExtensions
	{
		public static void Toolbar(this LayoutWriter w, Action<TagAttributes> attrs, Action<MenuBuilder> leftPart, Action<MenuBuilder> rightPart = null)
		{
			w.Div(a => a.Class("menutoolbar").Set(attrs), () => {
				var leftMenu = new MenuBuilder(w.Context);
				leftPart(leftMenu);
				w.Ul(a => a.Class("menutoolbar-left"), () => leftMenu.Render(w));
				if (rightPart != null)
				{
					var rightMenu = new MenuBuilder(w.Context);
					rightPart(rightMenu);
					w.Ul(a => a.Class("menutoolbar-right"), () => rightMenu.Render(w));
				}
			});
		}

		public static void Toolbar(this LayoutWriter w, Action<MenuBuilder> leftPart, Action<MenuBuilder> rightPart = null)
		{
			w.Toolbar(null, leftPart, rightPart);
		}
	}

	public static class BulkOperationsExtensions
	{
		public static void ItemActionTextBulk(this MenuBuilder b, Action<ActionLink> urlAttributes, string owner = null, string parmName = "selectedvalues")
		{
			b.Item(w => w.ActionTextButton(a => a.Set(urlAttributes), a => a.BulkOp(owner ?? w.IDPrefix).DataRef(parmName)));
		}

		public static void ItemDropDownButtonBulk(this MenuBuilder b, string id, string title,
			Action<ApiResponse> serverEvent, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			b.Item(w => {
				void attrs1(TagAttributes a) => a.BulkOp(w.IDPrefix).Set(btnAttrs);
				void attrs2(TagAttributes a) => a.DataEvent(serverEvent).Set(popupAttrs);
				w.DropDownButton(id, title, (Action)null, icon, attrs1, attrs2, options);
			});
		}

		public static void ItemDropDownButtonBulk(this MenuBuilder b, string id, string title,
			Action<IActionTarget> serverAction, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			b.Item(w => {
				void attrs0(IActionTarget a) => a.Set(serverAction).WithReturnUrlToCurrent(w.Context);
				void attrs1(TagAttributes a) => a.BulkOp(w.IDPrefix).Set(btnAttrs);
				void attrs2(TagAttributes a) => a.DataHref(w.Context, attrs0).DataParm("owner", w.IDPrefix).Set(popupAttrs);
				w.DropDownButton(id, title, (Action)null, icon, attrs1, attrs2, options);
			});
		}

		public static void ActionLinkBulk(this LayoutWriter w, Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null, Action content = null)
		{
			w.ActionLink(urlAttributes, a => a.DataRef(Constants.SelectedValues).Data("owner", w.IDPrefix).Set(attrs), content);
		}

		public static T BulkOp<T>(this TagAttributes<T> a, string owner)
			where T : TagAttributes<T>
		{
			return a.Class("bulkop hide").Data("owner", owner);
		}
	}
}