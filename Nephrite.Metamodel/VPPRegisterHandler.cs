using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Nephrite.Web;
using Nephrite.Web.ErrorLog;
using Nephrite.Web.SPM;

namespace Nephrite.Metamodel
{
    public class VPPRegisterHandler : IHttpModule
    {
        static bool fProviderInitialized = false;
        static object locker = new object();

        public void Init(HttpApplication context)
        {
            if (!fProviderInitialized)
            {
                lock (locker)
                {
                    if (!fProviderInitialized)
                    {
                        FormsVirtualPathProvider fVPP = new FormsVirtualPathProvider();
                        HostingEnvironment.RegisterVirtualPathProvider(fVPP);
                        fProviderInitialized = true;
                    }
                }
            }
			context.BeginRequest += new EventHandler(context_BeginRequest);
			context.Error += new EventHandler(context_Error);
			MacroManager.Register("sid", () => AppSPM.GetCurrentSubjectID());
        }

		void context_Error(object sender, EventArgs e)
		{
			foreach (var ex in HttpContext.Current.AllErrors)
				ErrorLogger.Log(ex);
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			HttpContext.Current.Items["RequestBeginDate"] = DateTime.Now;
			WebSiteCache.CheckValid();
		}

        public void Dispose()
        {
        }
    }
}
