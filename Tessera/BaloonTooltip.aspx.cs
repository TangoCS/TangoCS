using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;

namespace Tessera
{
	public partial class BaloonTooltip : System.Web.UI.Page
	{
		protected void Page_Init(object sender, EventArgs e)
		{
			help.ViewFormSysName = Query.GetString("view");
			help.SetViewData(Query.GetString("mode"), null);
		}
	}
}
