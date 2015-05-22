using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Nephrite.Identity;
using Nephrite.SettingsManager;
using Nephrite.Web.CoreDataContext;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.Web.TaskManager
{
	[WebService(Namespace = "http://nephritetech.com/tessera/integration/1.0")]
	[WebServiceBinding(Name = "TaskManagerService", ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class TaskManagerService : System.Web.Services.WebService
	{
		public class UserCredentials : System.Web.Services.Protocols.SoapHeader
		{
			public string Login;
			public string Password;
		}

		void CheckCredentials()
		{
			var settings = DI.RequestServices.GetService<IPersistentSettings>();
			if (user == null)
				throw new Exception("Аутентификация не пройдена");
			if (user.Login != settings.Get("ReplicationLogin"))
				throw new Exception("Неправильный логин или пароль");
			if (user.Password != settings.Get("ReplicationPassword"))
				throw new Exception("Неправильный логин или пароль");
		}

		public UserCredentials user;

		[WebMethod]
		[SoapDocumentMethod(Binding = "TaskManagerService")]
		[SoapHeader("user")]
		public void RunTasks()
		{
			A.Model = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);
			Subject.System.Run(() => Nephrite.Web.TaskManager.TaskManager.Run(HttpContext.Current == null));
		}
	}
}