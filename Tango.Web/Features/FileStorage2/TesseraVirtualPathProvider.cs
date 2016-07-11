using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.IO;

namespace Nephrite.Web.FileStorage
{
	public class TesseraVirtualPathProvider : VirtualPathProvider
	{
		static Dictionary<string, string> cache = new Dictionary<string, string>();
		static object locker = new object();
		static DateTime cacheDate = DateTime.MinValue;

		public static List<string> RootFolders = new List<string>();

		void CheckCache()
		{
			if (cacheDate < AppWeb.CacheTimeStamp)
			{
				lock (locker)
				{
					if (cacheDate < AppWeb.CacheTimeStamp)
					{
						if (RootFolders.Count == 0)
							RootFolders.Add(Settings.ControlsPath.ToLower());
						cacheDate = AppWeb.CacheTimeStamp;
						var dllDate = File.GetLastWriteTime(AppWeb.Assembly.Location).Ticks.ToString("X");
						cache = new Dictionary<string, string>();
						foreach (var rf in RootFolders)
						{
							foreach (var item in FileStorageManager.DbFiles.Where(o => ("/" + o.FullPath).StartsWith(rf + "/"))
								.Select(o => new { o.FullPath, o.LastModifiedDate }))
							{
								cache.Add("/" + item.FullPath.ToLower(), dllDate + item.LastModifiedDate.Ticks.ToString("X"));
							}
						}
					}
				}
			}
		}

		bool IsPathVirtual(string virtualPath)
		{
			CheckCache();
			var vp = virtualPath.ToLower().Replace("~", "");
			if (RootFolders.Any(o => vp.StartsWith(o)) && cache.ContainsKey(vp))
				return true;
			return false;
		}

		public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			if (IsPathVirtual(virtualPath))
				return null;
			return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}

		public override bool FileExists(string virtualPath)
		{
			if (IsPathVirtual(virtualPath))
				return true;
			else
				return Previous.FileExists(virtualPath);
		}

		public override VirtualDirectory GetDirectory(string virtualDir)
		{
			if (IsPathVirtual(virtualDir))
				return new TesseraVirtualDirectory(virtualDir);
			return Previous.GetDirectory(virtualDir);
		}

		public override bool DirectoryExists(string virtualDir)
		{
			if (IsPathVirtual(virtualDir))
				return true;
			return Previous.DirectoryExists(virtualDir);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			if (IsPathVirtual(virtualPath))
				return new TesseraVirtualFile(virtualPath);
			else
				return Previous.GetFile(virtualPath);
		}

		public override string GetCacheKey(string virtualPath)
		{
			if (IsPathVirtual(virtualPath))
				return cache[virtualPath.ToLower()];
			return Previous.GetCacheKey(virtualPath);
		}
	}
}
