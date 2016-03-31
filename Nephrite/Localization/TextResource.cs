using System.Collections.Generic;

namespace Nephrite.Localization
{
	public class TextResourceOptions
	{
		public IReadOnlyDictionary<string, TextResourceObject> Resources { get; set; }
	}

	public class TextResourceObject
	{
		public int ID { get; set; }
		/// <summary>
		/// Format: ResourceName-LanguageCode
		/// </summary>
		public string Name { get; set; }
		public string Text { get; set; }
	}

	public class TextResource : ITextResource
	{
		ILanguage _language;
		public static TextResourceOptions Options { get; set; }

		public TextResource(ILanguage language)
		{
			_language = language;
		}

		public void Init(bool editMode)
		{
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

			//if (!loaded)
			//{
			//	lock (locker)
			//	{
			//		var nrs = _dataContext.IN_TextResource.Select(o => new { Name = o.SysName + "-" + o.LanguageCode, ID = o.TextResourceID, Text = o.Text });
			//		foreach (var nr in nrs)
			//		{
			//			resources[nr.Name] = nr.Text;
			//			resourceids[nr.Name] = nr.ID.ToString();
			//		}
			//	}
			//	loaded = true;
			//}

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
					if (!Options.Resources[res].Text.IsEmpty())
						return "<span class='resedit' onclick='EditTextResource(" + Options.Resources[res].ID + ");'>" + Options.Resources[res].Text + "</span>";
					else
						return "<span class='resedit' onclick='EditTextResource(" + Options.Resources[res].ID + ");'>[" + sysName + "]</span>";
				}
				else
					return "<span class=\"resedit\" onclick=\"EditTextResource('" + sysName + "', '');\">{" + sysName + "}</span>";
			}
			return Options.Resources[res].Text;
		}

		/// <summary>
		/// Сброс кэша
		/// </summary>
		//public void ResetCache()
		//{
		//	resources.Clear();
		//	resourceids.Clear();
		//	loaded = false;
		//}

		//static Dictionary<string, string> resources = new Dictionary<string, string>();
		//static Dictionary<string, string> resourceids = new Dictionary<string, string>();
		//static bool loaded = false;
		//static object locker = new object();
	}

	//public interface IN_TextResource
	//{
	//	int TextResourceID { get; set; }
	//	string Title { get; set; }
	//	string SysName { get; set; }
	//	string Text { get; set; }
	//	string LanguageCode { get; set; }
	//}

	//public interface IDC_TextResources
	//{
	//	ITable<IN_TextResource> IN_TextResource { get; }
	//}
}
