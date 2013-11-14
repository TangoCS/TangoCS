using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Nephrite.Meta.Database
{
	public class DBScriptDB2 : IDBScript
	{
		private List<string> _MainScripts { get; set; }
		private List<string> _FkScripts { get; set; }
		private string _SchemaName { get; set; }

		public DBScriptDB2(string schemaName)
		{
			Scripts = new List<string>();
			_SchemaName = schemaName;
		}


		public List<string> Scripts
		{
			get
			{
				_MainScripts.AddRange(_FkScripts);
				return _MainScripts;
			}
			set { }
		}


		public void CreateTable(Table srcTable)
		{

			var tableScript = string.Format("CREATE TABLE \"{2}\".\"{0}\" ({1}) ", srcTable.Name, "{0} ;", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript =
				srcTable.Columns.Aggregate(string.Empty,
										   (current, srcColumn) =>
										   current +
										   (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("\"{0}\" {1} {2} {3},",
														 srcColumn.Value.Name,
														 srcColumn.Value.Type,
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL",
														 srcColumn.Value.IsPrimaryKey && srcTable.Identity ? "  GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )" : ""
														) :
														 string.Format(" {0} as {1} ", srcColumn.Value.Name, srcColumn.Value.ComputedText)
														 )).TrimEnd(',');

			tableScript = string.Format(tableScript, columnsScript);
			if (srcTable.ForeignKeys.Count > 0)
			{

				_FkScripts.Add(srcTable.ForeignKeys.Aggregate(tableScript, (current, foreignKey) => current + string.Format(
									"ALTER TABLE \"{6}\".\"{0}\"  ADD  CONSTRAINT {1} FOREIGN KEY({2}) \r\n" +
									"REFERENCES \"{6}\".\"{3}\" ({4}) {5} ;\r\n",
									srcTable.Name,
									foreignKey.Value.Name,
									string.Join(",", foreignKey.Value.Columns),
									foreignKey.Value.RefTable,
									string.Join(",", foreignKey.Value.RefTableColumns),
									"ON DELETE " + foreignKey.Value.DeleteOption.ToString(),
									_SchemaName)));
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(
								   "ALTER TABLE \"{3}\".\"{0}\"\r\n" +
								   "ADD CONSTRAINT {1} PRIMARY KEY ({2})  ;", srcTable.Name,
													  srcTable.PrimaryKey.Name,
													  string.Join(",", srcTable.PrimaryKey.Columns),
													  _SchemaName);// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}


			_MainScripts.Add(tableScript);
		}

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
					removeForeignKeys.ForEach(r => t.Value.ForeignKeys.Remove(r));
				}


			});


			// Удаляем все ссылки текущей таблицы в обьекте 

			if (currentTable.ForeignKeys != null && currentTable.ForeignKeys.Count > 0)
			{
				currentTable.ForeignKeys.Clear();
			}
			_MainScripts.Add(string.Format("if  OBJECT_ID('{1}.{0}') is not null drop table {1}.{0}  \r\n GO \r\n", currentTable.Name, _SchemaName));
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			throw new NotImplementedException();
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			throw new NotImplementedException();
		}

		public void DeleteColumn(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void ChangeColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			throw new NotImplementedException();
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentity(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public void DeleteView(View currentView)
		{
			throw new NotImplementedException();
		}

		public void CreateView(View srcView)
		{
			throw new NotImplementedException();
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			throw new NotImplementedException();
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			throw new NotImplementedException();
		}

		public void DeleteFunction(Function currentFunction)
		{
			throw new NotImplementedException();
		}

		public void CreateFunction(Function srcFunction)
		{
			throw new NotImplementedException();
		}

		public string GetIntType()
		{
			return string.Format("INTEGER");
		}

		public string GetGuidType()
		{
			return string.Format("varchar()");
		}

		public string GetStringType(int length)
		{
			return string.Format("varchar({0})", length == -1 ? "32000" : length.ToString());
		}

		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("decimal({0},{1})", precision, scale);
		}

		public string GetDateTimeType()
		{
			return "TIMESTAMP";
		}

		public string GetDateType()
		{
			return "TIMESTAMP";
		}

		public string GetZoneDateTimeType()
		{
			return "TIMESTAMP";
		}

		public string GetLongType()
		{
			return "bigint";
		}

		public string GetByteArrayType(int length)
		{
			return "BLOB";
		}

		public string GetBooleanType()
		{
			return "smallint";
		}

		string GetStringValue(SqlDataReader reader, int index)
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
		public string ImportData(Table t, bool identityInsert, SqlConnection DbConnection)
		{

			if (DbConnection.State == System.Data.ConnectionState.Closed)
				DbConnection.Open();

			var columns = string.Join(", ", t.Columns.Values.Select(c => string.Format("{0}", c.Name)).ToArray());
			SqlCommand cmd = DbConnection.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = string.Format("select {0} from [{1}] ", columns, t.Name);

			var sqlInsert = string.Empty;
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					StringCollection sc = new StringCollection();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						sc.Add(GetStringValue(reader, i));
					}
					sqlInsert += string.Format("INSERT INTO {0} ({1})  VALUES ({2}); \r\n", t.Name, columns, string.Join(",", sc.Cast<string>().ToArray<string>()));
				}
			}

			if (identityInsert && t.Identity)
			{
				sqlInsert = string.Format("ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n {2} ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED ALWAYS;", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, sqlInsert);
			}
			return sqlInsert;
		}


		public void AddComputedColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}



		public void DeleteDefaultValue(Column currentColumn)
		{
			throw new NotImplementedException();
		}

		public void AddDefaultValue(Column srcColumn)
		{
			throw new NotImplementedException();
		}



		public void DeleteIndex(Index currentIndex)
		{
			throw new NotImplementedException();
		}


	}
}