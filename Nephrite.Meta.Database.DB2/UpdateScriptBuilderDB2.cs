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
		DB2Connection conn;
		StringBuilder result;
		string _dbname;
		string _servername;
		Schema _schema;

		public string DbName { get { return _dbname; } }

		public bool RecreateIndexes = true;
		public bool IncludeStoredObjects = true;
		public bool IncludeSPM = true;
		public string LineDelimiter = "@";

		public UpdateScriptBuilderDB2(Schema schema, DB2Connection connection)
		{

			DB2ConnectionStringBuilder b = new DB2ConnectionStringBuilder(connection.ConnectionString);
			_servername = b.Server;
			_dbname = b.Database;
			_schema = schema;
			conn = connection;
		}

		public string Generate()
		{
			DateTime startDate = DateTime.Now;
			result = new StringBuilder();
			result.AppendLine("-- Tessera auto-generated update script");
			result.AppendLine("-- Server: " + _servername);
			result.AppendLine("-- Database: " + _dbname);
			result.AppendLine("-- Date: " + DateTime.Now.ToString());

			result.AppendLine("SAVEPOINT STARTSCRIPT ON ROLLBACK RETAIN CURSORS" + LineDelimiter);
			result.AppendLine("SELECT 'Отключение внешних ключей' from SYSIBM.SYSDUMMY1" + LineDelimiter);
			Constraints(false);
			result.AppendLine("SELECT 'Очистка метаданных' from SYSIBM.SYSDUMMY1" + LineDelimiter);
			Delete("N_NavigItemData");
			Delete("N_NavigItem");
			Delete("N_Navig");
			Delete("N_NodeData");
			Delete("N_Node");
			Delete("CMSFormView");
			Delete("N_MenuItem");
			if (IncludeSPM)
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

			result.AppendLine("SELECT 'Заливка метаданных' from SYSIBM.SYSDUMMY1" + LineDelimiter);

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
			if (IncludeSPM)
			{
				InsertMain("SPM_Action");
				Insert("SPM_ActionAsso");
				Insert("SPM_RoleAccess");
			}

			GenerateServerObjects();

			result.AppendLine("SELECT 'Включение внешних ключей' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
			Constraints(true);

			result.AppendLine("TRUNCATE TABLE DBO.N_CACHE IMMEDIATE" + LineDelimiter);
			result.AppendLine("INSERT INTO DBO.N_CACHE(TIMESTAMP) VALUES(CURRENT_DATE)" + LineDelimiter);
			result.AppendLine("COMMIT" + LineDelimiter);
			result.AppendLine("SELECT 'Database successfully updated!' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
			//result.AppendLine("DECLARE CONTINUE HANDLER FOR SQLEXCEPTION");
			//result.AppendLine("BEGIN");
			result.AppendLine("-- ROLLBACK TO SAVEPOINT STARTSCRIPT" + LineDelimiter);
			//result.AppendLine("END" + LineDelimiter);
			result.AppendLine("-- Generate time: " + DateTime.Now.Subtract(startDate).ToString());
			return result.ToString();
		}

		void GenerateServerObjects()
		{
			if (IncludeStoredObjects)
			{
				foreach (var v in _schema.Views.Values)
				{
					result.AppendLine("SELECT 'View " + v.Name + "' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
					result.AppendFormat("call dbo.db2perf_quiet_drop('VIEW {1}.\"{0}\"')", v.Name, _schema.Name.ToUpper()).AppendLine(LineDelimiter);
					result.Append(v.Text).AppendLine(LineDelimiter);
				}
				foreach (var p in _schema.Procedures.Values)
				{
					result.AppendLine("SELECT 'PROCEDURE " + p.Name + "' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
					result.AppendFormat("call dbo.db2perf_quiet_drop('PROCEDURE {1}.{0}')", p.Name.ToUpper(), _schema.Name.ToUpper()).AppendLine(LineDelimiter);
					result.Append(p.Text).AppendLine(LineDelimiter);
				}
				foreach (var f in _schema.Functions.Values)
				{
					result.AppendLine("SELECT 'FUNCTION " + f.Name + "' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
					result.AppendFormat("call dbo.db2perf_quiet_drop('FUNCTION {1}.{0}')", f.Name.ToUpper(), _schema.Name.ToUpper()).AppendLine(LineDelimiter);
					result.Append(f.Text).AppendLine(LineDelimiter);
				}
				foreach (Table t in _schema.Tables.Values)
				{
					foreach (var tr in t.Triggers.Values)
					{
						result.AppendLine("SELECT 'TRIGGER " + tr.Name + "' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
						result.AppendFormat("call dbo.db2perf_quiet_drop('TRIGGER {1}.{0}')", tr.Name.ToUpper(), _schema.Name.ToUpper()).AppendLine(LineDelimiter);
						result.Append(tr.Text).AppendLine(LineDelimiter);
					}
				}
			}

			if (RecreateIndexes)
			{
				foreach (var t in _schema.Tables.Values)
				{

					foreach (var indx in t.Indexes.Values)
					{
						result.AppendLine("SELECT 'Index " + indx.Name + "' FROM SYSIBM.SYSDUMMY1" + LineDelimiter);
						result.AppendFormat("call dbo.db2perf_quiet_drop('INDEX {1}.{0}')", indx.Name.ToUpper(), t.Schema.Name.ToUpper()).AppendLine(LineDelimiter);
						result.AppendFormat("CREATE {0} INDEX {1}.{2} ON {1}.{3} ({4})" + LineDelimiter,
							(indx.IsUnique ? "UNIQUE " : ""),
							t.Schema.Name.ToUpper(),
							indx.Name.ToUpper(),
							indx.CurrentTable.Name.ToUpper(),
							string.Join(", ", indx.Columns.ToArray()).ToUpper().Replace("+", ""));
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
						result.AppendFormat("ALTER TABLE {2}.{1} ALTER FOREIGN KEY {0} {3}", fk.Name.ToUpper(), t.Name.ToUpper(), t.Schema.Name.ToUpper(), enable ? "ENFORCED" : "NOT ENFORCED").AppendLine(LineDelimiter);
					}
				}
			}
		}

		void Delete(string table)
		{
			if (_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))

			result.AppendFormat("SET INTEGRITY FOR {1}.{0} ALL IMMEDIATE UNCHECKED", table.ToUpper(), _schema.Name).AppendLine(LineDelimiter);
			result.AppendFormat("CALL SYSPROC.ADMIN_CMD('REORG TABLE {1}.{0}')", table.ToUpper(), _schema.Name).AppendLine(LineDelimiter);
			result.AppendFormat("DELETE FROM {1}.{0}", table.ToUpper(), _schema.Name).AppendLine(LineDelimiter);
		}

		public List<string> dd
		{
			get { return _dd; }
		}
		private List<string> _dd = new List<string>();
		void InsertMain(string table)
		{

			if (!_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))
				return;

			_dd.Add(table);
			var currentTable = _schema.Tables.SingleOrDefault(t => t.Key.ToUpper() == table.ToUpper()).Value;
			bool hasIdentity = currentTable.Identity;

			if (hasIdentity)
				result.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED BY DEFAULT", table, currentTable.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, _schema.Name).AppendLine(LineDelimiter);

			result.Append(ExportTableData(table));

			if (hasIdentity)
				result.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1} SET GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 20 )", table, currentTable.Columns.Values.FirstOrDefault(c => c.IsPrimaryKey).Name, _schema.Name).AppendLine(LineDelimiter);
		}

		void Insert(string table)
		{
			if (!_schema.Tables.Values.Any(t => t.Name.ToUpper() == table.ToUpper()))
				return;

			result.Append(ExportTableData(table));
		}


		int top = 0;


		public string ExportTableData(string tableName)
		{
			return ExportTableData(tableName, "", new string[0], (top > 0 ? "TOP " + top.ToString() + " " : "") + "*", "", "", "");
		}

		public string ExportTableData(string tableName, string where, string[] skipColumns, string select, string replaceColumnName, string replaceColumnValue, string replaceColumnSrcName)
		{
			if (conn.State == System.Data.ConnectionState.Closed)
				conn.Open();

			StringBuilder result = new StringBuilder();

			DB2Command cmd = conn.CreateCommand();
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = "select " + select + " from " + _schema.Name + "." + tableName;
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
					result.Append(")" + LineDelimiter + Environment.NewLine);
				}
			}


			return result.ToString();
		}

		public string GetStringValue(DB2DataReader reader, int index)
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

					DB2Command cmd = conn.CreateCommand();
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = "select " + (top > 0 ? "TOP " + top.ToString() + " " : "") + "* from " + _schema.Name + "." + tableName;

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