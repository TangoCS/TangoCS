using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Tango.Meta.Database;
using Npgsql;
using System.Xml.Linq;
using System.Data;
using Tango.Meta;

namespace ImportData2
{
	static class Program
	{
		static void Main(string[] args)
		{
			string mydocpath = Directory.GetCurrentDirectory();

			IDatabaseMetadataReader readerFrom;
			IDatabaseMetadataReader readerTo;

			var dbFromName = string.Empty;
			var dbToName = string.Empty;
			var connectFrom = ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString();
			var connectTo = ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString();
			var tablesImport = ConfigurationManager.AppSettings["TablesForImport"];
			var tablesExclude = ConfigurationManager.AppSettings["TablesExclude"];
			//var tableSeparat = ConfigurationManager.AppSettings["TableSeparation"];
			var result = new StringBuilder();
			var resultcopy = new StringBuilder();
			var resultBeg = new StringBuilder();
			var resultEnd = new StringBuilder();

			if (string.IsNullOrEmpty(connectFrom) || string.IsNullOrEmpty(connectTo) || 
				string.IsNullOrEmpty(tablesImport)/* || string.IsNullOrEmpty(tableSeparat)*/)
			{
				Console.Write(@"Некорректные параметры");
				Console.ReadKey();
				return;
			}

			//var tableSeparation = Convert.ToBoolean(tableSeparat);
			var tablesForImport = tablesImport.ToLower().Split(',');

			string[] tablesForExclude = null;
			if (tablesExclude != null)
				tablesForExclude = tablesExclude.Split(',');

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
				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionFrom"].ToString());
				dbFromName = strBuilder.DBName;
				readerFrom = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}*/
			if (connectTo.Contains("Data Source"))
			{
				SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(connectTo);
				dbToName = strBuilder.InitialCatalog;
				readerTo = new SqlServerMetadataReader(strBuilder.ConnectionString);
			}
			else
			//if (connectFrom.Contains("Port"))
			{
				NpgsqlConnectionStringBuilder strBuilder = new NpgsqlConnectionStringBuilder(connectTo);
				dbToName = strBuilder.Database;
				readerTo = new PostgreSQLMetadataReader(strBuilder.ConnectionString);
			}
			/*else
			{
				DB2ConnectionStringBuilder strBuilder = new DB2ConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionTo"].ToString());
				dbToName = strBuilder.DBName;
				readerTo = new DB2ServerMetadataReader(strBuilder.ConnectionString);
			}*/
			var dbScript = new DBScriptMSSQL("dbo");
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

			IEnumerable<Table> tableListObjects;
			if (tablesForImport[0].ToLower() == "all")
				tableListObjects = schemaFrom.Tables.Values;
			else
				tableListObjects = schemaFrom.Tables.Values.Where(t => tablesForImport.Any(c => t.Name.ToLower() == c.ToLower()) /*|| t.ForeignKeys.Any(f => tablesForImport.Any(l => l.ToLower() == f.Value.RefTable.ToLower()))*/);

			if (tablesForExclude != null)
				tableListObjects = tableListObjects.Where(t => !tablesForExclude.Any(c => t.Name.ToLower() == c.ToLower()));

			var tableListTo = schemaTo.Tables.Values.Where(t => tableListObjects.Any(c => t.Name.ToLower() == c.Name.ToLower())).ToArray();
			tableListObjects = tableListObjects.Where(t => tableListTo.Any(c => c.Name.ToLower() == t.Name.ToLower())).ToArray();

			resultBeg.AppendLine("DO LANGUAGE plpgsql");
			resultBeg.AppendLine("$$");
			resultBeg.AppendLine("BEGIN");

			resultEnd.AppendLine("EXCEPTION WHEN OTHERS THEN");
			resultEnd.AppendLine("RAISE EXCEPTION 'Error state: %, Error message: %', SQLSTATE, SQLERRM;");
			resultEnd.AppendLine("RAISE NOTICE 'Database structure successfully updated!';");
			resultEnd.AppendLine("END;");
			resultEnd.AppendLine("$$");

			var droppath = path + dbFromName + "__DROP_CONSTRAINTS.sql";
			File.WriteAllText(droppath, resultBeg.ToString());
			DropConstraints(tableListTo, result);
			File.AppendAllText(droppath, result.ToString());
			File.AppendAllText(droppath, resultEnd.ToString());
			result.Clear();

			var addpath = path + dbFromName + "__ADD_CONSTRAINTS.sql";
			File.WriteAllText(addpath, resultBeg.ToString());
			CreateConstraints(tableListObjects, result);
			File.AppendAllText(addpath, result.ToString());
			File.AppendAllText(addpath, resultEnd.ToString());
			result.Clear();

			var newfilePath = path + dbFromName + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + "_{0}.sql";
			int num = 1;
			int i = 0;
			decimal allsize = 0;

			SqlConnection sqlCon = new SqlConnection(connectFrom);

			foreach (var table in tableListObjects.OrderBy(t => t.Name))
			{
				Console.Write(@"Таблица " + table.Name + " start...");

				var tableto = tableListTo.First(t => t.Name.ToLower() == table.Name.ToLower());

				var clmsfrom = table.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText));
				var clms = clmsfrom.Where(c => tableto.Columns.Values.Any(c2 => string.IsNullOrEmpty(c2.ComputedText) &&
																		c2.Name.ToLower() == c.Name.ToLower()));
				var columns = string.Join("+", clms.Select(c => (c.Type.GetDBType(dbScript).Contains("varbinary") || c.Type.GetDBType(dbScript).Contains("image") || c.Type.GetDBType(dbScript).Contains("xml")) ?
				string.Format("isnull(datalength([{0}])*4,0)", c.Name) : string.Format("isnull(len([{0}]),0)", c.Name)).ToArray());

				if (sqlCon.State == ConnectionState.Closed)
					sqlCon.Open();

				SqlCommand cmd = sqlCon.CreateCommand();
				cmd.CommandTimeout = 600;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = string.Format("select isnull(sum({0}),0) from [{1}]", columns, table.Name);

				//object robj = cmd.ExecuteScalar();
				//decimal tsize = (robj != null && robj != DBNull.Value ? decimal.Parse(robj.ToString()) : 0) / 1024;
				decimal tsize = decimal.Parse(cmd.ExecuteScalar().ToString()) / 1024;
				tsize = tsize + (tsize * table.Columns.Count() * 2M / 100M);

				if (tsize > 20480)
				{
					var filecopy = path + dbFromName.ToLower() + "_" + table.Name.ToLower() + ".txt";
					File.WriteAllText(filecopy, "");

					ImportData2(filecopy, dbFromName.ToLower(), table, tableto, cmd, resultcopy);

					var filePath = path + dbFromName + "_" + table.Name + ".sql";
					File.WriteAllText(filePath, resultBeg.ToString());
					File.AppendAllText(filePath, resultcopy.ToString());
					File.AppendAllText(filePath, resultEnd.ToString());
					resultcopy.Clear();
				}
				else
				{
					decimal fsize = 0;
					var filePath = string.Format(newfilePath, num);
					if (allsize == 0)
					{
						File.WriteAllText(filePath, resultBeg.ToString());
						if (result.Length > 0)
						{
							File.AppendAllText(filePath, result.ToString());
							result.Clear();
							fsize = (new FileInfo(filePath)).Length / 1024M;
							allsize = fsize;
						}
					}

					ImportData(table, tableto, cmd, result);

					if (((allsize + (result.Length / 1024M)) > 10240M) && tsize > 0M)
					{
						if (i == 0)
						{
							File.AppendAllText(filePath, result.ToString());
							result.Clear();
						}
						File.AppendAllText(filePath, resultEnd.ToString());
						allsize = 0;
						num++;
					}
					else
					{
						File.AppendAllText(filePath, result.ToString());
						result.Clear();
						fsize = (new FileInfo(filePath)).Length / 1024M;
						allsize = fsize;
					}
					i++;
				}

				Console.WriteLine(@"end");
			}
			result.Clear();
			resultcopy.Clear();

			sqlCon.Close();

			if (allsize > 0M)
			{
				File.AppendAllText(string.Format(newfilePath, num), resultEnd.ToString());
			}

			resultBeg.Clear();
			resultEnd.Clear();

			Console.WriteLine("End");
			Console.ReadKey();
		}

		public static void ImportData(Table tfrom, Table tto, SqlCommand cmd, StringBuilder sqlInsert)
		{
			bool identityInsert = tfrom.Columns.Any(o => o.Value.Identity);

			var clmsfrom = tfrom.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText));
			var clms = clmsfrom.Where(c => tto.Columns.Values.Any(c2 => string.IsNullOrEmpty(c2.ComputedText) && 
																	c2.Name.ToLower() == c.Name.ToLower()));

			sqlInsert.AppendFormat("TRUNCATE TABLE dbo.{0};\r\n", tfrom.Name.ToLower());

			var columns = string.Join(",", clms.Select(c => string.Format("[{0}]", c.Name)).ToArray());

			cmd.CommandText = string.Format("select {0} from [{1}]", columns, tfrom.Name);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i));
					}
					sqlInsert.AppendFormat("INSERT INTO dbo.{0} ({1}) VALUES ({2});\r\n", tfrom.Name.ToLower(), columns.ToLower().Replace("[", "").Replace("]", ""), string.Join(",", sc.Cast<string>().ToArray()));
				}
			}
			if (identityInsert && tfrom.Identity)
			{
				cmd.CommandText = string.Format("select max({0}) from [{1}]", tfrom.Columns.Values.FirstOrDefault(c => c.Identity).Name, tfrom.Name);
				var r = cmd.ExecuteScalar();
				var nextval = r != null && r != DBNull.Value ? int.Parse(r.ToString()) : 0;
				nextval++;
				sqlInsert.AppendFormat("ALTER SEQUENCE dbo.{0}_{1}_seq RESTART WITH {2};\r\n", tfrom.Name.ToLower(), tfrom.Columns.Values.FirstOrDefault(c => c.Identity).Name.ToLower(), nextval);
			}
		}

		public static void ImportData2(string filecopy, string baza, Table tfrom, Table tto, SqlCommand cmd, StringBuilder sqltab)
		{
			StringBuilder sqlcopy = new StringBuilder();
			
			bool identityInsert = tfrom.Columns.Any(o => o.Value.Identity);

			var clmsfrom = tfrom.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText));
			var clms = clmsfrom.Where(c => tto.Columns.Values.Any(c2 => string.IsNullOrEmpty(c2.ComputedText) && 
																	c2.Name.ToLower() == c.Name.ToLower()));

			var columns = string.Join(",", clms.Select(c => string.Format("[{0}]", c.Name)).ToArray());

			sqltab.AppendFormat("TRUNCATE TABLE dbo.{0};\r\n", tfrom.Name.ToLower());
			sqltab.AppendFormat("COPY dbo.{0} ({1}) FROM '{2}_{0}.txt';\r\n", tfrom.Name.ToLower(), columns.ToLower().Replace("[", "").Replace("]", ""), baza);

			cmd.CommandText = string.Format("select {0} from [{1}]", columns, tfrom.Name);

			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue2(reader, i));
					}
					sqlcopy.AppendFormat("{0}\r\n", string.Join("\t", sc.Cast<string>().ToArray()));

					if (sqlcopy.Length > 1000000)
					{
						File.AppendAllText(filecopy, sqlcopy.ToString());
						sqlcopy.Clear();
					}
				}
				if (sqlcopy.Length > 0)
				{
					File.AppendAllText(filecopy, sqlcopy.ToString());
					sqlcopy.Clear();
				}
			}
			if (identityInsert && tfrom.Identity)
			{
				cmd.CommandText = string.Format("select max({0}) from [{1}]", tfrom.Columns.Values.FirstOrDefault(c => c.Identity).Name, tfrom.Name);
				var r = cmd.ExecuteScalar();
				var nextval = r != null && r != DBNull.Value ? int.Parse(r.ToString()) : 0;
				nextval++;
				sqltab.AppendFormat("ALTER SEQUENCE dbo.{0}_{1}_seq RESTART WITH {2};\r\n", tfrom.Name.ToLower(), tfrom.Columns.Values.FirstOrDefault(c => c.Identity).Name.ToLower(), nextval);
			}
		}

		static void DropConstraints(IEnumerable<Table> tables, StringBuilder result)
		{
			foreach (var t in tables)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{

					//if (fk.RefTable.ToLower() == "N_File".ToLower() && fk.Columns.Any(c => c.ToLower() == "FILEID".ToLower() || c.ToLower() == "FILEGUID".ToLower() || c.ToLower() == "PHOTOGUID".ToLower()))
					//	continue;

					var fkname = fk.Name.Length > 63 ? fk.Name.Remove(63) : fk.Name;
					result.AppendFormat("ALTER TABLE {2}.{1} DROP CONSTRAINT IF EXISTS {0};\r\n", fkname.ToLower(), t.Name.ToLower(), t.Schema.Name.ToLower());
				}
			}
		}

		static void CreateConstraints(IEnumerable<Table> tables, StringBuilder result)
		{
			foreach (var t in tables)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{

					//if (fk.RefTable.ToLower() == "N_File".ToLower() && fk.Columns.Any(c => c.ToLower() == "FILEID".ToLower() || c.ToLower() == "FILEGUID".ToLower() || c.ToLower() == "PHOTOGUID".ToLower()))
					//	continue;

					result.AppendFormat("ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};\r\n", t.Name.ToLower(), fk.Name.ToLower(),
						string.Join(",", fk.Columns).ToLower(), fk.RefTable.ToLower(), string.Join(",", fk.RefTableColumns).ToLower(),
						"ON DELETE " + fk.DeleteOption.ToString().ToUpper(), t.Schema.Name.ToLower());
				}
			}
		}

		public static string GetStringValue(SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
				return "null";
			else
			{
				switch (reader.GetDataTypeName(index))
				{
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
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
					case "bit":
						return reader.GetBoolean(index) ? "true" : "false";
					case "nvarchar":
					case "varchar":
					case "nchar":
					case "text": //.Replace("'", "''")
						return string.Format("N'{0}'", reader.GetString(index).Replace("\0", " ").Replace("'", "''"));
					case "char":
						return string.Format("N'{0}'", reader.GetString(index).Replace("\0", " "));
					case "uniqueidentifier":
						return string.Format("CAST('{0}' AS uuid)", reader.GetGuid(index).ToString());
					case "date":
						return string.Format("CAST('{0}' AS date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return string.Format("CAST('{0}' AS timestamp)", reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF")); //, reader.GetSqlDateTime(index).TimeTicks.ToString("X8")
					case "datetime2":
						return string.Format("CAST('{0}' AS timestamp)", reader.GetDateTime(index).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"));
					case "image":
					case "varbinary":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return string.Format("bytea('\\x{0}')", result.ToString());
					case "xml":
						return string.Format("'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}

		public static string GetStringValue2(SqlDataReader reader, int index)
		{
			if (reader.IsDBNull(index))
				return "\\N";
			else
			{
				switch (reader.GetDataTypeName(index))
				{
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
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
					case "bit":
						return reader.GetBoolean(index) ? "t" : "f";
					case "nvarchar":
					case "varchar":
					case "nchar":
					case "text":
						return reader.GetString(index).Replace("\0", " ").Replace("\\", "\\\\").Replace("\r\n", "\\r\\n").Replace("\n", "\\r\\n").Replace("\t", "\\t");
					case "char":
						return reader.GetString(index).Replace("\0", " ");
					case "uniqueidentifier":
						return reader.GetGuid(index).ToString().ToLower();
					case "date":
						return reader.GetDateTime(index).ToString("yyyy-MM-dd");
					case "datetime":
						return reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF");
					case "datetime2":
						return reader.GetDateTime(index).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF");
					case "image":
					case "varbinary":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.AppendFormat("\\x{0}", data[x].ToString("X2"));
						return result.ToString();
					case "xml":
						return reader.GetSqlXml(index).Value;
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
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
	}
}
