﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Tango.Meta.Database
{
	public class DBScriptMSSQL : IDBScript, IDBTypeMapper
	{
		private List<string> _MainScripts { get; set; }
		private List<string> _FkScripts { get; set; }
		private string _SchemaName { get; set; }
		int _defCnt = 0;

		public DBScriptMSSQL(string schemaName)
		{
			_MainScripts = new List<string>();
			_FkScripts = new List<string>();
			_SchemaName = schemaName;
		}


		public void Comment(string comment)
		{
			_MainScripts.Add("-- " + comment);
		}

		public override string ToString()
		{
			var res = new List<string>(_MainScripts.Count + _FkScripts.Count + 10);
			res.Add("BEGIN TRY");
			res.Add("BEGIN TRANSACTION");
			//res.Add("DECLARE @ConstraintName nvarchar(200)");
			res.AddRange(_MainScripts);
			res.AddRange(_FkScripts);
			res.Add("COMMIT TRANSACTION");
			res.Add("print 'Database structure successfully updated!'");
			res.Add("END TRY");
			res.Add("BEGIN CATCH");
			res.Add("ROLLBACK TRANSACTION");
			res.Add("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			res.Add("print ERROR_MESSAGE()");
			res.Add("GOTO RunupEnd");
			res.Add("END CATCH");
			res.Add("RunupEnd:");

			return res.Join("\r\n");
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
			_MainScripts.Add(string.Format("DROP TABLE {1}.[{0}]", currentTable.Name, _SchemaName));
		}

		public void CreateTable(Table srcTable)
		{

			var tableScript = string.Format("CREATE TABLE {2}.[{0}] (\r\n{1}\r\n)", srcTable.Name, "{0}", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript = srcTable.Columns.Aggregate(string.Empty, (current, srcColumn) =>
										   current + (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("\t{0} {1} {2} {3},\r\n", srcColumn.Value.Name,
														 srcColumn.Value.Type.GetDBType(this),
														 srcColumn.Value.IsPrimaryKey && srcTable.Identity ? " IDENTITY(1,1)" : "",
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL") :
														 string.Format(" {0} as {1} ", srcColumn.Value.Name, srcColumn.Value.ComputedText)
														 )).TrimEnd(new char[] {',', '\r', '\n'});

			tableScript = string.Format(tableScript, columnsScript);
			_MainScripts.Add(tableScript);
			if (srcTable.ForeignKeys.Count > 0)
			{
				var result = srcTable.ForeignKeys.Aggregate("", (current, key) => current + string.Format("ALTER TABLE {5}.[{0}] WITH CHECK ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {5}.{3} ({4})", srcTable.Name, key.Value.Name, string.Join(",", key.Value.Columns), key.Value.RefTable, string.Join(",", key.Value.RefTableColumns), _SchemaName));
				_FkScripts.Add(result);
			}

			if (srcTable.PrimaryKey != null)
			{

				_MainScripts.Add(string.Format(
								   "ALTER TABLE {3}.[{0}] ADD CONSTRAINT {1} PRIMARY KEY ({2})", srcTable.Name,
													  srcTable.PrimaryKey.Name,
													  string.Join(",", srcTable.PrimaryKey.Columns), _SchemaName));// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}

			var defaultColumns = srcTable.Columns.Where(t => !string.IsNullOrEmpty(t.Value.DefaultValue));
			if (defaultColumns.Any())
			{
				foreach (var defaultColumn in defaultColumns)
				{
					AddDefaultValue(defaultColumn.Value);
				}
			}
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{

			var currentTable = srcforeignKey.Table;
			_FkScripts.Add(string.Format("ALTER TABLE {5}.[{0}] WITH NOCHECK ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {5}.{3} ({4}) ON UPDATE NO ACTION ON DELETE {6}", 
													currentTable.Name,
													srcforeignKey.Name,
													string.Join(",", srcforeignKey.Columns),
													srcforeignKey.RefTable,
													string.Join(",", srcforeignKey.RefTableColumns),
													_SchemaName,
													srcforeignKey.DeleteOption == DeleteOption.Cascade ? "CASCADE" : 
													(srcforeignKey.DeleteOption == DeleteOption.SetNull ? "SET NULL" : "NO ACTION")));
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			var currentTable = currentForeignKey.Table;
			_FkScripts.Add(string.Format("ALTER TABLE {2}.[{0}] DROP CONSTRAINT {1}", currentTable.Name,
										  currentForeignKey.Name, _SchemaName));
		}

		public void DeleteColumn(Column currentColumn)
		{

			var currentTable = currentColumn.Table;
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

			if (!string.IsNullOrEmpty(currentColumn.DefaultValue))
			{
				DeleteDefaultValue(currentColumn);
			}
			if (currentColumn.Table.Indexes != null && currentColumn.Table.Indexes.Values.Any(t => t.Columns.Any(c => c == currentColumn.Name)))
			{
				foreach (var index in currentColumn.Table.Indexes)
				{
					if (index.Value.Columns.Any(c => c == currentColumn.Name))
					{
						DeleteIndex(index.Value);
					}
				}

			}
			_MainScripts.Add(string.Format("ALTER TABLE {2}.[{0}] DROP COLUMN [{1}] ", currentTable.Name, currentColumn.Name, _SchemaName));
		}

		public void DeleteDefaultValue(Column currentColumn)
		{
			string c = "C" + _defCnt.ToString();
			_defCnt++;
			_MainScripts.Add(string.Format("DECLARE @{0} nvarchar(200)", c));
			_MainScripts.Add(string.Format("SELECT @{3} = Name FROM SYS.DEFAULT_CONSTRAINTS WHERE PARENT_OBJECT_ID = OBJECT_ID('{1}.[{0}]') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = '{2}' AND object_id = OBJECT_ID('{1}.[{0}]'))", currentColumn.Table.Name, _SchemaName, currentColumn.Name, c));
			_MainScripts.Add(string.Format("IF @{2} IS NOT NULL EXEC('ALTER TABLE {1}.[{0}] DROP CONSTRAINT ' + @{2})", currentColumn.Table.Name, _SchemaName, c));

			//_MainScripts.Add(string.Format("ALTER TABLE {2}.[{0}] DROP {1}", currentColumn.CurrentTable.Name, "DF_" + currentColumn.CurrentTable.Name + "_" + currentColumn.Name, _SchemaName));
		}

		public void AddDefaultValue(Column srcColumn)
		{
			_MainScripts.Add(string.Format("ALTER TABLE {4}.[{0}] ADD CONSTRAINT {1} DEFAULT ({2}) FOR {3}", srcColumn.Table.Name,
											  "DF_" + srcColumn.Table.Name + "_" + srcColumn.Name, srcColumn.DefaultValue, srcColumn.Name, _SchemaName));
		}

		public void AddColumn(Column srcColumn)
		{

			var currentTable = srcColumn.Table;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				AddComputedColumn(srcColumn);
			}
			else
			{
				_MainScripts.Add(string.Format("ALTER TABLE {5}.[{4}] ADD [{0}] {1} {2} {3}",
										srcColumn.Name,
										srcColumn.Type.GetDBType(this),
										srcColumn.IsPrimaryKey && currentTable.Identity ? "IDENTITY(1,1)" : "",
										srcColumn.Nullable ? "NULL" : "NOT NULL",
										
										currentTable.Name, _SchemaName));//    // {0}- Название колонки, {1} - Тип колонки, {2} - IDENTITY, {3}- NULL
				if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
				{
					AddDefaultValue(srcColumn);
				}

			}
		}

		public void DeleteIndex(Index currentIndex)
		{
			_MainScripts.Add(string.Format("DROP INDEX {2} ON {0}.[{1}]", _SchemaName, currentIndex.Table.Name, currentIndex.Name));
		}

		public void AddComputedColumn(Column srcColumn)
		{
			var currentTable = srcColumn.Table;

			_MainScripts.Add(string.Format("ALTER TABLE {3}.[{0}] ADD [{1}] AS ({2})",
									currentTable.Name,
									srcColumn.Name,
									srcColumn.ComputedText,
									_SchemaName));//    // {0}- Название таблицы, {1} - Название колонки, {2} - ComputedText
		}

		public void ChangeColumn(Column srcColumn)
		{

			var currentTable = srcColumn.Table;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				DeleteColumn(srcColumn);
				AddComputedColumn(srcColumn);
			}
			else
			{
				_MainScripts.Add(string.Format("ALTER TABLE {4}.[{0}] ALTER COLUMN [{1}] {2} {3}",
											  currentTable.Name,
											  srcColumn.Name,
											  srcColumn.Type.GetDBType(this),
											  
											  srcColumn.Nullable ? "NULL" : "NOT NULL",
											  _SchemaName));
			}
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			var currentTable = currentPrimaryKey.Table;
			_MainScripts.Add(string.Format("ALTER TABLE {2}.[{0}] DROP CONSTRAINT {1}", currentTable.Name,
										  currentPrimaryKey.Name,
										  _SchemaName));
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			var curentTable = srcPrimaryKey.Table;
			_MainScripts.Add(string.Format("ALTER TABLE {3}.[{0}] ADD CONSTRAINT {1} PRIMARY KEY ({2})", curentTable.Name,
													   srcPrimaryKey.Name,
													   string.Join(",", srcPrimaryKey.Columns),
													   _SchemaName));
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			_MainScripts.Add(string.Format("DROP TRIGGER {1}.{0}", currentTrigger.Name, _SchemaName));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			_MainScripts.Add(srcTrigger.Text);
		}

		public void DeleteView(View currentView)
		{
			_MainScripts.Add(string.Format("DROP VIEW {0}", currentView.Name, _SchemaName));
		}

		public void CreateView(View srcView)
		{
			_MainScripts.Add(srcView.Text);
		}

		public void SyncIdentityColumn(Column srcColumn)
		{
		}

		public void SyncIdentity(Table srcTable)
		{
			if (srcTable.Identity)
			{
				var tableScript = string.Format("CREATE TABLE {2}.Tmp_{0} ({1})", srcTable.Name, "{0} ", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
				var columnsScript =
					srcTable.Columns.Aggregate(string.Empty,
											   (current, srcColumn) =>
											   current +
											   string.Format("{0} {1} {2} {3},", srcColumn.Value.Name,
															 srcColumn.Value.Type.GetDBType(this),
															 srcTable.Identity && srcColumn.Value.IsPrimaryKey ? "IDENTITY(1,1)" : "",
															 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

				tableScript = string.Format(tableScript, columnsScript);
				_MainScripts.Add(tableScript);
				_MainScripts.Add(string.Format("ALTER TABLE {1}.Tmp_{0} SET (LOCK_ESCALATION = TABLE)\r\n", srcTable.Name, _SchemaName));
				_MainScripts.Add(string.Format("SET IDENTITY_INSERT {1}.Tmp_{0} ON \r\n", srcTable.Name, _SchemaName));
				_MainScripts.Add(string.Format("IF EXISTS(SELECT * FROM {2}.{0})  \r\n" +
												"EXEC('INSERT INTO {2}.Tmp_{0} ({1})  \r\n" +
												"SELECT {1} FROM {2}.{0} WITH (HOLDLOCK TABLOCKX)')", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray()), _SchemaName));
				_MainScripts.Add(string.Format("SET IDENTITY_INSERT {1}.Tmp_{0} OFF DROP TABLE {1}.{0}; EXECUTE sp_rename N'{1}.Tmp_{0}', N'{1}.{0}', 'OBJECT'", srcTable.Name));
			}
			else
			{

				var tableScript = string.Format("CREATE TABLE {2}.Tmp_{0} ({1})  ", srcTable.Name, "{0}", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
				var columnsScript =
					srcTable.Columns.Aggregate(string.Empty,
											   (current, srcColumn) =>
											   current +
											   string.Format("{0} {1} {2} {3}, \r\n", srcColumn.Value.Name,
															 srcColumn.Value.Type,
															 "",
															 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

				tableScript = string.Format(tableScript, columnsScript);
				_MainScripts.Add(tableScript);
				_MainScripts.Add(string.Format("ALTER TABLE {1}.Tmp_{0} SET (LOCK_ESCALATION = TABLE)\r\n", srcTable.Name, _SchemaName));
				_MainScripts.Add(string.Format("IF EXISTS(SELECT * FROM {2}.{0})\r\n" +
												"EXEC('INSERT INTO {2}.Tmp_{0} ({1})\r\n" +
												"SELECT {1} FROM {2}.{0} WITH (HOLDLOCK TABLOCKX)')\r\n", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray()), _SchemaName));
				_MainScripts.Add(string.Format("DROP TABLE {1}.{0}; EXECUTE sp_rename N'{1}.Tmp_{0}', N'{1}.{0}', 'OBJECT'", srcTable.Name, _SchemaName));
			}
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			_MainScripts.Add(string.Format("DROP PROCEDURE {1}.{0}", currentProcedure.Name, _SchemaName));
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			_MainScripts.Add(srcProcedure.Text);
		}

		public void DeleteFunction(Function currentFunction)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION {1}.{0}", currentFunction.Name, _SchemaName));
		}

		public void CreateFunction(Function srcFunction)
		{
			_MainScripts.Add(srcFunction.Text);
		}

		public void DeleteTableFunction(TableFunction currentFunction)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION {1}.{0}", currentFunction.Name, _SchemaName));
		}

		public void CreateTableFunction(TableFunction srcFunction)
		{
			_MainScripts.Add(srcFunction.Text);
		}

		public string GetIntType()
		{
			return "int";
		}
		public string GetStringType(int length)
		{
			return string.Format("nvarchar({0})", length < 1 ? "max" : length.ToString());
		}
		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("decimal({0},{1})", precision, scale);
		}
		public string GetMoneyType()
		{
			return "money";
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

	    public string GetXmlType()
	    {
            return "xml";
	    }

	    public string GetZoneDateTimeType()
		{
			return "datetimeoffset(7)";
		}

		public string GetLongType()
		{
			return "bigint";
		}
		public string GetShortType()
		{
			return "smallint";
		}
		public string GetByteArrayType(int length)
		{
			return string.Format("varbinary({0})", length < 1 ? "max" : length.ToString());
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
					case "numeric":
						return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
					case "date":
						return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
					case "datetime":
						return String.Format("CAST(0x{0}{1} AS DateTime)", reader.GetSqlDateTime(index).DayTicks.ToString("X8"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
					case "datetime2":
						return String.Format("CAST(0x{0}{1} AS DateTime2)", reader.GetSqlDateTime(index).DayTicks.ToString("X8"), reader.GetSqlDateTime(index).TimeTicks.ToString("X8"));
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

		//public XElement GetMeta(string connectionString)
		//{
		//	using (var con = new SqlConnection(connectionString))
		//	{
		//		using (var cmd = new SqlCommand("EXEC [dbo].[usp_model]", con))
		//		{
		//			con.Open();
		//			using (XmlReader reader = cmd.ExecuteXmlReader())
		//			{
		//				while (reader.Read())
		//				{
		//					var s = reader.ReadOuterXml();
		//					return XElement.Parse(s);
		//				}
		//			}
		//		}
		//	}
		//	return null;
		//}

		public XDocument GetSchemaFromDatabase(string connectionString)
		{
			XDocument doc = null;
			using (var con = new SqlConnection(connectionString))
			{
				using (var cmd = new SqlCommand("EXEC dbo.usp_dbschema @s", con))
				{
					con.Open();
					cmd.Parameters.Add(new SqlParameter("s", _SchemaName));
					using (var reader = cmd.ExecuteXmlReader())
					{
						while (reader.Read())
						{
							string s = reader.ReadOuterXml();
							doc = XDocument.Parse(s);
						}
					}
				}
			}
			return doc;
		}


		#region IDBScript Members


		public IMetaPrimitiveType GetType(string dataType, bool notNull)
		{
			var type = dataType.Contains("(") ? dataType.Substring(0, dataType.IndexOf("(", StringComparison.Ordinal)) : dataType;
			var precision = string.Empty;
			var scale = string.Empty;
			var match = Regex.Match(dataType, @"\((.*?)\)");

			if (match.Groups.Count > 1)
			{
				var value = match.Groups[1].Value;
				string[] arrayVal = value.Split(',');
				precision = arrayVal[0];
				if (arrayVal.Length > 1)
				{
					scale = arrayVal[1];
				}
			}
			switch (type)
			{
				case "int":
					return TypeFactory.Int;
				case "nvarchar":
				case "varchar":
					return string.IsNullOrEmpty(precision) ?
							TypeFactory.CustomString(-1) :
							TypeFactory.CustomString(Int32.Parse(precision == "max" ? "-1" : precision));
				case "decimal":
					return TypeFactory.CustomDecimal(Int32.Parse(precision), Int32.Parse(scale));
				case "numeric":
					return TypeFactory.CustomDecimal(Int32.Parse(precision), Int32.Parse(scale));
				case "money":
					return TypeFactory.Money;
				case "uniqueidentifier":
					return TypeFactory.Guid;
				case "datetime":
				case "datetime2":
					return TypeFactory.DateTime;
				case "date":
					return TypeFactory.Date;
				case "bit":
					return TypeFactory.Boolean;
				case "smallint":
				case "tinyint":
					return TypeFactory.Short;
				case "bigint":
					return TypeFactory.Long;
				case "char":
					return TypeFactory.CustomString(precision.IsEmpty() ? 1 : Int32.Parse(precision));
				case "varbinary":
					var s = precision.ToLower() == "max" || precision.IsEmpty() ? "-1" : precision;
					if (!Int32.TryParse(s, out var n))
						throw new Exception("Unknown precision: " + s);
					return TypeFactory.CustomByteArray(n);
				case "image":
					return TypeFactory.ByteArray;
				case "xml":
					return TypeFactory.Xml;
				default:
					return TypeFactory.String;
			}

		}

		#endregion
	}
}
