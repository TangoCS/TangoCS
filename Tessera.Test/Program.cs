using System;
using Nephrite.Web.Hibernate;

namespace Tessera.Test
{
    class Program
    {
        private static void Main(string[] args)
        {
			//A.DBScript = new DBScriptMSSQL("DBO");
			//ConnectionManager.SetConnectionString("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Servants;Data Source=(local)");
			//HDataContext.DBType = DBType.MSSQL;

            //ConnectionManager.SetConnectionString("Database=SRVNTS;UserID=dbo;Password=123*(iop;Server=176.227.213.5:50000");
            //A.DBType = DBType.DB2;
			//A.DBScript = new DBScriptDB2("DBO");

			//ConnectionManager.SetConnectionString("Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=Qq121212;");
			//A.DBType = DBType.POSTGRESQL;
			//A.DBScript = new DBScriptPostgreSQL("dbo");
			//var schema = new PostgreSQLMetadataReader().ReadSchema("dbo");

			//GetDict(App.DataContext.SPM_Subject);
	
			//Listeners l = new Listeners();
			//var ael = new AuditEventListener();
			//l.PreDeleteEventListeners.Add(ael);
			//l.PreUpdateEventListeners.Add(ael);

			//l.PostDeleteEventListeners.Add(ael);
			//l.PostInsertEventListeners.Add(ael);
			//l.PostUpdateEventListeners.Add(ael);

			//var dc1 = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), l);
			//A.Model = dc1;
			//A.Model.ExecuteCommand("SET SCHEMA = 'DBO';");

			//ModelFactory m = new ModelFactory();
			//m.CreateSolution();
			


			
          
            //Console.WriteLine(App.DataContext.Log.ToString());
            //Console.WriteLine(A.Model.Log.ToString());
            Console.ReadKey();
        }
    }
}
