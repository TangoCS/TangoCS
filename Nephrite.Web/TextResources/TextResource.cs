using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using Nephrite.Web.Multilanguage;

namespace Nephrite.Web.TextResources
{
	public static class TextResource
	{
		static bool EditMode
		{
			get
			{
				if (HttpContext.Current.Items["reseditmode"] == null)
					HttpContext.Current.Items["reseditmode"] = HttpContext.Current.Request.Cookies["resourceeditmode"] != null && HttpContext.Current.Request.Cookies["resourceeditmode"].Value == "1";
				return (bool)HttpContext.Current.Items["reseditmode"];
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
					var nrs = ((IDC_TextResources)A.Model).V_N_TextResource.Select(o => new { Res = o.SysName + "-" + o.LanguageCode, ID = o.TextResourceID, Text = o.Text });
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
					return "<span class=\"resedit\" onclick=\"EditTextResource('" + sysName + "', '" + HttpUtility.HtmlEncode(resources[res]) + "');\">{" + sysName + "}</span>";
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

	public interface IV_N_TextResource
	{
		int TextResourceID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		string Text { get; set; }
		string LanguageCode { get; set; }
	}

	public interface IDC_TextResources
	{
		IQueryable<IV_N_TextResource> V_N_TextResource { get; }
	}
}
