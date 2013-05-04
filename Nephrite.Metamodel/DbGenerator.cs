using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Nephrite.Metamodel.Model;
using Nephrite.Web;

namespace Nephrite.Metamodel
{
    /// <summary>
    /// Генерация таблиц БД по модели
    /// </summary>
    public class DbGenerator
    {
        Database db;
        TableCollection tables;
        ViewCollection views;
        ServerConnection sc;
        bool scriptOnly = false;
        bool identity = true;
        public DbGenerator(bool scriptOnly, bool identity)
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            sc = b.IntegratedSecurity ? new ServerConnection(b.DataSource) : new ServerConnection(b.DataSource, b.UserID, b.Password);
            sc.Connect();
            Server server = new Server(sc);
            db = server.Databases[b.InitialCatalog];
            this.scriptOnly = scriptOnly;
            this.identity = identity;
            if (scriptOnly)
            {
                tables = server.Databases["master"].Tables; // В этой БД заведомо нет наших таблиц
                views = server.Databases["master"].Views;
            }
            else
            {
                tables = db.Tables;
                views = db.Views;
            }
            ResultScript = new List<string>();
        }

		public Table GetTable(string tableName)
		{
			if (tables.Contains(tableName))
				return tables[tableName];
			return null;
		}

		public Table CreateMLTable(string tableName, bool guidpk, string pkName)
		{
			var tableData = new Table(db, tableName + "Data", "dbo");
			tableData.Columns.Add(new Column(tableData, tableData.Name + (guidpk ? "GUID" : "ID"), guidpk ? DataType.UniqueIdentifier : DataType.Int));
			tableData.Columns[0].Identity = !guidpk;

			var pkIndex = new Microsoft.SqlServer.Management.Smo.Index(tableData, "PK_" + tableData.Name);
			pkIndex.IndexKeyType = Microsoft.SqlServer.Management.Smo.IndexKeyType.DriPrimaryKey;
			foreach (var column in tableData.Columns.OfType<Microsoft.SqlServer.Management.Smo.Column>())
				pkIndex.IndexedColumns.Add(new Microsoft.SqlServer.Management.Smo.IndexedColumn(pkIndex, column.Name));
			tableData.Indexes.Add(pkIndex);

			tableData.Columns.Add(new Column(tableData, pkName, guidpk ? DataType.UniqueIdentifier : DataType.Int));
			tableData.Columns[1].Nullable = false;
			tableData.Columns.Add(new Column(tableData, "LanguageCode", DataType.NChar(2)));
			tableData.Columns[2].Nullable = false;
			tableData.Create();
			var fk1 = new ForeignKey(tableData, "FK_" + tableData.Name + "_" + tableName);
			fk1.DeleteAction = ForeignKeyAction.Cascade;
			fk1.ReferencedTable = tableName;
			fk1.Columns.Add(new ForeignKeyColumn(fk1, tableData.Columns[1].Name, pkName));
			fk1.Create();
			var fk2 = new ForeignKey(tableData, "FK_" + tableData.Name + "_C_Language");
			fk2.DeleteAction = ForeignKeyAction.Cascade;
			fk2.ReferencedTable = "C_Language";
			fk2.Columns.Add(new ForeignKeyColumn(fk2, tableData.Columns[2].Name, "LanguageCode"));
			fk2.Create();
			return tableData;
		}

		public void GenerateTriggers(List<string> tableNames)
		{
			foreach (Table table in tables)
			{
				if (!tableNames.Contains(table.Name.ToLower()))
				{
					var trg = table.Triggers.Cast<Trigger>().FirstOrDefault(o => o.Name == "TIUD_" + table.Name + "_UpdateTableInfo");
					if (trg != null)
					{
						trg.Drop();
						ResultScript.Add("DROP TRIGGER " + trg.Name);
					}
				}
				else
				{
					var trg = table.Triggers.Cast<Trigger>().FirstOrDefault(o => o.Name == "TIUD_" + table.Name + "_UpdateTableInfo");
					if (trg == null)
					{
						trg = new Trigger(table, "TIUD_" + table.Name + "_UpdateTableInfo");
						trg.TextMode = false;
						trg.Insert = true;
						trg.Update = true;
						trg.Delete = true;
						trg.TextBody = String.Format(updateTableInfoTriggerCode, table.Name);
						trg.Create();
						ResultScript.Add("CREATE TRIGGER " + trg.Name);
					}
				}
			}
		}

		public IEnumerable<Trigger> GetTableTriggers()
		{
			return tables.OfType<Table>().OrderBy(o => o.Name).SelectMany(o => o.Triggers.OfType<Trigger>());
		}

		public IEnumerable<Table> GetTables()
		{
			return tables.OfType<Table>().OrderBy(o => o.Name);
		}

		public Trigger GetTableTrigger(string tablename, string triggername)
		{
			Table t = tables[tablename];
			return t.Triggers[triggername];
		}

		public Trigger CreateTrigger(string tablename, string name)
		{
			Table t = tables[tablename];
			return new Trigger(t, name);
		}

		public void BeginTransaction()
		{
			sc.BeginTransaction();
		}

		public void CommitTransaction()
		{
			sc.CommitTransaction();
		}

		public void RollBackTransaction()
		{
			sc.RollBackTransaction();
		}

        public List<string> ResultScript { get; private set; }

        //Dictionary<int, Table> generatedTables = new Dictionary<int, Table>();
        //Dictionary<int, Table> generatedHistoryTables = new Dictionary<int, Table>();
        List<string> log;

        public List<string> GenerateObjectType(int[] ids)
        {
            log = new List<string>();
            var model = new DbSync.Model();
			
            foreach (var id in ids)
            {
                var objectType = AppMM.DataContext.MM_ObjectTypes.Single(o => o.ObjectTypeID == id);

                var table = model.AddTable();
                table.Name = objectType.SysName;
                table.PkName = objectType.PrimaryKey.Select(o => o.ColumnName).Join(",");
				if (objectType.PrimaryKey.Select(o => o.DataType).Distinct().Count() > 1)
					throw new Exception("В таблице " + objectType.FullTitle + " типы данных для столбцов первичных ключей различаются");
				table.PkType = objectType.PrimaryKey.Select(o => o.DataType).FirstOrDefault();
                table.Identity = objectType.PrimaryKey.Length == 1 && !objectType.BaseObjectTypeID.HasValue && !objectType.IsDataReplicated && !objectType.MM_Package.IsDataReplicated;

                UpdateColumns(objectType.MM_ObjectProperties.Where(o => !o.IsMultilingual && (!o.IsPrimaryKey || o.TypeCode == ObjectPropertyType.Object)).OrderBy(o => o.SeqNo), table);
                UpdateColumnsStandartFK(objectType.MM_ObjectProperties.Where(o => !o.IsMultilingual && !o.IsPrimaryKey), table);
                UpdateReferences(objectType.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.Object), table);

				if (objectType.MM_ObjectProperties.Any(o => o.SysName == "Activity" && o.RefObjectType.SysName == "WF_Activity" && !AppMM.DataContext.MM_ObjectTypes.Any(o1 => o1.SysName == objectType.SysName + "Transition")))
				{
					GenerateTransitionTable(model, objectType);
				}

				if (objectType.BaseObjectTypeID.HasValue)
				{
					table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
					{
						Cascade = true,
						Name = "FK_" + table.Name + "_" + objectType.BaseObjectType.SysName,
						RefColumnName = objectType.BaseObjectType.PrimaryKey.Select(o => o.ColumnName).ToArray(),
						RefTableName = objectType.BaseObjectType.SysName
						});
				}

                if (objectType.IsMultiLingual)
                {
                    var tableData = model.AddTable();
                    tableData.Name = table.Name + "Data";
                    tableData.PkName = tableData.Name + (table.PkType.SqlDataType == SqlDataType.UniqueIdentifier ? "GUID" : "ID");
					tableData.PkType = table.PkType;
                    tableData.Identity = !objectType.IsDataReplicated && !objectType.MM_Package.IsDataReplicated;
					tableData.Description = "Таблица содержит данные многоязычных свойств объектов класса " + objectType.Title;
                    tableData.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = table.PkName,
                        Nullable = false,
						Type = table.PkType,
						Description = "Ссылка на таблицу " + table.Name
                    });
					tableData.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
					{
						Cascade = true,
						Column = tableData.Columns[0],
						Name = "FK_" + tableData.Name + "_" + table.Name,
						RefTableName = table.Name,
						RefColumnName = table.PkName.Split(',')
					});

                    tableData.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = "LanguageCode",
                        Type = DataType.NChar(2),
                        Nullable = false,
						Description = "Ссылка на таблицу C_Language"
                    });
                    tableData.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                    {
                        Cascade = true,
                        Column = tableData.Columns[1],
                        Name = "FK_" + tableData.Name + "_C_Language",
                        RefTableName = "C_Language",
                        RefColumnName = new string[] { "LanguageCode" }
                    });

                    UpdateColumns(objectType.MM_ObjectProperties.Where(o => o.IsMultilingual), tableData);

                    string props = "";
                    foreach (var p in objectType.MM_ObjectProperties.Where(o => o.IsMultilingual))
                    {
                        props += ", " + objectType.SysName + "Data." + p.ColumnName;
                    }

                    string propsInh = "";
                    if (objectType.BaseObjectTypeID.HasValue)
                    {
                        foreach (var p in objectType.BaseObjectType.MM_ObjectProperties.Where(o => o.KindCode != "C"))
                        {
                            propsInh += ", " + objectType.BaseObjectType.SysName + (p.IsMultilingual ? "Data." : ".") + p.ColumnName;
                        }
                    }

                    string view = objectType.BaseObjectTypeID.HasValue ?
						String.Format(viewMLInh, objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","), props, objectType.BaseObjectType.SysName,
						objectType.BaseObjectType.PrimaryKey.Select(o => o.SysName).Join(","), propsInh) :
						String.Format(viewML, objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","), props);
                    var v = model.AddView();
                    v.Name = "V_" + objectType.SysName;
                    v.Text = view + (objectType.BaseObjectTypeID.HasValue && objectType.BaseObjectType.IsMultiLingual ?
                        String.Format(" LEFT OUTER JOIN [{0}Data] ON [{2}].[{3}] = [{0}Data].[{1}] AND {0}Data.LanguageCode = C_Language.LanguageCode",
						objectType.BaseObjectType.SysName, objectType.BaseObjectType.PrimaryKey.Select(o => o.ColumnName).Join(","),
						objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(",")) : "");

					UpdateColumnsStandartFK(objectType.MM_ObjectProperties.Where(o => o.IsMultilingual), tableData);
                }
                else
                {
                    if (objectType.BaseObjectTypeID.HasValue)
                    {
                        string propsInh = "";
                        if (objectType.BaseObjectTypeID.HasValue)
                        {
                            foreach (var p in objectType.BaseObjectType.MM_ObjectProperties.Where(o => o.KindCode != "C" && o.UpperBound == 1 && !o.IsPrimaryKey))
                            {
                                propsInh += ", " + objectType.BaseObjectType.SysName + (p.IsMultilingual ? "Data." : ".") + p.ColumnName;
                            }
                        }

                        var v = model.AddView();
                        v.Name = "V_" + objectType.SysName;
						string view = String.Format(viewInhML, objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","), "",
							objectType.BaseObjectType.SysName, objectType.BaseObjectType.PrimaryKey.Select(o => o.ColumnName).Join(","), propsInh);

                        v.Name = "V_" + objectType.SysName;
                        v.Text = view + (objectType.BaseObjectTypeID.HasValue && objectType.BaseObjectType.IsMultiLingual ?
                        String.Format(" LEFT OUTER JOIN [{0}Data] ON [{2}].[{3}] = [{0}Data].[{1}] AND {0}Data.LanguageCode = C_Language.LanguageCode",
						objectType.BaseObjectType.SysName, objectType.BaseObjectType.PrimaryKey.Select(o => o.ColumnName).Join(","),
						objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(",")) : "");
                    }
                }

                DbSync.Table hstTable = null;
                DbSync.Table chstTable = null;

                if (objectType.HistoryTypeCode != HistoryType.None)
                {
                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        chstTable = model.AddTable();
                        chstTable.Name = "CHST_" + table.Name;
                        chstTable.PkName = "ClassVersionID";
                        chstTable.Identity = !objectType.IsDataReplicated && !objectType.MM_Package.IsDataReplicated;

                        chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "ClassVersionDate",
                            Type = DataType.DateTime,
                            Nullable = false
                        });
                        chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "ClassVersionNumber",
                            Type = DataType.Int,
                            Nullable = false
                        });
                        chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "IsCurrent",
                            Type = DataType.Bit,
                            Nullable = false,
							ComputedText = "(isnull(CONVERT([bit],case when [VersionStartDate]<getdate() AND [VersionEndDate] IS NULL then (1) when [VersionStartDate]<getdate() AND getdate()<[VersionEndDate] then (1) else (0) end,(0)),(0)))"
                        });
                        chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "CreateUserID",
                            Type = DataType.Int,
                            Nullable = false
                        });
						chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
						{
							Name = "VersionStartDate",
							Type = DataType.DateTime,
							Nullable = false
						});
						chstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
						{
							Name = "VersionEndDate",
							Type = DataType.DateTime,
							Nullable = true
						});
						chstTable.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = false,
                            Column = chstTable.Columns[3],
                            Name = "FK_" + chstTable.Name + "_CreateUser",
                            RefTableName = "SPM_Subject",
                            RefColumnName = new string[] { "SubjectID" }
                        });
                    }

                    hstTable = model.AddTable();
                    hstTable.Name = "HST_" + table.Name;
                    hstTable.Identity = !objectType.IsDataReplicated && !objectType.MM_Package.IsDataReplicated;
					string pkName = objectType.PrimaryKey.Select(o => o.ColumnName).Join(",");
					if (pkName.EndsWith("GUID"))
						pkName = pkName.Substring(0, pkName.Length - 4);
					else if (pkName.EndsWith("ID"))
                        pkName = pkName.Substring(0, pkName.Length - 2);
                    hstTable.PkName = pkName + "Version" + (table.PkType.SqlDataType == SqlDataType.UniqueIdentifier ? "GUID" : "ID");
					hstTable.PkType = table.PkType;
                    hstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = table.PkName,
                        Nullable = false,
                        Type = table.PkType
                    });
                    hstTable.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                    {
                        Cascade = true,
                        Name = "FK_" + hstTable.Name + "_" + table.Name,
                        Column = hstTable.Columns[0],
                        RefTableName = table.Name,
                        RefColumnName = table.PkName.Split(',')
                    });
                    hstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = "VersionNumber",
                        Nullable = false,
                        Type = DataType.Int
                    });
                    hstTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = "IsCurrentVersion",
                        Nullable = false,
                        Type = DataType.Bit
                    });

                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        var classVersionColumn = new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "ClassVersionID",
                            Nullable = false,
                            Type = DataType.Int
                        };
                        hstTable.Columns.Add(classVersionColumn);
                        hstTable.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = false,
                            Column = classVersionColumn,
                            Name = "FK_" + hstTable.Name + "_CHST_" + table.Name,
                            RefTableName = "CHST_" + table.Name,
                            RefColumnName = new string[] { "ClassVersionID" }
                        });
                    }
                    UpdateColumns(objectType.MM_ObjectProperties.Where(o => !o.IsPrimaryKey && !o.IsMultilingual), hstTable);
                    UpdateColumnsStandartFK(objectType.MM_ObjectProperties.Where(o => !o.IsSystem), hstTable);
                    UpdateReferences(objectType.MM_ObjectProperties.Where(o => o.TypeCode == ObjectPropertyType.Object), hstTable);
                    if (objectType.IsMultiLingual)
                    {
                        var hstTableData = model.AddTable();
                        hstTableData.Name = hstTable.Name + "Data";
						hstTableData.PkName = pkName + "DataVersion" + (table.PkType.SqlDataType == SqlDataType.UniqueIdentifier ? "GUID" : "ID");
						hstTableData.PkType = hstTable.PkType;
                        hstTableData.Identity = !objectType.IsDataReplicated && !objectType.MM_Package.IsDataReplicated;
                        hstTableData.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = hstTable.PkName,
                            Nullable = false,
							Type = hstTable.PkType
                        });
                        hstTableData.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = true,
                            Column = hstTableData.Columns[0],
                            Name = "FK_" + hstTableData.Name + "_" + hstTable.Name,
                            RefTableName = hstTable.Name,
                            RefColumnName = hstTable.PkName.Split(',')
                        });

                        hstTableData.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = "LanguageCode",
                            Type = DataType.NChar(2),
                            Nullable = false
                        });
                        hstTableData.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = true,
                            Column = hstTableData.Columns[1],
                            Name = "FK_" + hstTableData.Name + "_C_Language",
                            RefTableName = "C_Language",
                            RefColumnName = new string[] { "LanguageCode" }
                        });

                        UpdateColumns(objectType.MM_ObjectProperties.Where(o => o.IsMultilingual), hstTableData);

                        string props = "";
                        foreach (var p in objectType.MM_ObjectProperties.Where(o => o.IsMultilingual))
                        {
                            props += ", HST_" + objectType.SysName + "Data." + p.SysName;
                        }

                        string view = String.Format(viewML, "HST_" + objectType.SysName, hstTable.PkName, props);
                        var v = model.AddView();
                        v.Name = "V_" + hstTable.Name;
                        v.Text = view;
                    }

                    var proc = model.AddProcedure();
                    proc.Name = objectType.SysName + "_CreateHistoryVersion";
					string inParam = (hstTable.PkType.SqlDataType == SqlDataType.UniqueIdentifier ? "guid" : "id");
                    proc.Parameters.Add(new Nephrite.Metamodel.DbSync.Parameter
                    {
                        Name = "@" + inParam,
						Type = hstTable.PkType
                    });
                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        proc.Parameters.Add(new Nephrite.Metamodel.DbSync.Parameter
                        {
                            Name = "@classversionid",
                            Type = DataType.Int
                        });
                    }
                    proc.Text = String.Format(@"
BEGIN
    SET NOCOUNT ON;

    declare @ver int,
            @verid int", hstTable.Name, table.PkName);
                    
                    proc.Text += String.Format(@"
    SELECT @ver = ISNULL(MAX(VersionNumber), 0) + 1
      FROM {0}
     WHERE {1} = @{2}
       and IsCurrentVersion = 1", hstTable.Name, table.PkName, inParam);

                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        proc.Text += @"
      and ClassVersionID = @classversionid";
                    }

           proc.Text += String.Format(@"
    if (@ver > 1)
        UPDATE {0}
           SET IsCurrentVersion = 0, LastModifiedDate = getdate()
         WHERE {1} = @{2}
           and IsCurrentVersion = 1
           ", hstTable.Name, table.PkName, inParam);

           if (objectType.HistoryTypeCode != HistoryType.Object)
           {
               proc.Text += String.Format(@"
      and ClassVersionID = @classversionid", hstTable.Name, table.PkName);
           }

           proc.Text += String.Format(@"
           
    INSERT INTO [{0}]
           ([VersionNumber]
           ,[IsCurrentVersion]", hstTable.Name, table.PkName);
                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        proc.Text += String.Format(@"
           ,[ClassVersionID]");
                    }

                    foreach(var p in objectType.MM_ObjectProperties.Where(o =>
                        !o.IsMultilingual && o.UpperBound == 1 &&
						o.KindCode != "D" &&
                        !String.IsNullOrEmpty(o.ColumnName) && 
                        o.KindCode != "C"))
                    {
						if (p.RefObjectPropertyID.HasValue && p.RefObjectProperty.UpperBound == 1 && p.RefObjectProperty.IsAggregate)
							continue;
                        proc.Text += String.Format(@"
            ,[{0}]", p.ColumnName);
                    }
                    proc.Text += @")
     SELECT @ver
           ,1";
                    if (objectType.HistoryTypeCode != HistoryType.Object)
                    {
                        proc.Text += String.Format(@"
           ,@classversionid");
                    }
                    foreach(var p in objectType.MM_ObjectProperties.Where(o =>
                        !o.IsMultilingual && o.UpperBound == 1 &&
						o.KindCode != "D" &&
						!String.IsNullOrEmpty(o.ColumnName) && 
                        o.KindCode != "C"))
                    {
						if (p.RefObjectPropertyID.HasValue && p.RefObjectProperty.UpperBound == 1 && p.RefObjectProperty.IsAggregate)
							continue;
                        proc.Text += String.Format(@"
            ,[{0}]", p.ColumnName);
                    }
                    proc.Text += String.Format(@"
      FROM [{0}]
     WHERE [{1}] = @{2}

    set @verid = SCOPE_IDENTITY()", table.Name, table.PkName, inParam);
                    foreach(var objectProperty in objectType.MM_ObjectProperties.Where(o => o.UpperBound == -1 &&
                        o.TypeCode == ObjectPropertyType.Object && (!o.RefObjectPropertyID.HasValue ||
                        o.RefObjectProperty.UpperBound == -1)))
                    {
                        proc.Text += String.Format(@"
    insert into [HST_{0}]([{1}], [{2}ID])
    select @verid, [{2}ID]
      from [{0}]
     where [{3}] = @id", objectProperty.AssoTableName, hstTable.PkName, objectProperty.SysName, table.PkName);
                    }
                    if (objectType.IsMultiLingual)
                    {
                        proc.Text += String.Format(@"
    
	
    INSERT INTO [{0}Data]
           ([{1}]
           ,[LanguageCode]", hstTable.Name, hstTable.PkName);
                        foreach(var p in objectType.MM_ObjectProperties.Where(o =>
                            o.IsMultilingual && o.KindCode != "C"))
                            proc.Text += String.Format(@"
           ,[{0}]", p.ColumnName);
                        proc.Text += String.Format(@")
    SELECT @verid
          ,[LanguageCode]");
                        foreach(var p in objectType.MM_ObjectProperties.Where(o =>
                            o.IsMultilingual && o.KindCode != "C"))
                            proc.Text += String.Format(@"
           ,[{0}]", p.ColumnName);
                        proc.Text += String.Format(@"
      FROM [{0}Data]
     WHERE [{1}] = @{2}", table.Name, table.PkName, inParam);
                    }
                    proc.Text += @"
END";

                    if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss ||
                        objectType.HistoryTypeCode == HistoryType.IdentifiersRetain)
                    {
                        proc = model.AddProcedure();
                        proc.Name = table.Name + "_CreateClassVersion";
                        proc.Parameters.Add(new Nephrite.Metamodel.DbSync.Parameter
                        {
                            Name = "@subjectid",
                            Type = DataType.Int
                        });
                        proc.Parameters.Add(new Nephrite.Metamodel.DbSync.Parameter
                        {
                            Name = "@copyitems",
                            Type = DataType.Bit
                        });
                        
                        proc.Text = String.Format(@"
BEGIN
    SET NOCOUNT ON;

    declare @ver int,
            @hstid int,
            @id int,
            @newid int
	
    SELECT @ver = ISNULL(MAX(ClassVersionNumber), 0) + 1
      FROM CHST_{0}

    INSERT INTO [CHST_{0}]
           ([ClassVersionDate]
           ,[ClassVersionNumber]
           ,[CreateUserID])
     VALUES
           (GetDate(),
           @ver,
           @subjectid)
    set @hstid = SCOPE_IDENTITY()

    if @copyitems = 0
        RETURN

    declare cur cursor local for
    select {1}
      from {0}
     where IsDeleted = 0

    open cur
    
    FETCH NEXT FROM cur INTO @id

    WHILE @@FETCH_STATUS = 0
    BEGIN", objectType.SysName, objectType.PrimaryKey.Single().ColumnName);
                        if (objectType.HistoryTypeCode == HistoryType.IdentifiersMiss)
                        {
                            string columns = String.Join(", ", objectType.MM_ObjectProperties.Where(o => !String.IsNullOrEmpty(o.ColumnName) &&
                            !o.IsPrimaryKey && o.KindCode != "C" && (o.UpperBound == 1 || o.TypeCode == ObjectPropertyType.String)).Select(o => o.ColumnName).ToArray());

                            proc.Text += String.Format(@"
        insert into {0}({1})
        select {1}
          from {0}
         where {2} = @id

        set @newid = SCOPE_IDENTITY()
", objectType.SysName, columns, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","));
                            if (objectType.IsMultiLingual)
                            {
                                columns = String.Join(", ", objectType.MM_ObjectProperties.Where(o => o.IsMultilingual && o.KindCode != "C" && (o.UpperBound == 1 || o.TypeCode == ObjectPropertyType.String)).Select(o => o.ColumnName).ToArray());
                                proc.Text += String.Format(@"
        insert into {0}Data({2}, {1})
        select {2}{1}
          from {0}Data
         where {2} = @id", objectType.SysName, columns, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","));
                            }
                        }
                        else
                        {
                            proc.Text += @"
        set @newid = @id";
                        }

                        proc.Text += String.Format(@"
        exec {0}_CreateHistoryVersion @newid, @hstid
        
        FETCH NEXT FROM cur INTO @id
    END

    CLOSE cur
    DEALLOCATE cur
END", objectType.SysName);

                        proc = model.AddProcedure();
                        proc.Name = table.Name + "_DeleteClassVersion";
                        proc.Parameters.Add(new Nephrite.Metamodel.DbSync.Parameter
                        {
                            Name = "@classversionid",
                            Type = DataType.Int
                        });

                        proc.Text = String.Format(@"
BEGIN
    SET NOCOUNT ON;

    delete from HST_{0}
	where ClassVersionID = @classversionid
	
	delete from {0}
	where not exists (select * from HST_{0} where HST_{0}.{1} = {0}.{1})

    delete from CHST_{0} where ClassVersionID = @classversionid
END", objectType.SysName, objectType.PrimaryKey.Select(o => o.ColumnName).Join(","));
                    }
                }

                if (objectType.IsReplicate)
                {
                    table.Triggers.Add(new Nephrite.Metamodel.DbSync.Trigger
                    {
                        Name = "TIU_" + table.Name + "_Replicate",
                        Text = String.Format(replicateTriggerCode, table.Name, table.PkName)
                    });
                }
            }
            try
            {
                log.Add("BEGIN TRAN");
                sc.BeginTransaction();
				model.ServerConnection = sc;
                model.Sync(db, ResultScript, log, identity, scriptOnly);
                
                log.Add("COMMIT");
                if (!scriptOnly)
                    sc.CommitTransaction();
                else
                    sc.RollBackTransaction();
            }
            catch (Exception e)
            {
                log.Add("ROLLBACK");
                sc.RollBackTransaction();
                while (e != null)
                {
					log.Add(e.Message + "\r\n" + e.StackTrace + "\r\n");
                    e = e.InnerException;
                }
            }
            return log;
        }

		private void GenerateTransitionTable(Nephrite.Metamodel.DbSync.Model model, MM_ObjectType objectType)
		{
			var table = model.AddTable();
			table.Name = objectType.SysName + "Transition";
			table.PkName = objectType.SysName + "TransitionID";
			table.Identity = true;
			table.Description = "История изменения состояния объектов " + objectType.Title;

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "CreateDate",
				Nullable = false,
				Type = DataType.DateTime,
				Description = "Дата установки состояния"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "Comment",
				Nullable = true,
				Type = DataType.NVarCharMax,
				Description = "Комментарий (резолюция)"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "IsCurrent",
				Nullable = false,
				Type = DataType.Bit,
				Description = "Признак Текущее состояние"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "IsLast",
				Nullable = false,
				Type = DataType.Bit,
				Description = "Признак Последнее состояние"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "SeqNo",
				Nullable = false,
				Type = DataType.Int,
				Description = "Порядковый номер"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "Parent" + (objectType.PrimaryKey.First().DataType.SqlDataType == SqlDataType.UniqueIdentifier ? "GUID" : "ID"),
				Nullable = false,
				Type = objectType.PrimaryKey.First().DataType,
				Description = "Ид объекта"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "SubjectID",
				Nullable = false,
				Type = DataType.Int,
				Description = "Ид учетной записи пользователя, установившего состояние"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "WorkflowID",
				Nullable = false,
				Type = DataType.Int,
				Description = "Ид рабочего процесса"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "ActivityID",
				Nullable = false,
				Type = DataType.Int,
				Description = "Ид состояния"
			});

			table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
			{
				Name = "TransitionID",
				Nullable = false,
				Type = DataType.Int,
				Description = "Ид перехода"
			});

			table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
			{
				Cascade = true,
				Column = new Nephrite.Metamodel.DbSync.Column { Name = "Parent" + (objectType.PrimaryKey.Single().DataType.SqlDataType == SqlDataType.UniqueIdentifier ? "GUID" : "ID") },
				Name = "FK_" + table.Name + "_Parent",
				RefTableName = objectType.SysName,
				RefColumnName = objectType.PrimaryKey.Select(o => o.ColumnName).ToArray()
			});

			table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
			{
				Cascade = false,
				Column = new Nephrite.Metamodel.DbSync.Column { Name = "SubjectID" },
				Name = "FK_" + table.Name + "_Subject",
				RefTableName = "SPM_Subject",
				RefColumnName = new string[] { "SubjectID" }
			});

			table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
			{
				Cascade = false,
				Column = new Nephrite.Metamodel.DbSync.Column { Name = "WorkflowID" },
				Name = "FK_" + table.Name + "_Workflow",
				RefTableName = "WF_Workflow",
				RefColumnName = new string[] { "WorkflowID" }
			});

			table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
			{
				Cascade = false,
				Column = new Nephrite.Metamodel.DbSync.Column { Name = "ActivityID" },
				Name = "FK_" + table.Name + "_Activity",
				RefTableName = "WF_Activity",
				RefColumnName = new string[] { "ActivityID" }
			});

			table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
			{
				Cascade = false,
				Column = new Nephrite.Metamodel.DbSync.Column { Name = "TransitionID" },
				Name = "FK_" + table.Name + "_Transition",
				RefTableName = "WF_Transition",
				RefColumnName = new string[] { "TransitionID" }
			});
		}
        
        void UpdateColumns(IEnumerable<MM_ObjectProperty> properties, DbSync.Table table)
        {
            // Проверка наличия нужных столбцов
            foreach (var objectProperty in properties)
            {
                if (objectProperty.KindCode == "C")
                    continue;
				if (objectProperty.RefObjectPropertyID.HasValue && objectProperty.RefObjectProperty.UpperBound == 1 && objectProperty.RefObjectProperty.IsAggregate)
					continue;
                if (objectProperty.UpperBound == 1 || objectProperty.TypeCode == ObjectPropertyType.File ||
					objectProperty.TypeCode == ObjectPropertyType.FileEx ||
                    objectProperty.TypeCode == ObjectPropertyType.String)
                {
                    table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                    {
                        Name = objectProperty.ColumnName,
                        Nullable = objectProperty.LowerBound == 0,
                        Type = objectProperty.DataType,
						DefaultValue = objectProperty.DefaultDBValue,
						Description = String.IsNullOrEmpty(objectProperty.Description) ? objectProperty.Title : objectProperty.Description,
						ComputedText = objectProperty.Expression
                    });

                    if (objectProperty.TypeCode == ObjectPropertyType.ZoneDateTime)
                    {
                        table.Columns.Add(new Nephrite.Metamodel.DbSync.Column
                        {
                            Name = objectProperty.ColumnName + "TimeZoneID",
                            Type = DataType.Int,
                            Nullable = objectProperty.LowerBound == 0
                        });
                    }
                }
                if (objectProperty.UpperBound == -1 && objectProperty.TypeCode == ObjectPropertyType.Object && 
                    (!objectProperty.RefObjectPropertyID.HasValue || objectProperty.RefObjectProperty.UpperBound == -1))
                {
                    var assoTable = table.Model.AddTable();
                    assoTable.Name = (table.Name.StartsWith("HST_") ? "HST_" : "") + objectProperty.AssoTableName;
                    assoTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column { Name = table.PkName, Type = table.PkType, Nullable = false, Description = "Ид объекта " + objectProperty.MM_ObjectType.Title });
					assoTable.Columns.Add(new Nephrite.Metamodel.DbSync.Column { Name = objectProperty.ColumnName, Type = objectProperty.RefObjectType.PrimaryKey.Single().DataType, Nullable = false, Description = "Ид объекта " + objectProperty.RefObjectType.Title });
                    assoTable.PkName = assoTable.Columns[0].Name + "," + assoTable.Columns[1].Name;
                    assoTable.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                    {
                        Cascade = true,
                        Name = "FK_" + assoTable.Name + "_" + table.Name,
                        RefTableName = table.Name,
                        Column = assoTable.Columns[0],
                        RefColumnName = table.PkName.Split(',')
                    });
                    assoTable.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                    {
                        Cascade = true,
                        Name = "FK_" + assoTable.Name + "_" + objectProperty.SysName,
                        RefTableName = objectProperty.RefObjectType.SysName,
                        Column = assoTable.Columns[1],
						RefColumnName = objectProperty.RefObjectType.PrimaryKey.Select(o => o.ColumnName).ToArray()
                    });
					assoTable.Description = "Таблица хранит значения свойства " + objectProperty.Title + " объектов класса " + objectProperty.MM_ObjectType.Title;
                }
            }
        }

        void UpdateColumnsStandartFK(IEnumerable<MM_ObjectProperty> properties, DbSync.Table table)
        {
            foreach (var objectProperty in properties)
            {
                if (!String.IsNullOrEmpty(objectProperty.Expression))
                    continue;
                if (objectProperty.UpperBound == 1 || objectProperty.TypeCode == ObjectPropertyType.File)
                {
                    string columnName = objectProperty.ColumnName;
                    
                    if (objectProperty.TypeCode == ObjectPropertyType.File)
                    {
                        string refTable = objectProperty.UpperBound == 1 ? "N_File" : "N_FileList";
                        table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = false,
                            Column = new Nephrite.Metamodel.DbSync.Column { Name = columnName },
                            Name = "FK_" + table.Name + "_" + objectProperty.SysName,
                            RefTableName = refTable,
                            RefColumnName = new string[] { objectProperty.UpperBound == 1 ? "FileID" : "FileListID" }
                        });
                    }
					// Внешний ключ будет мешать работе INSERT
					if (objectProperty.TypeCode == ObjectPropertyType.FileEx)
					{
						string refTable = "N_File";
						table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
						{
							Cascade = false,
							Column = new Nephrite.Metamodel.DbSync.Column { Name = columnName },
							Name = "FK_" + table.Name + "_" + objectProperty.SysName,
							RefTableName = refTable,
							RefColumnName = new string[] { "Guid" }
						});
					}
                    if (objectProperty.TypeCode == ObjectPropertyType.ZoneDateTime)
                    {
                        table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
                        {
                            Cascade = false,
							Column = new Nephrite.Metamodel.DbSync.Column { Name = columnName + "TimeZoneID" },
							Name = "FK_" + table.Name + "_" + objectProperty.SysName + "TimeZone",
                            RefTableName = "HST_N_TimeZone",
                            RefColumnName = new string[] { "TimeZoneVersionID" }
                        });
                    }
                }
            }
        }
       

        void UpdateReferences(IEnumerable<MM_ObjectProperty> properties, DbSync.Table table)
        {
            // Навесить внешние ключи
            foreach (var prop in properties)
            {
				if (prop.RefObjectPropertyID.HasValue && prop.RefObjectProperty.UpperBound == 1 && prop.RefObjectProperty.IsAggregate)
					continue;
				if (prop.UpperBound == 1 && prop.RefObjectTypeID.HasValue)
                {
					try
					{
						string[] refPK = prop.RefObjectType.PrimaryKey.Select(o => o.ColumnName).ToArray();
						table.ForeignKeys.Add(new Nephrite.Metamodel.DbSync.ForeignKey
						{
							Cascade = prop.DeleteRule == "C",
							SetNull = prop.DeleteRule == "N",
							Column = table.Columns.Single(o => o.Name == prop.ColumnName),
							Name = "FK_" + table.Name + "_" + prop.SysName,
							RefTableName = (prop.IsReferenceToVersion ? "HST_" : "") + prop.RefObjectType.SysName,
							RefColumnName = prop.IsReferenceToVersion ? (
								prop.RefObjectType.PrimaryKey.First().DataType.SqlDataType == SqlDataType.UniqueIdentifier ?
								refPK.Select(o => o.Substring(0, o.Length - 4) + "VersionGUID").ToArray() :
								refPK.Select(o => o.Substring(0, o.Length - 2) + "VersionID").ToArray()
							) : refPK
						});
					}
					catch(Exception e)
					{
						throw new Exception("Ошибка при обновлении внешнего ключа для свойства " + prop.FullSysName, e);
					}
                }
            }
        }

        const string replicateTriggerCode = @"BEGIN
    declare @VersionNumber int
    declare @VersionID int
    
	SET NOCOUNT ON;
    
    UPDATE N_ReplicationObject
       SET ChangeDate = getdate()
      FROM INSERTED
     WHERE N_ReplicationObject.ObjectTypeSysName = '{0}' AND N_ReplicationObject.ObjectID = convert(nvarchar(50), INSERTED.[{1}])

    INSERT INTO N_ReplicationObject(ObjectTypeSysName, ObjectID, ChangeDate)
    SELECT '{0}', [{1}], getdate()
      FROM INSERTED
     WHERE NOT EXISTS(SELECT * FROM N_ReplicationObject WHERE N_ReplicationObject.ObjectTypeSysName = '{0}' AND N_ReplicationObject.ObjectID = convert(nvarchar(50), INSERTED.[{1}]))
    
    SET NOCOUNT OFF;
END";
		const string updateTableInfoTriggerCode = @"BEGIN
	SET NOCOUNT ON;
	UPDATE N_TableInfo SET LastDataModify = GETDATE() WHERE TableName = '{0}'
	INSERT INTO N_TableInfo(TableName, LastDataModify)
	SELECT '{0}', GETDATE()
	 WHERE NOT EXISTS(SELECT * FROM N_TableInfo WHERE TableName = '{0}')
	SET NOCOUNT OFF;
END";

        const string viewML = @"SELECT {0}.*,
C_Language.LanguageCode{2}
  FROM {0} CROSS JOIN C_Language
 LEFT OUTER JOIN {0}Data ON {0}.{1} = {0}Data.{1}
 AND {0}Data.LanguageCode = C_Language.LanguageCode";
        const string viewMLInh = @"SELECT {0}.*,
C_Language.LanguageCode{2}{5}
  FROM {0} INNER JOIN {3} ON {0}.{1} = {3}.{4} CROSS JOIN C_Language
 LEFT OUTER JOIN {0}Data ON {0}.{1} = {0}Data.{1}
 AND {0}Data.LanguageCode = C_Language.LanguageCode";
        const string viewInhML = @"SELECT {0}.*,
C_Language.LanguageCode{2}{5}
  FROM {0} INNER JOIN {3} ON {0}.{1} = {3}.{4} CROSS JOIN C_Language";
    }
}
