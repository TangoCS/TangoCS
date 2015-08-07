using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.RSS
{
    public class RssLinker
    {
        static List<IN_RssFeed> _feeds = null;
		static StringBuilder _sb;
		public IDC_RSS _dc { get; set; }

		public RssLinker(IDC_RSS dataContext)
		{
			_dc = dataContext;
        }

        public string RenderLinks()
        {
			if (_feeds == null)
            {
				_feeds = _dc.IN_RssFeed.OrderBy(o => o.Title).ToList();
				_sb = new StringBuilder(1000);
                foreach (var feed in _feeds)
                {
					_sb.AppendFormat(@"<link rel=""alternate"" type=application/rss+xml title=""{0}"" href=""/rss.ashx?rss={1}"">", feed.Title, feed.SysName);
                }
            }
            
            return _sb.ToString();
        }

        public string RenderLink(string sysName, string querystring)
        {
			if (_feeds == null)
			{
				_feeds = _dc.IN_RssFeed.OrderBy(o => o.Title).ToList();
            }

            var feed = _feeds.SingleOrDefault(o => o.SysName.ToUpper() == sysName.ToUpper());
            if (feed != null)
                return String.Format(@"<a class=""rsslink"" title=""{1}"" href=""/rss.ashx?rss={0}{3}""><img src=""{2}rss_16.png"" alt=""{1}"" /></a>", feed.SysName, feed.Title, IconSet.RootPath, querystring);
            return "";
        }
    }
}
