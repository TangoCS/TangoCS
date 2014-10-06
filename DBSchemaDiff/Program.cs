using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;
using Nephrite.Meta.Database;
using Nephrite.Web;

namespace DBSchemaDiff
{
	class Program
	{
		static void Main(string[] args)
		{
			string mydocpath = System.IO.Directory.GetCurrentDirectory();

			IDatabaseMetadataReader readerFrom;
			IDatabaseMetadataReader readerTo;
			IDBScript dbScript;
			var dbFromName = string.Empty;
			var dbToName = string.Empty;
			if (ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString().Contains("Data Source"))
			{
				readerFrom = new SqlServerMetadataReader();
				SqlConnectionStringBuilder mssqlBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
				dbFromName = mssqlBuilder.InitialCatalog;
			}
			else
			{
				readerFrom = new DB2ServerMetadataReader();
				DB2ConnectionStringBuilder db2Builder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
				dbFromName = db2Builder.DBName;
			}

			if (ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString().Contains("Data Source"))
			{
				readerTo = new SqlServerMetadataReader();
				dbScript = new DBScriptMSSQL("dbo");

				SqlConnectionStringBuilder mssqlBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString());
				dbToName = mssqlBuilder.InitialCatalog;

			}
			else
			{
				readerTo = new DB2ServerMetadataReader();
				dbScript = new DBScriptDB2("dbo");

				DB2ConnectionStringBuilder db2Builder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString());
				dbToName = db2Builder.DBName;
			}

			ConnectionManager.SetConnectionString(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ConnectionString);
			var fromSchema = readerFrom.ReadSchema("dbo");

			ConnectionManager.SetConnectionString(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ConnectionString);
			var toSchema = readerTo.ReadSchema("dbo");

			foreach (var rsctable in fromSchema.Tables)
			{
				Console.WriteLine(@"Таблица - " + rsctable.Key);
				var table = toSchema.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == rsctable.Key.ToUpper());

				if (table == null)
				{
					dbScript.CreateTable(rsctable.Value);
				}
				else
					table.Sync(dbScript, rsctable.Value);

			}
			var script = dbScript.ToString();

			var filePath = mydocpath + "\\DbScripts\\" + dbFromName + "_" + dbToName + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".txt";
			TextWriter tw = File.CreateText(filePath);
			tw.Write(script);
			tw.Close();

			//Console.ReadKey();
		}
	}
}
