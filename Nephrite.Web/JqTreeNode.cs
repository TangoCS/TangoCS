using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
    public class JqTreeNode
    {
        public string text { get; set; }
        public bool expanded { get; set; }
        public bool hasChildren { get; set; }
        public string classes { get; set; }
        public string id { get; set; }
        public JqTreeNode[] children { get; set; }
    }
}
