using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tango.Localization
{
	public interface ITextResource
	{
		string Get(string key);
		bool TryGet(string key, out string result);

		void SetNotFound(string key);
		IEnumerable<string> GetNotFound();

		string GetImageName(string key);
	}

	public interface ILanguage
	{
		IReadOnlyList<LanguageObject> List { get; }
		LanguageObject Current { get; }
		LanguageObject Default { get; }
		CultureInfo CurrentCulture { get; }

		void WithLang(string lang, Action action);
	}

	public class LanguageObject
	{
		public string Code { get; set; }
		public string Title { get; set; }
	}
}
