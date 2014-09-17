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
using System.IO;
using System.Web.Hosting;

namespace Nephrite.Metamodel.Controllers
{
    public abstract class MMObjectController : BaseController
    {
        protected string ObjectTypeSysName;

		public MMObjectController(string objectTypeSysName)
	    {
			ObjectTypeSysName = objectTypeSysName;
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
                string text = "";
                if (ctl != null)
                    text = "Класс представления: " + ctl.GetType().FullName + ", " + objectTypeSysName + ", FormView=" + viewname + "<br />";

                while (e != null)
                {
                    text += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
                    e = e.InnerException;
                }
                LiteralControl lc = new LiteralControl("<pre>" + text + "</pre>");
				container.Controls.Add(lc);
                if (line > 0)
                {
					using (Stream s = VirtualPathProvider.OpenFile(path))
					{
						using (StreamReader sr = new StreamReader(s))
						{
							string text2 = "";
							string[] lines = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
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
            }
			if (!container.Page.IsPostBack)
			{
				if (viewData is IMMObject && viewData.GetType().Name != "MM_FormView" && ((IMMObject)viewData).LastModifiedDate.Year > 2000)
				{
					IMMObject obj = viewData as IMMObject;
					container.Controls.Add(new LiteralControl(String.Format("<input name='TESS_ObjectLastModifiedInfo' type='hidden' value='{0}' />",
						objectTypeSysName.ToLower() + " " + (obj.ObjectID > 0 ? obj.ObjectID.ToString() : obj.ObjectGUID.ToString()) + " " + obj.LastModifiedDate.Ticks.ToString())));
				}
			}
        }

		public void RenderMMView(string viewname, MM_ObjectType objectType, object viewData)
		{
			MMObjectController.RenderMMView(WebPart, objectType.SysName, viewname, viewData);
		}
		public void RenderMMView(string viewname)
        {
			MMObjectController.RenderMMView(WebPart, ObjectTypeSysName, viewname, null);
        }
		public void RenderMMView(string viewname, object viewData)
        {
			MMObjectController.RenderMMView(WebPart, ObjectTypeSysName, viewname, viewData);
        }

		public virtual void Update(IModelObject obj)
        {
			//@Sad
			if (obj.GetType().Assembly.GetName().Name == "erms")
			{
				if (obj is IMMObject && ((IMMObject)obj).LastModifiedDate.Year > 2000)
				{
					string olmd = HttpContext.Current.Request.Form["TESS_ObjectLastModifiedInfo"];
					var mmobj = obj as IMMObject;
					if (!olmd.IsEmpty() && mmobj != null && (mmobj.ObjectID > 0 || mmobj.ObjectGUID != Guid.Empty))
					{
						string[] olmd_info = olmd.Split(' ');
						if (olmd_info.Length == 3 &&
							olmd_info[0].ToLower() == mmobj.GetType().Name.ToLower() &&
							olmd_info[1] == (mmobj.ObjectID > 0 ? mmobj.ObjectID.ToString() : mmobj.ObjectGUID.ToString()) &&
							olmd_info[2].ToInt64(0) < mmobj.LastModifiedDate.Ticks)
						{
							// возникают ложные срабатывания
							Response.Write(TextResource.Get("Common.Warning.AccessConflict", "Вы или другой пользователь изменили информацию по объекту."));
							Response.End();
						}
					}
				}
			}
            obj.SetPropertyValue("LastModifiedDate", DateTime.Now);
            obj.SetPropertyValue("LastModifiedUserID", AppSPM.GetCurrentSubjectID());
			Base.Model.SubmitChanges();
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
