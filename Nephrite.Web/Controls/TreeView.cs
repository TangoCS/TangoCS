using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Nephrite.Html.Controls;

namespace Nephrite.Web.Controls
{
	//public class TreeView : System.Web.UI.UserControl
	//{
	//	public List<NodeTemplate<Guid>> Template { get; set; }

	//	public TreeView()
	//	{
	//		Template = new List<NodeTemplate<Guid>>();
	//		PersistCookie = true;
	//	}

	//	StringBuilder res = new StringBuilder(100000);
		
	//	HiddenField hLoadedTemplates = new HiddenField();
	//	Literal lTv = new Literal();


	//	protected void Page_Init(object sender, EventArgs e)
	//	{
	//		lTv.EnableViewState = false;
	//		LiteralControl lDynamic = new LiteralControl("<ul id='" + ClientID + "_tv' class='treeview-gray treeview' ></ul>");

	//		Controls.Add(hLoadedTemplates);
	//		Controls.Add(lTv);
	//		if (IsDynamic) Controls.Add(lDynamic);

	//	}

	//	public bool PersistCookie { get; set; }

	//	protected void Page_Load(object sender, EventArgs e)
	//	{
	//		Page.ClientScript.RegisterClientScriptInclude("jquery.cookie", Settings.JSPath + "jquery.cookie.js");
	//		Page.ClientScript.RegisterClientScriptInclude("jquery.treeview", Settings.JSPath + "jquery-treeview/jquery.treeview.js");

	//		if (!IsDynamic)
	//		{
	//			Page.ClientScript.RegisterStartupScript(typeof(Page), ClientID + "_inittreeview", @"
	//		$(document).ready(function() {
	//			$('#" + ClientID + @"_tv').attr('style', '');
	//			$('#" + ClientID + @"_tv').treeview(
	//				{
	//				collapsed: true,
	//				persist: " + (PersistCookie ? "'cookie'" : "'location'") + @",
	//				cookieId: " + (PersistCookie ? "'" + ClientID + "'" : "'" + Guid.NewGuid().ToString() + "'") + @"
	//				}
	//			);
				
	//			});", true);

	//			res.Append("<ul id='");
	//			res.Append(ClientID);
	//			res.Append("_tv' class='treeview-gray treeview' style='display:none'>");
	//			foreach (NodeTemplate<Guid> t in Template)
	//			{
	//				List<Node<Guid>> nodes = t.Load(new Node<Guid>());
	//				foreach (Node<Guid> n in nodes)
	//					RenderNode(n, t);
	//			}
	//			res.Append("</ul>");
				
	//			lTv.Text = res.ToString();
	//		}
	//		else
	//		{
	//			Page.ClientScript.RegisterClientScriptInclude("jquery.treeview.async", Settings.JSPath + "jquery-treeview/jquery.treeview.async.js");
	//		}
	//	}

	//	public void SetPopulatingAction(string actionUrl)
	//	{
	//		string script = @"$(document).ready(function(){$('#" + ClientID + @"_tv').treeview({url: '" + actionUrl + "'})});";
	//		string script2 = @"$('#" + ClientID + @"_tv').treeview({url: '" + actionUrl + "'});";
	//		//Page.ClientScript.RegisterStartupScript(typeof(Page), ClientID + "_inittreeview", script, true);
	//		ScriptManager.RegisterStartupScript(this, this.GetType(), ClientID + "_inittreeviewmodal", script2, true);
	//	}

	//	void RenderTemplate(List<NodeTemplate<Guid>> templates, Node<Guid> node)
	//	{
	//		if (node.Level == 20)
	//			return;
	//		bool renderLevel = false;
	//		Dictionary<NodeTemplate<Guid>, List<Node<Guid>>> nodes = new Dictionary<NodeTemplate<Guid>, List<Node<Guid>>>();
	//		foreach (NodeTemplate<Guid> t in templates)
	//		{
	//			//t.ParentNode = node;
	//			List<Node<Guid>> ns = t.Load(node);
	//			foreach (var n in ns)
	//				n.Level = node.Level + 1;
	//			renderLevel = renderLevel || ns.Count > 0;
	//			nodes.Add(t, ns);
	//		}

	//		if (renderLevel)
	//		{
	//			res.Append("<ul>");

	//			foreach (NodeTemplate<Guid> t in nodes.Keys)
	//				foreach (Node<Guid> n in nodes[t])
	//					RenderNode(n, t);

	//			res.Append("</ul>");
	//		}
	//	}

	//	void RenderNode(Node<Guid> n, NodeTemplate<Guid> t)
	//	{
	//		string nodeid = n.ID.ToString();
	//		res.Append("<li>");
	//		if (!String.IsNullOrEmpty(t.DefaultMethod) || t.DefaultMethodFunc != null)
	//		{
	//			if (t.DefaultMethodFunc != null)
	//				res.Append(t.DefaultMethodFunc(n));
	//			else
	//				res.AppendFormat(t.DefaultMethod.Replace("oid=0", "oid=" + nodeid), n.Title);
	//		}
	//		else
	//		{
	//			res.Append(n.Title);
	//		}

	//		foreach (string m in t.Methods)
	//		{
	//			res.Append("&nbsp;");
	//			res.Append(m.Replace("oid=0", "oid=" + nodeid));
	//		}

	//		foreach (var m in t.MethodFuncs)
	//		{
	//			res.Append("&nbsp;");
	//			res.Append(m(n));
	//		}

	//		if (t.Children.Count > 0) RenderTemplate(t.Children, n);
	//		res.Append("</li>");
	//	}

	//	public bool IsDynamic { get; set; }
	//}

	

}