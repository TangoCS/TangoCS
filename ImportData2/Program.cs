using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Nephrite.Meta.Database;
using Npgsql;

namespace ImportData2
{
	static class Program
	{
		static void Main(string[] args)
		{
			string mydocpath = System.IO.Directory.GetCurrentDirectory();

			IDatabaseMetadataReader readerFrom;
			IDatabaseMetadataReader readerTo;
			IDBScript dbScript;
			var dbFromName = string.Empty;
			var dbToName = string.Empty;
			var connectFrom = ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString();
			var connectTo = ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString();
			var tablesImport = ConfigurationManager.AppSettings["TablesForImport"];
			var tableSeparat = ConfigurationManager.AppSettings["TableSeparation"];
			var result = new StringBuilder();

			if (string.IsNullOrEmpty(connectFrom) || string.IsNullOrEmpty(connectTo) || 
				string.IsNullOrEmpty(tablesImport) || string.IsNullOrEmpty(tableSeparat))
			{
				Console.Write(@"Некорректные параметры");
				Console.ReadKey();
				return;
			}

			var tableSeparation = Convert.ToBoolean(tableSeparat);
			var tablesForImport = tablesImport.ToLower().Split(',');

			if (connectFrom.Contains("Data Source"))
			{
				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(connectFrom);
				dbFromName = strBuilder.InitialCatalog;
				readerFrom = new SqlServerMetadataReader(strBuilder.ConnectionString);
			}
			else
				//if (connectFrom.Contains("Port"))
				{
					NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(connectFrom);
					dbFromName = strBuilder.Database;
					readerFrom = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
				}
				/*else
				{
					DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionStringFrom"].ToString());
					dbFromName = strBuilder.DBName;
					readerFrom = new DB2ServerMetadataReader(strBuilder.ConnectionString);
				}*/

			var path = mydocpath + "\\DbScripts";
			var filePath = mydocpath + "\\DbScripts\\" + dbFromName + "_" + dbToName + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".sql";
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			var schemaFrom = readerFrom.ReadSchema("dbo");
			var tabelListObjects = schemaFrom.Tables.Values.Where(t => tablesForImport.Any(c => t.Name.ToLower() == c) || t.ForeignKeys.Any(f => tablesForImport.Any(l => l.ToLower() == f.Value.RefTable.ToLower())));

			result.AppendLine("DO LANGUAGE plpgsql");
			result.AppendLine("$$");
			result.AppendLine("BEGIN");
			File.AppendAllText(filePath, result.ToString());
			result.Clear();

			DropConstraints(tabelListObjects, result);
			File.AppendAllText(filePath, result.ToString());
			result.Clear();
			
			SqlConnection sqlCon = new SqlConnection(connectFrom);

			foreach (var table in tabelListObjects.Where(t => tablesForImport.Any(c => t.Name.ToLower() == c)))
			{
				Console.WriteLine(@"Таблица " + table.Name + " start");

				if (table.Name.ToLower() != "n_filedata")
					ImportData(table, table.Columns.Any(t => t.Value.Identity), sqlCon, result);

				File.AppendAllText(filePath, result.ToString());

				result.Clear();
				
				Console.WriteLine(@"Таблица " + table.Name + " end");
			}
			sqlCon.Close();
			result.Clear();

			CreateConstraints(tabelListObjects, result);
			File.AppendAllText(filePath, result.ToString());
			result.Clear();
			
			result.AppendLine("EXCEPTION WHEN OTHERS THEN");
			result.AppendLine("RAISE EXCEPTION 'Error state: %, Error message: %', SQLSTATE, SQLERRM;");
			result.AppendLine("RAISE NOTICE 'Database structure successfully updated!';");
			result.AppendLine("END;");
			result.AppendLine("$$");
			File.AppendAllText(filePath, result.ToString());

			Console.WriteLine("End");
			Console.ReadKey();
		}

		static void DropConstraints(IEnumerable<Table> tables, StringBuilder result)
		{
			foreach (var t in tables)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{

					if (fk.RefTable.ToLower() == "N_File".ToLower() && fk.Columns.Any(c => c.ToLower() == "FILEID".ToLower() || c.ToLower() == "FILEGUID".ToLower() || c.ToLower() == "PHOTOGUID".ToLower()))
						continue;

					result.AppendFormat("ALTER TABLE {2}.{1} DROP CONSTRAINT IF EXISTS {0};\r\n", fk.Name.ToLower(), t.Name.ToLower(), t.Schema.Name.ToLower());
				}
			}

		}

		static void CreateConstraints(IEnumerable<Table> tables, StringBuilder result)
		{
			foreach (var t in tables)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{

					if (fk.RefTable.ToLower() == "N_File".ToLower() && fk.Columns.Any(c => c.ToLower() == "FILEID".ToLower() || c.ToLower() == "FILEGUID".ToLower() || c.ToLower() == "PHOTOGUID".ToLower()))
						continue;

					result.AppendFormat("ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};\r\n", t.Name.ToLower(), fk.Name.ToLower(), 
						string.Join(",", fk.Columns).ToLower(),	fk.RefTable.ToLower(), string.Join(",", fk.RefTableColumns).ToLower(), 
						"ON DELETE " + fk.DeleteOption.ToString().ToUpper(), t.Schema.Name.ToLower());
				}
			}

		}
		
		public static string ImportData(Table t, bool identityInsert, SqlConnection DbConnection, StringBuilder sqlInsert)
		{

			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = string.Format("select {0} from [{1}]", columns, t.Name);

			sqlInsert.AppendFormat("DELETE FROM dbo.{0};\r\n", t.Name.ToLower());
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i).Replace("\\", ""));
					}
					sqlInsert.AppendFormat("INSERT INTO dbo.{0} ({1}) VALUES ({2});\r\n", t.Name.ToLower(), columns.ToLower().Replace("[", "").Replace("]", ""), string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}
			if (identityInsert && t.Identity)
			{
				cmd.CommandText = string.Format("select max({0}) from [{1}]", t.Columns.Values.FirstOrDefault(c => c.Identity).Name, t.Name);
				var r = cmd.ExecuteScalar();
				var nextval = r != null && r != DBNull.Value ? (int)r : 0;
				nextval++;
				sqlInsert.AppendFormat("ALTER SEQUENCE dbo.{0}_{1}_seq RESTART WITH {2};\r\n", t.Name.ToLower(), t.Columns.Values.FirstOrDefault(c => c.Identity).Name.ToLower(), nextval);
			}

			return sqlInsert.ToString();
		}

		public static IEnumerable<string> SplitByLength(this string str, int maxLength)
		{
			for (int index = 0; index < str.Length; index += maxLength)
			{
				yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
			}
		}

		public static string CuttingText(this string value)
		{
			if (value.StartsWith("N'<%=HtmlHelperWSS.FormTableBegin(\"700px\")%>"))
			{

			}

			if (value.Length > 15000)
			{
				var arrayText = value.SplitByLength(15000);
				return string.Format("CAST({0}' as bytea) || '{1}", arrayText.First(),
									 string.Join("'||'", arrayText.Skip(1).ToArray()));
			}
			return value;
		}
		public static string GetStringValue(SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
				return "null";
			else
			{
				switch (reader.GetDataTypeName(index))
				{
					case "money":
						return reader.GetSqlMoney(index).Value.ToString(CultureInfo.InvariantCulture);
					case "float":
						return reader.GetSqlDouble(index).Value.ToString(CultureInfo.InvariantCulture);
					case "int":
						return reader.GetInt32(index).ToString();
					case "smallint":
						return reader.GetInt16(index).ToString();
					case "tinyint":
						return reader.GetByte(index).ToString();
					case "bigint":
						return reader.GetInt64(index).ToString();
					case "nvarchar":
						return ("N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'").CuttingText();
					case "varchar":
						return ("N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'").CuttingText();
					case "bit":
						return reader.GetBoolean(index) ? "true" : "false";
					case "uniqueidentifier":
						return String.Format("CAST('{0}' AS uuid)", reader.GetGuid(index).ToString());
					case "char":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "nchar":
						return ("N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'").CuttingText(); ;
					case "text":
						return ("N'" + reader.GetString(index).Replace("'", "''") + "'").CuttingText(); ;
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST('{0}' AS timestamp)", reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "image":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return string.Format("bytea('{0}')", result.ToString());
					case "xml":
						return String.Format("'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					case "varbinary":
						StringBuilder result1 = new StringBuilder();
						byte[] data1 = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data1.Length; x++)
							result1.Append(data1[x].ToString("X2"));
						return string.Format("bytea('{0}')", result1.ToString());
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}
	}
}
