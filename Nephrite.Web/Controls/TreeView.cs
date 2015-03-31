using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;


namespace Nephrite.Web.Controls
{
	public class TreeView : System.Web.UI.UserControl
	{
		public List<NodeTemplate> Template { get; set; }

		public TreeView()
		{
			Template = new List<NodeTemplate>();
			PersistCookie = true;
		}

		StringBuilder res = new StringBuilder(100000);
		
		HiddenField hLoadedTemplates = new HiddenField();
		Literal lTv = new Literal();


		protected void Page_Init(object sender, EventArgs e)
		{
			lTv.EnableViewState = false;
			LiteralControl lDynamic = new LiteralControl("<ul id='" + ClientID + "_tv' class='treeview-gray treeview' ></ul>");

			Controls.Add(hLoadedTemplates);
			Controls.Add(lTv);
			if (IsDynamic) Controls.Add(lDynamic);

		}

		public bool PersistCookie { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptInclude("jquery.cookie", Settings.JSPath + "jquery.cookie.js");
			Page.ClientScript.RegisterClientScriptInclude("jquery.treeview", Settings.JSPath + "jquery-treeview/jquery.treeview.js");

			if (!IsDynamic)
			{
				Page.ClientScript.RegisterStartupScript(typeof(Page), ClientID + "_inittreeview", @"
			$(document).ready(function() {
				$('#" + ClientID + @"_tv').attr('style', '');
				$('#" + ClientID + @"_tv').treeview(
					{
					collapsed: true,
					persist: " + (PersistCookie ? "'cookie'" : "'location'") + @",
					cookieId: " + (PersistCookie ? "'" + ClientID + "'" : "'" + Guid.NewGuid().ToString() + "'") + @"
					}
				);
				
				});", true);

				res.Append("<ul id='");
				res.Append(ClientID);
				res.Append("_tv' class='treeview-gray treeview' style='display:none'>");
				foreach (NodeTemplate t in Template)
				{
					List<Node> nodes = t.Load(new Node());
					foreach (Node n in nodes)
						RenderNode(n, t);
				}
				res.Append("</ul>");
				
				lTv.Text = res.ToString();
			}
			else
			{
				Page.ClientScript.RegisterClientScriptInclude("jquery.treeview.async", Settings.JSPath + "jquery-treeview/jquery.treeview.async.js");
			}
		}

		public void SetPopulatingAction(string actionUrl)
		{
			string script = @"$(document).ready(function(){$('#" + ClientID + @"_tv').treeview({url: '" + actionUrl + "'})});";
			string script2 = @"$('#" + ClientID + @"_tv').treeview({url: '" + actionUrl + "'});";
			//Page.ClientScript.RegisterStartupScript(typeof(Page), ClientID + "_inittreeview", script, true);
			ScriptManager.RegisterStartupScript(this, this.GetType(), ClientID + "_inittreeviewmodal", script2, true);
		}

		void RenderTemplate(List<NodeTemplate> templates, Node node)
		{
			if (node.Level == 20)
				return;
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
				res.Append("<ul>");

				foreach (NodeTemplate t in nodes.Keys)
					foreach (Node n in nodes[t])
						RenderNode(n, t);

				res.Append("</ul>");
			}
		}

		void RenderNode(Node n, NodeTemplate t)
		{
			string nodeid = n.ID == 0 ? n.GUID.ToString() : n.ID.ToString();
			res.Append("<li>");
			if (!String.IsNullOrEmpty(t.DefaultMethod) || t.DefaultMethodFunc != null)
			{
				if (t.DefaultMethodFunc != null)
					res.Append(t.DefaultMethodFunc(n));
				else
					res.AppendFormat(t.DefaultMethod.Replace("oid=0", "oid=" + nodeid), n.Title);
			}
			else
			{
				res.Append(n.Title);
			}

			foreach (string m in t.Methods)
			{
				res.Append("&nbsp;");
				res.Append(m.Replace("oid=0", "oid=" + nodeid));
			}

			foreach (var m in t.MethodFuncs)
			{
				res.Append("&nbsp;");
				res.Append(m(n));
			}

			if (t.Children.Count > 0) RenderTemplate(t.Children, n);
			res.Append("</li>");
		}

		public bool IsDynamic { get; set; }
	}

	public abstract class NodeTemplate
	{
		static int i = 0;

		public int Index { get; set; }

		public List<NodeTemplate> Children { get; set; }

		/*protected Node ParentNode { get; set; }*/

		public List<string> Methods { get; set; }
		public List<Func<Node, string>> MethodFuncs { get; set; }
		public string DefaultMethod { get; set; }
		public Func<Node, string> DefaultMethodFunc { get; set; }
		public NodeTemplate()
		{
			Children = new List<NodeTemplate>();
			Methods = new List<string>();
			MethodFuncs = new List<Func<Node, string>>();
			i++;
			Index = i;
			/*ParentNode = new Node();*/
		}

		public abstract List<Node> Load(Node parentNode);

	}

	public class NodeTemplateStatic : NodeTemplate
	{
		public readonly Node Node = new Node();
		public int ID
		{
			get { return Node.ID; }
			set { Node.ID = value; }
		}
		public Guid GUID
		{
			get { return Node.GUID; }
			set { Node.GUID = value; }
		}
		public string Title
		{
			get { return Node.Title; }
			set { Node.Title = value; }
		}

		public override List<Node> Load(Node parentNode)
		{
			Node.Template = this;
			Node.Parent = parentNode;
			return new List<Node> { Node };
		}
	}

	public class NodeTemplateQueryable : NodeTemplate
	{
		//public IQueryable<Node> DataSource { get; set; }
		public Func<Node, IEnumerable<Node>> DataSource { get; set; }
		public override List<Node> Load(Node parentNode)
		{
			//if (DataSource2 != null)
			return DataSource(parentNode).ToList();
			//return DataSource.ToList();
		}
	}

	public class Node
	{
		Node _parent;
		public Node Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
				if (_parent != null) _parent.Children.Add(value);
			}
		}
		public List<Node> Children { get; set; }

		public int ID { get; set; }
		public Guid GUID { get; set; }
		public string Title { get; set; }
		public string ClassName { get; set; }
		public NodeTemplate Template { get; set; }

		public int Level { get; set; }
		public Node()
		{
			Children = new List<Node>();
			Populated = true;
			HasChildren = false;
		}
		public bool Populated { get; set; }
		public bool HasChildren { get; set; }
	}

}