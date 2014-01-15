using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;
using IBM.Data.DB2;
using Nephrite.Web;
using System.Web;
using Nephrite.Web.FileStorage;
using System.IO;


namespace Nephrite.Meta.Database
{
	public class UpdateScriptBuilderDB2
	{
		SqlConnection conn;
		TableExport export;
		StringBuilder result;
		string _dbname;
		string _servername;
		bool _isdb2;
		Schema _schema;
		public string DbName { get { return _dbname; } }
		public bool RecreateIndexes = true;

		public UpdateScriptBuilderDB2(Schema schema, SqlConnection connection, bool isdb2)
		{

			DB2ConnectionStringBuilder b = new DB2ConnectionStringBuilder(connection.ConnectionString);
			_servername = b.Server;
			_dbname = b.Database;
			_schema = schema;
			conn = connection;
			export = new TableExport(connection, schema);
			_isdb2 = isdb2;
		}

		public string Generate(bool includeSPM, bool includeFiles)
		{

			//List<string> additionalTables = new List<string>();
			//additionalTables.AddRange(HttpUtility.UrlDecode(Query.GetString("tables")).Split(','));
			DateTime startDate = DateTime.Now;
			result = new StringBuilder();
			result.AppendLine("-- Tessera auto-generated update script");
			result.AppendLine("-- Server: " + _servername);
			//result.AppendLine("-- Instance: " + sr.InstanceName);
			result.AppendLine("-- Database: " + _dbname);
			result.AppendLine("-- Date: " + DateTime.Now.ToString());
			result.AppendLine("-- Tables: " + _schema.Tables.Values.Select(t => t.Name).ToArray().Join(", "));
			//result.AppendLine("use [" + _dbname + "]");
			//result.AppendLine("go");
			//result.AppendLine("BEGIN TRY");

			result.AppendFormat("CALL DBO.DROPOBJECTS('{0}'); \r\n", _schema.Name);
			result.AppendLine(" SAVEPOINT STARTSCRIPT ON ROLLBACK RETAIN CURSORS;");

			// Добавить содержимое всех *.sql файлов, найденных в папке Scripts
			//var files = FileStorageManager.DbFiles.Where(o => o.Extension.ToLower() == ".sql" && o.Path == "Scripts").ToList();
			//foreach (var file in files)
			//{
			//	result.AppendLine(Encoding.UTF8.GetString(file.GetBytes()));
			//}
			result.AppendLine("SELECT  'Отключение внешних ключей' from SYSIBM.SYSDUMMY1;");
			Constraints(false);
			result.AppendLine("SELECT  'Очистка метаданных' from SYSIBM.SYSDUMMY1;");
			Delete("N_NavigItemData");
			Delete("N_NavigItem");
			Delete("N_Navig");
			Delete("N_NodeData");
			Delete("N_Node");
			Delete("CMSFormView");
			Delete("N_MenuItem");
			if (includeSPM)
			{
				Delete("SPM_RoleAccess");
				Delete("SPM_ActionAsso");
				Delete("SPM_Action");
			}
			Delete("MM_Predicate");
			Delete("MM_CodifierValue");
			Delete("MM_DataValidation");
			Delete("MM_FormFieldAttribute");
			Delete("MM_FormField");
			Delete("MM_FormFieldGroup");
			Delete("MM_ObjectProperty");
			Delete("MM_Codifier");
			Delete("MM_MethodGroupItem");
			Delete("MM_MethodGroup");
			Delete("MM_Method");
			Delete("HST_MM_FormView");
			Delete("MM_FormView");
			Delete("WF_Transition");
			Delete("WF_Activity");
			Delete("WF_Workflow");
			Delete("MM_ObjectType");
			Delete("MM_Package");
			if (includeFiles)
			{
				result.AppendFormat("DELETE FROM N_FILE WHERE FOLDERID IN (SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources%');\r\n");
				result.AppendFormat("DELETE FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources/%'; \r\n");
			}
			//foreach (var t in additionalTables)
			//	Delete(t);
			result.AppendLine("SELECT 'Заливка метаданных' from SYSIBM.SYSDUMMY1;");
			
			//foreach (var t in additionalTables)
			//InsertMain(t);

			InsertMain("MM_Package");
			InsertMain("MM_ObjectType");
			InsertMain("MM_FormView");
			InsertMain("MM_Method");
			InsertMain("N_MenuItem");
			InsertMain("MM_MethodGroup");
			InsertMain("MM_MethodGroupItem");
			InsertMain("MM_Codifier");
			InsertMain("MM_ObjectProperty");
			InsertMain("MM_FormFieldGroup");
			InsertMain("MM_FormField");
			InsertMain("MM_FormFieldAttribute");
			InsertMain("MM_DataValidation");
			InsertMain("MM_CodifierValue");
			InsertMain("MM_Predicate");
			InsertMain("WF_Workflow");
			InsertMain("WF_Activity");
			InsertMain("WF_Transition");

			InsertMain("CMSFormView");
			InsertMain("N_Node");
			InsertMain("N_NodeData");
			InsertMain("N_Navig");
			InsertMain("N_NavigItem");
			InsertMain("N_NavigItemData");
			if (includeSPM)
			{
				InsertMain("SPM_Action");
				Insert("SPM_ActionAsso");
				Insert("SPM_RoleAccess");
			}

			if (includeFiles)
			{

				result.Append(export.ExportTableData("N_FOLDER", "FULLPATH LIKE 'SolutionSources/%'", new string[] { "FOLDERID" }, "*", "", "", ""));
				result.Append(export.ExportTableData("N_FILE",
						"FOLDERID IN (SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources%' AND FULLPATH <> 'SolutionSources/Autogenerate')",
						new string[] { "FILEID", "FOLDERFULLPATH" },
						"*, (SELECT FULLPATH FROM N_FOLDER WHERE N_FOLDER.FOLDERID = N_FILE.FOLDERID) AS FOLDERFULLPATH",
						"FOLDERID",
						"(SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH = {0})",
						"FOLDERFULLPATH"));

				result.Append(export.ExportTableData("N_FILEDATA",
						"FILEID IN (SELECT F.FILEID FROM N_FOLDER FL JOIN N_FILE AS F ON FL.FOLDERID = F.FOLDERID WHERE FULLPATH LIKE 'SolutionSources%' AND FULLPATH <> 'SolutionSources/Autogenerate')",
						new string[] { "GUID" },
						"*, (SELECT GUID FROM N_FILE WHERE N_FILE.FILEID = N_FILEDATA.FILEID) AS GUID",
						"FILEID",
						"(SELECT FILEID FROM N_File WHERE GUID = {0})",
						"GUID"));
			}

			GenerateServerObjects();

			result.AppendLine("SELECT 'Включение внешних ключей' FROM SYSIBM.SYSDUMMY1;");
			Constraints(true);

			result.AppendLine("TRUNCATE TABLE DBO.N_CACHE IMMEDIATE;");
			result.AppendLine("INSERT INTO DBO.N_CACHE(TIMESTAMP) VALUES(CURRENT_DATE);");
			result.AppendLine("COMMIT;");
			result.AppendLine("SELECT 'Database successfully updated!' FROM SYSIBM.SYSDUMMY1;");
			//result.AppendLine("END TRY");
			//result.AppendLine("BEGIN CATCH");
			//result.AppendLine("ROLLBACK TRANSACTION");
			//result.AppendLine("SELECT 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			//result.AppendLine("print ERROR_MESSAGE()");
			//result.AppendLine("END CATCH");
			result.AppendLine("DECLARE CONTINUE HANDLER FOR SQLEXCEPTION");
			result.AppendLine("BEGIN");
			result.AppendLine("ROLLBACK TO SAVEPOINT STARTSCRIPT;");
			result.AppendLine("END;");
			result.AppendLine("-- Generate time: " + DateTime.Now.Subtract(startDate).ToString());
			return result.ToString();
		}

		public string GenerateFiles()
		{
			DateTime startDate = DateTime.Now;
			result = new StringBuilder();
			result.AppendLine("-- Tessera auto-generated update script");
			result.AppendLine("-- Server: " + _servername);
			result.AppendLine("-- Contents: SolutionSources files");
			result.AppendLine("-- Database: " + _dbname);
			result.AppendLine("-- Date: " + DateTime.Now.ToString());
			//result.AppendLine("use [" + _dbname + "]");
			//result.AppendLine("go");
			//result.AppendLine("BEGIN TRY");


			result.AppendLine("SAVEPOINT STARTFILESCRIPT ON ROLLBACK RETAIN CURSORS;");

			result.AppendFormat("DELETE FROM N_FILE WHERE FOLDERID IN (SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources%');\r\n");
			result.AppendFormat("DELETE FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources/%'; \r\n");

			result.Append(export.ExportTableData("N_FOLDER", "FULLPATH LIKE 'SolutionSources/%'", new string[] { "FOLDERID" }, "*", "", "", ""));
			result.Append(export.ExportTableData("N_FILE",
					"FOLDERID IN (SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH LIKE 'SolutionSources%' AND FULLPATH <> 'SolutionSources/Autogenerate')",
					new string[] { "FILEID", "FOLDERFULLPATH" },
					"*, (SELECT FULLPATH FROM N_FOLDER WHERE N_FOLDER.FOLDERID = N_FILE.FOLDERID) AS FOLDERFULLPATH",
					"FOLDERID",
					"(SELECT FOLDERID FROM N_FOLDER WHERE FULLPATH = {0})",
					"FOLDERFULLPATH"));

			result.Append(export.ExportTableData("N_FILEDATA",
					"FILEID IN (SELECT F.FILEID FROM N_FOLDER FL JOIN N_FILE AS F ON FL.FOLDERID = F.FOLDERID WHERE FULLPATH LIKE 'SolutionSources%' AND FULLPATH <> 'SolutionSources/Autogenerate')",
					new string[] { "GUID" },
					"*, (SELECT GUID FROM N_FILE WHERE N_FILE.FILEID = N_FILEDATA.FILEID) AS GUID",
					"FILEID",
					"(SELECT FILEID FROM N_File WHERE GUID = {0})",
					"GUID"));

			result.AppendLine("COMMIT;");
			result.AppendLine("SELECT 'Database successfully updated!' FROM SYSIBM.SYSDUMMY1;");
			//result.AppendLine("END TRY");
			//result.AppendLine("BEGIN CATCH");
			//result.AppendLine("ROLLBACK TRANSACTION");
			//result.AppendLine("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			//result.AppendLine("print ERROR_MESSAGE()");
			//result.AppendLine("END CATCH");

			result.AppendLine("DECLARE CONTINUE HANDLER FOR SQLEXCEPTION");
			result.AppendLine("BEGIN");
			result.AppendLine(" ROLLBACK TO SAVEPOINT STARTFILESCRIPT;");
			result.AppendLine("END;");
			result.AppendLine("-- Generate time: " + DateTime.Now.Subtract(startDate).ToString());
			return result.ToString();
		}


		void GenerateServerObjects()
		{



			/*Пока исключаем так как скрипты не подходют*/

			if (_isdb2)
			{
				foreach (var v in _schema.Views.Values)
				{

					result.AppendLine("SELECT 'View " + v.Name + "' FROM SYSIBM.SYSDUMMY1;");
					result.AppendFormat("DECLARE {0}_text CLOB(2M); \r\n", v.Name);
					result.AppendFormat("DECLARE {0} STATEMENT; \r\n", v.Name);
					//result.AppendFormat(" IF EXISTVIEW('{0}','{1}') IS NOT NULL THEN \r\n", v.Name.ToUpper(), _schema.Name.ToUpper());
					result.AppendFormat(" DROP FUNCTION {1}.{0};\r\n", v.Name.ToUpper(), _schema.Name.ToUpper());
					//result.AppendFormat("END IF; \r\n");
					result.AppendFormat(" SET {0}_text = {1};\r\n", v.Name, v.Text);
					result.AppendFormat(" PREPARE {0} FROM {0}_text;\r\n", v.Name);
					result.AppendFormat(" EXECUTE {0};\r\n", v.Name);



					foreach (var tr in v.Triggers.Values)
					{

						result.AppendLine("SELECT 'TRIGGER " + tr.Name + "' FROM SYSIBM.SYSDUMMY1;");


						result.AppendFormat("DECLARE {0}_text CLOB(2M); \r\n", tr.Name);
						result.AppendFormat("DECLARE {0} STATEMENT; \r\n", tr.Name);
						//result.AppendFormat(" IF EXISTTRIGER('{0}','{1}') IS NOT NULL THEN \r\n", tr.Name.ToUpper(), _schema.Name.ToUpper());
						result.AppendFormat(" DROP TRIGGER {1}.{0};\r\n", tr.Name.ToUpper(), _schema.Name.ToUpper());
						//result.AppendFormat("END IF; \r\n");
						result.AppendFormat(" SET {0}_text = {1};\r\n", tr.Name, tr.Text);
						result.AppendFormat(" PREPARE {0} FROM {0}_text;\r\n", tr.Name);
						result.AppendFormat(" EXECUTE {0};\r\n", tr.Name);

					}
				}
				foreach (var p in _schema.Procedures.Values)
				{

					result.AppendLine("SELECT 'PROCEDURE " + p.Name + "' FROM SYSIBM.SYSDUMMY1;");
					result.AppendFormat("DECLARE {0}_text CLOB(2M); \r\n", p.Name);
					result.AppendFormat("DECLARE {0} STATEMENT; \r\n", p.Name);
					//result.AppendFormat(" IF EXISTFUNC('{0}','{1}') IS NOT NULL THEN \r\n", p.Name.ToUpper(), _schema.Name.ToUpper());
					result.AppendFormat(" DROP FUNCTION {1}.{0};\r\n", p.Name.ToUpper(), _schema.Name.ToUpper());
					//result.AppendFormat("END IF; \r\n");
					result.AppendFormat(" SET {0}_text = {1};\r\n", p.Name, p.Text);
					result.AppendFormat(" PREPARE {0} FROM {0}_text;\r\n", p.Name);
					result.AppendFormat(" EXECUTE {0};\r\n", p.Name);


				}
				foreach (var f in _schema.Functions.Values)
				{
					result.AppendLine("SELECT 'FUNCTION " + f.Name + "' FROM SYSIBM.SYSDUMMY1;");

					result.AppendFormat("DECLARE {0}_text CLOB(2M); \r\n", f.Name);
					result.AppendFormat("DECLARE {0} STATEMENT; \r\n", f.Name);
					//result.AppendFormat(" IF EXISTFUNC('{0}','{1}') IS NOT NULL THEN \r\n", f.Name.ToUpper(), _schema.Name.ToUpper());
					result.AppendFormat(" DROP FUNCTION {1}.{0};\r\n", f.Name.ToUpper(), _schema.Name.ToUpper());
					//result.AppendFormat("END IF; \r\n");
					result.AppendFormat(" SET {0}_text = {1};\r\n", f.Name, f.Text);
					result.AppendFormat(" PREPARE {0} FROM {0}_text;\r\n", f.Name);
					result.AppendFormat(" EXECUTE {0};\r\n", f.Name);

				}
				foreach (Table t in _schema.Tables.Values)
				{

					foreach (var tr in t.Triggers.Values)
					{
						result.AppendLine("SELECT 'TRIGGER " + tr.Name + "' FROM SYSIBM.SYSDUMMY1;");


						result.AppendFormat("DECLARE {0}_text CLOB(2M); \r\n", tr.Name);
						result.AppendFormat("DECLARE {0} STATEMENT; \r\n", tr.Name);
						//result.AppendFormat(" IF EXISTTRIGER('{0}','{1}') IS NOT NULL THEN \r\n", tr.Name.ToUpper(), _schema.Name.ToUpper());
						result.AppendFormat(" DROP TRIGGER {1}.{0};\r\n", tr.Name.ToUpper(), _schema.Name.ToUpper());
						//result.AppendFormat("END IF; \r\n");
						result.AppendFormat(" SET {0}_text = {1};\r\n", tr.Name, tr.Text);
						result.AppendFormat(" PREPARE {0} FROM {0}_text;\r\n", tr.Name);
						result.AppendFormat(" EXECUTE {0};\r\n", tr.Name);

					}
				}

			}


			if (RecreateIndexes)
			{
				foreach (var t in _schema.Tables.Values)
				{

					foreach (var indx in t.Indexes.Values)
					{


						result.AppendLine("SELECT 'Index " + indx.Name + "' FROM SYSIBM.SYSDUMMY1;");


						//result.AppendFormat(" IF EXISTINDEX('{0}','{1}') IS NOT NULL THEN \r\n", indx.Name.ToUpper(), t.Schema.Name.ToUpper());
						result.AppendFormat(" DROP INDEX {1}.{0};\r\n", indx.Name.ToUpper(), t.Schema.Name.ToUpper());
						//result.AppendFormat("END IF; \r\n");
						result.AppendFormat(" CREATE {0} INDEX {1}.{2} ON {1}.{3} ({4}) ;\r\n",
							(indx.IsUnique ? "UNIQUE " : ""),
							t.Schema.Name.ToUpper(),
							indx.Name.ToUpper(),
							indx.CurrentTable.Name.ToUpper(),
							string.Join(", ", indx.Columns.ToArray()).ToUpper().Replace("+",""));



					}
				}
			}
		}

		void Constraints(bool enable)
		{
			foreach (var t in _schema.Tables.Values)
			{
				foreach (var fk in t.ForeignKeys.Values)
				{
					if (fk.IsEnabled)
					{
						result.AppendFormat("ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} {3} ;\r\n", fk.Name.ToUpper(), t.Name.ToUpper(), t.Schema.Name.ToUpper(), enable ? "ENFORCED" : "NOT ENFORCED");

						//result.AppendFormat(" IF EXISTCONSTRAINT('{0}','{2}') IS NOT NULL THEN \r\n" +
						//						   "ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} NOT ENFORCED \r\n;" +
						//					"END IF; \r\n", fk.Name.ToUpper(), t.Name.ToUpper(), t.Schema.Name.ToUpper());
					}
				}
			}
		}

		void Delete(string table)
		{
			if (_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))




			result.AppendFormat("SET INTEGRITY FOR  {1}.{0} ALL IMMEDIATE UNCHECKED; ;\r\n", table.ToUpper(), _schema.Name);
			result.AppendFormat("CALL SYSPROC.ADMIN_CMD( 'REORG TABLE {1}.{0}' ); ;\r\n", table.ToUpper(), _schema.Name);
			result.AppendFormat("DELETE FROM  {1}.{0} ;\r\n", table.ToUpper(), _schema.Name);
		}

		public List<string> dd {
			get { return _dd; }
		}
		private List<string> _dd  = new List<string>();
		void InsertMain(string table)
		{
			
			if (!_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))
				return;

			_dd.Add(table);
			var currentTable = _schema.Tables.SingleOrDefault(t => t.Key.ToUpper() == table.ToUpper()).Value;
			bool hasIdentity = currentTable.Identity;

			if (hasIdentity)
				result.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT;\r\n ", table, currentTable.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name,_schema.Name);

			result.Append(export.ExportTableData(table));

			if (hasIdentity)
				result.AppendFormat(" ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 );\r\n", table, currentTable.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, _schema.Name);
		}

		void Insert(string table)
		{
			if (!_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))
				return;

			result.Append(export.ExportTableData(table));
		}

		public class TableExport
		{
			int top = 0;
			Schema _schema;
			SqlConnection conn;
			public TableExport(SqlConnection connection, Schema schema)
			{
				conn = connection;
				_schema = schema;
			}

			public TableExport(SqlConnection connection, int top, Schema schema)
			{
				conn = connection;
				_schema = schema;
				this.top = top;
			}

			public string ExportTableData(string tableName)
			{
				return ExportTableData(tableName, "", new string[0], (top > 0 ? "TOP " + top.ToString() + " " : "") + "*", "", "", "");
			}

			public string ExportTableData(string tableName, string where, string[] skipColumns, string select, string replaceColumnName, string replaceColumnValue, string replaceColumnSrcName)
			{
				if (conn.State == System.Data.ConnectionState.Closed)
					conn.Open();

				StringBuilder result = new StringBuilder();

				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = "select " + select + " from [" + tableName + "]";
				if (where != "")
					cmd.CommandText += " where " + where;

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						result.AppendFormat("INSERT INTO {1}.{0}(", tableName.ToUpper(), _schema.Name.ToUpper());
						bool first = true;
						for (int i = 0; i < reader.FieldCount; i++)
						{
							if (skipColumns.Contains(reader.GetName(i).ToLower()))
								continue;

							if (first)
								first = false;
							else
								result.Append(",");

							result.Append(reader.GetName(i));
						}
						result.Append(")" + Environment.NewLine + "VALUES(");
						first = true;
						for (int i = 0; i < reader.FieldCount; i++)
						{
							if (skipColumns.Contains(reader.GetName(i).ToLower()))
								continue;

							if (first)
								first = false;
							else
								result.Append(",");

							int index = i;
							if (reader.GetName(i).ToLower() == replaceColumnName)
							{
								result.Append(String.Format(replaceColumnValue, GetStringValue(reader, reader.GetOrdinal(replaceColumnSrcName))));
							}
							else
							{
								result.Append(GetStringValue(reader, i));
							}
						}
						result.Append(");" + Environment.NewLine);
					}
				}


				return result.ToString();
			}

			public string GetStringValue(SqlDataReader reader, int index)
			{
				return new DBScriptDB2("DBO").GetStringValue(reader, index);
			}

			public string ExportTableBinary(string tableName)
			{
				if (conn.State == System.Data.ConnectionState.Closed)
					conn.Open();

				string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tableName + ".bin");
				using (var fs = new FileStream(fileName, FileMode.Create))
				{
					using (var bw = new BinaryWriter(fs))
					{
						StringBuilder result = new StringBuilder();

						SqlCommand cmd = conn.CreateCommand();
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.CommandText = "select " + (top > 0 ? "TOP " + top.ToString() + " " : "") + "* from [" + tableName + "]";

						using (var reader = cmd.ExecuteReader())
						{
							bool headerWrited = false;
							while (reader.Read())
							{
								if (!headerWrited)
								{
									// Количество столбцов
									bw.Write(reader.FieldCount);
									for (int i = 0; i < reader.FieldCount; i++)
										bw.Write(reader.GetName(i));
									headerWrited = true;
								}
								for (int i = 0; i < reader.FieldCount; i++)
								{
									bw.Write(GetStringValue(reader, i));
								}
							}
						}
					}
				}
				return fileName;
			}
		}
	}
}