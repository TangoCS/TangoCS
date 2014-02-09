using System;
using System.Text;
using IBM.Data.DB2;
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
		////[TestMethod]
		////public void TestMethod1()
		////{
		////	string cs = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servantsnew;Data Source=TOSHIBA-TOSH\\SQL2008";
		////	Base.Model = new DataContext(cs);
		////	var s = MetaSolution.Load();
		////	var srcSchema = new Schema();

		////	var sadawsd = string.Join(",", s.Classes.Where(cls => (cls.CompositeKey.Count > 0 ? cls.CompositeKey.Count > 1 ? cls.CompositeKey.Where(c => c is MetaAttribute).Any(a => (a as MetaAttribute).IsIdentity) : cls.Key is MetaAttribute && (cls.Key as MetaAttribute).IsIdentity : false) && cls.CompositeKey.Any(c => c.Type is MetaGuidType))
		////		.Select(t => string.Join(";", t.CompositeKey.Select(cm => "update MM_ObjectProperty  set IsIdentity = 0  where GUID ='" + cm.ID + "' \r\n").ToList().ToArray())).ToArray());
		////	foreach (var d in s.Classes)
		////	{
		////		srcSchema.Generate(d, s.Classes);
		////	}

		////	ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
		////	var ownSchema = new DB2ServerMetadataReader().ReadSchema("DBO");

		////	ConnectionManager.SetConnectionString("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008");
		////	var srcSchema = new SqlServerMetadataReader().ReadSchema("dbo");
		////	var dbScript = new DBScriptDB2("dbo");
		////	foreach (var rsctable in srcSchema.Tables)
		////	{

		////		var table = ownSchema.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == rsctable.Key.ToUpper());
		////		if (table.Key == "SPM_Subject")
		////		{
		////			// Тестирование импорта
		////			using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
		////			{
		////				con.Open();
		////				var strSql = new DBScriptDB2().ImportData(table.Value, true, con);
		////				//using (SqlCommand cmd = new SqlCommand(strSql, con))
		////				//{
		////				//	cmd.CommandType = System.Data.CommandType.Text;
		////				//	cmd.ExecuteNonQuery();
		////				//}
		////			}
		////		}
		////		if (table.Value.Name == "HST_N_TimeZone")
		////		{

		////		}
		////		if (table == null)
		////		{
		////			dbScript.CreateTable(rsctable.Value);
		////		}
		////		else
		////			table.Sync(dbScript, rsctable.Value);

		////	}

		////	var strSql = string.Join(" ", dbScript.Scripts.ToArray());

		////	using (SqlConnection con = new SqlConnection(ConnectionManager.ConnectionString))
		////	{
		////		con.Open();
		////		using (SqlCommand cmd = new SqlCommand(strSql, con))
		////		{
		////			cmd.CommandType = System.Data.CommandType.Text;
		////			cmd.ExecuteNonQuery();
		////		}
		////	}
		////}
		[TestMethod]
		public void GenerateResetDB2()
		{
			ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
			var ownSchema = new DB2ServerMetadataReader().ReadSchema("dbo");

			var stringBuilder = new StringBuilder();
			DB2Connection db3con = new DB2Connection(ConnectionManager.ConnectionString);
			db3con.Open();
			foreach (var table in ownSchema.Tables.Values)
			{
				if (table.Identity)
				{
					var columnIdenity = table.Columns.Values.FirstOrDefault(t => t.Identity);
					if (columnIdenity != null)
					{
				
						DB2Command cmd2 = db3con.CreateCommand();
						cmd2.CommandType = System.Data.CommandType.Text;
						cmd2.CommandText = string.Format(@"SELECT MAX({0})
												FROM DBO.{1}", columnIdenity.Name, table.Name);
						if (cmd2.ExecuteScalar() == DBNull.Value)
							continue;
						var count  =  Convert.ToInt32(cmd2.ExecuteScalar());

						stringBuilder.AppendFormat("ALTER TABLE DBO.{0} ALTER COLUMN {1}  RESTART WITH {2} ;\n\r", table.Name, columnIdenity.Name, count);
					}
				}
			}
			db3con.Close();
			var ss = stringBuilder.ToString();
		}

		//[TestMethod]
		//public void TestSynchDB2()
		//{
		//	//var dd = new DBScriptDB2("DBO") as IDBScript;
		//	//var ddss = dd.GetType().ToString();
		//	//var dd = TypeFactory.TypeFactory.Char(true).ToString();

		//	//var metaSolution = MetaSolution.Load();

		//	//foreach (var _class in metaSolution.Classes)
		//	//{
		//	//	foreach (var attribute in _class.Operations)
		//	//	{
		//	//		foreach (var param in attribute.Parameters)
		//	//		{
		//	//			//param.Value.
		//	//		}
		//	//		//attribute.Parameters
		//	//		//var metaReference = attribute.IsDefault
		//	//		//metaReference.Type
		//	//		//metaReference.DataType
		//	//	}
		//	//	//_class.Name

		//	//}
		//	ConnectionManager.SetConnectionString(
		//		"Password=q121212;Persist Security Info=True;User ID=servantsuser;Initial Catalog=servants;Data Source=srvsql.refactorx.ru\\mssqlserver2008");
		//	var sqlSchema = new SqlServerMetadataReader().ReadSchema("dbo");


		//	//ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
		//	ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
		//	var ownSchema = new DB2ServerMetadataReader().ReadSchema("dbo");

		//	//var update = new UpdateScriptBuilderDB2(db2Schema,
		//	//										new SqlConnection(
		//	//											"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008"),
		//	//										false);
		//	//var d = update.Generate(false, false);
		//	//var sss = string.Join(",", update.dd.ToArray());
		//	var dbScript = new DBScriptDB2("dbo");

		//	foreach (var rsctable in sqlSchema.Tables)
		//	{

		//		var table = ownSchema.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == rsctable.Key.ToUpper());



		//		if (table == null)
		//		{
		//			dbScript.CreateTable(rsctable.Value);
		//		}
		//		else
		//			table.Sync(dbScript, rsctable.Value);

		//	}
		//	var strSql = string.Join(" ", dbScript.Scripts.ToArray());
		//}
	}
}
