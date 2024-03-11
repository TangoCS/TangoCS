using System;
using System.Collections.Generic;
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
				var (rootItems, removed) = MenuHelper.GetMenu(Cache, MenuLoader, AccessControl, "adminmenu", Context.Lang);
				w.RenderTwoLevelMenu(rootItems, removed);
			}));
		}
	}
}
