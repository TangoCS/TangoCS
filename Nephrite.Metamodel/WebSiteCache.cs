using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.App;
using Nephrite.Metamodel.Model;
using Nephrite.Web.SPM;
using Nephrite.Web;
using System.Data.Linq;
using System.IO;
using Nephrite.Web.FileStorage;

namespace Nephrite.Metamodel
{
    public static class WebSiteCache
    {
        static Dictionary<string, string> viewKeys = null;
        static Dictionary<string, MM_FormView> formViews = null;
        static Dictionary<string, string> viewPaths = null;
        static object locker = new object();
		static bool resetKeys;
        static DateTime timeStamp = DateTime.Now;

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
                
                AppMM.DataContext.N_Caches.DeleteAllOnSubmit(AppMM.DataContext.N_Caches);
                AppMM.DataContext.N_Caches.InsertOnSubmit(new Nephrite.Metamodel.Model.N_Cache { TimeStamp = DateTime.Now });
                AppMM.DataContext.SubmitChanges();
				TextResource.ResetCache();
				MView.ResetCache();
				DynamicClassActivator.Clear();
            }
        }

		static Func<modelDataContext, Nephrite.Metamodel.Model.N_Cache> getN_Caches;

        public static void CheckValid()
        {
			if (getN_Caches == null)
				getN_Caches = CompiledQuery.Compile<modelDataContext, Nephrite.Metamodel.Model.N_Cache>(db =>
						db.N_Caches.FirstOrDefault());

			var stamp = getN_Caches(AppMM.DataContext);
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
							DynamicClassActivator.Clear();
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
                        AppMM.DataContext.N_Caches.InsertOnSubmit(new Nephrite.Metamodel.Model.N_Cache { TimeStamp = DateTime.Now });
                        AppMM.DataContext.SubmitChanges();
                    }
                }
            }
        }
		public static bool IsViewExists(string controlPath)
		{
			if (controlPath.EndsWith(".ascx.cs"))
				controlPath = controlPath.Substring(0, controlPath.Length - 3);
			if (formViews == null)
				GetViewCacheKey("");

			return formViews.ContainsKey(controlPath.ToLower());
		}
        public static MM_FormView GetView(string controlPath)
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

		public static MM_FormView GetView(string className, string sysName)
		{
			return GetView(GetViewPath(className, sysName));
		}

		public static MM_FormView GetPackageView(string sysName)
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
							var formViews1 = new Dictionary<string, MM_FormView>();
							foreach (var v in AppMM.DataContext.MM_FormViews.ToList())
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
					foreach (var v in formViews.Values.Select(o => new { o.FormViewID, o.SysName, ClassName = o.ObjectTypeID.HasValue ? o.MM_ObjectType.SysName : "", o.ControlPath }))
						viewPaths.Add(v.ClassName == String.Empty ? v.SysName.ToLower() : (v.ClassName.ToLower() + "_" + v.SysName.ToLower()), v.ControlPath);
					foreach (var v in formViews.Values.Where(o => o.PackageID.HasValue).Select(o => new { o.FormViewID, o.SysName, ClassName = o.MM_Package.SysName, o.ControlPath }))
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
    }
}
