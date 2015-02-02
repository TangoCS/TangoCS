using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Multilanguage;

namespace Nephrite.TextResources
{
	public static class TextResource
	{
		static IDC_TextResources DataContext;
		static IAppContext AppContext;

		public static void Init(IAppContext appContext, IDC_TextResources dataContext)
		{
			AppContext = appContext;
			DataContext = dataContext;
		}

		static bool EditMode
		{
			get
			{
				if (AppContext.Items["reseditmode"] == null)
					AppContext.Items["reseditmode"] = AppContext.Request.Cookies["resourceeditmode"] != null && AppContext.Request.Cookies["resourceeditmode"] == "1";
				return (bool)AppContext.Items["reseditmode"];
			}
		}

		public static string Get(string sysName)
		{
			return Get(sysName, "");
		}
		
		/// <summary>
		/// Получить текстовый ресурс для текущего языка
		/// </summary>
		/// <param name="sysName">Системное имя текстового ресурса</param>
		/// <returns></returns>
		public static string Get(string sysName, string defaultText)
		{
			string res = sysName + "-" + Language.Current.Code;

			if (!loaded)
			{
				lock (locker)
				{
					var nrs = DataContext.IN_TextResource.Select(o => new { Res = o.SysName + "-" + o.LanguageCode, ID = o.TextResourceID, Text = o.Text });
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

		static string get(string res, string sysName)
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
					return "<span class=\"resedit\" onclick=\"EditTextResource('" + sysName + "', '" + HtmlHelpers.HtmlEncode(resources[res]) + "');\">{" + sysName + "}</span>";
			}
			return resources[res];
		}

		/// <summary>
		/// Сброс кэша
		/// </summary>
		public static void ResetCache()
		{
			resources.Clear();
			resourceids.Clear();
			loaded = false;
		}

		static Dictionary<string, string> resources = new Dictionary<string, string>();
		static Dictionary<string, string> resourceids = new Dictionary<string, string>();
		static bool loaded = false;
		static object locker = new object();
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
