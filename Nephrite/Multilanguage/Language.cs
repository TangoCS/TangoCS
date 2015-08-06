using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nephrite.Http;
using Microsoft.Framework.DependencyInjection;
using Nephrite.Data;

namespace Nephrite.Multilanguage
{
	public interface IDC_Multilanguage
	{
		ITable<ILanguageObject> IC_Language { get; }
	}

	public class Language : ILanguage
	{
		IDC_Multilanguage _dataContext;
		IHttpContext _httpContext;

		static List<ILanguageObject> _langs;

		public Language(IHttpContext httpContext, IDC_Multilanguage dataContext)
		{
			_httpContext = httpContext;
			_dataContext = dataContext;
		}

		public List<ILanguageObject> List
		{
			get
			{
				if (_langs == null)
					_langs = _dataContext.IC_Language.OrderByDescending(o => o.IsDefault).ToList();
				return _langs;
			}
		}

		public ILanguageObject Current
		{
			get
			{
				string lang = _httpContext.Request.Query["lang"];

				if (_httpContext.Request.Cookies["lcid"] != null)
					lang = _httpContext.Request.Cookies["lcid"] == "1033" ? "en" : "ru";
				if (_httpContext.Items["Lang"] != null)
					lang = (string)_httpContext.Items["Lang"];

				lang = lang ?? "ru";
			    var l = List.SingleOrDefault(o => o.Code == lang);
				if (l == null)
					l = List.Single(o => o.IsDefault);
				return l;
			}
		}

		public ILanguageObject DefaultLanguage
		{
			get
			{
				return List.Single(o => o.IsDefault);
			}
		}
		public void WithLang(string lang, Action action)
		{
			string prevLang = (string)_httpContext.Items["Lang"];
			_httpContext.Items["Lang"] = lang;
			action();
			_httpContext.Items["Lang"] = prevLang;
		}

		public CultureInfo CurrentCulture
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
			var language = DI.RequestServices.GetService<ILanguage>();
			return money.ToString("###,###,###,###,##0.00", language.CurrentCulture);
		}

		public static string MoneyToString(this decimal? money)
		{
			if (money == null)
				return "";

			var language = DI.RequestServices.GetService<ILanguage>();
			return money.Value.ToString("###,###,###,###,##0.00", language.CurrentCulture);
		}
	}
}