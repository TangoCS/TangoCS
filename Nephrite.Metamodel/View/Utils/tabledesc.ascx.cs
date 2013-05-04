using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using System.Configuration;
using Microsoft.SqlServer.Management.Smo;

namespace Nephrite.Metamodel.View.Utils
{
	public partial class tabledesc : ViewControl
	{
		protected List<string> Columns = new List<string>();
		protected List<string> Descriptions = new List<string>();
		List<Microsoft.SqlServer.Management.Smo.Table> tbllist;

		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle("Описания таблиц БД");

			List<string> tables;
			
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

			ServerConnection sc = b.IntegratedSecurity ? new ServerConnection(b.DataSource) : new ServerConnection(b.DataSource, b.UserID, b.Password);
			sc.Connect();
			Server server = new Server(sc);
			Database db = server.Databases[b.InitialCatalog];
			tbllist = db.Tables.OfType<Microsoft.SqlServer.Management.Smo.Table>().Where(o => o.Name != "sysdiagrams").ToList();
			tables = tbllist.Select(o => o.Name).ToList();
			tables.Sort();

			ddlTable.DataBindOnce(tbllist);

			if (!IsPostBack)
				Bind();
		}

		protected void ddlTable_SelectedIndexChanged(object sender, EventArgs e)
		{
			Bind();
		}

		void Bind()
		{
			var t = tbllist.FirstOrDefault(o => o.Name == ddlTable.SelectedValue);
			if (t != null)
			{
				var cols = t.Columns.Cast<Column>().OrderBy(o => o.Name);
				foreach (var c in cols)
				{
					Columns.Add(c.Name);
					Descriptions.Add(c.ExtendedProperties["MS_Description"] != null ? c.ExtendedProperties["MS_Description"].Value.ToString() : "");
				}
			}
			tbDescription.Text = t.ExtendedProperties["MS_Description"] != null ? t.ExtendedProperties["MS_Description"].Value.ToString() : "";
		}

		protected void ok_Click(object sender, EventArgs e)
		{
			var t = tbllist.FirstOrDefault(o => o.Name == ddlTable.SelectedValue);
			var ep = t.ExtendedProperties["MS_Description"];
			if (ep != null)
				ep.Value = tbDescription.Text;
			else
				t.ExtendedProperties.Add(new ExtendedProperty(t, "MS_Description", tbDescription.Text));
			foreach (Column c in t.Columns)
			{
				var ep1 = c.ExtendedProperties["MS_Description"];
				var desc = Request.Form["Col_" + c.Name];
				if (ep1 != null)
					ep1.Value = desc;
				else
					c.ExtendedProperties.Add(new ExtendedProperty(c, "MS_Description", desc));
			}
			t.Alter();
			ddlTable.SelectedIndex++;
			Bind();
		}
	}
}