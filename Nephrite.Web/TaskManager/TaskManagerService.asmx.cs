using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.SPM;

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
			if (user == null)
				throw new Exception("Аутентификация не пройдена");
			if (user.Login != AppSettings.Get("ReplicationLogin"))
				throw new Exception("Неправильный логин или пароль");
			if (user.Password != AppSettings.Get("ReplicationPassword"))
				throw new Exception("Неправильный логин или пароль");
		}

		public UserCredentials user;

		[WebMethod]
		[SoapDocumentMethod(Binding = "TaskManagerService")]
		[SoapHeader("user")]
		public void RunTasks()
		{
			AppSPM.RunWithElevatedPrivileges(() => TaskManager.Run());
		}
	}
}