using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using IBM.Data.DB2;
using Nephrite.Web;

namespace Nephrite.Meta.Database
{
	public class DBScriptDB2 : IDBScript
	{
		private List<string> _MainScripts { get; set; }
		private List<string> _FkScripts { get; set; }
		private string _SchemaName { get; set; }

		public DBScriptDB2(string schemaName)
		{
			_MainScripts = new List<string>();
			_FkScripts = new List<string>();
			Scripts = new List<string>();
			_SchemaName = schemaName.ToUpper();
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

			var tableScript = string.Format("CREATE TABLE {2}.{0} ({1})  ;\r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n  ", srcTable.Name.ToUpper(), "{0}", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript =
				srcTable.Columns.Aggregate(string.Empty,
										   (current, srcColumn) =>
										   current +
										   (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("{0} {1} {2} {3} {4},\r\n ",
														 srcColumn.Value.Name.ToUpper(),
														 srcColumn.Value.Type.GetDBType(this),
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL",
														 srcColumn.Value.IsPrimaryKey && srcTable.Identity ? "  GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )" : "",
														 (!string.IsNullOrEmpty(srcColumn.Value.DefaultValue) ? string.Format(" WITH DEFAULT {0}",GetDefaultValue(srcColumn.Value.DefaultValue)) : "")
														) :
														 string.Format(" {0}  GENERATED ALWAYS AS  (\"{1}\") ", srcColumn.Value.Name.ToUpper(), srcColumn.Value.ComputedText)
														 )).Trim().TrimEnd(',');

			tableScript = string.Format(tableScript, columnsScript);
			if (srcTable.ForeignKeys.Count > 0)
			{
				var result = srcTable.ForeignKeys.Aggregate("", (current, key) => current + string.Format("ALTER TABLE {6}.{0}  ADD  CONSTRAINT {1} FOREIGN KEY({2}) \r\n" + "REFERENCES {6}.{3} ({4}) {5} ;\r\n", srcTable.Name.ToUpper(), key.Value.Name.ToUpper(), string.Join(",", key.Value.Columns).ToUpper(), key.Value.RefTable.ToUpper(), string.Join(",", key.Value.RefTableColumns).ToUpper(), "ON DELETE " + key.Value.DeleteOption.ToString().ToUpper(), _SchemaName));
				_FkScripts.Add(result);
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(
								   "ALTER TABLE {3}.{0}\r\n" +
								   "ADD CONSTRAINT {1} PRIMARY KEY ({2})  ;\r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {3}.{0}' ); \r\n", srcTable.Name.ToUpper(),
													  srcTable.PrimaryKey.Name.ToUpper(),
													  string.Join(",", srcTable.PrimaryKey.Columns),
													  _SchemaName);// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}


			_MainScripts.Add(tableScript);
		}

		public void DeleteTable(Table currentTable)
		{
			//Находим таблицы ссылающиеся на текущую и удаляем их
			var childrenForeignKeys = currentTable.Schema.Tables.Where(t => t.Value.ForeignKeys.Any(f => f.Value.RefTable.ToUpper() == currentTable.Name.ToUpper())).SelectMany(t => t.Value.ForeignKeys).ToList();
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
					var removeForeignKeys = t.Value.ForeignKeys.Where(f => f.Value.RefTable.ToUpper() == currentTable.Name.ToUpper()).Select(f => f.Value.Name.ToUpper()).ToList();
					removeForeignKeys.ForEach(r => t.Value.ForeignKeys.Remove(r));
				}


			});


			// Удаляем все ссылки текущей таблицы в обьекте 

			if (currentTable.ForeignKeys != null && currentTable.ForeignKeys.Count > 0)
			{
				currentTable.ForeignKeys.Clear();
			}
			_MainScripts.Add(string.Format("DROP TABLE {1}.{0}; \r\n ", currentTable.Name.ToUpper(), _SchemaName));
		}

		public void CreateForeignKey(ForeignKey srcforeignKey)
		{
			var srcTable = srcforeignKey.CurrentTable;
			_FkScripts.Add(
				string.Format(
					"ALTER TABLE {6}.{0}  ADD  CONSTRAINT {1} FOREIGN KEY({2}) \r\n" +
					"REFERENCES {6}.{3} ({4}) {5} ;\r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {6}.{0}' ); \r\n", srcTable.Name.ToUpper(), srcforeignKey.Name.ToUpper(), string.Join(",", srcforeignKey.Columns).ToUpper(),
					srcforeignKey.RefTable.ToUpper(), string.Join(",", srcforeignKey.RefTableColumns).ToUpper(), "ON DELETE " + srcforeignKey.DeleteOption.ToString().ToUpper(),
					_SchemaName));

		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			var currentTable = currentForeignKey.CurrentTable;
			_FkScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP CONSTRAINT {2}.{1}; \r\n ", currentTable.Name.ToUpper(),
										  currentForeignKey.Name.ToUpper(), _SchemaName));
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
		{

			var currentTable = currentPrimaryKey.CurrentTable;
			_MainScripts.Add(string.Format("ALTER TABLE {1}.{0} DROP PRIMARY KEY ;  \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' ); \r\n", currentTable.Name.ToUpper(), _SchemaName));

		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			var curentTable = srcPrimaryKey.CurrentTable;
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{0}\r\n" +
										   "ADD  PRIMARY KEY ({1}) ; \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n", curentTable.Name.ToUpper(),
													   string.Join(",", srcPrimaryKey.Columns),
													   _SchemaName));

		}

		public void DeleteColumn(Column currentColumn)
		{
			var currentTable = currentColumn.CurrentTable;
			// При удалении колонки  удаляем  и её pk и fk 
			if (currentTable.PrimaryKey != null && currentTable.PrimaryKey.Columns.Any(t => t.ToUpper() == currentColumn.Name.ToUpper()))
			{
				DeletePrimaryKey(currentTable.PrimaryKey);
				currentTable.PrimaryKey = null;
			}


			var toRemove = currentTable.ForeignKeys.Where(t => t.Value.Columns.Any(c => c.ToUpper() == currentColumn.Name.ToUpper())).Select(t => t.Key.ToUpper()).ToArray();
			foreach (var key in toRemove)
			{

				DeleteForeignKey(currentTable.ForeignKeys[key]);
				currentTable.ForeignKeys.Remove(key);
			}

			if (!string.IsNullOrEmpty(currentColumn.DefaultValue))
			{
				DeleteDefaultValue(currentColumn);
			}
			if (currentColumn.CurrentTable.Indexes != null && currentColumn.CurrentTable.Indexes.Values.Any(t => t.Columns.Any(c => c.ToUpper() == currentColumn.Name.ToUpper())))
			{
				foreach (var index in currentColumn.CurrentTable.Indexes)
				{
					if (index.Value.Columns.Any(c => c.ToUpper() == currentColumn.Name.ToUpper()))
					{
						DeleteIndex(index.Value);
					}
				}

			}
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP COLUMN {1} ; \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n ", currentTable.Name.ToUpper(),
										  currentColumn.Name.ToUpper(), _SchemaName));

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


				_MainScripts.Add(string.Format("ALTER TABLE {6}.{5} ADD {0} {1} {2} {3} {4} ; \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {6}.{5}' ); \r\n ",
										srcColumn.Name.ToUpper(),
										srcColumn.Type.GetDBType(this),
										srcColumn.Nullable ? "NULL" : "NOT NULL",
										srcColumn.IsPrimaryKey && currentTable.Identity ? "  GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )" : "",
									   (!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format(" WITH DEFAULT {0}", GetDefaultValue(srcColumn.DefaultValue)) : ""),
										currentTable.Name.ToUpper(), _SchemaName));
				if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
				{
					AddDefaultValue(srcColumn);
				}

			}
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
				_MainScripts.Add(string.Format("  ALTER TABLE {3}.{0} ALTER COLUMN {1} SET DATA TYPE {2}; \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {3}.{0}' ); \r\n ",
											  currentTable.Name.ToUpper(),
											  srcColumn.Name.ToUpper(),
											  srcColumn.Type.GetDBType(this),
											  _SchemaName));
				if (srcColumn.Nullable)
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} DROP NOT  NULL; \r\n     CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n",
												   currentTable.Name.ToUpper(),
												   srcColumn.Name.ToUpper(),
												   _SchemaName));
				}
				else
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET NOT  NULL;  \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n ",
													  currentTable.Name.ToUpper(),
													  srcColumn.Name.ToUpper(),
													  _SchemaName));

				}
				if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
				{
					_MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1}  SET DEFAULT {3}; \r\n    CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n",
													  currentTable.Name.ToUpper(),
													  srcColumn.Name.ToUpper(),
													  _SchemaName,
													  GetDefaultValue(srcColumn.DefaultValue)));
				}

			}
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			_MainScripts.Add(string.Format("DROP TRIGGER {1}.{0};  \r\n  ", currentTrigger.Name.ToUpper(), _SchemaName));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			_MainScripts.Add(srcTrigger.Text);
		}

		public void SyncIdentityColumn(Column srcColumn)
		{
			var srcTable = srcColumn.CurrentTable;
			if (srcColumn.Identity)
			{
				_MainScripts.Add(string.Format("ALTER TABLE {2}.{0}  ALTER COLUMN {1} DROP IDENTITY; \r\n CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n", srcTable.Name.ToUpper(), srcColumn.Name.ToUpper(), _SchemaName));
				_MainScripts.Add(string.Format("ALTER TABLE {2}.{0}  ALTER COLUMN {1}  SET GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 ); \r\n CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n", srcTable.Name.ToUpper(), srcColumn.Name.ToUpper(), _SchemaName));


			}
			else
			{
				_MainScripts.Add(string.Format("ALTER TABLE {2}.{0}  ALTER COLUMN {1} DROP IDENTITY; \r\n CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{0}' ); \r\n ", srcTable.Name, srcColumn.Name.ToUpper(), _SchemaName));
			}
		}

		public void DeleteView(View currentView)
		{
			_MainScripts.Add(string.Format("DROP VIEW  {1}.{0}; \r\n  ", currentView.Name.ToUpper(), _SchemaName));
		}

		public void CreateView(View srcView)
		{
			_MainScripts.Add(srcView.Text);
		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			_MainScripts.Add(string.Format("DROP PROCEDURE  {1}.{0}; \r\n  ", currentProcedure.Name.ToUpper(), _SchemaName));
		}

		public void CreateProcedure(Procedure srcProcedure)
		{
			_MainScripts.Add(srcProcedure.Text);
		}

		public void DeleteFunction(Function currentFunction)
		{
			_MainScripts.Add(string.Format("DROP FUNCTION   {1}.{0};  \r\n ", currentFunction.Name.ToUpper(), _SchemaName));
		}

		public void CreateFunction(Function srcFunction)
		{
			_MainScripts.Add(srcFunction.Text);
		}

		public string GetIntType()
		{
			return string.Format("INTEGER");
		}

		public string GetGuidType()
		{
			return string.Format("VARCHAR(100)");
		}

		public string GetStringType(int length)
		{
	         return string.Format("VARCHAR({0})", length == -1 ? "32000" : length.ToString());
		}

		public string GetDecimalType(int precision, int scale)
		{
			return string.Format("DECIMAL({0},{1})", precision, scale);
		}

		public string GetDateTimeType()
		{
			return "TIMESTAMP";
		}

		public string GetDateType()
		{
			return "DATE";
		}

		public string GetZoneDateTimeType()
		{
			return "TIMESTAMP";
		}

		public string GetLongType()
		{
			return "BIGINT";
		}

		public string GetByteArrayType(int length)
		{
			return "BLOB";
		}

		public string GetBooleanType()
		{
			return "SMALLINT";
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
				sqlInsert = string.Format("ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n {2} ALTER TABLE {0} ALTER COLUMN {1} SET GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 );", t.Name, t.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, sqlInsert);
			}
			return sqlInsert;
		}


		public void AddComputedColumn(Column srcColumn)
		{
			
			var currentTable = srcColumn.CurrentTable;

			if (currentTable.Name.ToUpper() == "C_POSTPART")
			{

			}
			var computedText = "";
			switch (srcColumn.ComputedText)
			{
				case "(isnull(([Surname]+isnull((' '+substring([Firstname],(1),(1)))+'.',''))+isnull((' '+substring([Patronymic],(1),(1)))+'.',''),''))":
					computedText =
						"AS ((COALESCE((SURNAME || COALESCE((' ' ||substring(FIRSTNAME,1,1, OCTETS)) || '.','')) || COALESCE((' ' || substring(PATRONYMIC,1,1, OCTETS)) || '.',''),'')))";
					break;
				case "(([Surname]+isnull(' '+[Firstname],''))+isnull(' '+[Patronymic],''))":
					computedText =
						"AS ((SURNAME || COALESCE(' ' || FIRSTNAME,'')) || COALESCE(' ' || PATRONYMIC,''))";
					break;
				case "((((([Surname]+isnull(' '+[Firstname],''))+isnull(' '+[Patronymic],''))+' ')+CONVERT([nvarchar],[Birthdate],(104)))+isnull(', '+[RFSubjectText],''))":
					computedText = "AS (((((SURNAME || COALESCE(' ' || FIRSTNAME,'')) || COALESCE(' ' || PATRONYMIC,'')) || ' ') || VARCHAR_FORMAT(BIRTHDATE, 'YYYY-MM-DD')  ) || COALESCE(', ' || RFSUBJECTTEXT,''))";
					break;
				case "('от '+isnull(CONVERT([nvarchar],[ComplaintDate],(120)),'<дата не задана>'))":
					computedText = "AS ('от ' || COALESCE(VARCHAR_FORMAT(COMPLAINTDATE, 'YYYY-MM-DD'),'<дата не задана>'))";
					break;
				case "(([HouseNum]+isnull(nullif(', CONVERT(NVARCHAR,VALUE,корп. '+[BuildNum],', корп. '),''))+isnull(nullif(', стр. '+[StrucNum],', стр. '),''))":
					computedText = "AS ((HOUSENUM  ||  COALESCE(nullif(', корп. '+BUILDNUM,', корп. '),'')) || COALESCE(nullif(', стр. ' || STRUCNUM,', стр. '),''))";
					break;
				case "(((([Part]+'-')+[Subpart])+'-')+[Chapter])":
					computedText = "AS ((((PART||'-') || SUBPART) || '-') || CHAPTER)";
					break;
				case "(((((([Part]+'-')+[Subpart])+'-')+[Chapter])+' ')+isnull([ShortDescription],''))":
					computedText = "AS ((((((PART || '-') || SUBPART) || '-') || CHAPTER) || ' ') || COALESCE(SHORTDESCRIPTION,''))";
					break;
				case "(((CONVERT([nvarchar],[Value],0)+' (')+TITLE)+')')":
					computedText = "AS (((CHAR(VALUE) || ' (') || TITLE) || ')')";
					break;
				case "(((CONVERT([nvarchar],[Value],0)+' ')+[Title])+isnull((' (дата окончания действия '+CONVERT([nvarchar],[EndDate],(104)))+')',''))":
					computedText = "AS (((CHAR(VALUE) || ' ') || TITLE) || COALESCE((' (дата окончания действия ' || VARCHAR_FORMAT(ENDDATE, 'YYYY-MM-DD') ) || ')',''))";
					break;
				case "((CONVERT([nvarchar],[Value],0)+' ')+[ATE])":
					computedText = "AS ((CHAR(VALUE) || ' ') || ATE)";
					break;
				case "(CONVERT([nvarchar],[Value],0)+isnull(' '+[Title],''))":
					computedText = "AS (CHAR(VALUE) || COALESCE(' ' || TITLE,''))";
					break;
				case "(isnull(CONVERT([bit],case when [EndDate]>getdate() AND [BeginDate]<getdate() then (0) else (1) end,(0)),(1)))":
					computedText = "AS (COALESCE(case when ENDDATE> current_date AND BEGINDATE< current_date then 0 else 1 end))";
					break;
				case "(((([CitizenSurname]+' ')+[CitizenFirstname])+' ')+[CitizenPatronymic])":
					computedText = "AS ((((CITIZENSURNAME || ' ') || CITIZENFIRSTNAME) || ' ') || CITIZENPATRONYMIC)";
					break;
				case "(case when [AssignmentDate] IS NULL then 'Создано' else case when [CompleteDate] IS NULL then isnull([State],'Создано') else case when [OnCheckDate] IS NULL then isnull([State],'Выполнено') else case when [CloseDate] IS NOT NULL then 'Завершено' when [AnnulmentDate] IS NOT NULL then 'Аннулировано' when [SuspendDate] IS NOT NULL then 'Отложено' else 'На проверке' end end end end)":
					computedText = "AS ((case when ASSIGNMENTDATE IS NULL then 'Создано' else case when COMPLETEDATE IS NULL then COALESCE(STATE,'Создано') else case when ONCHECKDATE IS NULL then COALESCE(STATE,'Выполнено') else case when CLOSEDATE IS NOT NULL then 'Завершено' when ANNULMENTDATE IS NOT NULL then 'Аннулировано' when SUSPENDDATE IS NOT NULL then 'Отложено' else 'На проверке' end end end end))";
					break;
				case "(right('000'+CONVERT([nvarchar],[opNo],0),(3))+case [IsTracking] when (1) then '-к' else '' end)":
					computedText = "AS ((right('000' || CHAR(OPNO),(3)) || case ISTRACKING when 1 then '-к' else '' end))";
					break;
				case "((isnull([Surname]+' ','')+isnull(substring([Firstname],(1),(1))+'. ',''))+isnull(substring([Patronymic],(1),(1))+'.',''))":
					computedText = "AS (((COALESCE(SURNAME || ' ','') || COALESCE(substring(FIRSTNAME,1,1,OCTETS) || '. ','')) || COALESCE(substring(Patronymic,1,1,OCTETS) || '.','')))";
					break;
				case "(([ShortName]+' ')+[OffName])":
					computedText = "AS ((SHORTNAME || ' ') || OFFNAME)";
					break;
				default:
					computedText = srcColumn.ComputedText.Replace("[", "").Replace("]", "").ToUpper();
					break;

			}
			_MainScripts.Add(string.Format("SET INTEGRITY FOR {3}.{0} OFF CASCADE DEFERRED;\r\n ALTER TABLE {3}.{0} ADD {1} {4}  GENERATED ALWAYS {2} ; \r\n SET INTEGRITY FOR {3}.{0}  IMMEDIATE CHECKED; CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {3}.{0}' ); \r\n",
									currentTable.Name.ToUpper(),
									srcColumn.Name.ToUpper(),
									computedText.Replace("\"", "").Replace("+","||"),
									_SchemaName,
									srcColumn.Type.GetDBType(this)
									));//    // {0}- Название таблицы, {1} - Название колонки, {2} - ComputedText
		}



		public void DeleteDefaultValue(Column currentColumn)
		{
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} DROP DEFAULT; \r\n CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{1}' ); \r\n", currentColumn.Name.ToUpper(), currentColumn.CurrentTable.Name.ToUpper(), _SchemaName));
		}

		public void AddDefaultValue(Column srcColumn)
		{
			_MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} SET DEFAULT {3}; \r\n CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {2}.{1}' ); \r\n", srcColumn.Name.ToUpper(), srcColumn.CurrentTable.Name.ToUpper(), _SchemaName, GetDefaultValue(srcColumn.DefaultValue)));
		}



		public void DeleteIndex(Index currentIndex)
		{
			_MainScripts.Add(string.Format("DROP INDEX {0}; \r\n", currentIndex.Name));
		}


		public void SyncIdentity(Table srcTable)
		{
			throw new NotImplementedException();
		}



		public XElement GetMeta()
		{
			using (var con = new DB2Connection(ConnectionManager.ConnectionString))
			{
				using (var cmd = new DB2Command("CALL DBO.USP_MODEL()", con))
				{
					con.Open();

					using (XmlReader reader = cmd.ExecuteXmlReader())
					{
						while (reader.Read())
						{
							var s = reader.ReadOuterXml();
							return XElement.Parse(s);
						}
					}
				}
			}
			return null;
		}


		public MetaPrimitiveType GetType(string dataType)
		{
			var type = dataType.Contains("(") ? dataType.Substring(0, dataType.IndexOf("(", System.StringComparison.Ordinal)) : dataType;
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
				case "INTEGER":
					return new MetaIntType();
				case "VARCHAR":
					return new MetaStringType() { Length = Int32.Parse(precision) };
				case "DECIMAL":
					return new MetaDecimalType() { Precision = Int32.Parse(precision), Scale = Int32.Parse(scale) };
				case "TIMESTAMP":
					return new MetaDateTimeType();
				case "DATE":
					return new MetaDateType();
				case "BIGINT":
					return new MetaLongType();
				case "BLOB":
					return new MetaByteArrayType();
				case "SMALLINT":
					return new MetaBooleanType();
				default:
					return new MetaStringType();
			}

		}

		private string GetDefaultValue(string Value)
		{


			var match = Regex.Match(Value, @"(?<=\(').*(?='\))");
			var defValue = match.Groups[0].Value;
			if (string.IsNullOrEmpty(defValue))
			{
				var match1 = Regex.Match(Value, @"\((.*)\)");
				defValue = match1.Groups[1].Value;
			}
			return Value == "(getdate())" ? "current_date" : "'" + defValue.Replace("'", "").Replace("(", "").Replace(")", "").Replace("\"", "") + "'";

		}
	}
}