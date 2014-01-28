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
					if (packageViewLocation.ContainsKey(PackageViewFormSysName))
						PackageSysName = packageViewLocation[PackageViewFormSysName];
					else
					{
						lock (packageViewLocation)
						{
							if (packageViewLocation.ContainsKey(PackageViewFormSysName))
								PackageSysName = packageViewLocation[PackageViewFormSysName];
							else
							{
								var dc = ((IDC_MetaStorage)A.Model);
								PackageSysName = (from fv in dc.IMM_FormView
												  join p in dc.IMM_Package on fv.PackageID equals p.PackageID
													where fv.ObjectTypeID == null && fv.SysName == PackageViewFormSysName
												select p.SysName).FirstOrDefault() ?? "";
								packageViewLocation[PackageViewFormSysName] = PackageSysName;
							}
						}
					}
				}
				addToUtils(PackageSysName + "Pck." + PackageViewFormSysName);
				return Settings.ControlsPath + "/" + PackageSysName + "Pck/" + PackageViewFormSysName + ".ascx";
			}
			else
			{
				string cls = ObjectTypeSysName.IsEmpty() ? (viewData as IMMObject).MetaClass.Name : ObjectTypeSysName;
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

	public class MViewWorkSpace : MView, IWebPart
	{
		Exception error = null;
		public bool SkipCreateMdm { get; set; }
		public bool DisableScriptManager { get; set; }
		public bool RenderTitle { get; set; }
		public string DefaultMode { get; set; }
		public string DefaultAction { get; set; }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			try
			{
				TraceContext tc = HttpContext.Current.Trace;
				tc.Write("Workspace class - begin");

				if (Query.GetString("notitle").Length == 0 && RenderTitle)
					Controls.Add(Page.LoadControl(Settings.ControlsPath + "/Common/title.ascx"));

				string mode = Query.GetString("mode");
				string action = Query.GetString("action");

				if (String.IsNullOrEmpty(mode) || String.IsNullOrEmpty(action))
				{
					mode = DefaultMode;
					action = DefaultAction;
				}

				if (!String.IsNullOrEmpty(mode) && !String.IsNullOrEmpty(action))
				{
					BaseController.Run(this, mode, action, DisableScriptManager, SkipCreateMdm);
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

		protected override void Render(HtmlTextWriter writer)
		{
			if (error != null)
			{
				if (Controls.Count > 1 && Controls[1].ID == "ChangeEmployee")
					Controls[1].RenderControl(writer);

				error.Render(writer);
			}
			else
			{
				try
				{
					base.Render(writer);
				}
				catch (Exception e)
				{
					ErrorLogger.Log(e);
					e.Render(writer);
				}
			}
		}
	}
}
