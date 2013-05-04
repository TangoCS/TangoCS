using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Nephrite.Metamodel.DbSync
{
    internal class Model
    {
        internal List<Table> Tables { get; private set; }
        internal List<View> Views { get; private set; }
        internal List<Procedure> Procedures { get; private set; }

        internal Model()
        {
            Tables = new List<Table>();
            Views = new List<View>();
            Procedures = new List<Procedure>();
        }

        internal Table AddTable()
        {
            var table = new Table { Model = this };
            Tables.Add(table);
            return table;
        }

        internal View AddView()
        {
            var view = new View { Model = this };
            Views.Add(view);
            return view;
        }

        internal Procedure AddProcedure()
        {
            var procedure = new Procedure { Model = this };
            Procedures.Add(procedure);
            return procedure;
        }

        internal Database Database { get; private set; }
		internal ServerConnection ServerConnection { get; set; }
        List<string> script;
        List<string> log;
        internal bool Identity { get; private set; }
        internal bool ScriptOnly { get; private set; }

        public void Sync(Database db, List<string> script, List<string> log, bool identity, bool scriptOnly)
        {
            Database = db;
            this.script = script;
            this.log = log;
            Identity = identity;
            ScriptOnly = scriptOnly;

            foreach (var o in Tables)
                o.Sync();

            foreach (var o in Tables)
                o.SyncFK();

            foreach (var o in Views)
                o.Sync();

            foreach (var o in Procedures)
                o.Sync();
        }

        public void Log(string s)
        {
            log.Add(s);
        }

        public void Script(string s)
        {
            script.Add(s);
        }
    }
}
