using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Nephrite.Web.Controls;
using System.IO;
using System.Web;

namespace Nephrite.Web
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

		// библиотека загружаемых картинок
		public static string ImageLibratyPath
		{
			get
			{
				return "images";
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

			addomain = ConfigurationManager.AppSettings["ADDomain"] ?? String.Empty;
			aduser = ConfigurationManager.AppSettings["ADUser"] ?? String.Empty;
			adpassword = ConfigurationManager.AppSettings["ADPassword"] ?? String.Empty;

            imageProviderAssembly = ConfigurationManager.AppSettings["ImageProviderAssembly"] ?? String.Empty;
            imageProviderClass = ConfigurationManager.AppSettings["ImageProviderClass"] ?? String.Empty;
			imagespath = (HttpRuntime.AppDomainAppVirtualPath + (ConfigurationManager.AppSettings["ImagesPath"] ?? "/i/n/")).Replace("//", "/");
			
            fileProviderAssembly = ConfigurationManager.AppSettings["FileProviderAssembly"] ?? String.Empty;
            fileProviderClass = ConfigurationManager.AppSettings["FileProviderClass"] ?? String.Empty;

			accessRightsManager = ConfigurationManager.AppSettings["AccessRightsManager"] ?? String.Empty;
			settingsManager = ConfigurationManager.AppSettings["SettingsManager"] ?? String.Empty;


			if (ConfigurationManager.AppSettings["NavMenuButtonsMode"] == "BigButtons")
				navMenuButtonsMode = NavMenuButtonsMode.BigButtons;

			if (ConfigurationManager.AppSettings["ViewCachePath"] != null)
				ViewCachePath = ConfigurationManager.AppSettings["ViewCachePath"];
			else
				ViewCachePath = HttpContext.Current.Server.MapPath("~/App_Data/ViewCache");

			if (ConfigurationManager.AppSettings["EnableViewCache"] != null)
				EnableViewCache = ConfigurationManager.AppSettings["EnableViewCache"] != "0" && ConfigurationManager.AppSettings["EnableViewCache"] != "false";
        }

		static string accessRightsManager;
		public static string AccessRightsManager
		{
			get
			{
				return accessRightsManager;
			}
		}
		static string settingsManager;
		public static string SettingsManager
		{
			get
			{
				return settingsManager;
			}
		}


		static string addomain;
		static string aduser;
		static string adpassword;

		public static string ADDomain
		{
			get { return addomain; }
		}

		public static string ADUser
		{
			get { return aduser; }
		}

		public static string ADPassword
		{
			get { return adpassword; }
		}

        static string imageProviderAssembly;
        internal static string ImageProviderAssembly
        {
            get { return imageProviderAssembly; }
        }

        static string imageProviderClass;
        internal static string ImageProviderClass
        {
            get { return imageProviderClass; }
        }

        static string fileProviderAssembly;
        internal static string FileProviderAssembly
        {
            get { return fileProviderAssembly; }
        }

        static string fileProviderClass;
        internal static string FileProviderClass
        {
            get { return fileProviderClass; }
        }
    }
}
