using System;
using System.Collections;
using System.Collections.Generic;

namespace Tango
{
	public class TreeNode<T>
	{
		public T Data { get; set; }
		public TreeNode<T> Parent { get; set; }
		public ICollection<TreeNode<T>> Children { get; set; } = new LinkedList<TreeNode<T>>();

		public bool IsRoot => Parent == null;
		public bool IsLeaf => Children.Count == 0;
		public int Level => IsRoot ? 0 : Parent.Level + 1;

		public TreeNode() { }
		public TreeNode(T data)
		{
			Data = data;
		}
		public TreeNode(T data, params T[] children)
		{
			Data = data;
			foreach (var c in children)
				AddChild(c);
		}

		public TreeNode<T> AddChild(T child)
		{
			TreeNode<T> childNode = new TreeNode<T>(child) { Parent = this };
			Children.Add(childNode);
			return childNode;
		}

		public override string ToString()
		{
			return Data != null ? Data.ToString() : "[data null]";
		}
	}
}
