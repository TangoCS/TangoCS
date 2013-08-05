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
			HttpContext.Current.Items["helpdata"] = "mode=c_help&view=view&form=" + fv.Guid; //@Sad
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
                string text1 = "";
				string text2 = "";
                if (ctl != null)
                    text1 = "<div>Класс представления: " + ctl.GetType().FullName + ", " + objectTypeSysName + ", FormViewID=" + fv.FormViewID.ToString() + "</div>";

                while (e != null)
                {
                    text2 += "<b>" + HttpUtility.HtmlEncode(e.Message) + "</b>\r\n" + e.StackTrace + "\r\n\r\n";
                    e = e.InnerException;
                }
                LiteralControl lc = new LiteralControl(text1 + "<pre>" + text2 + "</pre>");
				container.Controls.Add(lc);
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
