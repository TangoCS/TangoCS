using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;

namespace Nephrite.CMS.Controllers
{
	[ControllerControlsPath("/_controltemplates/Nephrite.CMS/")]
    public class SiteFolderController : BaseController
    {
        public void View(int id, string className)
        {
            Repository r = new Repository();
            var obj = r.Get(Base.Meta.GetClass(className), id);
            if (obj == null)
                RenderMessage("Раздел не существует");
            RenderView("view", obj, false);
        }

        public void ViewRoot()
        {
            RenderView("viewroot", false);
        }

        public void AddSPM()
        {

        }

        public void DeleteSPM()
        {

        }
    }
}
