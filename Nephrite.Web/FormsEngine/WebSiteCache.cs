using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using Nephrite.Web;
using System.Data.Linq;
using System.IO;
using Nephrite.Web.FileStorage;
using Nephrite.Web.SettingsManager;
using Nephrite.Web.TextResources;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.FormsEngine
{
    public static class WebSiteCache
    {
        static Dictionary<string, string> viewKeys = null;
        static Dictionary<string, IMM_FormView> formViews = null;
        static Dictionary<string, string> viewPaths = null;
        static object locker = new object();
		static bool resetKeys;
        static DateTime timeStamp = DateTime.Now;
		static IDC_MetaStorage dc = (IDC_MetaStorage)A.Model;

        public static DateTime TimeStamp { get { return timeStamp; } }

        public static void Reset()
        {
            lock (locker)
            {
				resetKeys = true;
                viewPaths = null;
                formViews = null;
                HttpContext.Current.Cache.Remove("menuitems");
                AppSettings.ResetCache();

				dc.IN_Cache.DeleteAllOnSubmit(dc.IN_Cache);
				var c = dc.NewIN_Cache();
				c.TimeStamp = DateTime.Now;
                dc.IN_Cache.InsertOnSubmit(c);
                A.Model.SubmitChanges();
				TextResource.ResetCache();
				MView.ResetCache();
				//DynamicClassActivator.Clear();
            }
        }

		//static Func<MMDataContext, Nephrite.Metamodel.Model.N_Cache> getN_Caches;

        public static void CheckValid()
        {
			//if (getN_Caches == null)
			//	getN_Caches = CompiledQuery.Compile<MMDataContext, Nephrite.Metamodel.Model.N_Cache>(db =>
			//			db.N_Caches.FirstOrDefault());

			var stamp = dc.IN_Cache.FirstOrDefault();
            if (stamp != null)
            {
                if (timeStamp < stamp.TimeStamp)
                {
					lock (locker)
					{
						if (timeStamp < stamp.TimeStamp)
						{
							timeStamp = stamp.TimeStamp;

							viewKeys = null;
							viewPaths = null;
							HttpContext.Current.Cache.Remove("menuitems");
							AppSettings.ResetCache();
							AppSPM.Instance.RefreshCache();
							AppSPM.AccessRightManager.RefreshCache();
							TextResource.ResetCache();
							//DynamicClassActivator.Clear();
							SPM2.ResetCache();
							/*string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
							foreach (var f in FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(ModelAssemblyGenerator.SourceFolder + "/bin")))
							{
								var fi = new FileInfo(Path.Combine(binPath, f.Title));
								if (fi.LastWriteTime < f.LastModifiedDate)
									File.WriteAllBytes(fi.FullName, f.GetBytes());
							}*/
						}
					}
                }
            }
            else
            {
                lock (locker)
                {
                    if (stamp == null)
                    {
						var c = dc.NewIN_Cache();
						c.TimeStamp = DateTime.Now;
						dc.IN_Cache.InsertOnSubmit(c);
                        A.Model.SubmitChanges();
                    }
                }
            }
        }

		/*public static bool IsViewExists(string controlPath)
		{
			if (controlPath.EndsWith(".ascx.cs"))
				controlPath = controlPath.Substring(0, controlPath.Length - 3);
			if (formViews == null)
				GetViewCacheKey("");

			return formViews.ContainsKey(controlPath.ToLower());
		}
        
		public static IMM_FormView GetView(string controlPath)
		{
			if (formViews == null)
				GetViewCacheKey("");

			if (controlPath == null)
				throw new Exception("controlPath не задан");
			if (formViews == null)
				throw new Exception("formViews не задан");

			if (!formViews.ContainsKey(controlPath.ToLower()))
				throw new Exception("Представление не существует: " + controlPath);
			return formViews[controlPath.ToLower()];
		}

		public static IMM_FormView GetView(string className, string sysName)
		{
			return GetView(GetViewPath(className, sysName));
		}

		public static IMM_FormView GetPackageView(string sysName)
		{
			return GetView(GetViewPath(sysName));
		}

		public static string GetViewCacheKey(string viewkey)
		{
			if (viewkey.EndsWith(".cs"))
				viewkey = viewkey.Substring(0, viewkey.Length - 3);
			var vk = viewKeys;
			if (vk == null || resetKeys)
			{
				lock (locker)
				{
					if (viewKeys == null || resetKeys)
					{
						resetKeys = false;
						try
						{
							string dllName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\" + ModelAssemblyGenerator.DllName);
							DateTime dllCreateDate = File.GetLastWriteTime(dllName);
							
							var viewKeys1 = new Dictionary<string, string>();
							var formViews1 = new Dictionary<string, IMM_FormView>();
							foreach (var v in ((IDC_MetaStorage)A.Model).IMM_FormView.ToList())
							{
								viewKeys1.Add(v.ControlPath.ToLower(), v.FormViewID.ToString() + "_" + dllCreateDate.Ticks.ToString() + "_" + v.LastModifiedDate.Ticks.ToString());
								formViews1.Add(v.ControlPath.ToLower(), v);
							}
							viewKeys = viewKeys1;
							formViews = formViews1;
						}
						catch(Exception e)
						{
							viewKeys = null;
							formViews = null;
							throw e;
						}
					}
					vk = viewKeys;
				}
			}

			if (vk.ContainsKey(viewkey))
				return vk[viewkey];

			return null;
		}

		static void initPaths()
		{
			GetViewCacheKey("");
			lock (locker)
			{
				if (viewPaths != null)
					return;
				try
				{
					viewPaths = new Dictionary<string, string>();
					foreach (var v in formViews.Values.Select(o => new { o.FormViewID, o.SysName, ClassName = o.ObjectTypeID.HasValue ? ((IDC_MetaStorage)A.Model).IMM_ObjectType.Where(o2 => o2.ObjectTypeID == o.ObjectTypeID).Select(o2 => o2.SysName).FirstOrDefault() : "", o.ControlPath }))
						viewPaths.Add(v.ClassName == String.Empty ? v.SysName.ToLower() : (v.ClassName.ToLower() + "_" + v.SysName.ToLower()), v.ControlPath);
					foreach (var v in formViews.Values.Where(o => o.PackageID.HasValue).Select(o => new { o.FormViewID, o.SysName, ClassName = ((IDC_MetaStorage)A.Model).IMM_Package.Where(o2 => o2.PackageID == o.PackageID).Select(o2 => o2.SysName).FirstOrDefault(), o.ControlPath }))
						viewPaths.Add(v.ClassName.ToLower() + "_" + v.SysName.ToLower(), v.ControlPath);
				}
				catch (Exception e)
				{
					viewPaths = null;
					throw e;
				}
			}
		}

		public static string GetViewPath(string sysName)
		{
			var vp = viewPaths;
			if (vp == null)
			{
				initPaths();
				vp = viewPaths;
			}
			sysName = sysName.ToLower();
			if (vp.ContainsKey(sysName))
				return vp[sysName];
			throw new Exception("Не найдено представление пакета " + sysName);
		}

		public static string GetViewPath(string className, string sysName)
		{
			var vp = viewPaths;
			if (vp == null)
			{
				initPaths();
				vp = viewPaths;
			}
			sysName = className.ToLower() + "_" + sysName.ToLower();
			if (vp.ContainsKey(sysName))
				return vp[sysName];
			throw new Exception("Не найдено представление " + className + "." + sysName);
		}
		*/
    }
}
