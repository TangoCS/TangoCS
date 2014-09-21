using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using Nephrite.Web;
using System.Data.Linq;
using System.IO;
using Nephrite.Web.FileStorage;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.TextResources;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.FormsEngine
{
    public static class WebSiteCache
    {
        static object locker = new object();
        static DateTime timeStamp = DateTime.Now;
		static IDC_MetaStorage dc = (IDC_MetaStorage)A.Model;

        public static DateTime TimeStamp { get { return timeStamp; } }

        public static void Reset()
        {
            lock (locker)
            {
                HttpContext.Current.Cache.Remove("menuitems");
                AppSettings.ResetCache();

				dc.IN_Cache.DeleteAllOnSubmit(dc.IN_Cache);
				var c = dc.NewIN_Cache();
				c.TimeStamp = DateTime.Now;
                dc.IN_Cache.InsertOnSubmit(c);
                A.Model.SubmitChanges();
				TextResource.ResetCache();
				MView.ResetCache();
				//DynamicClassActivator.Clear();
            }
        }

        public static void CheckValid()
        {
			var stamp = dc.IN_Cache.FirstOrDefault();
            if (stamp != null)
            {
                if (timeStamp < stamp.TimeStamp)
                {
					lock (locker)
					{
						if (timeStamp < stamp.TimeStamp)
						{
							timeStamp = stamp.TimeStamp;

							HttpContext.Current.Cache.Remove("menuitems");
							AppSettings.ResetCache();
							AppSPM.Instance.RefreshCache();
							AppSPM.AccessRightManager.RefreshCache();
							TextResource.ResetCache();
							//DynamicClassActivator.Clear();
							SPM2.ResetCache();
						}
					}
                }
            }
            else
            {
                lock (locker)
                {
                    if (stamp == null)
                    {
						var c = dc.NewIN_Cache();
						c.TimeStamp = DateTime.Now;
						dc.IN_Cache.InsertOnSubmit(c);
                        A.Model.SubmitChanges();
                    }
                }
            }
        }
    }
}
