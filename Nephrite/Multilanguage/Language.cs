using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nephrite.Http;

namespace Nephrite.Multilanguage
{
	public static class Language
	{
		static Func<IDC_Multilanguage> DataContext;
		static Func<IHttpContext> HttpContext;

		static List<IC_Language> _langs;

		public static void Init(Func<IHttpContext> httpContext, Func<IDC_Multilanguage> dataContext)
		{
			HttpContext = httpContext;
			DataContext = dataContext;
		}

		public static List<IC_Language> List
		{
			get
			{
				if (_langs == null)
					_langs = DataContext().IC_Language.OrderByDescending(o => o.IsDefault).ToList();
				return _langs;
			}
		}

		public static IC_Language Current
		{
			get
			{
				string lang = HttpContext().Request.Query["lang"];

				if (HttpContext().Request.Cookies["lcid"] != null)
					lang = HttpContext().Request.Cookies["lcid"] == "1033" ? "en" : "ru";
				if (HttpContext().Items["Lang"] != null)
					lang = (string)HttpContext().Items["Lang"];

				lang = lang ?? "ru";
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
			string prevLang = (string)HttpContext().Items["Lang"];
			HttpContext().Items["Lang"] = lang;
			action();
			HttpContext().Items["Lang"] = prevLang;
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