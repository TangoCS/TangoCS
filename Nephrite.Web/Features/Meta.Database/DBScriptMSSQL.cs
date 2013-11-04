using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
			Scripts.Add(string.Format("if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}') drop table {0};", currentTable.Name));
		}

		public void CreateTable(Table srcTable)
		{

			var tableScript = string.Format("CREATE TABLE {0} ({1}) ;", srcTable.Name, "{0} \r\n");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
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
									"REFERENCES {3} ({4}) ;\r\n" +
									"ALTER TABLE {0} CHECK CONSTRAINT {1} ;\r\n"
									, srcTable.Name, foreignKey.Value.Name, string.Join(",", foreignKey.Value.Columns), foreignKey.Value.RefTable, string.Join(",", foreignKey.Value.RefTableColumns)));
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(
								   "ALTER TABLE {0}\r\n" +
								   "ADD CONSTRAINT {1} PRIMARY KEY ({2}) ;\r\n", srcTable.Name,
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
									  "ALTER TABLE {0} CHECK CONSTRAINT {1}; \r\n", currentTable.Name,
													srcforeignKey.Name,
													string.Join(",", srcforeignKey.Columns),
													srcforeignKey.RefTable,
													string.Join(",", srcforeignKey.RefTableColumns)));
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey)
		{
			var currentTable = currentForeignKey.CurrentTable;
			Scripts.Add(string.Format("ALTER TABLE {0} DROP CONSTRAINT {1} ;\r\n ", currentTable.Name,
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

			Scripts.Add(string.Format("ALTER TABLE [{0}] DROP COLUMN [{1}]  ;\r\n", currentTable.Name,
										  currentColumn.Name));
		}
		public void AddColumn(Column srcColumn)
		{
			if (srcColumn.CurrentTable.Name == "C_DocType")
			{

			}
			var currentTable = srcColumn.CurrentTable;
			if (!string.IsNullOrEmpty(srcColumn.ComputedText))
			{
				AddComputedColumn(srcColumn);
			}
			else
				Scripts.Add(string.Format("ALTER TABLE [{5}] ADD [{0}] {1} {2} {3} {4} ; \r\n",
										srcColumn.Name,
										srcColumn.Type,
										srcColumn.IsPrimaryKey && currentTable.Identity ? "IDENTITY(1,1)" : "",
										srcColumn.Nullable ? "NULL" : "NOT NULL",
										(!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format("DEFAULT({0})", srcColumn.DefaultValue) : ""),
										currentTable.Name));//    // {0}- Название колонки, {1} - Тип колонки, {2} - IDENTITY, {3}- NULL
		}
		public void AddComputedColumn(Column srcColumn)
		{
			var currentTable = srcColumn.CurrentTable;

			Scripts.Add(string.Format("ALTER TABLE {0} ADD {1}  AS ({2}) \r\n",
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
										  "ALTER COLUMN [{1}] {2} {3} {4};\r\n",
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
			Scripts.Add(string.Format("ALTER TABLE [{0}] DROP CONSTRAINT {1}  ;\r\n", currentTable.Name,
										  currentPrimaryKey.Name));
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey)
		{
			var curentTable = srcPrimaryKey.CurrentTable;
			var currentTable = srcPrimaryKey.CurrentTable;
			Scripts.Add(string.Format("ALTER TABLE [{0}]\r\n" +
									  "ADD CONSTRAINT {1} PRIMARY KEY ({2}) ;\r\n", curentTable.Name,
													   srcPrimaryKey.Name,
													   string.Join(",", srcPrimaryKey.Columns)));
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			Scripts.Add(string.Format(" DROP TRIGGER {0} \r\n ", currentTrigger.Name));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			Scripts.Add(srcTrigger.Text);
		}

		public void DeleteView(View currentView)
		{
			Scripts.Add(string.Format("IF OBJECT_ID ('{0}', 'V') IS NOT NULL DROP VIEW {0} ;\r\n", currentView.Name));
		}

		public void CreateView(View srcView)
		{
			Scripts.Add(srcView.Text);
		}

		public void SyncIdentity(Table srcTable)
		{





			if (srcTable.Identity)
			{


				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1}) ", srcTable.Name, "{0} \r\n");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
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
				Scripts.Add(string.Format("ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE);\r\n", srcTable.Name));
				Scripts.Add(string.Format("SET IDENTITY_INSERT Tmp_{0} ON;\r\n", srcTable.Name));
				Scripts.Add(string.Format("IF EXISTS(SELECT * FROM {0}) \r\n" +
												"EXEC('INSERT INTO Tmp_{0} ({1}) \r\n" +
												"SELECT {1} FROM {0} WITH (HOLDLOCK TABLOCKX)');\r\n", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray())));
				Scripts.Add(string.Format("SET IDENTITY_INSERT Tmp_{0} OFF;DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT' ;\r\n", srcTable.Name));
			}
			else
			{

				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1}) \r\n ", srcTable.Name, "{0}");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
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
				Scripts.Add(string.Format("ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE);\r\n", srcTable.Name));
				Scripts.Add(string.Format("IF EXISTS(SELECT * FROM {0})\r\n" +
												"EXEC('INSERT INTO Tmp_{0} ({1})\r\n" +
												"SELECT {1} FROM {0} WITH (HOLDLOCK TABLOCKX)');\r\n", srcTable.Name, string.Join("\r\n,", srcTable.Columns.Select(t => t.Value.Name).ToArray())));
				Scripts.Add(string.Format("DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT' ;\r\n", srcTable.Name));
			}


		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			Scripts.Add(
				string.Format(
					"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0};\r\n",
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
					"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0};\r\n",
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


	}
}
