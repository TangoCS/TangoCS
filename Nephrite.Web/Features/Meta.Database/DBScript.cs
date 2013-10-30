using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Meta.Database
{
	public class DBScript : IDBScript
	{
		public DBScript()
		{
			Scripts = new List<string>();
		}

		public List<string> Scripts { get; set; }

		public void DeleteTable(Table currentTable)
		{
			Scripts.Add(string.Format(@"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}') drop table {0};", currentTable.Name));
		}

		public void CreateTable(Table srcTable)
		{

			var tableScript = string.Format("CREATE TABLE {0} ({1}) ;", srcTable.Name, "{0}");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
			var columnsScript =
				srcTable.Columns.Aggregate(string.Empty,
										   (current, srcColumn) =>
										   current +
										   string.Format("{0} {1} {2} {3},", srcColumn.Value.Name,
														 srcColumn.Value.Type,
														srcColumn.Value.IsPrimaryKey && srcTable.Identity ? " IDENTITY(1,1)" : "",
														 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

			tableScript = string.Format(tableScript, columnsScript);
			if (srcTable.ForeignKeys.Count > 0)
			{
				tableScript = srcTable.ForeignKeys.Aggregate(tableScript, (current, foreignKey) => current + string.Format(@"
                                    ALTER TABLE {0}  WITH NOCHECK ADD  CONSTRAINT {1} FOREIGN KEY({2})
                                    REFERENCES {3} ({4}) ;
                                    
                                    ALTER TABLE {0} CHECK CONSTRAINT {1} ;
                                    ", srcTable.Name, foreignKey.Value.Name, string.Join(",", foreignKey.Value.Columns), foreignKey.Value.RefTable, string.Join(",", foreignKey.Value.RefTableColumns)));
			}

			if (srcTable.PrimaryKey != null)
			{

				tableScript += string.Format(@" 
                                   ALTER TABLE {0}
                                   ADD CONSTRAINT {1} PRIMARY KEY ({2}) ;", srcTable.Name,
													  srcTable.PrimaryKey.Name,
													  string.Join(",", srcTable.PrimaryKey.Columns));// {0)- TableName  {1} - Constraint Name, {2} - Columns,{3} - Ref Table ,{4} - Ref Columns
			}


			Scripts.Add(tableScript);

		}




		public void CreateForeignKey(ForeignKey srcforeignKey, Table currentTable)
		{
			Scripts.Add(string.Format(@" ALTER TABLE {0}  WITH NOCHECK ADD  CONSTRAINT {1} FOREIGN KEY({2})
                                            REFERENCES {3} ({4});
                                            
                                            ALTER TABLE {0} CHECK CONSTRAINT {1};
                                             ", currentTable.Name,
													srcforeignKey.Name,
													string.Join(",", srcforeignKey.Columns),
													srcforeignKey.RefTable,
													string.Join(",", srcforeignKey.RefTableColumns)));
		}

		public void DeleteForeignKey(ForeignKey currentForeignKey, Table currentTable)
		{
			Scripts.Add(string.Format(@" ALTER TABLE {0} DROP CONSTRAINT {1} ; ", currentTable.Name,
										  currentForeignKey.Name));
		}

		public void DeleteColumn(Column currentColumn, Table currentTable)
		{
			// При удалении колонки  удаляем  и её pk и fk 
			if (currentTable.PrimaryKey.Columns.Any(t => t == currentColumn.Name))
			{
				DeletePrimaryKey(currentTable.PrimaryKey, currentTable);
				currentTable.PrimaryKey = null;
			}


			var toRemove = currentTable.ForeignKeys.Where(t => t.Value.Columns.Any(c => c == currentColumn.Name)).Select(t => t.Key).ToArray();
			foreach (var key in toRemove)
			{

				DeleteForeignKey(currentTable.ForeignKeys[key], currentTable);
				currentTable.ForeignKeys.Remove(key);
			}

			Scripts.Add(string.Format(@"  ALTER TABLE [{0}] DROP COLUMN [{1}]  ;", currentTable.Name,
										  currentColumn.Name));
		}
		public void AddColumn(Column srcColumn, Table currentTable, Table srcTable)
		{
			Scripts.Add(string.Format(@" ALTER TABLE [{5}] ADD [{0}] {1} {2} {3} {4} ; ",
									srcColumn.Name,
									srcColumn.Type,
									srcColumn.IsPrimaryKey && srcTable.Identity ? "IDENTITY(1,1)" : "",
									srcColumn.Nullable ? "NULL" : "NOT NULL",
									(!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format("DEFAULT({0})", srcColumn.DefaultValue) : ""),
									currentTable.Name));//    // {0}- Название колонки, {1} - Тип колонки, {2} - IDENTITY, {3}- NULL
		}
		public void ChangeColumn(Column srcColumn, Table currentTable)
		{
			Scripts.Add(string.Format(@" ALTER TABLE [{0}]
                                          ALTER COLUMN [{1}] {2} {3} {4};",
										  currentTable.Name,
										  srcColumn.Name,
										  srcColumn.Type,
										  (!string.IsNullOrEmpty(srcColumn.DefaultValue) ? string.Format("DEFAULT({0})", srcColumn.DefaultValue) : ""),
										  srcColumn.Nullable ? "NULL" : "NOT NULL"));
		}

		public void DeletePrimaryKey(PrimaryKey currentPrimaryKey, Table currentTable)
		{
			Scripts.Add(string.Format(@" ALTER TABLE [{0}] DROP CONSTRAINT {1}  ;", currentTable.Name,
										  currentPrimaryKey.Name));
		}

		public void CreatePrimaryKey(PrimaryKey srcPrimaryKey, Table curentTable)
		{
			Scripts.Add(string.Format(@" 
                                   ALTER TABLE [{0}]
                                   ADD CONSTRAINT {1} PRIMARY KEY ({2}) ;", curentTable.Name,
													   srcPrimaryKey.Name,
													   string.Join(",", srcPrimaryKey.Columns)));
		}

		public void DeleteTrigger(Trigger currentTrigger)
		{
			Scripts.Add(string.Format(@" DROP TRIGGER {0}  ", currentTrigger.Name));
		}

		public void CreateTrigger(Trigger srcTrigger)
		{
			Scripts.Add(srcTrigger.Text);
		}

		public void DeleteView(View currentView)
		{
			Scripts.Add(string.Format(@"IF OBJECT_ID ('{0}', 'V') IS NOT NULL DROP VIEW {0} ;", currentView.Name));
		}

		public void CreateView(View srcView)
		{
			Scripts.Add(srcView.Text);
		}

		public void SyncIdentity(Column currentColumn, Table currentTable, Table srcTable)
		{
			// Удаляем ссылки pk fk так же обнуляем их обьекты и таблицы для создания их в дальнейшем
			if (currentTable.PrimaryKey.Columns.Any(t => t == currentColumn.Name))
			{
				DeletePrimaryKey(currentTable.PrimaryKey, currentTable);
				currentTable.PrimaryKey = null;
			}
			var toRemove = currentTable.ForeignKeys.Select(t => t.Key).ToArray();
			foreach (var key in toRemove)
			{

				DeleteForeignKey(currentTable.ForeignKeys[key], currentTable);
				currentTable.ForeignKeys.Remove(key);
			}

			if (srcTable.Identity)
			{


				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1}) ", srcTable.Name, "{0}");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
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
				Scripts.Add(string.Format(@"ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE);", srcTable.Name));
				Scripts.Add(string.Format(@"SET IDENTITY_INSERT Tmp_{0} ON;", srcTable.Name));
				Scripts.Add(string.Format(@"IF EXISTS(SELECT * FROM {0})
                                                EXEC('INSERT INTO Tmp_{0}
                                                SELECT * FROM {0} WITH (HOLDLOCK TABLOCKX)');", srcTable.Name));
				Scripts.Add(string.Format(@"SET IDENTITY_INSERT Tmp_{0} OFF;DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT' ;", srcTable.Name));
			}
			else
			{

				var tableScript = string.Format("CREATE TABLE Tmp_{0} ({1}) ", srcTable.Name, "{0}");// {0}- Название таблицы, {1}- Список колонок, {2} - ON [PRIMARY]
				var columnsScript =
					srcTable.Columns.Aggregate(string.Empty,
											   (current, srcColumn) =>
											   current +
											   string.Format("{0} {1} {2} {3},", srcColumn.Value.Name,
															 srcColumn.Value.Type,
															 "",
															 srcColumn.Value.Nullable ? "NULL" : "NOT NULL")).TrimEnd(',');

				tableScript = string.Format(tableScript, columnsScript);
				Scripts.Add(tableScript);
				Scripts.Add(string.Format(@"ALTER TABLE Tmp_{0} SET (LOCK_ESCALATION = TABLE);", srcTable.Name));
				Scripts.Add(string.Format(@"IF EXISTS(SELECT * FROM {0})
                                                EXEC('INSERT INTO Tmp_{0}
                                                SELECT * FROM {0} WITH (HOLDLOCK TABLOCKX)');", srcTable.Name));
				Scripts.Add(string.Format(@"DROP TABLE {0}; EXECUTE sp_rename N'Tmp_{0}', N'{0}', 'OBJECT' ;", srcTable.Name));
			}


		}

		public void DeleteProcedure(Procedure currentProcedure)
		{
			Scripts.Add(
				string.Format(
					@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0};",
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
					@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = '{0}') DROP PROCEDURE {0};",
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
			return string.Format("varbinnary({0})", length == -1 ? "max" : length.ToString());
		}
	}
}
