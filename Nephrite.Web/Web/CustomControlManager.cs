using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Nephrite.Web
{
	public static class CustomControlManager
	{
		public const string Path = "/_controltemplates/Tessera/NephriteCustomControls";

		static Dictionary<string, string> dict = new Dictionary<string,string>();
		static Dictionary<string, DateTime> dictcache = new Dictionary<string, DateTime>();
		static Dictionary<string, Type> viewDatas = new Dictionary<string, Type>();
		public static string GetText(string key)
		{
			lock (dict)
			{
				if (!dict.ContainsKey(key))
					throw new Exception("Нет текста для ключа " + key + ", не был вызван CustomControlManager.Register");
				return dict[key];
			}
		}

		public static string GetCacheKey(string key)
		{
			lock (dictcache)
			{
				if (!dictcache.ContainsKey(key))
					throw new Exception("Нет текста для ключа " + key + ", не был вызван CustomControlManager.Register");
				return key + "_" + dictcache[key].Ticks.ToString();
			}
		}

		public static Type GetViewDataType(string key)
		{
			lock (viewDatas)
			{
				return viewDatas[key];
			}
		}

		public static void Register(string key, DateTime date, string text, Type viewDataType)
		{
			lock (dict)
			{
				if (dict.ContainsKey(key) && dictcache[key] >= date)
					return;
				dict[key] = text;
				dictcache[key] = date;
				viewDatas[key] = viewDataType;
			}
		}

		public static string Render(string key, object viewData)
		{
			try
			{
				Page pg = new Page();
				Control ctrl = pg.LoadControl("~" + Path + "/" + key + ".ascx");
				((CustomControlBase)ctrl).SetViewData(viewData);
				return ctrl.RenderControl();
			}
			catch (Exception e)
			{
				int line = 0;
				int col = 0;
				string srcPath = "";
				if (e is HttpCompileException)
				{
					var hce = (HttpCompileException)e;
					if (hce.Results.Errors.HasErrors)
					{
						line = hce.Results.Errors[0].Line;
						col = hce.Results.Errors[0].Column;
						srcPath = hce.SourceCode;
					}
				}
				else
				{
					var st = new StackTrace(e, true);
					var frame = st.GetFrame(0);
					line = frame.GetFileLineNumber();
				}

				string text = e.GetType().FullName + "\r\n";

				while (e != null)
				{
					text += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + HttpUtility.HtmlEncode(e.StackTrace) + "\r\n\r\n";
					e = e.InnerException;
				}
				text = "<pre>" + text + "</pre>";

				string text2 = "";

				string[] lines = GetText(key).Split(new string[] { "\n" }, StringSplitOptions.None);
				for (int i = 0; i < lines.Length; i++)
				{
					text2 += "<b>" + (i + 1).ToString() + "</b>\t";
					if (i + 1 == line)
						text2 += "<span style=\"color:Red; font-size:13px; font-weight:bold\">" + HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</span>";
					else
						text2 += HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
					text2 += "<br />";
				}
				text += "<br /><br />" + text2 + "<br /><br />";

				return "FATAL ERROR<br />" +text;
			}
		}
	}
}
