using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using System.Xml.Linq;
using Nephrite.Web;
using System.IO;
using System.Threading;
using System.Net;
using Nephrite.Web.SPM;
using System.Configuration;
using Nephrite.Web.TaskManager;
using Nephrite.Web.FileStorage;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.SettingsManager;

namespace Tessera
{
    public class ReplicationTask
    {
		public static void RunTasks(string interopServiceUrl, string connectionString)
		{
			ReplicationService.Replication svc = new ReplicationService.Replication();
			svc.Url = interopServiceUrl;
			svc.AllowAutoRedirect = true;
			svc.UseDefaultCredentials = true;
			svc.PreAuthenticate = true;
			svc.Timeout = 1000 * 60 * 30; // 30 минут

			using (var sdc = (IDC_Settings)A.Model.NewDataContext())
			{
				var sl = sdc.IN_Setting.FirstOrDefault(o => o.SystemName == "ReplicationLogin");
				if (sl == null)
					throw new Exception("Системный параметр ReplicationLogin отсутствует в БД");
				var sp = sdc.IN_Setting.FirstOrDefault(o => o.SystemName == "ReplicationPassword");
				if (sp == null)
					throw new Exception("Системный параметр ReplicationPassword отсутствует в БД");
				svc.UserCredentialsValue = new Tessera.ReplicationService.UserCredentials { Login = sl.Value, Password = sp.Value };
			}

			svc.RunTasks();

			TaskManager.RunService(connectionString);
		}
    }
}
