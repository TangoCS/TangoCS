using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Web;
using Nephrite.Web.Model;

namespace GenerateDB2Model
{
	class Program
	{
		static void Main(string[] args)
		{
			StringBuilder stringB = new StringBuilder();
			var db2 = new DB2ServerMetadataReader();
			ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");
			//Base.Model = new DataContext(ConnectionManager.ConnectionString);
			var d2schema = db2.ReadSchema("DBO");

			var sql = new SqlServerMetadataReader();
			ConnectionManager.SetConnectionString("Password=q121212;Persist Security Info=True;User ID=servantsuser;Initial Catalog=servants;Data Source=srvsql.refactorx.ru\\mssqlserver2008");
			//Base.Model = new DataContext(ConnectionManager.ConnectionString);
			var sqlschema = sql.ReadSchema("DBO");


			foreach (var dbTable in d2schema.Tables.Values)
			{
				if (string.IsNullOrEmpty(dbTable.Description))
				{
					stringB.AppendFormat("COMMENT ON TABLE DBO.{0} IS '{1}'; \r\n", dbTable.Name, "|" + dbTable.Name);
				}
				else if (!dbTable.Description.Contains("|"))
				{
					// Проверим опсание на каком языке  ХАК

					Regex regexTable = new Regex("[а-яА-ЯёЁъЪ]{1,32}");
					Match matchTable = regexTable.Match(dbTable.Description);


					stringB.AppendFormat("COMMENT ON TABLE DBO.{0} IS '{1}'; \r\n", dbTable.Name,
					                     matchTable.Success ? (dbTable.Description + "|") : "|" + dbTable.Description );
				}

				foreach (var dbColumn in dbTable.Columns.Values)
				{
					if (string.IsNullOrEmpty(dbColumn.Description))
					{
						stringB.AppendFormat("COMMENT ON COLUMN DBO.{0}.{1} IS '{2}'; \r\n", dbTable.Name, dbColumn.Name, "|" + dbColumn.Name);
					}
					else if (!dbColumn.Description.Contains("|"))
					{
						// Проверим опсание на каком языке  ХАК

						Regex regexTable = new Regex("[а-яА-ЯёЁъЪ]{1,32}");
						Match matchTable = regexTable.Match(dbColumn.Description);


						stringB.AppendFormat("COMMENT ON COLUMN DBO.{0}.{1} IS '{2}'; \r\n", dbTable.Name, dbColumn.Name, matchTable.Success ? (dbColumn.Description + "|") : "|" + dbColumn.Description);
					}
				}
				//if (d2schema.Tables.Values.Any(t => t.Name.ToLower() == sT.Value.Name.ToLower()))
				//{
				//	var dbTable = d2schema.Tables.Values.SingleOrDefault(t => t.Name.ToLower() == sT.Value.Name.ToLower());
				//	if (dbTable != null)
				//		stringB.AppendFormat("COMMENT ON TABLE DBO.{0} IS '{1}'; \r\n", dbTable.Name, string.IsNullOrEmpty(dbTable.Description) ? sT.Value.Name : dbTable.Description + "|" + sT.Value.Name);
				//	foreach (var sColumn in sT.Value.Columns.Values)
				//	{
				//		var dbColumn = dbTable.Columns.Values.SingleOrDefault(t => t.Name.ToLower() == sColumn.Name.ToLower());

				//		if (dbColumn != null)
				//			stringB.AppendFormat("COMMENT ON COLUMN DBO.{0}.{1} IS '{2}'; \r\n", dbTable.Name, dbColumn.Name, string.IsNullOrEmpty(dbColumn.Description) ? sColumn.Name : dbColumn.Description + "|" + sColumn.Name);
				//	}
				//}
			}



			//foreach (var sT in sqlschema.Tables)
			//{
			//	if (d2schema.Tables.Values.Any(t => t.Name.ToLower() == sT.Value.Name.ToLower()))
			//	{
			//		var dbTable = d2schema.Tables.Values.SingleOrDefault(t => t.Name.ToLower() == sT.Value.Name.ToLower());
			//		if (dbTable != null)
			//			stringB.AppendFormat("COMMENT ON TABLE DBO.{0} IS '{1}'; \r\n", dbTable.Name, string.IsNullOrEmpty(dbTable.Description) ? sT.Value.Name : dbTable.Description + "|" + sT.Value.Name);
			//		foreach (var sColumn in sT.Value.Columns.Values)
			//		{
			//			var dbColumn = dbTable.Columns.Values.SingleOrDefault(t => t.Name.ToLower() == sColumn.Name.ToLower());

			//			if (dbColumn != null)
			//				stringB.AppendFormat("COMMENT ON COLUMN DBO.{0}.{1} IS '{2}'; \r\n", dbTable.Name, dbColumn.Name, string.IsNullOrEmpty(dbColumn.Description) ? sColumn.Name : dbColumn.Description + "|" + sColumn.Name);
			//		}
			//	}
			//}

			var result = stringB.ToString();
			//string cs = "Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000";
			////new HCoreDataContext(AppWeb.DBConfig);//Solution.App.DataContext;
			//var d = new HCoreDataContext(AppWeb.DBConfig);
			//var s = MetaSolution.Load();
		}
	}
}
