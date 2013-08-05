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
		//[ThreadStatic]
		//static modelDataContext context = null;

		/*public static modelDataContext DataContext
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
		}*/

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
