using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel.DbSync
{
    internal class ForeignKey
    {
        internal string Name { get; set; }
        internal Column Column { get; set; }
        internal string RefTableName { get; set; }
        internal string[] RefColumnName { get; set; }
        internal bool Cascade { get; set; }
		internal bool SetNull { get; set; }

        internal void Sync(Table table, Microsoft.SqlServer.Management.Smo.Table t)
        {
            if (Column == null)
                Column = new Column { Name = table.PkName };

            if (!t.ForeignKeys.Contains(Name))
            {
                var fk = new Microsoft.SqlServer.Management.Smo.ForeignKey(t, Name)
                {
                    ReferencedTable = RefTableName,
					DeleteAction = FKA
                };

				for (int i = 0; i < RefColumnName.Length; i++ )
					fk.Columns.Add(new Microsoft.SqlServer.Management.Smo.ForeignKeyColumn(fk, Column.Name.Split(',')[i], RefColumnName[i]));
				table.Model.Log("CREATE FOREIGNKEY " + fk.Name + " ON DELETE " + FKA.ToString());
                foreach (var s in fk.Script().OfType<string>())
                    table.Model.Script(s);
                if (!table.Model.ScriptOnly)
                    fk.Create();
            }
            else
            {
                var f = t.ForeignKeys[Name];

                if (f.DeleteAction != FKA ||
                    f.Columns.Cast<Microsoft.SqlServer.Management.Smo.ForeignKeyColumn>().Select(o => o.Name).Join(",") != Column.Name ||
					f.Columns.Cast<Microsoft.SqlServer.Management.Smo.ForeignKeyColumn>().Select(o => o.ReferencedColumn).Join(",") != RefColumnName.Join(",") ||
                    f.ReferencedTable != RefTableName)
                {
                    table.Model.Log("DROP FOREIGNKEY " + f.Name);
                    f.Drop();

                    var fk = new Microsoft.SqlServer.Management.Smo.ForeignKey(t, Name)
                    {
                        ReferencedTable = RefTableName,
                        DeleteAction = FKA
                    };

					for (int i = 0; i < RefColumnName.Length; i++)
						fk.Columns.Add(new Microsoft.SqlServer.Management.Smo.ForeignKeyColumn(fk, Column.Name.Split(',')[i], RefColumnName[i]));
                    table.Model.Log("CREATE FOREIGNKEY " + fk.Name + " ON DELETE " + FKA.ToString());
                    foreach (var s in fk.Script().OfType<string>())
                        table.Model.Script(s);
                    if (!table.Model.ScriptOnly)
                        fk.Create();
                }
            }
            string indexName = "IX_" + Name.Replace("FK_","");
            if (!t.Indexes.Contains(indexName))
            {
                var ix = new Microsoft.SqlServer.Management.Smo.Index(t, indexName);
				for (int i = 0; i < Column.Name.Split(',').Length; i++)
					ix.IndexedColumns.Add(new Microsoft.SqlServer.Management.Smo.IndexedColumn { Parent = ix, Name = Column.Name.Split(',')[i] });
                table.Model.Log("CREATE INDEX " + ix.Name);
                foreach (var s in ix.Script().OfType<string>())
                    table.Model.Script(s);
                if (!table.Model.ScriptOnly)
                    ix.Create();  
            }
            else
            {
                var ix = t.Indexes[indexName];

                if (ix.IndexedColumns[0].Name != Column.Name)
                {
                    table.Model.Log("DROP INDEX " + ix.Name);
                    ix.Drop();

                    ix = new Microsoft.SqlServer.Management.Smo.Index(t, indexName);
					for (int i = 0; i < Column.Name.Split(',').Length; i++)
						ix.IndexedColumns.Add(new Microsoft.SqlServer.Management.Smo.IndexedColumn { Parent = ix, Name = Column.Name.Split(',')[i] });
                    table.Model.Log("CREATE INDEX " + ix.Name);
                    foreach (var s in ix.Script().OfType<string>())
                        table.Model.Script(s);
                    if (!table.Model.ScriptOnly)
                        ix.Create();
                }
            }
        }

		Microsoft.SqlServer.Management.Smo.ForeignKeyAction FKA
		{
			get { return Cascade ? Microsoft.SqlServer.Management.Smo.ForeignKeyAction.Cascade : (SetNull ? Microsoft.SqlServer.Management.Smo.ForeignKeyAction.SetNull : Microsoft.SqlServer.Management.Smo.ForeignKeyAction.NoAction); }
		}
	}
}
