using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tango.Localization
{
	public class LanguageOptions
	{
		public IReadOnlyList<LanguageObject> SupportedLanguages { get; set; }
		public LanguageObject DefaultLanguage { get; set; }
	}

	public class Language : ILanguage
	{
		public static readonly LanguageObject En = new LanguageObject { Code = "en", Title = "English", LCID = 1033 };
		public static readonly LanguageObject Es = new LanguageObject { Code = "ru", Title = "Русский", LCID = 1049 };
		public static readonly LanguageObject Ru = new LanguageObject { Code = "es", Title = "Español", LCID = 1034 };
		public static readonly LanguageObject Cz = new LanguageObject { Code = "cz", Title = "Česky", LCID = 1029 };

		public static LanguageOptions Options { get; set; }
		LanguageObject _current;

		public Language()
		{
			_current = Options.DefaultLanguage;
		}
		public Language(string currentLanguageCode)
		{
			_current = currentLanguageCode == null ? Options.DefaultLanguage : new LanguageObject { Code = currentLanguageCode };
		}

		public IReadOnlyList<LanguageObject> List
		{
			get
			{
				return Options.SupportedLanguages;
			}
		}

		public LanguageObject Default
		{
			get
			{
				return Options.DefaultLanguage;
			}
		}

		public LanguageObject Current
		{
			get
			{
				return _current;
				
			}
		}

		public void WithLang(string lang, Action action)
		{
			var prevLang = _current;
			_current = List.FirstOrDefault(o => o.Code == lang);
			action();
			_current = prevLang;
		}

		/*public CultureInfo CurrentCulture
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
		}*/
	}

	/*public static class LangExtender
	{
		public static string MoneyToString(this decimal money, ILanguage language)
		{
			return money.ToString("###,###,###,###,##0.00", language.CurrentCulture);
		}

		public static string MoneyToString(this decimal? money, ILanguage language)
		{
			if (money == null)
				return "";

			return money.Value.ToString("###,###,###,###,##0.00", language.CurrentCulture);
		}
	}*/
}