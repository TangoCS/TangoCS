using System.Collections.Generic;

namespace Tango.Localization
{
	public class TextResourceOptions
	{
		public IReadOnlyDictionary<string, string> Resources { get; set; }
		public IReadOnlyDictionary<string, string> Images { get; set; }
	}

	public class TextResource : ITextResource
	{
		ILanguage _language;
		public static TextResourceOptions Options { get; set; }

		public TextResource(ILanguage language)
		{
			_language = language;
		}

		public string Get(string key)
		{
			return Get(key, "");
		}
		
		/// <summary>
		/// Получить текстовый ресурс для текущего языка
		/// </summary>
		/// <param name="sysName">Системное имя текстового ресурса</param>
		/// <returns></returns>
		public string Get(string key, string defaultText)
		{
			string res = key + "-" + _language.Current.Code;

			var text = "";
			if (Options.Resources.TryGetValue(res, out text))
				return text;
			else
				return defaultText;
		}

		public string GetImageName(string key)
		{
			var text = "";
			if (Options.Images.TryGetValue(key, out text))
				return text;
			else
				return "";
		}
	}
}
