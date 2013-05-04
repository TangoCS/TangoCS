using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.DbSync
{
    internal class Trigger
    {
        internal string Name { get; set; }
        internal string Text { get; set; }
        
        internal void Sync(Table table)
        {
            var t = table.Model.Database.Tables[table.Name];
            if (t.Triggers.Contains(Name))
            {
                table.Model.Log("DROP TRIGGER " + Name);
                if (!table.Model.ScriptOnly)
                    t.Triggers[Name].Drop();
            }
            var trg = new Microsoft.SqlServer.Management.Smo.Trigger(t, Name)
            {
                TextMode = false,
                Insert = true,
                Update = true,
                Delete = false,
                TextBody = Text
            };
            table.Model.Log("CREATE TRIGGER " + Name);
            foreach (var s in trg.Script().OfType<string>())
                table.Model.Script(s);
            if (!table.Model.ScriptOnly)
                trg.Create();
        }
    }
}
