using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.Controllers
{
    [ControllerControlsPath("/_controltemplates/Nephrite.Web/")]
    public class MaintanceController : BaseController
    {
        public void DbBackup()
        {
            RenderView("dbbackup");
        }
    }
}
