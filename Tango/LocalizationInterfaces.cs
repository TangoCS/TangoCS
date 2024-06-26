﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tango.Localization
{
	public interface IResourceManager
	{
		string Get(string key);
		bool TryGet(string key, out string result);

		void SetNotFound(string key);
		IEnumerable<string> GetNotFound();
	}

	public interface ILanguage
	{
		IReadOnlyList<LanguageObject> List { get; }
		LanguageObject Current { get; }
		LanguageObject Default { get; }
		//CultureInfo CurrentCulture { get; }

		void WithLang(string lang, Action action);
	}

	public class LanguageObject
	{
		public int LCID { get; set; }
		public string Code { get; set; }
		public string Title { get; set; }
	}
}
