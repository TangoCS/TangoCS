﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
	public class NavMenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public string Icon { get; set; }
		public Control Control { get; set; }
		public bool Selected { get; set; }
		public string Expression { get; set; }
		List<NavMenuItem> _items = null;
		public List<NavMenuItem> Items 
		{ 
			get 
			{ 
				if (_items == null) _items = new List<NavMenuItem>();
				return _items;
			}
		}
				
		public string EvaluateExpression()
		{
			if (Expression.IsEmpty()) return "";
			return (string)MacroManager.Evaluate(Expression);
		}

		public NavMenuItem AddItem(string title, string url)
		{
			return AddItem(title, url, "");
		}
		public NavMenuItem AddItem(string title, string url, string icon)
		{
			var mi = new NavMenuItem { Title = title, Url = url, Icon = icon };
			Items.Add(mi);
			return mi;
		}
		public NavMenuItem AddItem(Control ctrl)
		{
			var mi = new NavMenuItem { Control = ctrl };
			Items.Add(mi);
			return mi;
		}
	}
}