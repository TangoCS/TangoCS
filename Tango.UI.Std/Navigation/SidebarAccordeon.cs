using System.Linq;
using Tango;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Html;

namespace Tango.UI.Navigation
{
	public class SidebarAccordeon : ViewComponent
	{
		[Inject]
		protected ICache Cache { get; set; }

		[Inject]
		protected IMenuDataLoader MenuLoader { get; set; }

		[Inject]
		protected IAccessControl AccessControl { get; set; }

		//public override bool UsePropertyInjection => true;

		public void Render(HtmlWriter w)
		{
			var rootItems = Cache.GetOrAdd("mainmenu", () => {
				return MenuLoader.Load("nav");
			});

			rootItems = rootItems.Where(o => AccessControl.Check(o.SecurableObjectKey));
			if (rootItems == null || rootItems.Count() == 0) return;

			var currentKey = $"section.{Context.GetArg("Section")}";
			var currentItem = rootItems.Where(o => o.ResourceKey == currentKey).FirstOrDefault();
			if (currentItem == null) currentItem = rootItems.First();

			foreach (var m in rootItems)
			{
				var children = m.Children.Where(o => AccessControl.Check(o.SecurableObjectKey));
				var url = children.Count() > 0 ? children.First().Url : m.Url;
				var target = children.Count() > 0 ? children.First().Target : null;
				var isCurr = m == currentItem;
				w.Div(a => a.Class("nav-button" + (isCurr ? " selected" : "")), () => {
					w.A(a => a.Href(url).OnClickRunHref().Data(Constants.ContainerNew, 1), () => {
						if (!m.Image.IsEmpty()) w.Icon(m.Image);
						w.Write(m.Title);
					});
				});

				if (isCurr)
				{
					w.Div(a => a.Class("nav-body"), () => {
						foreach (var item in m.Children.Where(o => AccessControl.Check(o.SecurableObjectKey)))
						{
							w.A(a => a.Href(item.Url).OnClickRunHref().Data(Constants.ContainerNew, 1), () => {
								if (!item.Image.IsEmpty()) w.Icon(item.Image);
								w.Write(item.Title);
							});
						}
					});
				}
			}
		}
	}

	//public class SidebarFilter : IResourceFilter
	//{
	//	public void OnResourceExecuting(ResourceExecutingContext context)
	//	{
	//		var ctx = context.ActionContext;
	//		var name = "section";
	//		if (!ctx.PersistentArgs.ContainsKey(name))
	//			ctx.PersistentArgs.Add(name, ctx.GetArg(name));
	//	}
	//}
}
