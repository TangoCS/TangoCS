using System;
using System.Collections.Generic;
using System.Globalization;

namespace Nephrite.Multilanguage
{
	public interface ITextResource
	{
		string Get(string sysName);
		string Get(string sysName, string defaultText);
		void ResetCache();
	}

	public interface ILanguage
	{
		List<ILanguageObject> List { get; }
		ILanguageObject Current { get; }
		ILanguageObject DefaultLanguage { get; }
		CultureInfo CurrentCulture { get; }

		void WithLang(string lang, Action action);
	}

	public interface ILanguageObject
	{
		string Code { get; set; }
		string Title { get; set; }
		bool IsDefault { get; set; }
	}
}
