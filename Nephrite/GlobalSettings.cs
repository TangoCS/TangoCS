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
		static Lazy<string> _jsPath = new Lazy<string>(() => {
			var s = ConfigurationManager.AppSettings["JSPath"] ?? "/js";
			if (!s.EndsWith("/")) s += "/";
			return s;
		});
		public static string JSPath { get { return _jsPath.Value; } }

		static Lazy<string> _controlsPath = new Lazy<string>(() => ConfigurationManager.AppSettings["ControlsPath"] ?? "Views");
		public static string ControlsPath { get { return _controlsPath.Value; } }
	}
}
