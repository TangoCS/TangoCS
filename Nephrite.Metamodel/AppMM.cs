using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Metamodel.Model;
using System.IO;
using System.Web.UI.WebControls;
using Nephrite.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;

namespace Nephrite.Metamodel
{
	public class AppMM
	{
		[ThreadStatic]
		static modelDataContext context = null;

		public static modelDataContext DataContext
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current != null)
					{
						if (HttpContext.Current.Items["MMmodelDataContext"] == null)
						{
							modelDataContext dc = new modelDataContext(ConnectionManager.Connection);
							dc.CommandTimeout = 300;
							HttpContext.Current.Items["MMmodelDataContext"] = dc;
							dc.Log = new DataContextLogWriter();
						}
						return (modelDataContext)HttpContext.Current.Items["MMmodelDataContext"];
					}
					else
						return new modelDataContext(ConnectionManager.Connection);
				}
				else
				{
					if (context == null)
					{
						context = new modelDataContext(ConnectionManager.Connection);
						context.Log = new DataContextLogWriter();
						context.CommandTimeout = 300;
					}
					return context;
				}
			}
		}

		internal static HiddenField TotalFilesSize
		{
			get
			{
				return (HiddenField)HttpContext.Current.Items["TotalFilesSize"];

			}
		}

		public static string DBName()
		{
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ModelNamespace"]))
                return ConfigurationManager.AppSettings["ModelNamespace"];
            if (AppDomain.CurrentDomain.GetData("DbName") != null)
                return (string)AppDomain.CurrentDomain.GetData("DbName");
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConnectionManager.Connection.ConnectionString);
			return csb.InitialCatalog;
		}

        static List<C_Language> _langs;
        public static List<C_Language> Languages
        {
            get
            {
                if (_langs == null)
                    _langs = DataContext.C_Languages.ToList();
                return _langs;
            }
        }

        static string defaultLanguage = null;
        public static string DefaultLanguage
        {
            get
            {
                if (defaultLanguage == null)
                    defaultLanguage = Languages.Single(o => o.IsDefault).LanguageCode;
                return defaultLanguage;
            }
        }

        public static C_Language CurrentLanguage
        {
            get
            {
                string lang = Query.GetString("lang");
				if (HttpContext.Current.Request.Cookies["lcid"] != null)
					lang = HttpContext.Current.Request.Cookies["lcid"].Value == "1033" ? "en" : "ru";
				if (HttpContext.Current.Items["Lang"] != null)
					lang = (string)HttpContext.Current.Items["Lang"];
                var l = Languages.SingleOrDefault(o => o.LanguageCode == lang);
                if (l == null)
                    l = Languages.Single(o => o.IsDefault);
                return l;
            }
        }

		public static void WithLang(string lang, Action action)
		{
			string prevLang = (string)HttpContext.Current.Items["Lang"];
			HttpContext.Current.Items["Lang"] = lang;
			action();
			HttpContext.Current.Items["Lang"] = prevLang;
		}

        public static CultureInfo CurrentCulture
        {
            get
            {
                switch (CurrentLanguage.LanguageCode.ToLower())
                {
                    case "en":
                        return CultureInfo.GetCultureInfo("en-US");
                    case "ru":
                        return CultureInfo.GetCultureInfo("ru-RU");
                    default:
                        return CultureInfo.InvariantCulture;
                }
            }
        }
	}
}
