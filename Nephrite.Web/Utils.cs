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

		public static string GetFIO(string surname, string firstname, string patronymic)
		{
			string f = String.IsNullOrEmpty(firstname) ? "" : " " + firstname[0] + ".";
			string p = String.IsNullOrEmpty(patronymic) ? "" : " " + patronymic[0] + ".";
			return surname + f + p;
		}

		public static string GetInitials(string firstname, string patronymic)
		{
			string f = String.IsNullOrEmpty(firstname) ? "" : firstname[0] + ".";
			string p = String.IsNullOrEmpty(patronymic) ? "" : " " + patronymic[0] + ".";
			return f + p;
		}

		static int tinSumm(string s, int[] c)
		{
			int r = 0;
			for (int i = 0; i < c.Length; i++)
				r += c[i] * int.Parse(s[i].ToString());
			return r;
		}

		public static bool ValidateTIN(this string str)
		{
			if (str.Length != 10 && str.Length != 12)
				return false;

			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;

			if (str.Length == 10)
			{
				int[] coeff = { 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int n10 = (tinSumm(str, coeff) % 11) % 10;
				return str[9].ToString() == n10.ToString();
			}
			if (str.Length == 12)
			{
				int[] coeff1 = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int[] coeff2 = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
				int n11 = (tinSumm(str, coeff1) % 11) % 10;
				int n12 = (tinSumm(str, coeff2) % 11) % 10;
				return str[10].ToString() == n11.ToString() && str[11].ToString() == n12.ToString();
			}
			return false;
		}

		public static bool ValidateOGRN(this string str)
		{
			if (str.Length != 13 && str.Length != 15)
				return false;
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			decimal d = decimal.Parse(str.Substring(0, str.Length - 1));
			string checknum = (d % (str.Length - 2)).ToString();
			return str[12].ToString() == checknum.Substring(checknum.Length - 1);
		}

		public static bool ValidateOKPO(this string str)
		{
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			return true;
		}

		public static bool ValidateKPP(this string str)
		{
			if (str.Length != 9)
				return false;
			for (int i = 0; i < str.Length; i++)
				if (!char.IsDigit(str[i]))
					return false;
			return true;
		}

		const string emailregex = "^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(\\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\\.)*(aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$";
		public static bool ValidateEmail(this string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), emailregex, RegexOptions.IgnoreCase).Success;
		}

		public static bool ValidateContainsEmail(this string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), "(" + emailregex.Substring(1, emailregex.Length - 2) + ")+").Success;
		}

		const string ipregex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
		public static bool ValidateIP(this string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), ipregex, RegexOptions.IgnoreCase).Success;
		}

		const string wwwregex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
		public static bool ValidateWWW(this string str)
		{
			if (str.IsEmpty())
				return true;
			return Regex.Match(str.Trim(), wwwregex, RegexOptions.IgnoreCase).Success;
		}

		public static string ReinstallToGac(string domain, string login, string password, string gacutilpath, string assembly)
		{
			string output = "";
			using (new Nephrite.Core.Impersonation(domain, login, password))
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
			using (new Nephrite.Core.Impersonation(domain, login, password))
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