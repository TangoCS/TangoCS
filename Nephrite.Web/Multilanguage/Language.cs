using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web.Multilanguage
{
	public static class Language
	{
		static List<IC_Language> _langs;
		public static List<IC_Language> List
		{
			get
			{
				if (_langs == null)
					_langs = ((IDC_Multilanguage)A.Model).C_Language.OrderByDescending(o => o.IsDefault).ToList();
				return _langs;
			}
		}

		public static IC_Language Current
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

		public static IC_Language DefaultLanguage
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

	public static class LangExtender
	{
		public static string Lang(this System.Web.UI.Control ctrl, string lang, string data)
		{
			if (Language.Current.Code.ToUpper() == lang.ToUpper())
				return data;
			return String.Empty;
		}

		public static string Lang(this System.Web.UI.Control ctrl, string lang, string data, params string[] otherLang)
		{
			string lc = Language.Current.Code.ToUpper();
			if (lc == lang.ToUpper())
				return data;
			for (int i = 0; i < otherLang.Length; i += 2)
				if (otherLang[i].ToUpper() == lc)
					return otherLang[i + 1];
			return String.Empty;
		}

		public static string MoneyToString(this decimal money)
		{
			return money.ToString("###,###,###,###,##0.00", Language.CurrentCulture);
		}

		public static string MoneyToString(this decimal? money)
		{
			if (money == null)
				return "";
			return money.Value.ToString("###,###,###,###,##0.00", Language.CurrentCulture);
		}

	}
}