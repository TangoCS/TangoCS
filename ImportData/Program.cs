using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;
using Nephrite.Meta.Database;
using Nephrite.Web;

namespace ImportData
{
	public class nfile
	{
		public byte[] data { get; set; }
		public string ext { get; set; }
		public string guid { get; set; }
	}

	static class Program
	{
		static void Main(string[] args)
		{

			var connectFrom = ConfigurationSettings.AppSettings["ConnectionFrom"];
			var connectTo = ConfigurationSettings.AppSettings["ConnectionTo"];
			var tabelesForInmport = ConfigurationSettings.AppSettings["TablesForImport"];
			var result = new StringBuilder();

			var tableName = string.Empty;
			if (string.IsNullOrEmpty(connectFrom) || string.IsNullOrEmpty(connectTo) || string.IsNullOrEmpty(connectTo))
			{
				Console.Write(@"Некорректные параметры");
				Console.ReadKey();
				return;
			}

			var tabelListString = tabelesForInmport.Split(',');
			// try
			// {
			ConnectionManager.SetConnectionString(connectFrom);
			SqlConnection sqlCon = new SqlConnection(connectFrom);
			var schemaFrom = new SqlServerMetadataReader().ReadSchema("DBO");


			var tabelListObjects = schemaFrom.Tables.Values.Where(t => tabelListString.Any(c => t.Name.ToUpper() == c.ToUpper()) || t.ForeignKeys.Any(f => tabelListString.Any(l => l.ToUpper() == f.Value.RefTable.ToUpper())));
			Constraints(false, tabelListObjects, result);

			DB2Connection db2Con = new DB2Connection(connectTo);
			db2Con.Open();
			foreach (var table in tabelListObjects.Where(t => tabelListString.Any(c => t.Name.ToUpper() == c.ToUpper())))
			{
				tableName = table.Name;
				Console.WriteLine(@"Таблица " + table.Name + " start");
				if (table.Name == "N_FileData")
				{
					result.Clear();
					var isdo = true;
					var page = 1;

					result.AppendFormat("SET INTEGRITY FOR  {1}.{0} ALL IMMEDIATE UNCHECKED;\r\n", table.Name.ToUpper(), "DBO");
					result.AppendFormat("CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' );\r\n", table.Name.ToUpper(), "DBO");
					result.AppendFormat("DELETE FROM  {1}.{0} ;\r\n", table.Name.ToUpper(), "DBO");

					while (isdo)
					{

						var list = ImportData(table, table.Columns.Any(t => t.Value.IsPrimaryKey), sqlCon, result, 10, page, ref isdo);
						Console.WriteLine("Записи с   " + (10 * page - 10) + "по " + (10 * page));
						foreach (var nfile in list)
						{
							Console.WriteLine(nfile.guid);


							DB2Command cmd2 = db2Con.CreateCommand();
							cmd2.CommandType = System.Data.CommandType.StoredProcedure;
							cmd2.CommandText = "DBO.INSERTNFILEd";
							cmd2.Parameters.Add("Data", nfile.data);
							cmd2.Parameters.Add("Extension", nfile.ext);
							cmd2.Parameters.Add("FileGUID", nfile.guid);
							cmd2.ExecuteReader();


						}


						page++;
					}


					continue;
				}

				////////////////////// Получаем данные
				int itemsCount = 0;
				ImportData(table, table.Columns.Any(t => t.Value.IsPrimaryKey), sqlCon, result, ref itemsCount);

				////////////////////// Резет счётчика
				if (table.Identity)
				{
					var columnIdenity = table.Columns.Values.FirstOrDefault(t => t.Identity);
					if (columnIdenity != null)
					{
						result.AppendFormat("ALTER TABLE DBO.{0} ALTER COLUMN {1}  RESTART WITH {2} ;\n\r", table.Name, columnIdenity.Name, itemsCount + 1);
					}
				}

				DB2Connection db2con = new DB2Connection(connectTo);
				db2con.Open();
				DB2Command cmd = db2con.CreateCommand();
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = result.ToString();
				cmd.ExecuteReader();
				result.Clear();
				Console.WriteLine(@"Таблица " + table.Name + " end");




			}

			db2Con.Close();

			Console.WriteLine("End");
			Console.ReadKey();
		}
		static void Constraints(bool enable, IEnumerable<Table> tables, StringBuilder result)
		{
			foreach (var t in tables)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{

					if (
						(fk.RefTable.ToUpper() == "N_File".ToUpper() && fk.Columns.Any(c => c.ToUpper() == "FILEID".ToUpper() || c.ToUpper() == "FILEGUID".ToUpper() || c.ToUpper() == "PHOTOGUID".ToUpper())))
						continue;

					{
						result.AppendFormat("ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} {3} ;\r\n", fk.Name.ToUpper(), t.Name.ToUpper(),
											t.Schema.Name.ToUpper(), enable ? "ENFORCED" : "NOT ENFORCED");

					}
				}
			}

		}
		public static string ImportData(Table t, bool identityInsert, SqlConnection DbConnection, StringBuilder sqlInsert, ref int countItems)
		{

			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = string.Format("select {0} from [{1}] ", columns, t.Name);

			sqlInsert.AppendFormat("SET INTEGRITY FOR  {1}.{0} ALL IMMEDIATE UNCHECKED;\r\n", t.Name.ToUpper(), "DBO");
			sqlInsert.AppendFormat("CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' );\r\n", t.Name.ToUpper(), "DBO");
			sqlInsert.AppendFormat("DELETE FROM  {1}.{0} ;\r\n", t.Name.ToUpper(), "DBO");
			if (identityInsert && t.Identity)
			{
				sqlInsert.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, "DBO");
			}
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i).Replace("\\", ""));
					}
					countItems++;
					sqlInsert.AppendFormat("INSERT INTO DBO.{0} ({1})  VALUES ({2}); \r\n", t.Name, columns.Replace("[", "").Replace("]", ""), string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}

			return sqlInsert.ToString();
		}
		public static string ImportCustomData(Table t, bool identityInsert, SqlConnection sqlConnection, StringBuilder db2Insert, int count, int page, ref bool isdo, ref List<DB2Parameter> listParametrs)
		{
			var list = new List<nfile>();
			isdo = false;
			page = page - 1;
			if (sqlConnection.State == System.Data.ConnectionState.Closed)
				sqlConnection.Open();

			var columnsArray = t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray();

			var columnsin = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : "m." + c.Name)).ToArray());
			SqlCommand cmd = sqlConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			var sqltext = string.Format("select  {0} from (SELECT {4} ,ROW_NUMBER()  OVER (ORDER BY [{5}]) AS rn " +
								"FROM {1} ) as m where m.rn>{2} and m.rn<={3}", columns, t.Name, page * count, (page * count) + count, columnsin, t.Columns.First().Key);
			cmd.CommandText = sqltext;


			if (identityInsert && t.Identity)
			{
				db2Insert.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, "DBO");
			}
			using (var reader = cmd.ExecuteReader())
			{
				int parametersName = 0;
				while (reader.Read())
				{
					isdo = true;

					db2Insert.AppendFormat("INSERT INTO DBO.{0} ({1})  VALUES ({2}); \r\n", t.Name, columnsin.Replace("[", "").Replace("]", ""),
					 string.Join(",", columnsArray.Select(c => "@" + parametersName + c)));
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						listParametrs.Add(new DB2Parameter(parametersName + reader.GetName(i), GetStringValue(reader, i).Replace("\\", "")));
					}
					parametersName++;
				}
			}

			return db2Insert.ToString();
		}
		public static List<nfile> ImportData(Table t, bool identityInsert, SqlConnection DbConnection, StringBuilder sqlInsert, int count, int page, ref bool isdo)
		{
			var list = new List<nfile>();
			isdo = false;
			page = page - 1;
			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columnsin = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : c.Name)).ToArray());
			var columns = string.Join(", ", t.Columns.Values.Where(c => string.IsNullOrEmpty(c.ComputedText)).Select(c => string.Format("{0}", c.Name == "Group" ? "[Group]" : c.Name == "Index" ? "[Index]" : "m." + c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			var sqltext = string.Format("select  {0} from (SELECT {4} ,ROW_NUMBER()  OVER (ORDER BY [FileGUID]) AS rn " +
								"FROM {1} ) as m where m.rn>{2} and m.rn<={3}", columns, t.Name, page * count, (page * count) + count, columnsin);
			cmd.CommandText = sqltext;


			if (identityInsert && t.Identity)
			{
				sqlInsert.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, "DBO");
			}
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					isdo = true;
					StringCollection sc = new StringCollection();
					var nfile = new nfile();
					nfile.data = (byte[])reader["Data"];
					nfile.ext = reader["Extension"].ToString();
					nfile.guid = reader["FileGUID"].ToString();
					list.Add(nfile);
				}
			}

			return list;
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
				return string.Format("CAST({0}' as CLOB) || '{1}", arrayText.First(),
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
						return reader.GetBoolean(index) ? "1" : "0";
					case "uniqueidentifier":
						return "N'" + reader.GetGuid(index).ToString() + "'";
					case "char":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "nchar":
						return ("N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'").CuttingText(); ;
					case "text":
						return ("N'" + reader.GetString(index).Replace("'", "''") + "'").CuttingText(); ;
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST('{0}' AS timestamp)", reader.GetSqlDateTime(index).Value.ToString("yyyy-MM-dd HH:mm:ss"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "image":
						StringBuilder result = new StringBuilder();
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result.ToString());
					case "xml":
						return String.Format("N'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					case "varbinary":
						StringBuilder result1 = new StringBuilder();
						byte[] data1 = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data1.Length; x++)
							result1.Append(data1[x].ToString("X2"));
						return string.Format("blob(X'{0}')", result1.ToString());
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}

	}
}
