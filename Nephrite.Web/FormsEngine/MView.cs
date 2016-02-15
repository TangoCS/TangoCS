using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using Nephrite.Web;
using Nephrite.ErrorLog;
using Nephrite.AccessControl;
using System.IO;
using Nephrite.Web.Controls;

namespace Nephrite.Web.FormsEngine
{
    public class MView : BaseControl
    {
		[Inject]
		public IErrorLogger ErrorLogger { get; set; }
		[Inject]
		public IAccessControlForRole<int> AccessControl { get; set; }

		Exception error = null;
		bool loaded = false;
        public string ViewFormSysName { get; set; }
        public string PackageViewFormSysName { get; set; }
		public string ObjectTypeSysName { get; set; }
		public string PackageSysName { get; set; }
		ViewControl ctl;
		
		static Dictionary<string, string> packageViewLocation = new Dictionary<string, string>();
		string GetPath(object viewData)
		{
			if (!PackageViewFormSysName.IsEmpty())
			{
				if (PackageSysName.IsEmpty())
				{
					throw new Exception("Не задано свойство PackageSysName");
				}
				addToUtils(PackageSysName + "." + PackageViewFormSysName);
				return Settings.ControlsPath + "/" + PackageSysName + "/" + PackageViewFormSysName + ".ascx";
			}
			else
			{
				string cls = ObjectTypeSysName.IsEmpty() ? (viewData as IModelObject).MetaClass.Name : ObjectTypeSysName;
				addToUtils(cls + "." + ViewFormSysName);
				return Settings.ControlsPath + "/" + cls + "/" + ViewFormSysName + ".ascx";
			}
		}


		void addToUtils(string fv)
		{
			if (AccessControl.CurrentUser.IsAdministrator)
			{
				if (HttpContext.Current.Items["MViewList"] == null)
					HttpContext.Current.Items["MViewList"] = new List<string>();
				var list = (List<string>)HttpContext.Current.Items["MViewList"];
				if (!list.Any(o => o == fv))
					list.Add(fv);
			}
		}

        public void SetViewData(object viewData)
        {
			if (!loaded)
			{
				Page pg = new Page();
				ctl = pg.LoadControl(GetPath(viewData)) as ViewControl;
				Controls.Add(ctl);
				loaded = true;
			}
			ctl.SetViewData(viewData);
		}

        public void SetViewData(string className, object viewData)
        {
			if (!loaded)
			{
				ObjectTypeSysName = className;
				Page pg = new Page();
				ctl = pg.LoadControl(GetPath(viewData)) as ViewControl;
				Controls.Add(ctl);
				loaded = true;
			}
			ctl.SetViewData(viewData);
        }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (!loaded && !String.IsNullOrEmpty(PackageViewFormSysName))
			{
				if (!PackageViewFormSysName.IsEmpty())
				{
					ctl = Page.LoadControl(GetPath(null)) as ViewControl;
					Controls.Add(ctl);
				}
				loaded = true;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				if (!loaded && Controls.Count == 0 && !String.IsNullOrEmpty(PackageViewFormSysName))
				{
					ctl = Page.LoadControl(GetPath(null)) as ViewControl;
					Controls.Add(ctl);
					loaded = true;
				}
			}
			catch (ThreadAbortException)
			{

			}
			catch (Exception ex)
			{
				error = ex;
				int errorID = ErrorLogger.Log(ex);
			}
		}

		public override void RenderControl(HtmlTextWriter writer)
		{
			if (error != null)
			{
				error.Render(writer);
			}
			else
			{
				try
				{
					StringWriter sw = new StringWriter();
					HtmlTextWriter hw = new HtmlTextWriter(sw);
					base.RenderControl(hw);
					writer.Write(sw.ToString());
				}
				catch (ThreadAbortException)
				{

				}
				catch (Exception e)
				{
					ErrorLogger.Log(e);
					e.Render(writer);
				}
			}
		}

		public string Title { get; set; }
	}
}
