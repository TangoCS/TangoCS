using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class SubmitButton : Button, IBarItem
	{
		public ButtonNextOption? Next { get; set; }
		public ILayoutBarItem Layout { get; set; }
		public string Image { get; set; }

		public SubmitButton()
		{
			Layout = AppWeb.Layout.Button;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			CssClass = Layout.CssClass;
			string s = Layout.Style(Image);
			if (!s.IsEmpty()) Attributes.Add("style", Layout.Style(Image));
			OnClientClick += "if (typeof(Page_ClientValidate) == 'function') { if (!Page_ClientValidate()) return false; } this.disabled = true; " + Page.ClientScript.GetPostBackEventReference(this, "") + "; return false;";

			base.Render(writer);
		}
	}
}