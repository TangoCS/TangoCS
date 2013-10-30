using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web;
using System.Data.Linq;
using System.Linq;
using Nephrite.Meta.Database;
using System.Data.SqlClient;

namespace TestSchema
{
	[TestClass]
	public class TestSchema
	{
		[TestMethod]
		public void TestMethod1()
		{
			string cs = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants;Data Source=TOSHIBA-TOSH\\SQL2008";
			Base.Model = new DataContext(cs);
			var s = MetaSolution.Load();
			var srcSchema = new Schema();
			foreach (var d in s.Classes)
			{
				srcSchema.Generate(d);
			}
			var ownSchema =  new SqlServerMetadataReader().ReadSchema("dbo");
			var dbScript = new DBScriptMSSQL();
			foreach (var table in ownSchema.Tables)
			{
				var srcTable = srcSchema.Tables.Values.SingleOrDefault(t=>t.Name==table.Key);
				if (table.Key == "EmployeeData")
				{

				}
				 table.Value.Sync(dbScript , srcTable);
				
			}

			var strSql = string.Join(" ", dbScript.Scripts.ToArray());

			using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
			{
				con.Open();
				using (SqlCommand cmd = new SqlCommand(strSql, con))
				{
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
