﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using System.Text.RegularExpressions;

namespace Nephrite.Web.Controls
{
    public partial class NavMenuEx : System.Web.UI.UserControl
    {
		protected List<NavMenuItem> Groups { get; set; }
		protected NavMenuItem CurrentGroup { get; set; }

		public NavMenuItem AddGroup(string title, string url, string icon)
        {
			if (Groups == null) Groups = new List<NavMenuItem>();
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
                if (Groups[i].Url.GetQueryParameter("bgroup").ToLower() == Query.GetString("bgroup"))
                {
                    CurrentGroup = Groups[i];
                    //Groups.RemoveAt(i);
                }
            }
            if (CurrentGroup == null && Groups.Count>0)
            {
                CurrentGroup = Groups[0];
                //Groups.RemoveAt(0);
            }
        }

		public string RenderMenuGroup(NavMenuItem menuItem)
		{
			string res = "";
			for (int i = 0; i < menuItem.Items.Count; i++)
			{
				res += RenderMenuItem(menuItem.Items[i]) + "<br />";
			}
			return res;
		}

		public string RenderMenuItem(NavMenuItem menuItem)
		{
			if (menuItem.Control != null)
				return menuItem.Control.RenderControl();
			string img = "";
			if (!String.IsNullOrEmpty(menuItem.Icon))
				img = HtmlHelperBase.Instance.Image(menuItem.Icon, menuItem.Title) + " ";
			return img + String.Format(@"<a href=""{0}"">{1}</a>", menuItem.Url, menuItem.Title + (menuItem.Expression.IsEmpty() ? "" : (" " + menuItem.EvaluateExpression())));
		}
    }

    public enum NavMenuButtonsMode
	{
		SmallButtons,
		BigButtons
	}
}