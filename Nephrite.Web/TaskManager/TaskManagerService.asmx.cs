using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Nephrite.Data;
using Nephrite.Identity;

namespace Nephrite.Web.TaskManager
{
	[WebService(Namespace = "http://nephritetech.com/tessera/integration/1.0")]
	[WebServiceBinding(Name = "TaskManagerService", ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class TaskManagerService : WebService
	{
		IIdentityManager<int> _identity;
		IPersistentSettings _settings;
		IDataContextActivator _dcActivator;

		public TaskManagerService()
		{
			_identity = A.RequestServices.GetService<IIdentityManager<int>>();
			_settings = A.RequestServices.GetService<IPersistentSettings>();
			_dcActivator = A.RequestServices.GetService<IDataContextActivator>();
		}

		public class UserCredentials : SoapHeader
		{
			public string Login;
			public string Password;
		}

		void CheckCredentials()
		{
			if (user == null)
				throw new Exception("Аутентификация не пройдена");
			if (user.Login != _settings.Get("ReplicationLogin"))
				throw new Exception("Неправильный логин или пароль");
			if (user.Password != _settings.Get("ReplicationPassword"))
				throw new Exception("Неправильный логин или пароль");
		}

		public UserCredentials user;

		[WebMethod]
		[SoapDocumentMethod(Binding = "TaskManagerService")]
		[SoapHeader("user")]
		public void RunTasks()
		{
			//A.Model = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);
			A.Model = _dcActivator.CreateInstance();
			_identity.RunAs(_identity.SystemSubject, () => TaskManager.Run(HttpContext.Current == null));
		}
	}
}