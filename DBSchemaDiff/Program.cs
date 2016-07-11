using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;
using Tango.Meta.Database;
using Npgsql;

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
			var cfgTables = ConfigurationManager.AppSettings["Tables"].Split(',');

			if (ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString().Contains("Data Source"))
			{
				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
				dbFromName = strBuilder.InitialCatalog;
				readerFrom = new SqlServerMetadataReader(strBuilder.ConnectionString);
			}
			else
			if (ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString().Contains("Port"))
			{
				NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
				dbFromName = strBuilder.Database;
				readerFrom = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
			}
			else
			{
				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
				dbFromName = strBuilder.DBName;
				readerFrom = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}

			if (ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString().Contains("Data Source"))
			{
				
				dbScript = new DBScriptMSSQL("dbo");

				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString());
				dbToName = strBuilder.InitialCatalog;
				readerTo = new SqlServerMetadataReader(strBuilder.ConnectionString);

			}
			else
			if (ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString().Contains("Port"))
			{				
				dbScript = new DBScriptPostgreSQL("dbo");

				NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString());
				dbToName = strBuilder.Database;
				readerTo = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
			}
			else
			{			
				dbScript = new DBScriptDB2("dbo");

				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringTo"].ToString());
				dbToName = strBuilder.DBName;
				readerTo = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}

			var fromSchema = readerFrom.ReadSchema("dbo");
			var toSchema = readerTo.ReadSchema("dbo");
			
			List<Table> Tables;
			if (cfgTables[0].ToLower() == "all")
				Tables = fromSchema.Tables.Values.ToList();
			else
				Tables = fromSchema.Tables.Values.Where(t => cfgTables.Any(c => t.Name.ToLower() == c.ToLower()) /*|| t.ForeignKeys.Any(f => cfgTables.Any(l => l.ToLower() == f.Value.RefTable.ToLower()))*/).ToList();
			 
			foreach (var rsctable in Tables)
			{
				Console.WriteLine(@"Таблица - " + rsctable.Name);
				var table = toSchema.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == rsctable.Name.ToUpper());

				if (table == null)
				{
					dbScript.CreateTable(rsctable);
				}
				else
					table.Sync(dbScript, rsctable);

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
