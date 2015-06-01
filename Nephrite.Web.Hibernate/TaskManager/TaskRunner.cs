using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite.SettingsManager;
using Nephrite.Web.CoreDataContext;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.Web.TaskManager
{
	public static class TaskRunner
	{
		public static void RunTasks(string interopServiceUrl, string connectionString, string dbType)
		{
			Nephrite.Web.Hibernate.TaskManagerServiceRerefence.TaskManagerService svc =
				new Nephrite.Web.Hibernate.TaskManagerServiceRerefence.TaskManagerService();
			svc.Url = interopServiceUrl;
			svc.AllowAutoRedirect = true;
			svc.UseDefaultCredentials = true;
			svc.PreAuthenticate = true;
			svc.Timeout = 1000 * 60 * 30; // 30 минут

			ConnectionManager.SetConnectionString(connectionString);

			DBType? dbtypeenum = Enum.Parse(typeof(DBType), dbType.ToUpper()) as DBType?;
			if (dbtypeenum == null) throw new Exception("Incorrect DBType parameter");

			A.DBType = dbtypeenum.Value;
			A.Model = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);

			var settings = DI.RequestServices.GetService<IPersistentSettings>();

			var sl = settings.Get("ReplicationLogin");
			if (sl.IsEmpty())
				throw new Exception("Системный параметр ReplicationLogin отсутствует или не задан");
			var sp = settings.Get("ReplicationPassword");
			if (sp.IsEmpty())
				throw new Exception("Системный параметр ReplicationPassword отсутствует или не задан");

			svc.UserCredentialsValue = new Nephrite.Web.Hibernate.TaskManagerServiceRerefence.UserCredentials { Login = sl, Password = sp };

			Nephrite.Web.TaskManager.TaskManager.RunService(connectionString);
			A.Model.Dispose();

			svc.RunTasks();
		}
	}
}
