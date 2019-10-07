using System.Linq;
using Tango;
using Tango.AccessControl;
using Tango.Cache;
using Tango.Html;
using Tango.UI.Std;

namespace Tango.UI.Navigation
{
	public class MenuController : BaseController
	{
		[Inject]
		protected ICache Cache { get; set; }

		[Inject]
		protected IMenuDataLoader MenuLoader { get; set; }

		[Inject]
		protected IAccessControl AccessControl { get; set; }

		public ActionResult AdminMenu()
		{
			return new ApiResult(response => response.AddWidget(Context.Sender, w => {
				var rootItems = Cache.GetOrAdd("adminmenu", () => {
					return MenuLoader.Load("adminmenu");
				});

				foreach (var m in rootItems)
				{
					w.Div(() => {
						w.Div(m.Title);
						foreach (var c in m.Children.Where(o => AccessControl.Check(o.SecurableObjectKey)))
						{
							w.A(a => a.Href(c.Url).OnClickRunHref(), () => {
								if (!c.Image.IsEmpty()) w.Icon(c.Image);
								w.Write(c.Title);
							});
						}
					});
				}
			}));
		}
	}
}
