using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite
{
	public static class GlobalSettings
	{
		static Lazy<string> _jsPath = new Lazy<string>(() => ConfigurationManager.AppSettings["JSPath"] ?? "/js/");
		public static string JSPath { get { return _jsPath.Value; } }
	}
}
