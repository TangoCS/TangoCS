using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nephrite.Data;
using Nephrite.Http;

namespace Nephrite.Multilanguage
{
	public class TextResource : ITextResource
	{
		IDC_TextResources _dataContext;
		IHttpContext _httpContext;
		ILanguage _language;

		public TextResource(IHttpContext httpContext, IDC_TextResources dataContext, ILanguage language)
		{
			_httpContext = httpContext;
			_dataContext = dataContext;
			_language = language;
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

			if (!loaded)
			{
				lock (locker)
				{
					var nrs = _dataContext.IN_TextResource.Select(o => new { Res = o.SysName + "-" + o.LanguageCode, ID = o.TextResourceID, Text = o.Text });
					foreach (var nr in nrs)
					{
						resources[nr.Res] = nr.Text;
						resourceids[nr.Res] = nr.ID.ToString();
					}
				}
				loaded = true;
			}

			if (resources.ContainsKey(res))
				return get(res, sysName);
			else
				return defaultText;
		}


		bool EditMode
		{
			get
			{
				if (_httpContext.Items["reseditmode"] == null)
					_httpContext.Items["reseditmode"] = _httpContext.Request.Cookies["resourceeditmode"] != null && _httpContext.Request.Cookies["resourceeditmode"] == "1";
				return (bool)_httpContext.Items["reseditmode"];
			}
		}

		string get(string res, string sysName)
		{
			if (EditMode)
			{
				if (resources.ContainsKey(res))
				{
					if (!resources[res].IsEmpty())
						return "<span class='resedit' onclick='EditTextResource(" + resourceids[res] + ");'>" + resources[res] + "</span>";
					else
						return "<span class='resedit' onclick='EditTextResource(" + resourceids[res] + ");'>[" + sysName + "]</span>";
				}
				else
					return "<span class=\"resedit\" onclick=\"EditTextResource('" + sysName + "', '" + WebUtility.HtmlEncode(resources[res]) + "');\">{" + sysName + "}</span>";
			}
			return resources[res];
		}

		/// <summary>
		/// Сброс кэша
		/// </summary>
		public void ResetCache()
		{
			resources.Clear();
			resourceids.Clear();
			loaded = false;
		}

		Dictionary<string, string> resources = new Dictionary<string, string>();
		Dictionary<string, string> resourceids = new Dictionary<string, string>();
		bool loaded = false;
		object locker = new object();
	}

	public interface IN_TextResource
	{
		int TextResourceID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		string Text { get; set; }
		string LanguageCode { get; set; }
	}

	public interface IDC_TextResources
	{
		ITable<IN_TextResource> IN_TextResource { get; }
	}
}
