using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Nephrite.Web
{
	public static class Utils
	{
		/// <summary>
		/// Копирование свойств объекта
		/// </summary>
		/// <param name="src">Исходный объект</param>
		/// <param name="dest">Целевой объект</param>
		public static void CopyProperties(this object src, object dest, params string[] skipCopyProperty)
		{
			if (src == null || dest == null)
				return;

			foreach (var prop in src.GetType().GetProperties())
			{
				if (skipCopyProperty != null && skipCopyProperty.Contains(prop.Name))
					continue;
				if (prop.PropertyType.IsSubclassOf(typeof(ValueType)) || prop.PropertyType == typeof(String))
				{
					PropertyInfo pi = dest.GetType().GetProperty(prop.Name, prop.PropertyType);
					if (pi != null && pi.CanWrite)
						pi.SetValue(dest, prop.GetValue(src, null), null);
				}
			}
		}

		public static byte[] StretchImage(byte[] image, int nw, int nh)
		{
			MemoryStream msOut = new MemoryStream();
			MemoryStream msImage = new MemoryStream(image);
			Bitmap b = new Bitmap(msImage);

			int curwidth = b.Width;
			int curheight = b.Height;
			int direction = curwidth > curheight ? 1 : -1;

			if (direction > 0)
			{
				while (curwidth > nw)
				{
					curwidth -= 1;
					curheight = curwidth * b.Height / b.Width;
				}
			}
			else
			{
				while (curheight > nh)
				{
					curheight -= 1;
					curwidth = curheight * b.Width / b.Height;
				}
			}

			b = new Bitmap(b, new Size(curwidth, curheight));
			b.Save(msOut, ImageFormat.Png);

			return msOut.GetBuffer();
		}

		static IEnumerable<Assembly> _assemblies = null;
		public static IEnumerable<Assembly> GetSolutionAssemblies()
		{
			if (_assemblies == null)
			{
				IEnumerable<string> s = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin", "Nephrite*.dll").Select(o => Path.GetFileName(o).ToLower());
				IEnumerable<Assembly> res = AppDomain.CurrentDomain.GetAssemblies().Where(o => s.Contains(o.ManifestModule.ScopeName.ToLower()) || o.ManifestModule.ScopeName.ToLower() == ConfigurationManager.AppSettings["ModelAssembly"].ToLower() + ".dll");
				_assemblies = res;
			}
			return _assemblies;
		}

		



		public static string ReinstallToGac(string domain, string login, string password, string gacutilpath, string assembly)
		{
			string output = "";
			using (new Impersonation(domain, login, password))
			{
				string args;
				Process gu;
				args = String.Format("/nologo /u \"{0}\"", assembly);
				output += "gacutil " + args + Environment.NewLine;
				gu = new Process();
				gu.StartInfo = new ProcessStartInfo(gacutilpath, args);
				gu.StartInfo.UseShellExecute = false;
				gu.StartInfo.RedirectStandardOutput = true;
				gu.Start();
				gu.WaitForExit();
				output += gu.StandardOutput.ReadToEnd();

				args = String.Format("/nologo /i \"{0}\"", AppDomain.CurrentDomain.BaseDirectory + "bin\\" + assembly + ".dll");
				output += "gacutil " + args + "<br />";
				gu = new Process();
				gu.StartInfo = new ProcessStartInfo(gacutilpath, args);
				gu.StartInfo.UseShellExecute = false;
				gu.StartInfo.RedirectStandardOutput = true;
				gu.Start();
				gu.WaitForExit();
				output += gu.StandardOutput.ReadToEnd();
			}
			return output;
		}
		public static void IisReset(string domain, string login, string password)
		{
			using (new Impersonation(domain, login, password))
			{
				string args = String.Format("/noforce");
				var p = new Process();
				p.StartInfo = new ProcessStartInfo("iisreset", args);
				p.StartInfo.UseShellExecute = true;
				p.Start();
			}
		}
	}
}