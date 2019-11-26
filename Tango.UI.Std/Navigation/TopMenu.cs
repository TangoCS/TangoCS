using System;
using System.Linq;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Html;

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

		public override bool UsePropertyInjection => true;

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
						w.Span(() => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});

						w.DropDownForElement(menuid, () => {
							w.RenderTwoLevelMenu(children, removed);
							//foreach (var cm in children)
							//{
							//	w.A(a => a.Href(cm.Url).OnClickRunHref(), () => {
							//		if (!cm.Image.IsEmpty()) w.Icon(cm.Image);
							//		w.Write(cm.Title);
							//	});
							//}
						});
					}
					else
					{
						w.A(a => a.Href(m.Url).OnClickRunHref(), () => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});
					}
				});
			}
		}
	}
}
