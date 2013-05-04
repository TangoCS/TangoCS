using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.Model;

namespace Nephrite.Web
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
			string res = sysName + "-" + CurrentLanguageCode;

			if (!loaded)
			{
				lock (locker)
				{
					var nrs = AppWeb.DataContext.N_TextResourceDatas.Select(o => new { Res = o.N_TextResource.SysName + "-" + o.LanguageCode, ID = o.TextResourceID, Text = o.Text });
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

		#region Languages
		static object locker = new object();
		static List<C_Language> _langs;
		static List<C_Language> Languages
		{
			get
			{
				if (_langs == null)
				{
					lock (locker)
					{
						if (_langs == null)
							_langs = AppWeb.DataContext.C_Languages.ToList();
					}
				}
				return _langs;
			}
		}

		static string defaultLanguageCode = null;
		static string DefaultLanguageCode
		{
			get
			{
				if (defaultLanguageCode == null)
					defaultLanguageCode = Languages.Single(o => o.IsDefault).LanguageCode;
				return defaultLanguageCode;
			}
		}

		static string CurrentLanguageCode
		{
			get
			{
				string lang = Query.GetString("lang");
				if (HttpContext.Current.Request.Cookies["lcid"] != null)
					lang = HttpContext.Current.Request.Cookies["lcid"].Value == "1033" ? "en" : "ru";
				if (HttpContext.Current.Items["Lang"] != null)
					lang = (string)HttpContext.Current.Items["Lang"];
				var l = Languages.SingleOrDefault(o => o.LanguageCode == lang);
				if (l == null)
					l = Languages.Single(o => o.IsDefault);
				return l.LanguageCode;
			}
		}
		#endregion
	}
}
