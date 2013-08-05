using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Web;
using Nephrite.Metamodel.Model;
using Nephrite.Web.SPM;
using System.Configuration;
using System.Web.UI.WebControls.WebParts;
using System.Threading;
using Nephrite.Web.Controllers;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Nephrite.Web.ErrorLog;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Metamodel
{
    public class MView : Control
    {
		Exception error = null;
		bool loaded = false;
        public string ViewFormSysName { get; set; }
        public string PackageViewFormSysName { get; set; }
		public string ObjectTypeSysName { get; set; }
		ViewControl ctl;
		MM_FormView formView;
		string contentToRender = null;
		
		public static void ResetCache()
		{
			cache = new Dictionary<int, Dictionary<string, CachedData>>();
			if (Directory.Exists(Settings.ViewCachePath))
				Directory.Delete(Settings.ViewCachePath, true);
		}

		void addToUtils(MM_FormView fv)
		{
			if (AppSPM.IsCurrentUserHasRole(ConfigurationManager.AppSettings["AdministratorsRole"]))
			{
				if (HttpContext.Current.Items["MViewList"] == null)
					HttpContext.Current.Items["MViewList"] = new List<MM_FormView>();
				var list = (List<MM_FormView>)HttpContext.Current.Items["MViewList"];
				if (!list.Any(o => o.FormViewID == fv.FormViewID))
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
					formView = viewData is IMMObject ? WebSiteCache.GetView((viewData as IMMObject).MetaClass.Name, ViewFormSysName) :
						(PackageViewFormSysName.IsEmpty() ? WebSiteCache.GetView(ObjectTypeSysName, ViewFormSysName) :
						WebSiteCache.GetPackageView(PackageViewFormSysName));
					addToUtils(formView);
					if (CheckCache())
						return;
					ctl = pg.LoadControl("~" + formView.ControlPath) as ViewControl;
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
					formView = WebSiteCache.GetView(className, ViewFormSysName);
					if (formView == null)
						throw new Exception("В классе " + className + " не найдено представление " + ViewFormSysName);
					addToUtils(formView);
					if (CheckCache())
						return;
					Page pg = new Page();
					ctl = pg.LoadControl("~" + formView.ControlPath) as ViewControl;
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
					formView = WebSiteCache.GetPackageView(PackageViewFormSysName);
					if (formView != null)
					{
						addToUtils(formView);
						if (CheckCache())
							return;
						ctl = Page.LoadControl("" + formView.ControlPath) as ViewControl;
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
					formView = WebSiteCache.GetPackageView(PackageViewFormSysName);
					if (formView != null)
					{
						addToUtils(formView);
						if (CheckCache())
							return;
						ctl = Page.LoadControl("" + formView.ControlPath) as ViewControl;
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

		static JavaScriptSerializer j = new JavaScriptSerializer();
		static HashAlgorithm ha = HashAlgorithm.Create();
		string getCacheFileName(MM_FormView formView)
		{
			var i = formView.CacheKeyParams.IsEmpty() ? new FormViewCacheKeyInfo() : j.Deserialize<FormViewCacheKeyInfo>(formView.CacheKeyParams);
			string key = "";
			//if (i.Tables.Length > 0)
			//	key += "&t=" + TableInfo.GetMaxLastModifyDate(i.Tables);
			for (int n = 0; n < i.QueryParams.Length; n++)
				key += "&q" + n.ToString() + "=" + Query.GetString(i.QueryParams[n]);
			for (int n = 0; n < i.Macro.Length; n++)
				key += "&m" + n.ToString() + "=" + MacroManager.Evaluate(i.Macro[n]);
			byte[] b;
			lock(ha)
				b = ha.ComputeHash(Encoding.UTF8.GetBytes(key));
			var fn = "";
			for (int n = 0; n < b.Length; n++)
				fn += b[n].ToString("X");
			//return fn;
			string path = Settings.ViewCachePath + "/" + formView.FormViewID.ToString();
			Directory.CreateDirectory(path);
			return Path.Combine(path, fn);
		}

		class CachedData
		{
			public DateTime Date { get; set; }
			public string Data { get; set; }
		}

		static Dictionary<int, Dictionary<string, CachedData>> cache = new Dictionary<int, Dictionary<string, CachedData>>();

		bool CheckCache()
		{
			if (Settings.EnableViewCache && formView.IsCaching && HttpContext.Current.Request.HttpMethod == "GET")
			{
				string fullPath = getCacheFileName(formView);
				/*if (cache.ContainsKey(formView.FormViewID))
				{
					var c1 = cache[formView.FormViewID];
					if (c1.ContainsKey(fullPath) && (formView.CacheTimeout == 0 || DateTime.Now.Subtract(c1[fullPath].Date).TotalSeconds < formView.CacheTimeout))
					{
						contentToRender = c1[fullPath].Data;
						return true;
					}
				}*/
				
				if (File.Exists(fullPath))
				{
					if (formView.CacheTimeout == 0 || DateTime.Now.Subtract(File.GetLastWriteTime(fullPath)).TotalSeconds < formView.CacheTimeout)
					{
						lock (writeLocker)
						{
							contentToRender = File.ReadAllText(fullPath);
						}
						return true;
					}
				}
			}
			return false;
		}
		static object writeLocker = new object();
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
					if (Settings.EnableViewCache && formView != null && formView.IsCaching && HttpContext.Current.Request.HttpMethod == "GET" && Controls.Count > 0 && Controls[0] is ViewControl)
					{
						string str = Controls[0].RenderControl();
						//if (!cache.ContainsKey(formView.FormViewID))
						//	cache[formView.FormViewID] = new Dictionary<string, CachedData>();
						//cache[formView.FormViewID][getCacheFileName(formView)] = new CachedData { Data = str, Date = DateTime.Now };
						lock (writeLocker)
						{
							File.WriteAllText(getCacheFileName(formView), str);
						}
						writer.Write(str);
					}
					else
						base.RenderControl(writer);
					if (contentToRender != null)
						writer.Write(contentToRender);
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
