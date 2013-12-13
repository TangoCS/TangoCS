using System;
using System.Web;

namespace Nephrite.Web.TaskManager
{
	public class TaskManagerModule : IHttpModule
	{
		/// <summary>
		/// You will need to configure this module in the web.config file of your
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: http://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpModule Members

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PostAuthorizeRequest += new EventHandler(context_PostAuthorizeRequest);
		}

		void context_PostAuthorizeRequest(object sender, EventArgs e)
		{
			TaskManager.Run();
		}

		#endregion

	}
}
