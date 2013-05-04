using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Metamodel
{
    public interface ISiteMapProvider
    {
        string GetTitle(string nodename, string nodekey);
        string GetParentKey(string nodename, string nodekey);
        IEnumerable<SiteNode> Fill(string nodename, string nodekey);
    }

    public class SiteNode
    {
        public string Title { get; set; }
        public string Key { get; set; }
    }

    public class NavigationNode
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsSelected { get; set; }
        public int SiteNodeID { get; set; }
        public string Key { get; set; }
    }
}
