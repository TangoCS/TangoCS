using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Nephrite.Web;
using Nephrite.Web.ErrorLog;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.SPM;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.FormsEngine
{
    public class MView : Control
    {
		Exception error = null;
		bool loaded = false;
        public string ViewFormSysName { get; set; }
        public string PackageViewFormSysName { get; set; }
		public string ObjectTypeSysName { get; set; }
		public string PackageSysName { get; set; }
		ViewControl ctl;
		
		public static void ResetCache()
		{
		}

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
			if (AppSPM.IsCurrentUserHasRole(ConfigurationManager.AppSettings["AdministratorsRole"]))
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
			try
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
			catch (Exception ex)
			{
				error = ex;
				int errorID = ErrorLogger.Log(ex);
			}
        }

        public void SetViewData(string className, object viewData)
        {
			try
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
			catch (Exception ex)
			{
				error = ex;
				int errorID = ErrorLogger.Log(ex);
			}
        }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			try
			{
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
			catch (ThreadAbortException)
			{

			}
			catch (Exception ex)
			{
				error = ex;
				int errorID = ErrorLogger.Log(ex);
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
					base.RenderControl(writer);
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

		#region IWebPart Members
		public string CatalogIconImageUrl { get; set; }
		public string Description { get; set; }
		public string Subtitle { get; set; }
		public string Title { get; set; }
		public string TitleIconImageUrl { get; set; }
		public string TitleUrl { get; set; }
		#endregion
	}
}
