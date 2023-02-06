using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tango.UI;

namespace Tango.Html
{
	internal class Node
	{
		public Node PreviousSibling { get; private set; }
		public Node NextSibling { get; private set; }

		public Node FirstChild { get; private set; }
		public Node LastChild { get; private set; }

		public Node ParentNode { get; private set; }

		internal bool IsDeleted { get; set; }

		public Node AppendChild(Node newChild)
		{
			newChild.ParentNode = this;
			newChild.NextSibling = null;

			if (FirstChild == null)
				FirstChild = newChild;

			if (LastChild != null)
				LastChild.NextSibling = newChild;

			newChild.PreviousSibling = LastChild;
			LastChild = newChild;

			return newChild;
		}

		public Node PrependChild(Node newChild)
		{
			newChild.ParentNode = this;
			newChild.PreviousSibling = null;

			if (LastChild == null)
				LastChild = newChild;

			if (FirstChild != null)
				FirstChild.PreviousSibling = newChild;

			newChild.NextSibling = this.FirstChild;
			FirstChild = newChild;

			return newChild;
		}

		public Node InsertBefore(Node newChild)
		{
			newChild.ParentNode = ParentNode;

			if (PreviousSibling == null && ParentNode != null)
				ParentNode.FirstChild = newChild;

			newChild.PreviousSibling = PreviousSibling;
			newChild.NextSibling = this;
			PreviousSibling = newChild;

			return newChild;
		}



		public IEnumerable<Node> Children
		{
			get
			{
				var el = FirstChild;
				while (el != null)
				{
					yield return el;
					el = el.NextSibling;
				}
			}
		}

		public IEnumerable<Node> ChildrenReversed
		{
			get
			{
				var el = LastChild;
				while (el != null)
				{
					yield return el;
					el = el.PreviousSibling;
				}
			}
		}

		public void RemoveChildren()
		{
			foreach (var el in Children)
				el.IsDeleted = true;
			FirstChild = null;
			LastChild = null;
		}

		public void Remove()
		{
			if (ParentNode == null)
				return;

			IsDeleted = true;

			if (NextSibling != null)
				NextSibling.PreviousSibling = PreviousSibling;
			else
				ParentNode.LastChild = PreviousSibling;

			if (PreviousSibling != null)
				PreviousSibling.NextSibling = NextSibling;
			else
				ParentNode.FirstChild = NextSibling;
		}

		public Node InsertAdjacent(AdjacentHTMLPosition position, Node newElement)
		{
			switch (position)
			{
				case AdjacentHTMLPosition.BeforeEnd:
					AppendChild(newElement);

					break;
				case AdjacentHTMLPosition.BeforeBegin:
					newElement.ParentNode = this.ParentNode;

					if (this.PreviousSibling != null)
						this.PreviousSibling.NextSibling = newElement;
					else if (this.ParentNode != null)
						this.ParentNode.FirstChild = newElement;
					
					this.PreviousSibling = newElement;

					break;
				case AdjacentHTMLPosition.AfterBegin:
					newElement.ParentNode = this;

					if (LastChild == null)
						LastChild = newElement;

					if (FirstChild != null)
					{
						FirstChild.PreviousSibling = newElement;
						newElement.NextSibling = FirstChild;
					}

					FirstChild = newElement;

					break;
				case AdjacentHTMLPosition.AfterEnd:
					newElement.ParentNode = this.ParentNode;

					if (this.NextSibling != null)
						this.NextSibling.PreviousSibling = newElement;
					else if (this.ParentNode != null)
						this.ParentNode.LastChild = newElement;

					this.NextSibling = newElement;

					break;
				default:
					break;
			}

			return newElement;
		}

		public void AppendChildText<T>(T data) => AppendChild(Text.Create(data));

		public virtual void Serialize(StringBuilder w)
		{
			var n = FirstChild;
			while (n != null)
			{
				n.Serialize(w);
				n = n.NextSibling;
			}
		}
	}

	internal class Element : Node
	{
		public string TagName { get; set; }
		public string ID { get; set; }
		public List<ElementAttribute> Attributes { get; set; }
		public bool IsSelfClosing { get; set; }

		public override void Serialize(StringBuilder w)
		{
			w.Append('<');
			w.Append(TagName);
			if (Attributes?.Count > 0)
				foreach (var attr in Attributes)
					attr.Serialize(w);

			if (IsSelfClosing)
			{
				w.Append("/>");
			}
			else
			{
				w.Append('>');
				base.Serialize(w);
				w.Append("</");
				w.Append(TagName);
				w.Append('>');
			}
		}
	}

	internal class Text : Node
	{
		public static Text Create<T>(T data) => new Text(data?.ToString());
		
		public Text(string data)
		{
			Data = data;
		}

		public string Data { get; }

		public override void Serialize(StringBuilder w)
		{
			w.Append(Data);
		}
	}

	internal struct ElementAttribute
	{
		public string Name { get; set; }
		public List<string> Value { get; set; }

		public void Serialize(StringBuilder w)
		{
			if (Value != null)
				w.Append($" {Name}=\"{Value.Join(" ")}\"");
			else
				w.Append($" {Name}");
		}
	}
}
