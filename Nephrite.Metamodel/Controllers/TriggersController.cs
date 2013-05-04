using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel.Controllers
{
	[ControllerControlsPath("/_controltemplates/Nephrite.Metamodel/")]
	public class TriggersController : BaseController
	{
		public void ViewList()
		{
			RenderView("list", false);
		}

		public void Edit(string tablename, string triggername, string returnurl)
		{
			RenderView("edit", true);
		}

		public void Delete(string tablename, string triggername, string returnurl)
		{
			RenderView("delete", true);
		}

		public void CreateNew(string returnurl)
		{
			RenderView("edit", true);
		}
	}
}
