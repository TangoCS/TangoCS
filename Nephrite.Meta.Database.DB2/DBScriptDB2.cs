using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
//using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
//using System.Web;
using System.Xml;
using System.Xml.Linq;
using IBM.Data.DB2;
using Nephrite.Web;

namespace Nephrite.Meta.Database
{
    public static class ExtendString
    {
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }

        public static string CuttingText(this string value)
        {
            if (value.Length > 15000)
            {
                var arrayText = value.SplitByLength(15000);
                return string.Format("CAST({0}' as CLOB) || '{1}", arrayText.First(), string.Join("'||'", arrayText.Skip(1).ToArray()));
            }
            return value;
        }
    }
    public class DBScriptDB2 : IDBScript
    {
        private List<string> _MainScripts { get; set; }
        private List<string> _FkScripts { get; set; }
        private string _SchemaName { get; set; }

        public DBScriptDB2(string schemaName)
        {
            _MainScripts = new List<string>();
            _FkScripts = new List<string>();
            _SchemaName = schemaName.ToUpper();
        }

		public void Comment(string comment)
		{
			_MainScripts.Add("-- " + comment);
		}

		public override string ToString()
		{
			var res = new List<string>(_MainScripts.Count + _FkScripts.Count + 10);
			//res.Add("BEGIN TRY");
			//res.Add("BEGIN TRANSACTION");
			res.AddRange(_MainScripts);
			res.AddRange(_FkScripts);
			//res.Add("COMMIT TRANSACTION");
			//res.Add("print 'Database structure successfully updated!'");
			//res.Add("END TRY");
			//res.Add("BEGIN CATCH");
			//res.Add("ROLLBACK TRANSACTION");
			//res.Add("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			//res.Add("print ERROR_MESSAGE()");
			//res.Add("GOTO RunupEnd");
			//res.Add("END CATCH");
			//res.Add("RunupEnd:");

			return res.Join("\r\n");
		}


        public void CreateTable(Table srcTable)
        {

            var tableScript = string.Format("CREATE TABLE {2}.{0} ({1});", srcTable.Name.ToUpper(), "{0}", _SchemaName);// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
            var columnsScript =
                srcTable.Columns.Aggregate(string.Empty,
                                           (current, srcColumn) =>
                                           current +
                                           (string.IsNullOrEmpty(srcColumn.Value.ComputedText) ? string.Format("\t{0} {1} {2} {3} {4},\r\n ",
                                                         srcColumn.Value.Name.ToUpper(),
                                                         srcColumn.Value.Type.GetDBType(this),
                                                         srcColumn.Value.Nullable ? "NULL" : "NOT NULL",
														 srcColumn.Value.IsPrimaryKey && srcColumn.Value.Identity ? " GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )" : "",
                                                         (!string.IsNullOrEmpty(srcColumn.Value.DefaultValue) ? string.Format(" WITH DEFAULT {0}", GetDefaultValue(srcColumn.Value.DefaultValue, srcColumn.Value.Type.GetDBType(this))) : "")
                                                        ) :
                                                         string.Format(" {0} GENERATED ALWAYS AS (\"{1}\") ", srcColumn.Value.Name.ToUpper(), srcColumn.Value.ComputedText)
                                                         )).Trim().TrimEnd(',');

            tableScript = string.Format(tableScript, columnsScript);
            if (srcTable.ForeignKeys.Count > 0)
            {
                var result = srcTable.ForeignKeys.Aggregate("", (current, key) => current + string.Format("ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};", srcTable.Name.ToUpper(), key.Value.Name.ToUpper(), string.Join(",", key.Value.Columns).ToUpper(), key.Value.RefTable.ToUpper(), string.Join(",", key.Value.RefTableColumns).ToUpper(), "ON DELETE " + key.Value.DeleteOption.ToString().ToUpper(), _SchemaName));
                _FkScripts.Add(result);
            }

            if (srcTable.PrimaryKey != null)
            {

                tableScript += string.Format(
                                   "ALTER TABLE {3}.{0} ADD CONSTRAINT {1} PRIMARY KEY ({2});", srcTable.Name.ToUpper(),
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
            _MainScripts.Add(string.Format("DROP TABLE {1}.{0};", currentTable.Name.ToUpper(), _SchemaName));
        }

        public void CreateForeignKey(ForeignKey srcforeignKey)
        {
            var srcTable = srcforeignKey.Table;
            _FkScripts.Add(
                string.Format(
                    "ALTER TABLE {6}.{0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {6}.{3} ({4}) {5};", srcTable.Name.ToUpper(), srcforeignKey.Name.ToUpper(), string.Join(",", srcforeignKey.Columns).ToUpper(),
                    srcforeignKey.RefTable.ToUpper(), string.Join(",", srcforeignKey.RefTableColumns).ToUpper(), "ON DELETE " + srcforeignKey.DeleteOption.ToString().ToUpper(),
                    _SchemaName));

        }

        public void DeleteForeignKey(ForeignKey currentForeignKey)
        {
            var currentTable = currentForeignKey.Table;
            _FkScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP CONSTRAINT {2}.{1};", currentTable.Name.ToUpper(),
                                          currentForeignKey.Name.ToUpper(), _SchemaName));
        }

        public void DeletePrimaryKey(PrimaryKey currentPrimaryKey)
        {

            var currentTable = currentPrimaryKey.Table;
            _MainScripts.Add(string.Format("ALTER TABLE {1}.{0} DROP PRIMARY KEY;", currentTable.Name.ToUpper(), _SchemaName));

        }

        public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
        {
            var curentTable = srcPrimaryKey.Table;
            _MainScripts.Add(string.Format("SET INTEGRITY FOR {2}.{0} IMMEDIATE CHECKED; ALTER TABLE {2}.{0} ADD PRIMARY KEY ({1});", curentTable.Name.ToUpper(),
                                                       string.Join(",", srcPrimaryKey.Columns),
                                                       _SchemaName));

        }

        public void DeleteColumn(Column currentColumn)
        {
            var currentTable = currentColumn.Table;
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
            if (currentColumn.Table.Indexes != null && currentColumn.Table.Indexes.Values.Any(t => t.Columns.Any(c => c.ToUpper() == currentColumn.Name.ToUpper())))
            {
                foreach (var index in currentColumn.Table.Indexes)
                {
                    if (index.Value.Columns.Any(c => c.ToUpper() == currentColumn.Name.ToUpper()))
                    {
                        DeleteIndex(index.Value);
                    }
                }

            }

			//--SET INTEGRITY FOR DBO.DOCTASKOPERATION ALL IMMEDIATE UNCHECKED;
			//--SET INTEGRITY FOR DBO.DOCTASKOPERATION OFF CASCADE DEFERRED;
			//--SET INTEGRITY FOR DBO.DOCTASKOPERATION IMMEDIATE CHECKED;

            _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} DROP COLUMN {1};", currentTable.Name.ToUpper(),
                                          currentColumn.Name.ToUpper(), _SchemaName));

			//--CALL SYSPROC.ADMIN_CMD('REORG TABLE DBO.DOCTASKOPERATION');

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


                _MainScripts.Add(string.Format("ALTER TABLE {6}.{5} ADD {0} {1} {2} {3} {4};",
                                        srcColumn.Name.ToUpper(),
                                        srcColumn.Type.GetDBType(this),
                                        srcColumn.Nullable ? "NULL" : "NOT NULL",
										srcColumn.IsPrimaryKey && srcColumn.Identity ? " GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )" : "",
                                       (!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format(" WITH DEFAULT {0}", GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))) : ""),
                                        currentTable.Name.ToUpper(), _SchemaName));
                if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
                {
                    AddDefaultValue(srcColumn);
                }

            }
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
                _MainScripts.Add(string.Format("ALTER TABLE {3}.{0} ALTER COLUMN {1} SET DATA TYPE {2};",
                                              currentTable.Name.ToUpper(),
                                              srcColumn.Name.ToUpper(),
                                              srcColumn.Type.GetDBType(this),
                                              _SchemaName));
                if (srcColumn.Nullable)
                {
                    _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} DROP NOT NULL;",
                                                   currentTable.Name.ToUpper(),
                                                   srcColumn.Name.ToUpper(),
                                                   _SchemaName));
                }
                else
                {
                    _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET NOT NULL;",
                                                      currentTable.Name.ToUpper(),
                                                      srcColumn.Name.ToUpper(),
                                                      _SchemaName));

                }
                if (!string.IsNullOrEmpty(srcColumn.DefaultValue))
                {
                    _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET DEFAULT {3};",
                                                      currentTable.Name.ToUpper(),
                                                      srcColumn.Name.ToUpper(),
                                                      _SchemaName,
                                                      GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))));
                }

            }
            //_MainScripts.Add(Checked(currentTable.Name.ToUpper()));
        }

        public void DeleteTrigger(Trigger currentTrigger)
        {
            _MainScripts.Add(string.Format("DROP TRIGGER {1}.{0};", currentTrigger.Name.ToUpper(), _SchemaName));
        }

        public void CreateTrigger(Trigger srcTrigger)
        {
            _MainScripts.Add(srcTrigger.Text);
        }

        public void SyncIdentityColumn(Column srcColumn)
        {
            var srcTable = srcColumn.Table;
            if (srcColumn.Identity)
            {
                _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} DROP IDENTITY;", srcTable.Name.ToUpper(), srcColumn.Name.ToUpper(), _SchemaName));
                _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 );", srcTable.Name.ToUpper(), srcColumn.Name.ToUpper(), _SchemaName));
            }
            else
            {
                _MainScripts.Add(string.Format("ALTER TABLE {2}.{0} ALTER COLUMN {1} DROP IDENTITY;", srcTable.Name, srcColumn.Name.ToUpper(), _SchemaName));
            }
        }

        public void DeleteView(View currentView)
        {
            _MainScripts.Add(string.Format("DROP VIEW {1}.{0};  ", currentView.Name.ToUpper(), _SchemaName));
        }

        public void CreateView(View srcView)
        {
            _MainScripts.Add(srcView.Text);
        }

        public void DeleteProcedure(Procedure currentProcedure)
        {
            _MainScripts.Add(string.Format("DROP PROCEDURE {1}.{0};  ", currentProcedure.Name.ToUpper(), _SchemaName));
        }

        public void CreateProcedure(Procedure srcProcedure)
        {
            _MainScripts.Add(srcProcedure.Text);
        }

        public void DeleteFunction(Function currentFunction)
        {
            _MainScripts.Add(string.Format("DROP FUNCTION {1}.{0}; ", currentFunction.Name.ToUpper(), _SchemaName));
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
        public string GetXmlType()
        {
            return "XML";
        }


        public string GetStringValue(DB2DataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return "null";
            else
            {
                switch (reader.GetDataTypeName(index).ToLower())
                {

                    case "float":
                        return reader.GetDB2Double(index).Value.ToString(CultureInfo.InvariantCulture);
                    case "integer":
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
                    case "clob":
                        return ("N'" + reader.GetString(index).Replace("'", "''") + "'").CuttingText(); ;
                    case "decimal":
                        return reader.GetDecimal(index).ToString(CultureInfo.InvariantCulture);
                    case "date":
                        return String.Format("CAST('{0}' AS Date)", reader.GetDateTime(index).ToString("yyyy-MM-dd"));
                    case "timestamp":
                        return String.Format("CAST('{0}' AS timestamp)", reader.GetDateTime(index).ToString("yyyy-MM-dd HH:mm:ss"));
                    case "xml":
                        return String.Format("N'{0}'", reader.GetDB2Xml(index).GetString().Replace("'", "''"));
                    case "varbinary":
                        StringBuilder result1 = new StringBuilder();
                        byte[] data1 = reader.GetDB2Blob(index).Value;
                        for (int x = 0; x < data1.Length; x++)
                            result1.Append(data1[x].ToString("X2"));
                        return string.Format("blob(X'{0}')", result1.ToString());
                    default:
                        throw new Exception("unknown data type: " + reader.GetDataTypeName(index));
                }
            }
        }

		public void AddComputedColumn(Column srcColumn)
        {

            var currentTable = srcColumn.Table;

            if (currentTable.Name.ToUpper() == "DOCTASK")
            {

            }
            var computedText = "";
            switch (srcColumn.ComputedText)
            {
                case "(((CONVERT([nvarchar],[Value],0)+' (')+[Title])+')')":
                    computedText = "AS (((CHAR(VALUE)||' (')||TITLE)||')')";
                    break;
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
                    computedText = "AS (COALESCE(case when ENDDATE> dbo.getdate() AND BEGINDATE< dbo.getdate() then 0 else 1 end,0))";
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
                    computedText = srcColumn.ComputedText.Trim().ToUpper().Replace("[", "").Replace("]", "").Replace("NVARCHAR", "VARCHAR").ToUpper();
                    computedText = computedText.StartsWith("AS") ? computedText : " AS" +  computedText;
                    break;

            }
            //_MainScripts.Add(Checked(currentTable.Name.ToUpper()));
			_MainScripts.Add(string.Format("SET INTEGRITY FOR {1}.{0} OFF;", currentTable.Name.ToUpper(), _SchemaName));
            _MainScripts.Add(string.Format("ALTER TABLE {3}.{0} ADD {1} {4} GENERATED ALWAYS {2} NULL;",
                                    currentTable.Name.ToUpper(),
                                    srcColumn.Name.ToUpper(),
                                    computedText.Replace("\"", "").Replace("+", "||").Replace("ISNULL", "COALESCE"),
                                    _SchemaName,
                                    srcColumn.Type.GetDBType(this)
                                    ));//    // {0}- Название таблицы, {1} - Название колонки, {2} - ComputedText
			_MainScripts.Add(string.Format("SET INTEGRITY FOR {1}.{0} IMMEDIATE CHECKED FORCE GENERATED;", currentTable.Name.ToUpper(), _SchemaName));
        }



        public void DeleteDefaultValue(Column currentColumn)
        {
            _MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} DROP DEFAULT;", currentColumn.Name.ToUpper(), currentColumn.Table.Name.ToUpper(), _SchemaName));
            //_MainScripts.Add(Checked(currentColumn.CurrentTable.Name.ToUpper()));
        }

        public void AddDefaultValue(Column srcColumn)
        {
            _MainScripts.Add(string.Format("ALTER TABLE {2}.{1} ALTER COLUMN {0} SET DEFAULT {3};", srcColumn.Name.ToUpper(), srcColumn.Table.Name.ToUpper(), _SchemaName, GetDefaultValue(srcColumn.DefaultValue, srcColumn.Type.GetDBType(this))));
            //_MainScripts.Add(Checked(srcColumn.CurrentTable.Name.ToUpper()));
        }



        public void DeleteIndex(Index currentIndex)
        {
            _MainScripts.Add(string.Format("DROP INDEX {0};", currentIndex.Name));
        }


        public void SyncIdentity(Table srcTable)
        {

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

            switch (type)
            {
                case "INTEGER":
					return notNull ? MetaIntType.NotNull() : MetaIntType.Null();
                case "VARCHAR":
					if (precision == 36)
						return notNull ? MetaGuidType.NotNull() : MetaGuidType.Null();
					else if (precision == -1)
						return notNull ? MetaStringType.NotNull() : MetaStringType.Null();
					else
						return new MetaStringType() { Length = precision, NotNullable = notNull };
                case "DECIMAL":
					return new MetaDecimalType() { Precision = precision, Scale = scale, NotNullable = notNull };
                case "TIMESTAMP":
					return notNull ? MetaDateTimeType.NotNull() : MetaDateTimeType.Null();
                case "DATE":
					return notNull ? MetaDateType.NotNull() : MetaDateType.Null();
                case "BIGINT":
					return notNull ? MetaLongType.NotNull() : MetaLongType.Null();
                case "BLOB":
					return notNull ? MetaByteArrayType.NotNull() : MetaByteArrayType.Null();
                case "SMALLINT":
					return notNull ? MetaBooleanType.NotNull() : MetaBooleanType.Null();
                case "XML":
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
            return Value == "(getdate())" ? (Type.ToUpper() == "TIMESTAMP" ? "CURRENT_TIMESTAMP" : "CURRENT_DATE") : "'" + defValue.Replace("'", "").Replace("(", "").Replace(")", "").Replace("\"", "") + "'";

        }

        private string Checked(string tableName)
        {
            return @"
				 CALL SYSPROC.ADMIN_CMD( 'REORG TABLE DBO." + tableName + @"' );
				 SET INTEGRITY FOR DBO." + tableName + @" ALL IMMEDIATE UNCHECKED;
				 SET INTEGRITY FOR DBO." + tableName + @" OFF CASCADE DEFERRED;
				 SET INTEGRITY FOR DBO." + tableName + @" IMMEDIATE CHECKED; 
					";
        }
    }
}