using System.Collections.Generic;

namespace Nephrite.Localization
{
	public class TextResourceOptions
	{
		public IReadOnlyDictionary<string, string> Resources { get; set; }
	}

	public class TextResource : ITextResource
	{
		ILanguage _language;
		public static TextResourceOptions Options { get; set; }

		public TextResource(ILanguage language, bool editMode)
		{
			_language = language;
			_editMode = editMode;
		}

		public string Get(string sysName)
		{
			return Get(sysName, "");
		}
		
		/// <summary>
		/// Получить текстовый ресурс для текущего языка
		/// </summary>
		/// <param name="sysName">Системное имя текстового ресурса</param>
		/// <returns></returns>
		public string Get(string sysName, string defaultText)
		{
			string res = sysName + "-" + _language.Current.Code;

			if (_editMode || Options.Resources.ContainsKey(res))
				return get(res, sysName);
			else
				return defaultText;
		}


		bool _editMode = false;
		//{
		//	get
		//	{
		//		if (_httpContext.Items["reseditmode"] == null)
		//			_httpContext.Items["reseditmode"] = _httpContext.Request.Cookies["resourceeditmode"] != null && _httpContext.Request.Cookies["resourceeditmode"] == "1";
		//		return (bool)_httpContext.Items["reseditmode"];
		//	}
		//}

		string get(string res, string sysName)
		{
			if (_editMode)
			{
				if (Options.Resources.ContainsKey(res))
				{
					if (!Options.Resources[res].IsEmpty())
						return "<span class='resedit' onclick='EditTextResource(" + sysName + ");'>" + Options.Resources[res] + "</span>";
					else
						return "<span class='resedit' onclick='EditTextResource(" + sysName + ");'>[" + sysName + "]</span>";
				}
				else
					return "<span class=\"resedit\" onclick=\"EditTextResource('" + sysName + "', '');\">{" + sysName + "}</span>";
			}
			return Options.Resources[res];
		}
	}
}
