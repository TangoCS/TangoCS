using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Nephrite.Web.SettingsManager;
using Nephrite.Meta;

namespace Nephrite.Web.RSS
{
    public static class RssLinker
    {
        static DateTime listLoadDate = DateTime.MinValue;
        static StringBuilder sb;
        static List<IN_RssFeed> feeds;

        public static string RenderLinks()
        {
            if (DateTime.Now.Subtract(listLoadDate).TotalMinutes > 1)
            {
				feeds = ((IDC_RSS)A.Model).IN_RssFeed.OrderBy(o => o.Title).ToList();
                listLoadDate = DateTime.Now;

                sb = new StringBuilder(1000);
                foreach (var feed in feeds)
                {
                    sb.AppendFormat(@"<link rel=""alternate"" type=application/rss+xml title=""{0}"" href=""/Rss.aspx?rss={1}"">", feed.Title, feed.SysName);
                }
            }
            
            return sb.ToString();
        }

        public static string RenderLink(string sysName, string querystring)
        {
            if (DateTime.Now.Subtract(listLoadDate).TotalMinutes > 1)
            {
				feeds = ((IDC_RSS)A.Model).IN_RssFeed.OrderBy(o => o.Title).ToList();
                listLoadDate = DateTime.Now;
            }

            var feed = feeds.SingleOrDefault(o => o.SysName.ToUpper() == sysName.ToUpper());
            if (feed != null)
                return String.Format(@"<a title=""RSS"" href=""/Rss.aspx?rss={0}{3}""><img align=""absmiddle"" src=""{2}rss_16.png"" alt=""{1}"" /></a>", feed.SysName, feed.Title, Settings.ImagesPath, querystring);
            return "";
        }
    }
}
