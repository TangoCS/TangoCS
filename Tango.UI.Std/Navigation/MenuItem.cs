using System;
using System.Collections.Generic;
using System.Linq;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Data;
using Tango.Html;
using Tango.UI.Std;

namespace Tango.UI.Navigation
{
	public class MenuItem
	{
		public Guid ID { get; } = Guid.NewGuid();
		public string ResourceKey { get; set; }
		public string SecurableObjectKey { get; set; }

		public string MenuItemType { get; set; }

		//public ActionTarget Target { get; set; }

		public string Title { get; set; }
		public string Url { get; set; }
		public string Image { get; set; }
		public int SeqNo { get; set; }

		public bool Enabled { get; set; } = true;
		public string Description { get; set; }

		public string CssClass { get; set; }

		public List<MenuItem> Children { get; } = new List<MenuItem>();
	}

	//public class MenuNode
	//{
	//	public MenuItem Item { get; set; }
	//	public List<MenuNode> Children { get; } = new List<MenuNode>();
	//}

	public static class MenuHelper
	{
		public static HashSet<Guid> CheckMenuItems(IAccessControl ac, IEnumerable<MenuItem> rootItems)
		{
			HashSet<Guid> removed = new HashSet<Guid>();
			var aac = ac as IActionAccessControl;

			void checkChildren(IEnumerable<MenuItem> items)
			{
				foreach (var item in items)
				{
					if (item.Children.Count > 0)
					{
						checkChildren(item.Children);
						if (item.Children.All(o => removed.Contains(o.ID)))
							removed.Add(item.ID);
					}
					else if (!aac.CheckWithPredicate(item.SecurableObjectKey, item))
						removed.Add(item.ID);
				}
			}

			checkChildren(rootItems);

			return removed;
		}

		public static void AdminTopMenuIcon(this LayoutWriter w)
		{
			var cache = w.Context.RequestServices.GetService(typeof(ICache)) as ICache;
			var loader = w.Context.RequestServices.GetService(typeof(IMenuDataLoader)) as IMenuDataLoader;
			var ac = w.Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;

			var (rootItems, removed) = GetMenu(cache, loader, ac, "adminmenu", w.Context.Lang);
			if (rootItems.Count() == 0) return;

			w.Li(a => a.ID("header-adminmenu").Class("headerimgbtn").Title("Администрирование"), () => {
				w.Span(() => w.Icon("header-settings2"));
				w.DropDownForElement("header-adminmenu", a => a.Class("iw-contextMenu-fixed"), () => {
					w.RenderTwoLevelMenu(rootItems, removed);
				}, new PopupOptions { CloseOnScroll = false });
			});
		}

		public static void HelpTopMenuIcon(this LayoutWriter w)
		{
			var cache = w.Context.RequestServices.GetService(typeof(ICache)) as ICache;
			var loader = w.Context.RequestServices.GetService(typeof(IMenuDataLoader)) as IMenuDataLoader;
			var ac = w.Context.RequestServices.GetService(typeof(IAccessControl)) as IAccessControl;

			var (rootItems, removed) = GetMenu(cache, loader, ac, "helpmenu");
			if (rootItems.Count() == 0) return;

			w.Li(a => a.ID("header-helpmenu").Class("headerimgbtn").Title("Руководства"), () => {
				w.Span(() => w.Icon("header-help"));
				w.DropDownForElement("header-helpmenu", a => a.Class("iw-contextMenu-fixed"), () => {
					w.RenderTwoLevelMenu(rootItems, removed);
				}, new PopupOptions { CloseOnScroll = false });
			});
		}

		public static void ChangeDBMenuIcon(this LayoutWriter w)
		{
			var settings = w.Context.RequestServices.GetService(typeof(IPersistentSettings)) as IPersistentSettings;

			if (!settings.GetBool("canchangedb")) return;

			w.Li(a => a.ID("header-db").Title("Выбрать базу данных"), () => {
				w.Span(() => {
					w.Icon("header-database");
					var conn = w.Context.PersistentArgs.Get("conn") ?? w.Context.GetArg("conn");
					w.Write(conn ?? ConnectionManager.DefaultConnection);
				});

				w.DropDownForElement("header-db", a => a.Class("iw-contextMenu-fixed"), () => {
					w.Div(() => {
						foreach (var cs in ConnectionManager.ConnectionStrings)
						{
							w.ActionLink(al => al.To<ConnectionController>("changeconnection").WithTitle(cs.Key).WithArg("newconn", cs.Key));
						}
					});
				});
			});
		}

		public static void ChangeThemeIcon(this LayoutWriter w)
		{
			var settings = w.Context.RequestServices.GetService(typeof(IPersistentSettings)) as IPersistentSettings;

			if (!settings.GetBool("canchangetheme")) return;

			w.Li(a => a.ID("header-theme"), () => {
				w.Span(() => {
					w.Icon("theme");
					w.Write("&nbsp;");
					var theme = w.Context.PersistentArgs.Get("theme") ?? w.Context.GetArg("theme") ?? settings.Get("theme");
					w.Write(theme);
				});

				var themes = new List<string> { "SIMPLE", "SIMPLE2" };

				w.DropDownForElement("header-theme", a => a.Class("iw-contextMenu-fixed"), () => {
					w.Div(() => {
						foreach (var t in themes)
							w.ActionLink(al => al.To<ThemeController>("changetheme").WithTitle(t).WithArg("newtheme", t));
					});
				});
			});
		}

		public static void VersionMenuIcon(this LayoutWriter w, IVersionProvider verProvider)
		{
			w.Li(a => a.Title("Версия приложения"), () => w.Span(() => {
				var v = verProvider.Version;
				w.Write($"v. {v}");
			}));
		}

		public static (IEnumerable<MenuItem> rootItems, HashSet<Guid> removed) GetMenu(ICache cache, IMenuDataLoader loader, IAccessControl ac, string menuName, string lang = null)
		{
			var rootItems = cache.GetOrAdd(lang.IsEmpty() ? menuName : (menuName + "-" + lang), () => {
				return loader.Load(menuName);
			});

			var removed = CheckMenuItems(ac, rootItems);

			return (rootItems.Where(o => !removed.Contains(o.ID)), removed);
		}

		public static void MenuItem(this LayoutWriter w, MenuItem c)
		{
			w.A(a => a.Href(c.Url).OnClickRunHref().Data(Constants.ContainerNew, 1), () => {
				if (!c.Image.IsEmpty()) w.Icon(c.Image);
				w.Write(c.Title);
			});
		}

		public static void RenderTwoLevelMenu(this LayoutWriter w, IEnumerable<MenuItem> rootItems, HashSet<Guid> removed)
		{
			foreach (var m in rootItems)
			{
				var children = m.Children.Where(o => !removed.Contains(o.ID));

				if (children.Count() > 0)
				{
					w.Div(() => {
						w.Div(m.Title);
						foreach (var c in children)
							w.MenuItem(c);
					});
				}
				else
					w.MenuItem(m);
			}
		}
	}
}
