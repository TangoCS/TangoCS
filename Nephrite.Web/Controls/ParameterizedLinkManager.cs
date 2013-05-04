using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
	public class ParameterizedLinkManager : CompositeControl
	{
		public event EventHandler Click;
		HiddenField hArg = new HiddenField();
		LinkButton btn = new LinkButton();

		protected override void OnInit(EventArgs e)
		{
			base.OnLoad(e);

			Controls.Add(hArg);
			Controls.Add(btn);
			hArg.ID = "hArg";
			btn.ID = "btn";
			btn.Click += new EventHandler(btn_Click);

			string script = String.Format(@"function {0}_click(arg){{document.getElementById('{1}').value = arg;{2};return false;}}",
				   ClientID, hArg.ClientID, Page.ClientScript.GetPostBackEventReference(btn, "", false));

			Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script, true);
		}

		void btn_Click(object sender, EventArgs e)
		{
			if (Click != null) Click(sender, e);
		}

		public string Value
		{
			get
			{
				return hArg.Value;
			}
			set
			{
				hArg.Value = value;
			}
		}

		public string RenderLink(string text, object arg)
		{
			if (!Enabled)
				return "";
			return String.Format("<a href='#' onclick=\"return {0}_click('{1}');\">{2}</a>", ClientID, arg, text);
		}

		public string RenderImage(string image, string text, object arg)
		{
			if (!Enabled)
				return "";
			return String.Format("<a href='#' onclick=\"return {0}_click('{1}');\"><img src='" + Settings.ImagesPath + "{2}' alt='{3}' class='middle'/></a>", ClientID, arg, image, text);
		}

		public string RenderImageConfirm(string image, string text, string confirmString, object arg)
		{
			if (!Enabled)
				return "";
			return String.Format("<a href='#' onclick=\"if (confirm('{4}')) {{ return {0}_click('{1}'); }}\"><img src='" + Settings.ImagesPath + "{2}' alt='{3}' class='middle'/></a>", ClientID, arg, image, text, confirmString);
		}

		public override bool Enabled
		{
			get { return btn.Enabled; }
			set { btn.Enabled = value; }
		}
	}
}
