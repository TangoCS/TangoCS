﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nephrite.Meta.Database
{
	public class DBScriptPostgreSQL : IDBScript
	{
		private List<string> _MainScripts { get; set; }
		private List<string> _FkScripts { get; set; }
		private string _SchemaName { get; set; }

		public DBScriptPostgreSQL(string schemaName)
        {
            _MainScripts = new List<string>();
            _FkScripts = new List<string>();
            _SchemaName = schemaName.ToLower();
        }

		public override string ToString()
		{
			var res = new List<string>(_MainScripts.Count + _FkScripts.Count + 8);
			res.Add("DO LANGUAGE plpgsql");
			res.Add("$$");
			res.Add("BEGIN");
			res.AddRange(_MainScripts);
			res.AddRange(_FkScripts);
			res.Add("EXCEPTION WHEN OTHERS THEN");
			res.Add("RAISE EXCEPTION 'Error state: %, Error message: %', SQLSTATE, SQLERRM;");
			res.Add("RAISE NOTICE 'Database structure successfully updated!';");
			res.Add("END;");
			res.Add("$$");

			return res.Join("\r\n");
		}

		public void Comment(string comment)
		{
			_MainScripts.Add("-- " + comment);
		}

		public void CreateTable(Table srcTable)
		{
			var tableScript = string.Format("CREATE TABLE {2}.{0} ({1});", srcTable.Name.ToLower(), "{0}", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript =
				srcTable.Columns.Aggregate(string.Empty,
										   (current, srcColumn) =>
										   current +
										   (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("\t{0} {1} {2} {3},\r\n",
														 srcColumn.Value.Name.ToLower(),
														 srcColumn.Value.Identity ? "serial" : srcColumn.Value.Type.GetDBType(this),
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL",
														 (!srcColumn.Value.Identity && !string.IsNullOrEmpty(srcColumn.Value.DefaultValue) ? string.Format(" DEFAULT {0}", GetDefaultValue(srcColumn.Value.DefaultValue, srcColumn.Value.Type.GetDBType(this))) : "")
														) : ""
														 )).Trim().TrimEnd(',');

			tableScript = string.Format(tableScript, columnsScript);
			if (srcTable.ForeignKeys.Count > 0)
			{
				var result = srcTable.ForeignKeys.Aggregate("", (current, key) => current + string.Format("ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};", srcTable.Name.ToLower(), key.Value.Name.ToLower(), string.Join(",", key.Value.Columns).ToLower(), key.Value.RefTable.ToLower(), string.Join(",", key.Value.RefTableColumns).ToLower(), "ON DELETE " + key.Value.DeleteOption.ToString().ToUpper(), _SchemaName));
				_FkScripts.Add(result);
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(
								   "\r\nALTER TABLE {3}.{0} ADD CONSTRAINT {1} PRIMARY KEY ({2});", srcTable.Name.ToLower(),
													  srcTable.PrimaryKey.Name.ToLower(),
													  string.Join(",", srcTable.PrimaryKey.Columns).ToLower(),
													  _SchemaName);// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}
			
			_MainScripts.Add(tableScript);
			foreach (var srcColumn in srcTable.Columns.Where(o => !string.IsNullOrEmpty(o.Value.ComputedText)).Select(o => o.Value))
			{
				AddComputedColumn(srcColumn);
			}
		}

		public void DeleteTable(Table currentTable)
		{
			//Находим таблицы ссылающиеся на текущую и удаляем их
			var childrenForeignKeys = currentTable.Schema.Tables.Where(t => t.Value.ForeignKeys.Any(f => f.Value.RefTable.ToLower() == currentTable.Name.ToLower())).SelectMany(t => t.Value.ForeignKeys).ToList();
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
					var removeForeignKeys = t.Value.ForeignKeys.Where(f => f.Value.RefTable.ToLower() == currentTable.Name.ToLower()).Select(f => f.Value.Name.ToLower()).ToList();
					removeForeignKeys.ForEach(r => t.Value.ForeignKeys.Remove(r));
				}


			});

			// Удаляем все ссылки текущей таблицы в обьекте 

			if (currentTable.ForeignKeys != null && currentTable.ForeignKeys.Count > 0)
			{
				currentTable.ForeignKeys.Clear();
			}

			foreach (var srcColumn in currentTable.Columns.Where(o => !string.IsNullOrEmpty(o.Value.ComputedText)).Select(o => o.Value))
			{
				DeleteComputedColumn(srcColumn);
			}
			_MainScripts.Add(string.Format("DROP TABLE IF EXISTS {1}.{0};", currentTable.Name.ToLower(), _SchemaName));
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
			var srcTable = srcforeignKey.Table;
			_FkScripts.Add(
				string.Format(
					"ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};", srcTable.Name.ToLower(), srcforeignKey.Name.ToLower(), string.Join(",", srcforeignKey.Columns).ToLower(),
					srcforeignKey.RefTable.ToLower(), string.Join(",", srcforeignKey.RefTableColumns).ToLower(), "ON DELETE " + srcforeignKey.DeleteOption.ToString().ToUpper(),
					_SchemaName));
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			var currentTable = currentForeignKey.Table;
			_FkScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP CONSTRAINT IF EXISTS {1};", currentTable.Name.ToLower(), currentForeignKey.Name.ToLower(), _SchemaName));
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{
			var currentTable = currentPrimaryKey.Table;
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP CONSTRAINT IF EXISTS {1};", currentTable.Name.ToLower(), currentPrimaryKey.Name.ToLower(), _SchemaName));
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			var curentTable = srcPrimaryKey.Table;
			_MainScripts.Add(string.Format("ALTER TABLE {3}.{0} ADD CONSTRAINT {1} PRIMARY KEY ({2});", curentTable.Name.ToLower(), srcPrimaryKey.Name.ToLower(),
													   string.Join(",", srcPrimaryKey.Columns).ToLower(), _SchemaName));
		}

		public void DeleteColumn(Column currentColumn)
		{
			var currentTable = currentColumn.Table;
			// При удалении колонки  удаляем  и её pk и fk 
			if (currentTable.PrimaryKey != null && currentTable.PrimaryKey.Columns.Any(t => t.ToLower() == currentColumn.Name.ToLower()))
			{
				DeletePrimaryKey(currentTable.PrimaryKey);
				currentTable.PrimaryKey = null;
			}

			var toRemove = currentTable.ForeignKeys.Where(t => t.Value.Columns.Any(c => c.ToLower() == currentColumn.Name.ToLower())).Select(t => t.Key.ToLower()).ToArray();
			foreach (var key in toRemove)
			{

				DeleteForeignKey(currentTable.ForeignKeys[key]);
				currentTable.ForeignKeys.Remove(key);
			}

			if (!currentColumn.Identity && !string.IsNullOrEmpty(currentColumn.DefaultValue))
			{
				DeleteDefaultValue(currentColumn);
			}

			if (currentColumn.Table.Indexes != null && currentColumn.Table.Indexes.Values.Any(t => t.Columns.Any(c => c.ToLower() == currentColumn.Name.ToLower())))
			{
				foreach (var index in currentColumn.Table.Indexes)
				{
					if (index.Value.Columns.Any(c => c.ToLower() == currentColumn.Name.ToLower()))
					{
						DeleteIndex(index.Value);
					}
				}

			}

			_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP COLUMN IF EXISTS {1};", currentTable.Name.ToLower(), currentColumn.Name.ToLower(), _SchemaName));
			if (!string.IsNullOrEmpty(currentColumn.ComputedText))
			{
				DeleteComputedColumn(currentColumn);
			}
		}

		public void AddColumn(Column srcColumn)
		{
			var srcTable = srcColumn.Table;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				AddComputedColumn(srcColumn);
			}
			else
			{
				_MainScripts.Add(string.Format("ALTER TABLE {5}.{4} ADD {0} {1} {2} {3};",
										srcColumn.Name.ToLower(),
										srcColumn.Identity ? "serial" : srcColumn.Type.GetDBType(this),
										srcColumn.Nullable ? "NULL" : "NOT NULL",
									   (!srcColumn.Identity && !string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format(" DEFAULT {0}", GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))) : ""),
										srcTable.Name.ToLower(), _SchemaName));
			}
		}

		public void ChangeColumn(Column srcColumn)
		{
			var srcTable = srcColumn.Table;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP COLUMN IF EXISTS {1};", srcColumn.Table.Name.ToLower(), srcColumn.Name.ToLower(), _SchemaName));
				DeleteComputedColumn(srcColumn);
				AddComputedColumn(srcColumn);
			}
			else
			{
				_MainScripts.Add(string.Format("ALTER TABLE {3}.{0} ALTER COLUMN {1} TYPE {2};",
											  srcTable.Name.ToLower(),
											  srcColumn.Name.ToLower(),
											  srcColumn.Type.GetDBType(this),
											  _SchemaName));
				if (srcColumn.Nullable)
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} DROP NOT NULL;",
												   srcTable.Name.ToLower(),
												   srcColumn.Name.ToLower(),
												   _SchemaName));
				}
				else
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET NOT NULL;",
													  srcTable.Name.ToLower(),
													  srcColumn.Name.ToLower(),
													  _SchemaName));

				}
				if (!srcColumn.Identity && !string.IsNullOrEmpty(srcColumn.DefaultValue))
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET DEFAULT {3};",
													  srcTable.Name.ToLower(),
													  srcColumn.Name.ToLower(),
													  _SchemaName,
													  GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))));
				}

			}
		}

		public void DeleteComputedColumn(Column srcColumn)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION IF EXISTS {2}.{0}({1});",	srcColumn.Name.ToLower(), srcColumn.Table.Name.ToLower(), _SchemaName));
		}
		
		public void AddComputedColumn(Column srcColumn)
		{
			_MainScripts.Add(string.Format("CREATE OR REPLACE FUNCTION {4}.{0}({1}) RETURNS {2} AS $BODY$ {3} $BODY$ LANGUAGE sql STABLE STRICT;",
					srcColumn.Name.ToLower(), srcColumn.Table.Name.ToLower(), srcColumn.Type.GetDBType(this), srcColumn.ComputedText, _SchemaName));
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			_MainScripts.Add(string.Format("DROP TRIGGER IF EXISTS {0} ON {2}.{1};", currentTrigger.Name.ToLower(), currentTrigger.Owner.ToLower(), _SchemaName));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			_MainScripts.Add(srcTrigger.Text);
		}

		public void DeleteView(View currentView)
		{
			_MainScripts.Add(string.Format("DROP VIEW IF EXISTS {1}.{0};", currentView.Name.ToLower(), _SchemaName));
		}

		public void CreateView(View srcView)
		{
			_MainScripts.Add(srcView.Text);
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION IF EXISTS {1}.{0};", currentProcedure.Name.ToLower(), _SchemaName));
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			_MainScripts.Add(srcProcedure.Text);
		}

		public void DeleteFunction(Function currentFunction)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION IF EXISTS {2}.{0}({1});", currentFunction.Name.ToLower(), currentFunction.Parameters.Select(o => o.Value.Type.GetDBType(this).ToLower()).Join(","), _SchemaName));
		}

		public void CreateFunction(Function srcFunction)
		{
			_MainScripts.Add(srcFunction.Text);
		}

		public void DeleteTableFunction(TableFunction currentFunction)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION IF EXISTS {2}.{0}({1});", currentFunction.Name.ToLower(), currentFunction.Parameters.Select(o => o.Value.Type.GetDBType(this).ToLower()).Join(","), _SchemaName));
		}

		public void CreateTableFunction(TableFunction srcFunction)
		{
			_MainScripts.Add(srcFunction.Text);
		}

		public void DeleteDefaultValue(Column currentColumn)
		{
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} DROP DEFAULT;", currentColumn.Name.ToLower(), currentColumn.Table.Name.ToLower(), _SchemaName));
		}

		public void AddDefaultValue(Column srcColumn)
		{
			if (!string.IsNullOrEmpty(srcColumn.DefaultValue)) 
				_MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} SET DEFAULT {3};", srcColumn.Name.ToLower(), srcColumn.Table.Name.ToLower(), _SchemaName, GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))));
		}

		public void DeleteIndex(Index currentIndex)
		{
			_MainScripts.Add(string.Format("DROP INDEX IF EXISTS {1}.{0};", currentIndex.Name.ToLower(), _SchemaName));
		}

		public void SyncIdentityColumn(Column srcColumn)
		{
			throw new NotImplementedException();
		}

		public void SyncIdentity(Table srcTable)
		{
			throw new NotImplementedException();
		}

		public System.Xml.Linq.XElement GetMeta(string connectionString)
		{
			throw new NotImplementedException();
		}

		public string GetIntType()
		{
			return "integer";
		}

		public string GetGuidType()
		{
			return "uuid";
		}

		public string GetStringType(int length)
		{
			return string.Format("varchar({0})", length == -1 ? "2048" : length == 0 ? "1" : length.ToString());
		}

		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("numeric({0},{1})", precision, scale);
		}

		public string GetDateTimeType()
		{
			return "timestamp";
		}

		public string GetDateType()
		{
			return "date";
		}

		public string GetZoneDateTimeType()
		{
			return "timestamptz";
		}

		public string GetLongType()
		{
			return "bigint";
		}

		public string GetByteArrayType(int length)
		{
			return "bytea";
		}

		public string GetBooleanType()
		{
			return "boolean";
		}

		public string GetXmlType()
		{
			return "xml";
		}

		public MetaPrimitiveType GetType(string dataType, bool notNull)
		{
			var type = dataType.Contains("(") ? dataType.Substring(0, dataType.IndexOf("(", System.StringComparison.Ordinal)) : dataType;
			int precision = -1;
			int scale = -1;
			var match = Regex.Match(dataType, @"\((.*?)\)");

			if (match.Groups.Count > 1)
			{
				var value = match.Groups[1].Value;
				string[] arrayVal = value.Split(',');
				precision = arrayVal[0].ToInt32(-1);
				if (arrayVal.Length > 1)
				{
					scale = arrayVal[1].ToInt32(-1);
				}
			}

			switch (type.ToLower())
			{
				case "integer":
					return notNull ? MetaIntType.NotNull() : MetaIntType.Null();
				case "uuid":
					return notNull ? MetaGuidType.NotNull() : MetaGuidType.Null();
				case "varchar":
					if (precision == -1)
						return notNull ? MetaStringType.NotNull() : MetaStringType.Null();
					else
						return new MetaStringType() { Length = precision, NotNullable = notNull };
				case "numeric":
					return new MetaDecimalType() { Precision = precision, Scale = scale, NotNullable = notNull };
				case "timestamp":
				case "timestamptz":
					return notNull ? MetaDateTimeType.NotNull() : MetaDateTimeType.Null();
				case "date":
					return notNull ? MetaDateType.NotNull() : MetaDateType.Null();
				case "bigint":
					return notNull ? MetaLongType.NotNull() : MetaLongType.Null();
				case "bytea":
					return notNull ? MetaByteArrayType.NotNull() : MetaByteArrayType.Null();
				case "boolean":
					return notNull ? MetaBooleanType.NotNull() : MetaBooleanType.Null();
				case "xml":
					return notNull ? MetaXmlType.NotNull() : MetaXmlType.Null();
				default:
					return new MetaStringType();
			}
		}

		private string GetDefaultValue(string Value, string Type)
		{
			var match = Regex.Match(Value, @"(?<=\(').*(?='\))");
			var defValue = match.Groups[0].Value;
			if (string.IsNullOrEmpty(defValue))
			{
				var match1 = Regex.Match(Value, @"\((.*)\)");
				defValue = match1.Groups[1].Value;
			}
			string retvalue;
			if (Value == "(getdate())")
				retvalue = (Type.ToLower() == "date" ? "current_date" : "current_timestamp");
			else
				if (Value == "(newid())")
					retvalue = "newid()";
				else
					retvalue = "'" + defValue.Replace("'", "").Replace("(", "").Replace(")", "").Replace("\"", "") + "'";
 
			return retvalue;
		}
	}
}