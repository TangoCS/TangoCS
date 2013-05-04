using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Web.App;
using System.Web.Script.Serialization;

namespace Nephrite.Web.Controls
{
    public class Tree : Control, ICallbackEventHandler
    {
        public List<NodeTemplate> Template { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Template = new List<NodeTemplate>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.ClientScript.RegisterClientScriptInclude("tree", Settings.JSPath + "tree/tree.js");

        }

        protected override void Render(HtmlTextWriter writer)
        {
            string res = "<div class='tree'><ul class='treelist'>";
            foreach (NodeTemplate t in Template)
            {
                List<Node> nodes = t.Load(null);
                foreach (Node n in nodes)
                    res += RenderNode(n, t);
            }
            res += "</ul></div>";
            writer.Write(res);
        }

        string RenderTemplate(List<NodeTemplate> templates, Node node)
        {
            if (node.Level == 20)
                return "";
            string res = "";
            bool renderLevel = false;
            Dictionary<NodeTemplate, List<Node>> nodes = new Dictionary<NodeTemplate, List<Node>>();
            foreach (NodeTemplate t in templates)
            {
                //t.ParentNode = node;
				List<Node> ns = t.Load(node);
                foreach (var n in ns)
                    n.Level = node.Level + 1;
                renderLevel = renderLevel || ns.Count > 0;
                nodes.Add(t, ns);
            }


            if (renderLevel)
            {
                res += "<ul class='treelist'>";

                foreach (NodeTemplate t in nodes.Keys)
                    foreach (Node n in nodes[t])
                        res += RenderNode(n, t);

                res += "</ul>";
            }
            return res;
        }

        string RenderNode(Node n, NodeTemplate t)
        {
            string res = "<li class='treelistitem'><span class='envelope'><span class='node'><a href='#' class='tree_marker' />";
            if (!String.IsNullOrEmpty(t.DefaultMethod) || t.DefaultMethodFunc != null)
            {
                if (t.DefaultMethodFunc != null)
                    res += t.DefaultMethodFunc(n);
                else
                    res += String.Format(t.DefaultMethod.Replace("oid=0", "oid=" + n.ID.ToString()), n.Title);
            }
            else
            {
                res += n.Title;
            }

            foreach (string m in t.Methods)
            {
                res += "&nbsp;" + m.Replace("oid=0", "oid=" + n.ID.ToString());
            }

            foreach (var m in t.MethodFuncs)
            {
                res += "&nbsp;" + m(n);
            }
            res += "</span></span>";
            if (t.Children.Count > 0) res += RenderTemplate(t.Children, n);
            //if (n.HasChildren)
            //{
            //    res += "<ul class='treelist'>";
            //    res += "<li class='treelistitem'><span class='envelope'><span class='node' loading='true'>загрузка...</span></span></li>";
            //    res += "</ul>";
            //}
            res += "</li>";
            return res;
        }

        public event EventHandler<TreePopulateEventArgs> Populate;

        #region ICallbackEventHandler Members
        TreeNode[] Child;
        string error;
        public string GetCallbackResult()
        {
            if (!String.IsNullOrEmpty(error))
                return error;
            
            return (new JavaScriptSerializer()).Serialize(Child);
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            if (Populate != null)
            {
                TreePopulateEventArgs e = new TreePopulateEventArgs(eventArgument.Substring(0, eventArgument.IndexOf(',')),
                    eventArgument.Substring(eventArgument.IndexOf(',') + 1).ToInt32(0));
                try
                {
                    Populate(this, e);
                    Child = e.ChildNodes;
                }
                catch (Exception x)
                {
                    ErrorLogger.Log(x);
                    error = "!Произошла ошибка: " + x.Message;
                }
            }
        }

        #endregion
    }

    public class TreePopulateEventArgs : EventArgs
    {
        public readonly int NodeID;
        public readonly string NodeClassName;
        public TreeNode[] ChildNodes;

        internal TreePopulateEventArgs(string nodeClassName, int nodeID)
        {
            NodeID = nodeID;
            NodeClassName = nodeClassName;
        }
    }

    public class TreeNode
    {
        public string Text { get; set; }
        public bool HasChildren { get; set; }
        public string ClassName { get; set; }
        public string Id { get; set; }
        public TreeNode[] Children { get; set; }
    }
}
