using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Controls
{
	public class TreeView : ViewComponent
	{
		public void AddTemplate(NodeTemplate<Guid> template)
		{
			_templateTree.Add(template);
			ProcessTemplateTree(template);
		}

		void ProcessTemplateTree(NodeTemplate<Guid> template)
		{
			if (!_allTemplates.ContainsKey(template.ID))
			{
				_allTemplates.Add(template.ID, template);
				if (template.Children.Count > 0)
					template.Children.ForEach(t => ProcessTemplateTree(t));
			}
		}

		Dictionary<Guid, NodeTemplate<Guid>> _allTemplates = new Dictionary<Guid, NodeTemplate<Guid>>();
		List<NodeTemplate<Guid>> _templateTree { get; set; } = new List<NodeTemplate<Guid>>();

		public void OnGetNode(ArrayResponse response)
		{
			var nodeid = Context.GetArg("nodeid");
			Guid parentid = nodeid == "#" ? Guid.Empty : nodeid.ToGuid();
			foreach (var t in _templateTree)
			{
				var nodes = t.Load(parentid);
				foreach (var node in nodes)
				{
					response.Data.Add(new { id = node.ID, text = node.Title, children = node.HasChildren, a_attr = new { href = "/" }, data = t.ID });
				}
			}
		}

		public void OnGetMenu(ArrayResponse response)
		{
			var n = new Node<Guid>();
			n.ID = Context.FormData.Parse<Guid>("id");
			n.HasChildren = Context.FormData.Parse<bool>("children");
			var templateID = Context.FormData.Parse<Guid>("data");
			var t = _allTemplates[templateID];

			int i = 0;
			foreach (var a in t.Actions)
			{
				var link = new ActionLink(Context);
				link = a(n, link);
				if (link != null)
				{
					var url = link.ToString();
					if (!url.IsEmpty())
					{
						response.Data.Add(new { name = "a" + i.ToString(), label = link.Title, url = url });
						i++;
					}
				}
			}
		}

		public void Render(LayoutWriter w)
		{
			w.Div(a => a.ID());

			w.Includes.Add("jstree/jstree.min.js");
			w.Includes.Add("Tango/treeview.js");
			w.AddClientAction("treeView", "defaultInit", f => new { id = f(ID) });
		}
	}

	public abstract class NodeTemplate<T>
	{
		public Guid ID { get; set; }
		public List<NodeTemplate<T>> Children { get; set; }
		public List<Func<Node<T>, ActionLink, ActionLink>> Actions { get; set; }

		public NodeTemplate()
		{
			ID = new Guid();
			Children = new List<NodeTemplate<T>>();
			Actions = new List<Func<Node<T>, ActionLink, ActionLink>>();
		}

		public abstract List<Node<T>> Load(T parentID);
	}

	public class StaticNodeTemplate<T> : NodeTemplate<T>
	{
		public readonly Node<T> Node = new Node<T>();

		public override List<Node<T>> Load(T parentID)
		{
			return new List<Node<T>> { Node };
		}
	}

	public class NodeListTemplate<T> : NodeTemplate<T>
	{
		public Func<T, IEnumerable<Node<T>>> DataSource { get; set; }
		public override List<Node<T>> Load(T parentID)
		{
			return DataSource(parentID).ToList();
		}
	}

	public class Node<T>
	{
		public T ID { get; set; }
		public string Title { get; set; }
		public bool HasChildren { get; set; }
	}

}
