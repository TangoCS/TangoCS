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
using Nephrite.Web.Hibernate;
using Nephrite.Web.CoreDataContext;

namespace Nephrite.Metamodel
{
	public class AppMM
	{
		[ThreadStatic]
		static MMDataContext context = null;

		public static MMDataContext DataContext
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current != null)
					{
						if (HttpContext.Current.Items["MMmodelDataContext"] == null)
						{
							MMDataContext dc = new MMDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString));
							//dc.CommandTimeout = 300;
							HttpContext.Current.Items["MMmodelDataContext"] = dc;
						}
						return (MMDataContext)HttpContext.Current.Items["MMmodelDataContext"];
					}
					else
						return new MMDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString));
				}
				else
				{
					if (context == null)
					{
						context = new MMDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString));
						//context.CommandTimeout = 300;
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
	}
}
