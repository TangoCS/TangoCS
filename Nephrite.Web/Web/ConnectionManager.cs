using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Threading;

namespace Nephrite.Web
{
    public static class ConnectionManager
    {
		public static string ConnectionString
		{
			get
			{
				if (connstr != null)
					return connstr;
				if (HttpContext.Current != null)
					return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
				else if (AppDomain.CurrentDomain.GetData("ConnectionString") != null)
					return (string)AppDomain.CurrentDomain.GetData("ConnectionString");
				else
				{
					var a = Assembly.GetCallingAssembly();

					string webConfigPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(a.Location)), "connectionStrings.config");
					if (!File.Exists(webConfigPath))
						webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connectionStrings.config");
					if (!File.Exists(webConfigPath))
						webConfigPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(a.Location)), "web.config");
					if (!File.Exists(webConfigPath))
						webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config");
					if (!File.Exists(webConfigPath))
						throw new Exception("Файл " + webConfigPath + " не найден");
					XmlDocument doc = new XmlDocument();
					doc.Load(webConfigPath);
					var cs = doc.SelectSingleNode("connectionStrings/add[@name='ConnectionString']");
					if (cs == null)
						throw new Exception("Строка подключения с именем ConnectionString не найдена в файле " + webConfigPath);

					return ((XmlElement)cs).GetAttribute("connectionString");
				}
			}
		}

		[ThreadStatic]
		static string connstr = null;
		public static void SetConnectionString(string connectionString)
		{
			connstr = connectionString;
		}
	}
}
