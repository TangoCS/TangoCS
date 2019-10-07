using System.Linq;
using Tango;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Html;

namespace Tango.UI.Navigation
{
	public class Sidebar : ViewComponent
	{
		[Inject]
		protected ICache Cache { get; set; }

		[Inject]
		protected IMenuDataLoader MenuLoader { get; set; }

		[Inject]
		protected IAccessControl AccessControl { get; set; }

		public override bool UsePropertyInjection => true;

		//public override void OnInit()
		//{
		//	Context.PersistentArgs.Add("section", Context.GetArg("Section"));
		//}

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

			w.Div(a => a.Class("nav-header"), () => {
				w.Write(currentItem.Title);
			});

			w.Div(a => a.Class("nav-body"), () => {
				foreach (var m in currentItem.Children.Where(o => AccessControl.Check(o.SecurableObjectKey)))
				{
					w.A(a => a.Href(m.Url).OnClickRunHref(), () => {
						if (!m.Image.IsEmpty()) w.Icon(m.Image);
						w.Write(m.Title);
					});
				}
			});

			w.Div(a => a.Class("nav-buttonsbar"), () => {
				foreach (var m in rootItems)
				{
					var children = m.Children.Where(o => AccessControl.Check(o.SecurableObjectKey));
					var url = children.Count() > 0 ? children.First().Url : m.Url;
					w.Div(a => a.Class("nav-button" + (m == currentItem ? " selected" : "")), () => {
						w.A(a => a.Href(url).OnClickRunHref(), () => {
							if (!m.Image.IsEmpty()) w.Icon(m.Image);
							w.Write(m.Title);
						});
					});
				}
			});
		}
	}
}
