﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tango.Html;
using Tango.UI.Std;

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

		public void ItemActionLink(Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null) => Item(w => w.ActionLink(urlAttributes, attrs));
		public void ItemActionText(Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null) => Item(w => w.ActionTextButton(urlAttributes, attrs));
		public void ItemActionImage(Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null) => Item(w => w.ActionImageButton(urlAttributes, attrs));
		public void ItemActionImageText(Action<ActionLink> urlAttributes, Action<ATagAttributes> attrs = null) => Item(w => w.ActionImageTextButton(urlAttributes, attrs));

		public void ItemField(string caption, Action<LayoutWriter> content)
		{
			Item(w => {
				w.Div(a => a.Class("menutoolbar-field"), () => {
					w.Div(() => w.Span(caption + ":"));
					w.Div(() => content(w));
				});
			});
		}

		public void ItemField(string labelFor, string labelTitle, Action<LayoutWriter> content)
		{
			Item(w => {
				w.Div(a => a.Class("menutoolbar-field"), () => {
					w.Div(() => w.Label(labelFor, labelTitle + ":"));
					w.Div(() => content(w));
				});
			});
		}

		public void ItemLabelFor(string labelFor, string labelTitle)
		{
			Item(w => {
				w.Div(a => a.Class("menutoolbar-field"), () => {
					w.Div(() => w.Label(labelFor, labelTitle + ":"));
				});
			});
		}

		public void ItemDropDownButton(string id, string title,
			Action<ApiResponse> serverEvent, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			Item(w => w.DropDownButton(id, title, (Action)null, icon, btnAttrs, a => a.DataEvent(serverEvent).Set(popupAttrs), options));
		}

		public void ItemDropDownButton(string id, string title,
			Action<LayoutWriter> content, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			Item(w => w.DropDownButton(id, title, () => content(w), icon, btnAttrs, popupAttrs, options));
		}

		public void QuickSearch<T, K>(abstract_list<T, K> list, Paging paging, InputName qSearchParmName, string tooltip = null, HttpMethod method = HttpMethod.GET)
		{
			Item(w => w.TextBox(qSearchParmName, w.Context.GetArg(qSearchParmName.Name), a =>
				{
					a.Class("filterInput").Autocomplete(false);
					if (list.Sections.RenderPaging)
						a.DataParm(paging.ClientID, 1);
						
					a.DataEvent("OnQuickSearch", list.ClientID);
					a.DataRef(list.Filter);

					switch (method)
					{
						case HttpMethod.POST:
							a.OnKeyUpPostHrefDelayed();
							break;
						default:
							a.OnKeyUpRunHrefDelayed();
							break;
					}
					
					a.Placeholder(w.Resources.Get("Common.Search"))
							.Data(list.DataCollection)
							.Title(tooltip);
				})
			);
		}

		public void ItemFilter(ListFilter filter, bool imageOnly = false)
		{
			void render(LayoutWriter w)
			{
				filter.LoadPersistent();

				var cls = filter.Criteria.Count > 0 ? "hascriteria" : "";

				void attrs(ActionLink a) => a.CallbackToCurrent().AsDialogPost<DialogNestedFormContainer>(filter.OpenFilterDialog)
					.WithImage("filter").WithTitle(r => r.Get("Common.Filter"));

				void tagAttrs(ATagAttributes a) => a.Data(filter.DataCollection).Class("filterbtn").Class(cls)
					.DataRef(filter);

				if (imageOnly)
					w.ActionImageButton(attrs, tagAttrs);
				else
					w.ActionImageTextButton(attrs, tagAttrs);

			}
			Item(render);
		}

		public void ItemViews(ListFilter filter)
		{
			Item(w => {
				filter.LoadPersistent();
				var text = filter.PersistentFilter.ID == 0 && filter.Criteria.Count == 0 ? w.Resources.Get("Common.AllItems") :
					(filter.PersistentFilter.Name.IsEmpty() ? w.Resources.Get("System.Filter.Custom") : filter.PersistentFilter.Name);
				w.DropDownButton("tableviews", text, filter.GetViewsMenu, "view", popupAttrs: a => a.DataNewContainer("popup", w.IDPrefix).DataRef(filter));
			});
		}

		public void ItemListSettings(List<List<IColumnHeader>> headerRows)
		{
			if (headerRows.Count == 0)
				return;

			Item(w => {
				w.DropDownImageButton("listsettings", "listsettings", () => {
					var i = 0;
					foreach (var header in headerRows[0]) // пока обрабатывается только первая строка заголовков
						foreach (var h in header.AsEnumerable())
						{
							if (!h.Title.IsEmpty())
							{
								var id = $"listsettings_hidecol_{i}";
								w.Label(a => a.For(id).Class("item"), () => {
									w.CheckBox(id, true, a => a.Style("vertical-align:middle").Data("colidx", i));
									w.Span(a => a.Style("vertical-align:middle"), h.Title);
								});
							}
							i++;
						}

				}, btnAttrs: a => a.Title("Скрытие столбцов"), options: new PopupOptions { CloseOnClick = false } );
			});
		}


		
	}

	public interface IToolbarRenderer
	{
		void Render(LayoutWriter w, Action<TagAttributes> attrs, Action<MenuBuilder> leftPart, Action<MenuBuilder> rightPart = null);
	}

	public class DefaultToolbarRenderer : IToolbarRenderer
	{
		public void Render(LayoutWriter w, Action<TagAttributes> attrs, Action<MenuBuilder> leftPart, Action<MenuBuilder> rightPart = null)
		{
			w.Div(a => a.Class("menutoolbar").Set(attrs), () => {
				var leftMenu = new MenuBuilder(w.Context);
				leftPart(leftMenu);
				w.Ul(a => a.Class("menutoolbar-left"), () => RenderItems(w, leftMenu));
				if (rightPart != null)
				{
					var rightMenu = new MenuBuilder(w.Context);
					rightPart(rightMenu);
					w.Ul(a => a.Class("menutoolbar-right"), () => RenderItems(w, rightMenu));
				}
			});
		}

		void RenderItems(LayoutWriter w, MenuBuilder t)
		{
			bool separator = false;
			bool allowSeparator = false;
			foreach (var item in t.Menu.Children)
			{
				if (item.Data is MenuBuilder.MenuItemSeparator)
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

	public class ToolbarOptions
	{
		public IToolbarRenderer Renderer { get; set; }
		public Action<TagAttributes> Attributes { get; set; }
	}

	public static class ToolbarExtensions
	{
		public static void Toolbar(this LayoutWriter w, Action<MenuBuilder> leftPart, Action<MenuBuilder> rightPart = null, ToolbarOptions opt = null)
		{
			(opt?.Renderer ?? new DefaultToolbarRenderer()).Render(w, opt?.Attributes, leftPart, rightPart);
		}
	}

	public static class BulkOperationsExtensions
	{
		public static void ItemActionTextBulk(this MenuBuilder b, Action<ActionLink> urlAttributes, string owner = null, string parmName = "selectedvalues")
		{
			b.Item(w => w.ActionTextButton(a => a.Set(urlAttributes), a => a.BulkOp(owner ?? w.IDPrefix).DataRef(parmName)));
		}
		public static void ItemDownloadActionTextBulk(this MenuBuilder b, Action<ActionLink> urlAttributes, string owner = null, string parmName = "selectedvalues")
		{
			b.Item(w => w.ActionTextButton(a => a.Set(urlAttributes), a => a.BulkOp(owner ?? w.IDPrefix).DataRef(parmName).Data("responsetype", "arraybuffer")));
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
			Action<ActionLink> serverAction, string icon = null,
			Action<TagAttributes> btnAttrs = null, Action<TagAttributes> popupAttrs = null, PopupOptions options = null)
		{
			b.Item(w => {
				void attrs0(ActionLink a) => a.Set(serverAction).WithReturnUrlToCurrent(w.Context);
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