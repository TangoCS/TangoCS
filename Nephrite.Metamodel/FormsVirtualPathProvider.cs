using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Nephrite.Web;
using Nephrite.Web.RSS;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Metamodel
{
    public class FormsVirtualPathProvider : VirtualPathProvider
    {
        string controlsPathRoot = Settings.ControlsPath.ToLower() + "/";

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPath.ToLower().StartsWith(controlsPathRoot))
                return null;
			if (virtualPath.StartsWith(CustomControlManager.Path))
				return null;
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        private bool IsPathVirtual(string virtualPath)
        {
            if (virtualPath.ToLower().StartsWith(controlsPathRoot) && WebSiteCache.IsViewExists(virtualPath))
                return true;
			if (virtualPath.StartsWith(CustomControlManager.Path))
				return true;
            String checkPath = VirtualPathUtility.ToAppRelative(virtualPath).ToLower();
            return checkPath.EndsWith(".rss.ascx");
        }

        public override bool FileExists(string virtualPath)
        {
            if (IsPathVirtual(virtualPath))
            {
                return true;
            }
            else
                return Previous.FileExists(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            if (virtualDir.ToLower().StartsWith(controlsPathRoot))
                return new FormsVirtualDirectory(virtualDir);
			if (virtualDir.StartsWith(CustomControlManager.Path))
				return new FormsVirtualDirectory(virtualDir);
            return base.GetDirectory(virtualDir);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            if (virtualDir.ToLower().StartsWith(controlsPathRoot))
                return true;
			if (virtualDir.StartsWith(CustomControlManager.Path))
				return true;
            return base.DirectoryExists(virtualDir);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsPathVirtual(virtualPath))
                return new FormsVirtualFile(virtualPath);
            else
                return Previous.GetFile(virtualPath);
        }
        
        public override string GetCacheKey(string virtualPath)
        {
            if (virtualPath.ToLower().EndsWith(".rss.ascx"))
            {
                string sysName = virtualPath.ToLower().Replace(".rss.ascx", "").Replace("/", "");
                var r = (from rf in (A.Model as IDC_RSS).IN_RssFeed
                     where rf.SysName.ToLower() == sysName
                     select (DateTime?)rf.LastModifiedDate).SingleOrDefault();
                if (r != null)
                    return sysName + "_" + r.Value.Ticks.ToString();
            }

			if (virtualPath.StartsWith(CustomControlManager.Path))
			{
				string key = virtualPath.Substring(CustomControlManager.Path.Length + 1).Replace(".ascx", "");
				if (key == "")
					return base.GetCacheKey(virtualPath);
				return CustomControlManager.GetCacheKey(key);
			}

			if (virtualPath.StartsWith(controlsPathRoot + "DynamicMainMenu"))
				return "DynamicMainMenu" + WebSiteCache.TimeStamp.Ticks.ToString();
            if (!virtualPath.ToLower().StartsWith(controlsPathRoot))
                return base.GetCacheKey(virtualPath);
            return WebSiteCache.GetViewCacheKey(virtualPath.ToLower());
        }
    }

    public class RssDir : VirtualDirectory
    {
        public RssDir(string virtualPath)
            : base(virtualPath)
        {

        }

        public override System.Collections.IEnumerable Children
        {
            get { return new List<object>(); }
        }

        public override System.Collections.IEnumerable Files
        {
            get { return new List<object>(); }
        }

        public override System.Collections.IEnumerable Directories
        {
            get { return new List<object>(); }
        }
    }

    public class FormsVirtualDirectory : VirtualDirectory
    {
        public FormsVirtualDirectory(string virtualPath) : base(virtualPath)
        {

        }
        public override System.Collections.IEnumerable Children
        {
			get { return null; }
        }

        public override System.Collections.IEnumerable Directories
        {
			get { return null; }
        }

        public override System.Collections.IEnumerable Files
        {
            get { return null; }
        }
    }
}
