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
		//[TestMethod]
		//public void TestMethod1()
		//{
		//	string cs = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants;Data Source=TOSHIBA-TOSH\\SQL2008";
		//	Base.Model = new DataContext(cs);
		//	var s = MetaSolution.Load();
		//	var srcSchema = new Schema();

		//	var sadawsd = string.Join(",", s.Classes.Where(cls => (cls.CompositeKey.Count > 0 ? cls.CompositeKey.Count > 1 ? cls.CompositeKey.Where(c => c is MetaAttribute).Any(a => (a as MetaAttribute).IsIdentity) : cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity : false) && cls.CompositeKey.Any(c => c.Type is MetaGuidType))
		//		.Select(t => string.Join(";", t.CompositeKey.Select(cm => "update MM_ObjectProperty  set IsIdentity = 0  where GUID ='" + cm.ID + "' \r\n").ToList().ToArray())).ToArray());
		//	foreach (var d in s.Classes)
		//	{
		//		srcSchema.Generate(d, s.Classes);
		//	}
		//	var ownSchema = new SqlServerMetadataReader().ReadSchema("dbo");
		//	var dbScript = new DBScriptMSSQL("dbo");
		//	foreach (var table in ownSchema.Tables)
		//	{

		//		var srcTable = srcSchema.Tables.Values.SingleOrDefault(t => t.Name == table.Key);
		//		//if (table.Key == "SPM_Subject")
		//		//{ 
		//		//	// Тестирование импорта
		//		//	using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
		//		//	{
		//		//		con.Open();
		//		//		var strSql = new DBScriptDB2().ImportData(table.Value, true, con);
		//		//		//using (SqlCommand cmd = new SqlCommand(strSql, con))
		//		//		//{
		//		//		//	cmd.CommandType = System.Data.CommandType.Text;
		//		//		//	cmd.ExecuteNonQuery();
		//		//		//}
		//		//	}
		//		//}
		//		//if (table.Value.Name == "HST_N_TimeZone")
		//		//{

		//		//}
		//		table.Value.Sync(dbScript, srcTable);

		//	}

		//	var strSql = string.Join(" ", dbScript.Scripts.ToArray());

		//	using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
		//	{
		//		con.Open();
		//		using (SqlCommand cmd = new SqlCommand(strSql, con))
		//		{
		//			cmd.CommandType = System.Data.CommandType.Text;
		//			cmd.ExecuteNonQuery();
		//		}
		//	}
		//}

		[TestMethod]
		public void TestDB2()
		{
			ConnectionManager.SetConnectionString(
				"Password=q121212;Persist Security Info=True;User ID=servantsuser;Initial Catalog=servants;Data Source=srvsql.refactorx.ru\\mssqlserver2008");
			var sqlSchema = new SqlServerMetadataReader().ReadSchema("dbo");


			ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
			var db2Schema = new DB2ServerMetadataReader().ReadSchema("dbo");

		
			var d = new UpdateScriptBuilderDB2(sqlSchema, new SqlConnection("Password=q121212;Persist Security Info=True;User ID=servantsuser;Initial Catalog=servants;Data Source=srvsql.refactorx.ru\\mssqlserver2008")).Generate(false, false);
			//var db2Script = new DBScriptDB2("dbo");
			//foreach (var table in db2Schema.Tables)
			//{

			//	var srcTable = sqlSchema.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == table.Key.ToUpper());

			//	table.Value.Sync(db2Script, srcTable);
			//}
			//var strSql = string.Join(" ", db2Script.Scripts.ToArray());
		}
	}
}
