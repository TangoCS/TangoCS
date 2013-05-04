using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
	public class SimpleButton : Button, IBarItem
	{
		public ButtonNextOption? Next { get; set; }
		public ILayoutBarItem Layout { get; set; }
		public string Image { get; set; }

		public SimpleButton()
		{
			Layout = AppWeb.Layout.Button;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			CssClass = Layout.CssClass;
			string s = Layout.Style(Image);
			if (!s.IsEmpty()) Attributes.Add("style", Layout.Style(Image));
			base.Render(writer);
		}
	}
}