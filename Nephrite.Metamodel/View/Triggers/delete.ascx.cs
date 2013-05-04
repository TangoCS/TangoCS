using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Microsoft.SqlServer.Management.Smo;
using Nephrite.Metamodel.Controllers;

namespace Nephrite.Metamodel.View
{
	public partial class Triggers_delete : ViewControl
	{
		Trigger trigger;
		DbGenerator gen = new DbGenerator(false, false);
		protected string name;

		protected void Page_Load(object sender, EventArgs e)
		{
			trigger = gen.GetTableTrigger(Query.GetString("tablename"), Query.GetString("triggername"));

			SetTitle("Удаление триггера");
			Cramb.Add("Триггеры", Html.ActionUrl<TriggersController>(c => c.ViewList()));
			name = ((Microsoft.SqlServer.Management.Smo.Table)trigger.Parent).Name + "." + trigger.Name;
			Cramb.Add("Удаление " + name);
		}

		protected void bDelete_Click(object sender, EventArgs e)
		{
			trigger.Drop();
			Query.RedirectBack();
		}
	}
}