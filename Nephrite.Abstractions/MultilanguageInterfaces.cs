using System;
using System.Collections.Generic;
using System.Globalization;

namespace Nephrite.Multilanguage
{
	public interface ITextResource
	{
		string Get(string sysName);
		string Get(string sysName, string defaultText);
		void Init(bool editMode);
		//void ResetCache();
	}

	public interface ILanguage
	{
		IReadOnlyList<LanguageObject> List { get; }
		LanguageObject Current { get; }
		LanguageObject Default { get; }
		CultureInfo CurrentCulture { get; }

		void Init(LanguageObject currentLanguage);
		void WithLang(string lang, Action action);
	}

	public class LanguageObject
	{
		public string Code { get; set; }
		public string Title { get; set; }
	}
}
