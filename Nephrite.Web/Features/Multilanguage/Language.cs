using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Nephrite.Web.Model;

namespace Nephrite.Web.Multilanguage
{
	public static class Language
	{
		static List<C_Language> _langs;
		public static List<C_Language> List
		{
			get
			{
				if (_langs == null)
					_langs = AppWeb.DataContext.C_Languages.OrderByDescending(o => o.IsDefault).ToList();
				return _langs;
			}
		}

		public static C_Language Current
		{
			get
			{
				string lang = Query.GetString("lang");
				if (HttpContext.Current.Request.Cookies["lcid"] != null)
					lang = HttpContext.Current.Request.Cookies["lcid"].Value == "1033" ? "en" : "ru";
				if (HttpContext.Current.Items["Lang"] != null)
					lang = (string)HttpContext.Current.Items["Lang"];
				var l = List.SingleOrDefault(o => o.Code == lang);
				if (l == null)
					l = List.Single(o => o.IsDefault);
				return l;
			}
		}

		public static C_Language DefaultLanguage
		{
			get
			{
				return List.Single(o => o.IsDefault);
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
				switch (Current.Code.ToLower())
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