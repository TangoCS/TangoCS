using System.Collections.Generic;

namespace Nephrite.Html.Controls
{
	public class Navigation
	{
		public List<Group> Groups { get; private set; }
		public Group CurrentGroup { get; set; }

		public Group AddGroup(string title, string url, string icon)
		{
			Group g = new Group(this) { Title = title, Url = url, Icon = icon };
			Groups.Add(g);
			return g;
		}

		public Navigation()
		{
			Groups = new List<Group>();
        }

		public class Group
		{
			public Navigation Owner { get; private set; }
			public string Title { get; set; }
			public string Url { get; set; }
			public string Icon { get; set; }
			public bool Selected { get; set; }
			public string Expression { get; set; }

			List<string> _items = new List<string>();
			List<Group> _groups;
			public List<Group> Groups { get { if (_groups == null) _groups = new List<Group>(); return _groups; } }
			public List<string> Items { get { return _items; } }

			public Group(Navigation owner)
			{
				Owner = owner;
			}
			
			public string EvaluateExpression()
			{
				if (Expression.IsEmpty()) return "";
				return (string)MacroManager.Evaluate(Expression);
			}
		}
	}
}
