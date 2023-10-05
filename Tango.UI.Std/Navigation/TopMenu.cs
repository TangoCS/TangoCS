using CronExpressionDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Html;
using Tango.Localization;

namespace Tango.UI.Navigation
{
	public class TopMenu : ViewComponent
	{
		[Inject]
		protected ICache Cache { get; set; }

		[Inject]
		protected IMenuDataLoader MenuLoader { get; set; }

		[Inject]
		protected IAccessControl AccessControl { get; set; }

		[Inject]
		protected ILanguage Language { get; set; }

		public void Render(LayoutWriter w)
		{
			var (rootItems, removed) = MenuHelper.GetMenu(Cache, MenuLoader, AccessControl, "topmenu");

			foreach (var m in rootItems)
			{
				var children = m.Children.Where(o => !removed.Contains(o.ID));
				var hasChildren = children.Count() > 0;
				var menuid = "m" + Guid.NewGuid().ToString();

				w.Li(a => a.ID(menuid), () => {
					if (hasChildren)
					{
						w.Span(a => a.Class("topmenu-item"), () => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});

						w.DropDownForElement(menuid, null, () => {
							w.RenderTwoLevelMenu(children, removed);
						}, new PopupOptions { CloseOnScroll = false } );
					}
					else
					{
						w.A(a => a.Class("topmenu-item").Href(m.Url.Replace("{lang}", Language.Current.Code)).OnClickRunHref().Data(Constants.ContainerNew, 1), () => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});
					}
				});
			}
		}
	}

	public class SidebarMenu : ViewComponent
	{
		[Inject]
		protected ICache Cache { get; set; }

		[Inject]
		protected IMenuDataLoader MenuLoader { get; set; }

		[Inject]
		protected IAccessControl AccessControl { get; set; }

		[Inject]
		public IIdentity User { get; set; }

		[Inject]
		protected IVersionProvider VersionProvider { get; set; }

		public string CallerID { get; set; }
		PopupOptions rootOptions = new PopupOptions {
			ProxyName = "sidebarmenuproxy",
			Type = PopupType.SliderMenuLeft,
			DisplaysAround = PopupDispaysAround.Custom,
			ShowOverlay = true,
			CloseOnScroll = false
		};

		public void Render(string buttonID, LayoutWriter w)
		{
			Action subMenus = null;
			w.Div(a => a.ID(ID).Class("sidebarmenu-container"), () => {
				w.Div(a => a.ID(ID + "_background").Class("sidebarmenu-background"));
				w.Div(a => a.ID(ID + "_body").Class("sidebarmenu"), () => {
					w.Ul(() => {
						var (rootItems, removed) = MenuHelper.GetMenu(Cache, MenuLoader, AccessControl, "topmenu");
						subMenus = RenderFirstLevel(w, rootItems, removed);
					});

					w.Ul(a => a.Class("sidebarmenu-bottom"), () => {

						var (rootItems, removed) = MenuHelper.GetMenu(Cache, MenuLoader, AccessControl, "adminmenu");

						w.Li(a => a.ID("msidebar-adminmenu"), () => {
							w.Span(a => a.Class("item"), () => {
								w.Icon("settings2");
								w.Write("Settings");
							});

							subMenus += () => RenderSecondLevel(w, "sidebar-adminmenu", "Settings", rootItems, removed);
						});

						if (!User.Name.IsEmpty()) 
							w.Li(() => w.A(a => a.Href("/logout"), Resources.Get("Account.SignOut")));

						w.Li(() => w.Span(a => a.Class("item"), () => {
							w.Icon("user");
							w.Write("&nbsp;");
							w.B(User.Name);
						}));

						w.Li(() => w.Span(a => a.Class("item"), () => {
							w.Div(a => a.ID("log"));
							var v = VersionProvider.Version;
							w.Div($"v. {v}");
						}));

					});
				});
				
			});

			subMenus?.Invoke();

			w.BindPopup(CallerID, ID, rootOptions);
		}

		Action RenderFirstLevel(LayoutWriter w, IEnumerable<MenuItem> rootItems, HashSet<Guid> removed)
		{
			var ids = new List<Guid>();
			Action subMenus = () => { };

			foreach (var m in rootItems)
			{
				var children = m.Children.Where(o => !removed.Contains(o.ID));

				w.Li(a => a.ID("m" + m.ID), () => {
					if (children.Count() > 0)
					{
						w.Span(a => a.Class("item"), () => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});

						subMenus += () => RenderSecondLevel(w, m.ID.ToString(), m.Title, children, removed);
					}
					else
					{
						w.MenuItem(m);
					}
				});
			}

			return subMenus;
		}

		void RenderSecondLevel(LayoutWriter w, string id, string title, IEnumerable<MenuItem> children, HashSet<Guid> removed)
		{
			w.PopupForElement("m" + id, a => a.Class("sidebarmenu-container iw-contextMenu iw-type-slidermenuleft"), () => {
				w.Div(a => a.Class("sidebarmenu-background"));
				w.Div(a => a.Class("sidebarmenu"), () => {
					var backid = "back" + id;

					w.A(a => a.ID(backid).Class("sidebarmenu-back"), () => {
						w.Icon("back");
						w.Write("Back");
					});
					w.BindPopup(backid, ID, rootOptions);


					w.Div(a => a.Class("sidebarmenu-title"), title);

					foreach (var child in children)
					{
						var children2 = child.Children.Where(o => !removed.Contains(o.ID));
						if (children2.Count() > 0)
						{
							w.Div(() => {
								w.Div(a => a.Class("sidebarmenu-title"), child.Title);
								foreach (var c in children2)
									w.MenuItem(c);
							});
						}
						else
							w.MenuItem(child);
					}
				});
			}, rootOptions);
		}
	}
}
