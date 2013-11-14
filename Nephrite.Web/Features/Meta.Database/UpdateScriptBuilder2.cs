using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;
using Nephrite.Web;
using System.Web;
using Nephrite.Web.FileStorage;
using System.IO;


namespace Nephrite.Meta.Database
{
	public class UpdateScriptBuilder2
	{
		SqlConnection conn;
		TableExport export;
		StringBuilder result;
		string _dbname;
		string _servername;
		Schema _schema;
		public string DbName { get { return _dbname; } }
		public bool RecreateIndexes = true;

		public UpdateScriptBuilder2(Schema schema)
		{
			SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
			_servername = b.DataSource;
			_dbname = b.InitialCatalog;
			_schema = schema;
		
		}

		public string Generate(bool includeSPM, bool includeFiles)
		{

			List<string> additionalTables = new List<string>();
			additionalTables.AddRange(HttpUtility.UrlDecode(Query.GetString("tables")).Split(','));
			DateTime startDate = DateTime.Now;
			result = new StringBuilder();
			result.AppendLine("-- Tessera auto-generated update script");
			result.AppendLine("-- Server: " + _servername);
			//result.AppendLine("-- Instance: " + sr.InstanceName);
			result.AppendLine("-- Database: " + _dbname);
			result.AppendLine("-- Date: " + DateTime.Now.ToString());
			result.AppendLine("-- Tables: " + _schema.Tables.Values.Select(t => t.Name).ToArray().Join(", "));
			result.AppendLine("use [" + _dbname + "]");
			result.AppendLine("go");
			result.AppendLine("BEGIN TRY");
			result.AppendLine("BEGIN TRANSACTION");

			// Добавить содержимое всех *.sql файлов, найденных в папке Scripts
			var files = FileStorageManager.DbFiles.Where(o => o.Extension.ToLower() == ".sql" && o.Path == "Scripts").ToList();
			foreach (var file in files)
			{
				result.AppendLine(Encoding.UTF8.GetString(file.GetBytes()));
			}
			result.AppendLine("print 'Отключение внешних ключей'");
			Constraints(false);
			result.AppendLine("print 'Очистка метаданных'");
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
				result.AppendFormat("delete from N_File where FolderID in (select FolderID from N_Folder where FullPath like 'SolutionSources%')\r\n");
				result.AppendFormat("delete from N_Folder where FullPath like 'SolutionSources/%'\r\n");
			}
			foreach (var t in additionalTables)
				Delete(t);
			result.AppendLine("print 'Заливка метаданных'");
			foreach (var t in additionalTables)
				InsertMain(t);

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
				result.Append(export.ExportTableData("N_Folder", "FullPath like 'SolutionSources/%'", new string[] { "folderid" }, "*", "", "", ""));
				result.Append(export.ExportTableData("N_File",
					"FolderID in (select FolderID from N_Folder where FullPath like 'SolutionSources%' and FullPath <> 'SolutionSources/Autogenerate')",
					new string[] { "fileid", "folderfullpath" },
					"*, (select FullPath from N_Folder where N_Folder.FolderID = N_File.FolderID) as FolderFullPath",
					"folderid",
					"(select FolderID from N_Folder where FullPath = {0})",
					"FolderFullPath"));

				result.Append(export.ExportTableData("N_FileData",
					"FileID in (select f.FileID from N_Folder fl join N_file f on fl.FolderID = f.FolderID where FullPath like 'SolutionSources%' and FullPath <> 'SolutionSources/Autogenerate')",
					new string[] { "guid" },
					"*, (select Guid from N_File where N_File.FileID = N_FileData.FileID) as guid",
					"fileid",
					"(select FileID from N_File where Guid = {0})",
					"guid"));
			}

			GenerateServerObjects();

			result.AppendLine("print 'Включение внешних ключей'");
			Constraints(true);

			result.AppendLine("delete from [N_Cache]");
			result.AppendLine("insert into [N_Cache](TimeStamp) values(getdate())");
			result.AppendLine("COMMIT TRANSACTION");
			result.AppendLine("print 'Database successfully updated!'");
			result.AppendLine("END TRY");
			result.AppendLine("BEGIN CATCH");
			result.AppendLine("ROLLBACK TRANSACTION");
			result.AppendLine("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			result.AppendLine("print ERROR_MESSAGE()");
			result.AppendLine("END CATCH");
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
			result.AppendLine("use [" + _dbname + "]");
			result.AppendLine("go");
			result.AppendLine("BEGIN TRY");
			result.AppendLine("BEGIN TRANSACTION");

			result.AppendFormat("delete from N_File where FolderID in (select FolderID from N_Folder where FullPath like 'SolutionSources%')\r\n");
			result.AppendFormat("delete from N_Folder where FullPath like 'SolutionSources/%'\r\n");

			result.Append(export.ExportTableData("N_Folder", "FullPath like 'SolutionSources/%'", new string[] { "folderid" }, "*", "", "", ""));
			result.Append(export.ExportTableData("N_File",
					"FolderID in (select FolderID from N_Folder where FullPath like 'SolutionSources%' and FullPath <> 'SolutionSources/Autogenerate')",
					new string[] { "fileid", "folderfullpath" },
					"*, (select FullPath from N_Folder where N_Folder.FolderID = N_File.FolderID) as FolderFullPath",
					"folderid",
					"(select FolderID from N_Folder where FullPath = {0})",
					"FolderFullPath"));

			result.Append(export.ExportTableData("N_FileData",
					"FileID in (select f.FileID from N_Folder fl join N_file f on fl.FolderID = f.FolderID where FullPath like 'SolutionSources%' and FullPath <> 'SolutionSources/Autogenerate')",
					new string[] { "guid" },
					"*, (select Guid from N_File where N_File.FileID = N_FileData.FileID) as guid",
					"fileid",
					"(select FileID from N_File where Guid = {0})",
					"guid"));

			result.AppendLine("COMMIT TRANSACTION");
			result.AppendLine("print 'Database successfully updated!'");
			result.AppendLine("END TRY");
			result.AppendLine("BEGIN CATCH");
			result.AppendLine("ROLLBACK TRANSACTION");
			result.AppendLine("print 'Error at line: ' + convert(varchar(50), ERROR_LINE())");
			result.AppendLine("print ERROR_MESSAGE()");
			result.AppendLine("END CATCH");
			result.AppendLine("-- Generate time: " + DateTime.Now.Subtract(startDate).ToString());
			return result.ToString();
		}


		void GenerateServerObjects()
		{
			foreach (var v in _schema.Views.Values)
			{


				result.AppendLine("print 'View " + v.Name + "'");
				result.AppendFormat("IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[{0}].[{1}]')) DROP VIEW [{0}].[{1}]\r\n", _schema.Name, v.Name);
				result.AppendFormat("EXEC dbo.sp_executesql @statement = N'{0}'\r\n", v.Text.Replace("'", "''"));

				foreach (var tr in v.Triggers.Values)
				{
					result.AppendLine("print 'Trigger " + tr.Name + "'");
					result.AppendFormat("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]')) DROP TRIGGER [{0}].[{1}]\r\n", _schema.Name, tr.Name);
					result.AppendFormat("EXEC dbo.sp_executesql @statement = N'{0}'\r\n", tr.Text.Replace("'", "''"));
				}
			}
			foreach (var p in _schema.Procedures.Values)
			{

				result.AppendLine("print 'Stored procedure " + p.Name + "'");
				result.AppendFormat("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]')) DROP PROCEDURE [{0}].[{1}]\r\n", _schema.Name, p.Name);
				result.AppendFormat("EXEC dbo.sp_executesql @statement = N'{0}'\r\n", p.Text.Replace("'", "''"));
			}
			foreach (var f in _schema.Functions.Values)
			{

				result.AppendLine("print 'Stored function " + f.Name + "'");
				result.AppendFormat("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]')) DROP FUNCTION [{0}].[{1}]\r\n", _schema.Name, f.Name);
				result.AppendFormat("EXEC dbo.sp_executesql @statement = N'{0}'\r\n", f.Text.Replace("'", "''"));
			}
			foreach (Table t in _schema.Tables.Values)
			{

				foreach (var tr in t.Triggers.Values)
				{
					result.AppendLine("print 'Trigger " + tr.Name + "'");
					result.AppendFormat("IF EXISTS (select * from sys.objects where type_desc = 'USER_TABLE' and OBJECT_NAME(OBJECT_ID) = '{0}' and SCHEMA_NAME(schema_id) = '{1}') BEGIN\r\n", t.Name, _schema);
					result.AppendFormat("	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]')) DROP TRIGGER [{0}].[{1}]\r\n", _schema.Name, tr.Name);
					result.AppendFormat("	EXEC dbo.sp_executesql @statement = N'{0}\r\n", tr.Text.Replace("'", "''"));
					result.AppendFormat("END\r\n");
				}
			}
			if (RecreateIndexes)
			{
				foreach (var t in _schema.Tables.Values)
				{

					foreach (var indx in t.Indexes.Values)
					{
						var createIndex = "CREATE {6} INDEX {0} ON {1}" +
	"({2}) WITH(IGNORE_DUP_KEY = {3}, ALLOW_ROW_LOCKS = {4}, ALLOW_PAGE_LOCKS = {5}) ";
						result.AppendLine("print 'Index " + indx.Name + "'");
						result.AppendFormat("IF EXISTS (select * from sys.objects where type_desc = 'USER_TABLE' and OBJECT_NAME(OBJECT_ID) = '{0}' and SCHEMA_NAME(schema_id) = '{1}') BEGIN\r\n", t.Name, t.Owner);
						result.AppendFormat("	IF EXISTS (select * from sys.indexes where name = '{1}') DROP INDEX [{1}] ON [{0}].[{2}]\r\n", _schema.Name, indx.Name, t.Name);
						result.AppendFormat("	EXEC dbo.sp_executesql @statement = N'{0}'\r\n", string.Format(createIndex,
							indx.Name,
							indx.CurrentTable.Name,
							string.Join(", ", indx.Columns.ToArray()),
							indx.IgnoreDupKey ? "ON" : "OFF",
							indx.AllowRowLocks ? "ON" : "OFF",
							indx.AllowPageLocks ? "ON" : "OFF"));
						result.AppendFormat("END\r\n");
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
						result.AppendFormat("IF EXISTS(SELECT 1 FROM sys.objects WHERE type_desc = 'FOREIGN_KEY_CONSTRAINT' and OBJECT_NAME(OBJECT_ID) = '{0}' and OBJECT_NAME(parent_object_id) = '{1}' and SCHEMA_NAME(schema_id) = '{2}')", fk.Name, t.Name, t.Owner);
						result.AppendFormat("\tALTER TABLE [{0}].[{1}] {3}CHECK CONSTRAINT [{2}]\r\n", _schema.Name, t.Name, fk.Name, enable ? "" : "NO");
					}
				}
			}
		}

		void Delete(string table)
		{
			if (_schema.Tables.Values.Any(t => t.Name == table))
				result.AppendFormat("delete from [{0}]\r\n", table);
		}

		void InsertMain(string table)
		{
			if (!_schema.Tables.Values.Any(t => t.Name == table))
				return;

			
			var currentTable = _schema.Tables[table];
			bool hasIdentity = currentTable.Identity;


			if (hasIdentity)
				result.AppendFormat("SET IDENTITY_INSERT [{0}] ON\r\n", table);

			result.Append(export.ExportTableData(table));

			if (hasIdentity)
				result.AppendFormat("SET IDENTITY_INSERT [{0}] OFF\r\n", table);
		}

		void Insert(string table)
		{
			if (!_schema.Tables.Values.Any(t => t.Name == table))
				return;

			result.Append(export.ExportTableData(table));
		}

		public class TableExport
		{
			int top = 0;
			SqlConnection conn;
			public TableExport(SqlConnection connection)
			{
				conn = connection;
			}

			public TableExport(SqlConnection connection, int top)
			{
				conn = connection;
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
						result.AppendFormat("insert into [{0}](", tableName);
						bool first = true;
						for (int i = 0; i < reader.FieldCount; i++)
						{
							if (skipColumns.Contains(reader.GetName(i).ToLower()))
								continue;

							if (first)
								first = false;
							else
								result.Append(",");

							result.Append("[" + reader.GetName(i) + "]");
						}
						result.Append(")" + Environment.NewLine + "values(");
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
						result.Append(")" + Environment.NewLine);
					}
				}


				return result.ToString();
			}

			public string GetStringValue(SqlDataReader reader, int index)
			{
				return new DBScriptMSSQL("dbo").GetStringValue(reader, index);
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