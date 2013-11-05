using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using Nephrite.Metamodel.Model;
using System.Web.UI;
using Nephrite.Web.SPM;
using System.Data.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Nephrite.Meta;
using Nephrite.Web.SettingsManager;
using System.IO;
using System.Web.Hosting;

namespace Nephrite.Metamodel.Controllers
{
	public abstract class MMObjectController : BaseController
	{
		string _className = "";

		public MMObjectController(string className)
		{
			_className = className;
		}

		public static void RenderMMView(Control container, string objectTypeSysName, string viewname, object viewData)
		{
			string path = Settings.ControlsPath + "/" + objectTypeSysName + "/" + viewname + ".ascx";
			HttpContext.Current.Items["FormView"] = objectTypeSysName + "." + viewname;
			HttpContext.Current.Items["ObjectType"] = objectTypeSysName;
			HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + objectTypeSysName + "." + viewname;

			Control ctl = null;
			try
			{
				ctl = container.Page.LoadControl(path);
				if (viewData != null)
					((ViewControl)ctl).SetViewData(viewData);
				var t = ctl.GetType();
				((ViewControl)ctl).RenderMargin = t.BaseType.BaseType.GetGenericArguments().Length == 1 && t.BaseType.BaseType.GetGenericArguments()[0].GetInterfaces().Contains(typeof(IMMObject));
				container.Controls.Add(ctl);
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
				string text1 = "";
				string text2 = "";
				if (ctl != null)
					text1 = "<div>Класс представления: " + ctl.GetType().FullName + ", " + objectTypeSysName + ", FormView=" + viewname + "</div>";

				while (e != null)
				{
					text2 += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
					e = e.InnerException;
				}
				LiteralControl lc = new LiteralControl(text1 + "<pre>" + text2 + "</pre>");
				container.Controls.Add(lc);
				if (line > 0)
				{
					text2 = "";

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
					container.Controls.Add(lc2);
				}

			}
		}

		public void RenderMMView(string viewname, string className, object viewData)
		{
			MMObjectController.RenderMMView(WebPart, className, viewname, viewData);
		}
		public void RenderMMView(string viewname)
		{
			MMObjectController.RenderMMView(WebPart, _className, viewname, null);
		}
		public void RenderMMView(string viewname, object viewData)
		{
			MMObjectController.RenderMMView(WebPart, _className, viewname, viewData);
		}

		public virtual void Update(IModelObject obj)
		{
			obj.SetPropertyValue("LastModifiedDate", DateTime.Now);
			obj.SetPropertyValue("LastModifiedUserID", AppSPM.GetCurrentSubjectID());
			A.Model.SubmitChanges();
		}

		new public static Result Run(Control control, string mode, string action, bool disableScriptManager, bool skipCreateMdm)
		{
			Result res = BaseController.Run(control, mode, action, disableScriptManager, skipCreateMdm);
			if (res.Code == -1)
			{
				MMObjectController.RenderMMView(control, mode, action, null);
				return new Result(0, "");
			}
			return res;
		}
	}
}