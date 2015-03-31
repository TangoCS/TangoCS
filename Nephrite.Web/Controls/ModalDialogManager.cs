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
using System.Xml.Linq;
using System.Collections.Generic;

namespace Nephrite.Web.Controls
{
	public class ModalDialogManager : System.Web.UI.UserControl
	{
		HiddenField hfOpenDialogs = new HiddenField();
		public List<ModalDialog> ModalWindows = new List<ModalDialog>();

		protected void Page_Init(object sender, EventArgs e)
		{
			hfOpenDialogs.EnableViewState = false;
			hfOpenDialogs.Value = "0";
			Controls.Add(hfOpenDialogs);

			ScriptManager.RegisterStartupScript(this, GetType(), ClientID, "mdm_hfOpenDialogs = '" + hfOpenDialogs.ClientID + "';", true);

			Page.Items["ModalDialogManager"] = this;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			hfOpenDialogs.Value = "0";
		}
		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);

			if (ModalWindows.Count() > 0)
			{
				writer.Write("<div id='ModalDialogPlace'>");
				foreach (ModalDialog mw in ModalWindows)
				{
					if (mw.Visible)
						mw.RenderControl(writer);
					mw.SkipRender = true;
				}
				writer.Write("</div>");
			}
		}
	}
}