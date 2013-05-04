using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Nephrite.Web.App;
using Nephrite.Metamodel;
//using Nephrite.Workflow;
using System.Web.Hosting;
using System.IO;
using Nephrite.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Tessera
{
	public class Global : System.Web.HttpApplication
	{
        bool check(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

		protected void Application_Start(object sender, EventArgs e)
		{
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(check);

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\" + AppMM.DBName() + ".Model.dll")))
                AppDomain.CurrentDomain.Load(AppMM.DBName() + ".Model");
			if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Nephrite.CMS.dll")))
				AppDomain.CurrentDomain.Load("Nephrite.CMS");
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\OppsExt.dll")))
                AppDomain.CurrentDomain.Load("OppsExt");

        }

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			AppBase.DataContext = AppMM.DataContext;
            WebSiteCache.CheckValid();
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
            foreach (var ex in HttpContext.Current.AllErrors)
                ErrorLogger.Log(ex, AppBase.DataContext);
		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}
