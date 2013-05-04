using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Nephrite.Web.Controls
{
	public partial class NavMenu : UserControl
	{
		protected List<NavMenuItem> Groups { get; private set; }
		public NavMenu()
		{
			Groups = new List<NavMenuItem>();
		}
		public NavMenuItem AddGroup(string title, string url)
		{
			NavMenuItem g = new NavMenuItem { Title = title, Url = url };
			Groups.Add(g);
			return g;
		}
		public NavMenuItem AddGroup(string title, string url, string icon)
		{
			NavMenuItem g = new NavMenuItem { Title = title, Url = url, Icon = icon };
			Groups.Add(g);
			return g;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (Groups == null) return;
			for (int i = Groups.Count - 1; i >= 0; i--)
			{
				if (Groups[i].Items.Count == 0)
				{
					if (Groups[i].Url == "#") Groups.RemoveAt(i);
					continue;
				}
				bool d = true;
				for (int j = Groups[i].Items.Count - 1; j >= 0; j--)
				{
					if (d) d = (Groups[i].Items[j].Url == "#");
					if (Groups[i].Items[j].Url == "#") Groups[i].Items.RemoveAt(j);
				}
				if (d) Groups.RemoveAt(i);
			} 
		}
	}

	

	
}