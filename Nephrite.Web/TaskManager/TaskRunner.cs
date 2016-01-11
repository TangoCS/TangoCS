using System;
using Nephrite.Data;

namespace Nephrite.Web.TaskManager
{
	public static class TaskRunner
	{
		public static void RunTasks(string interopServiceUrl, string connectionString, string dbType)
		{
			TaskManagerServiceReference.TaskManagerService svc = new TaskManagerServiceReference.TaskManagerService();
			svc.Url = interopServiceUrl;
			svc.AllowAutoRedirect = true;
			svc.UseDefaultCredentials = true;
			svc.PreAuthenticate = true;
			svc.Timeout = 1000 * 60 * 30; // 30 минут

			ConnectionManager.ConnectionString = connectionString;

			DBType? dbtypeenum = Enum.Parse(typeof(DBType), dbType.ToUpper()) as DBType?;
			if (dbtypeenum == null) throw new Exception("Incorrect DBType parameter");

			ConnectionManager.DBType = dbtypeenum.Value;
			var dca = A.RequestServices.GetService<IDataContextActivator>();
			A.Model = dca.CreateInstance(ConnectionManager.ConnectionString);
			//A.Model = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);

			var settings = A.RequestServices.GetService<IPersistentSettings>();

			var sl = settings.Get("ReplicationLogin");
			if (sl.IsEmpty())
				throw new Exception("Системный параметр ReplicationLogin отсутствует или не задан");
			var sp = settings.Get("ReplicationPassword");
			if (sp.IsEmpty())
				throw new Exception("Системный параметр ReplicationPassword отсутствует или не задан");

			svc.UserCredentialsValue = new TaskManagerServiceReference.UserCredentials { Login = sl, Password = sp };

			TaskManager.RunService(connectionString);
			A.Model.Dispose();

			svc.RunTasks();
		}
	}
}
