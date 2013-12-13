using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Web.SettingsManager;
using System.IO;
using System.Web.Hosting;

namespace Nephrite.Web.Controllers
{
	public class PackageFormViewController : BaseController
	{
		string packageSysName;
		public PackageFormViewController(string sysName)
		{
			packageSysName = sysName;
		}

		public void RenderMMView(string viewname)
		{
			string path = Settings.ControlsPath + "/" + packageSysName + "Pck/" + viewname + ".ascx";
			HttpContext.Current.Items["FormView"] = packageSysName + "Pck." + viewname;
			HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + packageSysName + "." + viewname;
			Control ctl = null;
			try
			{
				ctl = WebPart.Page.LoadControl(path);
				WebPart.Controls.Add(ctl);
			}
			catch (Exception e)
			{
				int line = 0;
				int col = 0;
				if (e is HttpCompileException)
				{
					var hce = (HttpCompileException)e;
					if (hce.Results.Errors.HasErrors)
					{
						line = hce.Results.Errors[0].Line;
						col = hce.Results.Errors[0].Column;
					}
				}
				string text = "";
				if (ctl != null)
					text = "Пакет представления: " + ctl.GetType().FullName + ", " + packageSysName + "<br />";

				while (e != null)
				{
					text += "<b>" + e.Message + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
					e = e.InnerException;
				}
				LiteralControl lc = new LiteralControl("<pre>" + text + "</pre>");
				WebPart.Controls.Add(lc);
				if (line > 0)
				{
					string text2 = "";

					string[] lines = (new StreamReader(VirtualPathProvider.OpenFile(path))).ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
					for (int i = 0; i < lines.Length; i++)
					{
						if (i + 1 == line)
							text2 += "<span style=\"color:Red; font-size:13px; font-weight:bold\">" + HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</span";
						else
							text2 += HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
						text2 += "<br />";
					}
					LiteralControl lc2 = new LiteralControl("<br /><br />" + text2);
					WebPart.Controls.Add(lc2);
				}
			}
		}
	}
}