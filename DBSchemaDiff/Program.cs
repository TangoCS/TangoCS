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
using System.Xml.Linq;

namespace DBSchemaDiff
{
	class Program
	{
		static void Main(string[] args)
		{
			string mydocpath = Directory.GetCurrentDirectory();

			IDatabaseMetadataReader readerFrom;
			IDatabaseMetadataReader readerTo;
			IDBScript dbScript;
			var dbFromName = string.Empty;
			var dbToName = string.Empty;
			var cfgTables = ConfigurationManager.AppSettings["Tables"].Split(',');
			var exclude = ConfigurationManager.AppSettings["ExcludeTables"];
			var tableIndex = ConfigurationManager.AppSettings["TableIndex"];

			string[] cfgExcludeTables = null;
			if (exclude != null)
				cfgExcludeTables = ConfigurationManager.AppSettings["ExcludeTables"].Split(',');
			var tableIndexes = Convert.ToBoolean(tableIndex);

			if (ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString().Contains("Data Source"))
			{
				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString());
				dbFromName = strBuilder.InitialCatalog;
				readerFrom = new SqlServerMetadataReader(strBuilder.ConnectionString);
			}
			else
			if (ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString().Contains("Port"))
			{
				NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString());
				dbFromName = strBuilder.Database;
				readerFrom = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
			}
			else
			{
				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString());
				dbFromName = strBuilder.DBName;
				readerFrom = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}

			if (ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString().Contains("Data Source"))
			{
				
				dbScript = new DBScriptMSSQL("dbo");

				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString());
				dbToName = strBuilder.InitialCatalog;
				readerTo = new SqlServerMetadataReader(strBuilder.ConnectionString);

			}
			else
			if (ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString().Contains("Port"))
			{				
				dbScript = new DBScriptPostgreSQL("dbo");

				NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString());
				dbToName = strBuilder.Database;
				readerTo = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
			}
			else
			{			
				dbScript = new DBScriptDB2("dbo");

				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString());
				dbToName = strBuilder.DBName;
				readerTo = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}

			var path = mydocpath + "\\DbScripts\\";
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			Schema schemaFrom = readerFrom.ReadSchema("dbo");
			Schema schemaTo;

			if (File.Exists(mydocpath + "\\dbschema_to.xml"))
			{
				var s = File.ReadAllText(mydocpath + "\\dbschema_to.xml");
				XDocument doc = XDocument.Parse(s);
				schemaTo = readerTo.ReadSchema("dbo", doc);
			}
			else
			{
				schemaTo = readerTo.ReadSchema("dbo");
			}
			
			List<Table> Tables;
			if (cfgTables[0].ToLower() == "all")
				Tables = schemaFrom.Tables.Values.ToList();
			else
				Tables = schemaFrom.Tables.Values.Where(t => cfgTables.Any(c => t.Name.ToLower() == c.ToLower()) /*|| t.ForeignKeys.Any(f => cfgTables.Any(l => l.ToLower() == f.Value.RefTable.ToLower()))*/).ToList();

			if (cfgExcludeTables != null)
				Tables = Tables.Where(t => !cfgExcludeTables.Any(c => t.Name.ToLower() == c.ToLower())).ToList();

			var result = new StringBuilder();

            var resBeg = new StringBuilder();
            resBeg.AppendLine("DO LANGUAGE plpgsql");
			resBeg.AppendLine("$$");
			resBeg.AppendLine("BEGIN");
            string resultBeg = resBeg.ToString();
            resBeg.Clear();

            var resEnd = new StringBuilder();
            resEnd.AppendLine("EXCEPTION WHEN OTHERS THEN");
			resEnd.AppendLine("RAISE EXCEPTION 'Error state: %, Error message: %', SQLSTATE, SQLERRM;");
			resEnd.AppendLine("RAISE NOTICE 'Database structure successfully updated!';");
			resEnd.AppendLine("END;");
			resEnd.AppendLine("$$");
            string resultEnd = resEnd.ToString();
            resEnd.Clear();

            var indexaddpath = path + dbFromName + "_ADD_INDEX.sql";
			var indexdroppath = path + dbFromName + "_DROP_INDEX.sql";
			if (tableIndexes)
			{
				File.WriteAllText(indexaddpath, resultBeg);
				File.WriteAllText(indexdroppath, resultBeg);
			}

			foreach (var rsctable in Tables)
			{
				Console.Write(@"Таблица - " + rsctable.Name + " start...");

				if (tableIndexes && rsctable.Indexes.Count() > 0)
				{
					CreateIndex(rsctable, result);
					File.AppendAllText(indexaddpath, result.ToString());
					result.Clear();
				}

				var table = schemaTo.Tables.Values.SingleOrDefault(t => t.Name.ToUpper() == rsctable.Name.ToUpper());

				if (table == null)
				{
					dbScript.CreateTable(rsctable);
				}
				else
				{
					table.Sync(dbScript, rsctable);
					if (tableIndexes && table.Indexes.Count() > 0)
					{
						DropIndex(table, result);
						File.AppendAllText(indexdroppath, result.ToString());
						result.Clear();
					}
				}

				Console.WriteLine(@"end");
			}
			var script = dbScript.ToString();

			var filePath = path + dbFromName + "_" + dbToName + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".sql";
			TextWriter tw = File.CreateText(filePath);
			tw.Write(script);
			tw.Close();

			if (tableIndexes)
			{
				File.AppendAllText(indexaddpath, resultEnd);
				File.AppendAllText(indexdroppath, resultEnd);
			}

			Console.WriteLine("End");
			Console.ReadKey();
		}

		public static void CreateIndex(Table t, StringBuilder result)
		{
			foreach (var indx in t.Indexes.Values)
			{
				result.AppendFormat("CREATE{4}INDEX IF NOT EXISTS {3} ON {0}.{1} ({2});\r\n", t.Schema.Name.ToLower(), t.Name.ToLower(), string.Join(",", indx.Columns).ToLower(), indx.Name.ToLower(), indx.IsUnique ? " UNIQUE " : " ");
			}
		}

		public static void DropIndex(Table t, StringBuilder result)
		{
			foreach (var indx in t.Indexes.Values)
			{
				result.AppendFormat("DROP INDEX IF EXISTS {0};\r\n", /*t.Schema.Name.ToLower(), */indx.Name.ToLower());
			}
		}
	}
}
