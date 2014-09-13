using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using Nephrite.Meta.Forms;

namespace Nephrite.Web.Controllers
{
	public abstract class MMObjectController : BaseController
	{
		string _className = "";

		public MMObjectController(string className)
		{
			_className = className;
		}

		

		public void RenderMMView(string viewname, string className, object viewData)
		{
			HttpContext.Current.Items["ViewContainer"] = WebPart;
			WebFormRenderer.RenderView(className, viewname, viewData);
		}
		public void RenderMMView(string viewname)
		{
			HttpContext.Current.Items["ViewContainer"] = WebPart;
			WebFormRenderer.RenderView(_className, viewname, null);
		}
		public void RenderMMView(string viewname, object viewData)
		{
			HttpContext.Current.Items["ViewContainer"] = WebPart;
			WebFormRenderer.RenderView(_className, viewname, viewData);
		}

		public virtual void Update(IModelObject obj)
		{
			obj.SetPropertyValue("LastModifiedDate", DateTime.Now);
			obj.SetPropertyValue("LastModifiedUserID", AppSPM.GetCurrentSubjectID());

			var dc = (IDataContext)System.Web.HttpContext.Current.Items["SolutionDataContext"];
			dc.SubmitChanges();
		}

		new public static Result Run(Control control, string mode, string action, bool disableScriptManager, bool skipCreateMdm)
		{
			Result res = BaseController.Run(control, mode, action, disableScriptManager, skipCreateMdm);
			if (res.Code == -1)
			{
				HttpContext.Current.Items["ViewContainer"] = control;
				WebFormRenderer.RenderView(mode, action, null);
				return new Result(0, "");
			}
			return res;
		}
	}
}