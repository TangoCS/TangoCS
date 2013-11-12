using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nephrite.Meta.Database
{

	public class DBScriptMSSQL : IDBScript
	{
		public DBScriptMSSQL()
		{
			Scripts = new List<string>();
		}


		public List<string> Scripts { get; set; }

		public void DeleteTable(Table currentTable)
		{
			//Находим таблицы ссылающиеся на текущую и удаляем их
			var childrenForeignKeys = currentTable.Schema.Tables.Where(t => t.Value.ForeignKeys.Any(f => f.Value.RefTable == currentTable.Name)).SelectMany(t => t.Value.ForeignKeys).ToList();
			if (currentTable.PrimaryKey != null)
			{
				foreach (var foreignKey in childrenForeignKeys)
					DeleteForeignKey(foreignKey.Value);
			}

			//Удаляем FK
			foreach (var constraint in currentTable.ForeignKeys.Values)
			{
				DeleteForeignKey(constraint);
			}

			//Удаляем PK
			if (currentTable.PrimaryKey != null)
			{
				DeletePrimaryKey(currentTable.PrimaryKey);
			}
			// Удаляем все ссылки на текущую таблицу в обьекте 


			currentTable.Schema.Tables.ToList().ForEach(t =>
			{
				if (t.Value.ForeignKeys != null && t.Value.ForeignKeys.Count > 0)
				{
					var removeForeignKeys = t.Value.ForeignKeys.Where(f => f.Value.RefTable == currentTable.Name).Select(f => f.Value.Name).ToList();
					removeForeignKeys.ForEach(r =>
					{
						t.Value.ForeignKeys.Remove(r);
					});
				}


			});


			// Удаляем все ссылки текущей таблицы в обьекте 

			if (currentTable.ForeignKeys != null && currentTable.ForeignKeys.Count > 0)
			{
				currentTable.ForeignKeys.Clear();
			}
			Scripts.Add(string.Format("if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}') drop table {0}  \r\n GO \r\n", currentTable.Name));
		}

		public void CreateTable(Table srcTable)
		{

			var tableScript = string.Format("CREATE TABLE {0} ({1}) ", srcTable.Name, "{0}  \r\n GO \r\n");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript =
				srcTable.Columns.Aggregate(string.Empty,
										   (current, srcColumn) =>
										   current +
										   (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("{0} {1} {2} {3},", srcColumn.Value.Name,
														 srcColumn.Value.Type,
														srcColumn.Value.IsPrimaryKey && srcTable.Identity ? " IDENTITY(1,1)" : "",
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL") :
														 string.Format(" {0} as {1} ", srcColumn.Value.Name, srcColumn.Value.ComputedText)
														 )).TrimEnd(',');

			tableScript = string.Format(tableScript, columnsScript);
			if (srcTable.ForeignKeys.Count > 0)
			{
				tableScript = srcTable.ForeignKeys.Aggregate(tableScript, (current, foreignKey) => current + string.Format(
									"ALTER TABLE {0}  WITH NOCHECK ADD  CONSTRAINT {1} FOREIGN KEY({2}) \r\n" +
									"REFERENCES {3} ({4}) \r\n" +
									"ALTER TABLE {0} CHECK CONSTRAINT {1}  \r\n GO \r\n"
									, srcTable.Name, foreignKey.Value.Name, string.Join(",", foreignKey.Value.Columns), foreignKey.Value.RefTable, string.Join(",", foreignKey.Value.RefTableColumns)));
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(
								   "ALTER TABLE {0}\r\n" +
								   "ADD CONSTRAINT {1} PRIMARY KEY ({2})  \r\n GO \r\n", srcTable.Name,
													  srcTable.PrimaryKey.Name,
													  string.Join(",", srcTable.PrimaryKey.Columns));// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}


			Scripts.Add(tableScript);

		}




		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
		
			var currentTable = srcforeignKey.CurrentTable;
			Scripts.Add(string.Format("ALTER TABLE {0}  WITH NOCHECK ADD  CONSTRAINT {1} FOREIGN KEY({2}) \r\n" +
									  "REFERENCES {3} ({4}); \r\n" +
									  "ALTER TABLE {0} CHECK CONSTRAINT {1};  \r\n GO \r\n", currentTable.Name,
													srcforeignKey.Name,
													string.Join(",", srcforeignKey.Columns),
													srcforeignKey.RefTable,
													string.Join(",", srcforeignKey.RefTableColumns)));
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			var currentTable = currentForeignKey.CurrentTable;
			Scripts.Add(string.Format("IF (OBJECT_ID('{1}', 'F') IS NOT NULL)" +
										"BEGIN \r\n ALTER TABLE {0} DROP CONSTRAINT {1} \r\n END \r\n GO \r\n", currentTable.Name,
										  currentForeignKey.Name));
		}

		public void DeleteColumn(Column currentColumn)
		{

			var currentTable = currentColumn.CurrentTable;
			// При удалении колонки  удаляем  и её pk и fk 
			if (currentTable.PrimaryKey != null && currentTable.PrimaryKey.Columns.Any(t => t == currentColumn.Name))
			{
				DeletePrimaryKey(currentTable.PrimaryKey);
				currentTable.PrimaryKey = null;
			}


			var toRemove = currentTable.ForeignKeys.Where(t => t.Value.Columns.Any(c => c == currentColumn.Name)).Select(t => t.Key).ToArray();
			foreach (var key in toRemove)
			{

				DeleteForeignKey(currentTable.ForeignKeys[key]);
				currentTable.ForeignKeys.Remove(key);
			}
			if (currentColumn.CurrentTable.Name == "CMSFormView")
			{

			}
			if (!string.IsNullOrEmpty(currentColumn.DefaultValue))
			{
				DeleteDefaultValue(currentColumn);
			}
			if (currentColumn.CurrentTable.Indexes != null && currentColumn.CurrentTable.Indexes.Values.Any(t => t.Columns.Any(c => c == currentColumn.Name)))
			{
				foreach (var index in currentColumn.CurrentTable.Indexes)
				{
					if (index.Value.Columns.Any(c => c == currentColumn.Name))
					{
						DeleteIndex(index.Value);
					}
				}

			}
			Scripts.Add(string.Format("ALTER TABLE [{0}] DROP COLUMN [{1}]   \r\n GO \r\n", currentTable.Name,
										  currentColumn.Name));
		}
		public void DeleteDefaultValue(Column currentColumn)
		{
			Scripts.Add(string.Format("IF (OBJECT_ID('{1}') IS NOT NULL)\r\n" +
										"BEGIN\r\n" +
											"ALTER TABLE {0}\r\n" +
											"DROP {1}\r\n" +
										"END\r\n GO \r\n", currentColumn.CurrentTable.Name, "DF_" + currentColumn.CurrentTable.Name + "_" + currentColumn.Name, currentColumn.Name));
		}

		public void AddDefaultValue(Column srcColumn)
		{
			Scripts.Add(string.Format("IF EXISTS (SELECT column_name FROM INFORMATION_SCHEMA.columns WHERE table_name = '{0}' and column_name = '{3}') \r\n" +
											  "ALTER TABLE {0} ADD CONSTRAINT" +
											  " {1} DEFAULT ({2}) FOR {3}  \r\n GO \r\n", srcColumn.CurrentTable.Name,
											  "DF_" + srcColumn.CurrentTable.Name + "_" + srcColumn.Name, srcColumn.DefaultValue, srcColumn.Name));
		}
		public void AddColumn(Column srcColumn)
		{

			var currentTable = srcColumn.CurrentTable;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				AddComputedColumn(srcColumn);
			}
			else
			{
				Scripts.Add(string.Format("if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = '{5}' and column_name = '{0}') \r\n" +
											  "ALTER TABLE [{5}] ADD [{0}] {1} {2} {3} {4}   \r\n GO \r\n",
										srcColumn.Name,
										srcColumn.Type,
										srcColumn.IsPrimaryKey && currentTable.Identity ? "IDENTITY(1,1)" : "",
										srcColumn.Nullable ? "NULL" : "NOT NULL",
										(!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format("DEFAULT({0})", srcColumn.DefaultValue) : ""),
										currentTable.Name));//    // {0}- Название колонки, {1} - Тип колонки, {2} - IDENTITY, {3}- NULL
				if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
				{
					AddDefaultValue(srcColumn);
				}

			}
		}
		public void DeleteIndex(Index currentIndex)
		{
			Scripts.Add(string.Format("DROP INDEX {0} ON {1} \r\n GO \r\n", currentIndex.Name, currentIndex.CurrentTable.Name));
		}

		public void AddComputedColumn(Column srcColumn)
		{
			var currentTable = srcColumn.CurrentTable;

			Scripts.Add(string.Format("if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = '{0}' and column_name = '{1}') \r\n" +
									   "ALTER TABLE {0} ADD {1}  AS ({2}) \r\n GO \r\n",
									currentTable.Name,
									srcColumn.Name, srcColumn.ComputedText));//    // {0}- Название таблицы, {1} - Название колонки, {2} - ComputedText
		}
		public void ChangeColumn(Column srcColumn)
		{

			var currentTable = srcColumn.CurrentTable;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				DeleteColumn(srcColumn);
				AddComputedColumn(srcColumn);
			}
			else
			{
			
				Scripts.Add(string.Format("ALTER TABLE [{0}] \r\n" +
										  "ALTER COLUMN [{1}] {2} {3} {4} \r\n GO \r\n",
											  currentTable.Name,
											  srcColumn.Name,
											  srcColumn.Type,
											  (!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format("DEFAULT({0})", srcColumn.DefaultValue) : ""),
											  srcColumn.Nullable ? "NULL" : "NOT NULL"));
			}
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			var currentTable = currentPrimaryKey.CurrentTable;
			Scripts.Add(string.Format("ALTER TABLE [{0}] DROP CONSTRAINT {1}   \r\n GO \r\n", currentTable.Name,
										  currentPrimaryKey.Name));
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			var curentTable = srcPrimaryKey.CurrentTable;
			var currentTable = srcPrimaryKey.CurrentTable;
			Scripts.Add(string.Format("ALTER TABLE [{0}]\r\n" +
									  "ADD CONSTRAINT {1} PRIMARY KEY ({2})  \r\n GO \r\n", curentTable.Name,
													   srcPrimaryKey.Name,
													   string.Join(",", srcPrimaryKey.Columns)));
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			Scripts.Add(string.Format(" DROP TRIGGER {0}  \r\n GO \r\n ", currentTrigger.Name));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			Scripts.Add(srcTrigger.Text);
		}

		public void DeleteView(View currentView)
		{
			Scripts.Add(string.Format("IF OBJECT_ID ('{0}', 'V') IS NOT NULL DROP VIEW {0}  \r\n GO \r\n", currentView.Name));
		}

		public void CreateView(View srcView)
		{
			Scripts.Add(srcView.Text);
		}

		public void SyncIdentity(Table srcTable)
		{





			if (srcTable.Identity)
			{


				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1}) \r\n GO \r\n", srcTable.Name, "{0} ");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
				var columnsScript =
					srcTable.Columns.Aggregate(string.Empty,
											   (current, srcColumn) =>
											   current +
											   string.Format("{0} {1} {2} {3},", srcColumn.Value.Name,
															 srcColumn.Value.Type,
															 srcTable.Identity && srcColumn.Value.IsPrimaryKey ? "IDENTITY(1,1)" : "",
															 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

				tableScript = string.Format(tableScript, columnsScript);
				Scripts.Add(tableScript);
				Scripts.Add(string.Format("ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE)  \r\n GO \r\n", srcTable.Name));
				Scripts.Add(string.Format("SET IDENTITY_INSERT Tmp_{0} ON \r\n GO \r\n", srcTable.Name));
				Scripts.Add(string.Format("IF EXISTS(SELECT * FROM {0})  \r\n" +
												"EXEC('INSERT INTO Tmp_{0} ({1})  \r\n" +
												"SELECT {1} FROM {0} WITH (HOLDLOCK TABLOCKX)')  \r\n GO \r\n", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray())));
				Scripts.Add(string.Format("SET IDENTITY_INSERT Tmp_{0} OFF DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT'  \r\n GO \r\n", srcTable.Name));
			}
			else
			{

				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1})  ", srcTable.Name, "{0}");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
				var columnsScript =
					srcTable.Columns.Aggregate(string.Empty,
											   (current, srcColumn) =>
											   current +
											   string.Format("{0} {1} {2} {3}, \r\n", srcColumn.Value.Name,
															 srcColumn.Value.Type,
															 "",
															 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

				tableScript = string.Format(tableScript, columnsScript);
				Scripts.Add(tableScript);
				Scripts.Add(string.Format("ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE)\r\n", srcTable.Name));
				Scripts.Add(string.Format("IF EXISTS(SELECT * FROM {0})\r\n" +
												"EXEC('INSERT INTO Tmp_{0} ({1})\r\n" +
												"SELECT {1} FROM {0} WITH (HOLDLOCK TABLOCKX)')\r\n", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray())));
				Scripts.Add(string.Format("DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT'  \r\n GO \r\n", srcTable.Name));
			}


		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			Scripts.Add(
				string.Format(
					"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0}  \r\n GO \r\n",
					currentProcedure.Name));
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			Scripts.Add(srcProcedure.Text);
		}

		public void DeleteFunction(Function currentFunction)
		{
			Scripts.Add(
				string.Format(
					"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0}  \r\n GO \r\n",
					currentFunction.Name));
		}

		public void CreateFunction(Function srcFunction)
		{
			Scripts.Add(srcFunction.Text);
		}

		public string GetIntType()
		{
			return "int";
		}
		public string GetStringType(int length)
		{
			return string.Format("nvarchar({0})", length == -1 ? "max" : length.ToString());
		}
		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("decimal({0},{1})", precision, scale);
		}
		public string GetGuidType()
		{
			return "uniqueidentifier";
		}
		public string GetDateTimeType()
		{
			return "datetime";
		}
		public string GetDateType()
		{
			return "datetime";
		}

		public string GetBooleanType()
		{
			return "bit";
		}

		public string GetZoneDateTimeType()
		{
			return "datetimeoffset(7)";
		}

		public string GetLongType()
		{
			return "bigint";
		}
		public string GetByteArrayType(int length)
		{
			return string.Format("varbinary({0})", length == -1 ? "max" : length.ToString());
		}

		public string ImportData(Table t, bool identityInsert, SqlConnection DbConnection)
		{
			throw new NotImplementedException();
		}
		public string GetStringValue(SqlDataReader reader, int index)
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
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "varchar":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "bit":
						return reader.GetBoolean(index) ? "1" : "0";
					case "uniqueidentifier":
						return "N'" + reader.GetGuid(index).ToString() + "'";
					case "char":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "nchar":
						return "N'" + reader.GetString(index).Replace("'", "''").Replace("\0", " ") + "'";
					case "text":
						return "N'" + reader.GetString(index).Replace("'", "''") + "'";
					case "decimal":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST(0x{0}{1} AS DateTime)", reader.GetSqlDateTime(index).DayTicks.ToString("X8"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "image":
						StringBuilder result = new StringBuilder("0x");
						byte[] data = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data.Length; x++)
							result.Append(data[x].ToString("X2"));
						return result.ToString();
					case "xml":
						return String.Format("N'{0}'", reader.GetSqlXml(index).Value.Replace("'", "''"));
					case "varbinary":
						StringBuilder result1 = new StringBuilder("0x");
						byte[] data1 = reader.GetSqlBytes(index).Value;
						for (int x = 0; x < data1.Length; x++)
							result1.Append(data1[x].ToString("X2"));
						return result1.ToString();
					default:
						throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
				}
			}
		}

	}
}
