using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;

namespace Nephrite
{
    public static class Settings
    {
		// Где лежит кэш представлений
		public static string ViewCachePath;
		public static bool EnableViewCache = true;

        // Где лежат контролы
        public static string ControlsPath;
		// Где лежат картинки
		static string imagespath;
		public static string ImagesPath
		{
			get
			{
				return imagespath;
			}
		}

		// Где лежат javascript
		public static string JSPath
		{
			get
			{
				return (HttpRuntime.AppDomainAppVirtualPath + "/_controltemplates/Nephrite.Web/js/").Replace("//", "/");
			}
		}
		// базовые контролы
		public static string BaseControlsPath
		{
			get
			{
				return (HttpRuntime.AppDomainAppVirtualPath + "/_controltemplates/Nephrite.Web/").Replace("//", "/");
			}
		}

		static NavMenuButtonsMode navMenuButtonsMode = NavMenuButtonsMode.SmallButtons;
		public static NavMenuButtonsMode NavMenuButtonsMode
		{
			get { return navMenuButtonsMode; }
		}

        static int pageSize;
		static int maxFileSize;
		static int maxTotalFilesSize;
        static string systemTitle;
        
        static Settings()
        {
			if (HttpContext.Current == null)
				return;
            Reset();
        }

        public static int PageSize
        {
            get { return pageSize; }
        }

		public static int MaxFileSize
		{
			get { return maxFileSize; }
		}
		public static int MaxTotalFilesSize
		{
			get { return maxTotalFilesSize; }
		}


        public static string SystemTitle
        {
            get { return systemTitle; }
        }

        public static void Reset()
        {
            if (!Int32.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageSize))
                pageSize = 100;
			if (!Int32.TryParse(ConfigurationManager.AppSettings["MaxFileSize"], out maxFileSize))
				maxFileSize = 5;
			if (!Int32.TryParse(ConfigurationManager.AppSettings["MaxTotalFilesSize"], out maxTotalFilesSize))
				maxTotalFilesSize = 25;

            systemTitle = ConfigurationManager.AppSettings["SystemTitle"] ?? String.Empty;
			ControlsPath = (HttpRuntime.AppDomainAppVirtualPath + ConfigurationManager.AppSettings["ControlsPath"]).Replace("//", "/");

			imagespath = (HttpRuntime.AppDomainAppVirtualPath + (ConfigurationManager.AppSettings["ImagesPath"] ?? "/i/")).Replace("//", "/");

			if (ConfigurationManager.AppSettings["NavMenuButtonsMode"] == "BigButtons")
				navMenuButtonsMode = NavMenuButtonsMode.BigButtons;

			if (ConfigurationManager.AppSettings["ViewCachePath"] != null)
				ViewCachePath = ConfigurationManager.AppSettings["ViewCachePath"];
			else
				ViewCachePath = HttpContext.Current.Server.MapPath("~/App_Data/ViewCache");

			if (ConfigurationManager.AppSettings["EnableViewCache"] != null)
				EnableViewCache = ConfigurationManager.AppSettings["EnableViewCache"] != "0" && ConfigurationManager.AppSettings["EnableViewCache"] != "false";
        }
    }

	public enum NavMenuButtonsMode
	{
		SmallButtons,
		BigButtons
	}
}
