using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using System.Text;
using Nephrite.Metamodel.Controllers;

namespace Nephrite.Metamodel.View
{
	public partial class Utils_runup : ViewControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Формирование наката структуры БД");
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			Button1.Text = "Нажалось!";

			/*string dbStructXML = Encoding.UTF8.GetString(FileUpload1.FileBytes);
			string runup = (new UtilsController()).CreateRunUp(DbStruct.Database.Deserialize(dbStructXML));

			Response.AppendHeader("Content-Type", "text/plain");
			Response.AppendHeader("Content-disposition", "attachment; filename=DbStructRunUp.sql");
			Response.Write(runup);
			Response.End();*/
		}

		
	}
}