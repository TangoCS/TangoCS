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
            var fv = WebSiteCache.GetView(objectTypeSysName, viewname);
            if (fv == null)
            {
				LiteralControl lc2 = new LiteralControl("Для класса " + objectTypeSysName + " не создано представление " + viewname);
				container.Controls.Add(lc2);
                return;
            }
			HttpContext.Current.Items["FormViewID"] = fv.FormViewID;
			HttpContext.Current.Items["ObjectTypeID"] = fv.ObjectTypeID;
			HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + fv.Guid;
			Control ctl = null;
            try
            {
                //ctl = WebPart.Page.LoadControl("~/nmf" + fv.FormViewID.ToString() + ".ascx");
				ctl = container.Page.LoadControl("" + fv.ControlPath);
                if (viewData != null)
                    ((ViewControl)ctl).SetViewData(viewData);
                ((ViewControl)ctl).RenderMargin = fv.IsSingleObjectView;
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
                    text = "Класс представления: " + ctl.GetType().FullName + ", " + objectTypeSysName + ", FormViewID=" + fv.FormViewID.ToString() + "<br />";

                while (e != null)
                {
                    text += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
                    e = e.InnerException;
                }
                LiteralControl lc = new LiteralControl("<pre>" + text + "</pre>");
				container.Controls.Add(lc);
                if (line > 0)
                {
                    string text2 = "";

                    string[] lines = AppMM.DataContext.MM_FormViews.Single(o => o.FormViewID == fv.FormViewID).ViewTemplate.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i + 1 == line)
                            text2 += "<span style=\"color:Red; font-size:13px; font-weight:bold\">" + HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") + "</span";
                        else
                            text2 += HttpUtility.HtmlEncode(lines[i]).Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                        text2 += "<br />";
                    }
                    LiteralControl lc2 = new LiteralControl("<br /><br />" + text2);// + "<br /><br />" + HtmlHelperBase.Instance.ActionLink<MM_FormViewController>(c => c.Edit(fv.FormViewID, Query.CreateReturnUrl()), "Редактировать представление"));
					container.Controls.Add(lc2);
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
            //repository.SubmitChanges();
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
