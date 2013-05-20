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
using System.ComponentModel;
using Nephrite.Web.SettingsManager;



namespace Nephrite.Web.Controls
{
	[ParseChildren(typeof(TabPage))]
	public class TabControl : System.Web.UI.UserControl
	{
		public int SelectedTab { get; set; }

		protected void Page_Init(object sender, EventArgs e)
		{
			string initScript = "<script type='text/javascript'>var t=new ddtabcontent('" + ClientID + "'); t.setpersist(true); t.setselectedClassTarget('link'); t.init();</script>";
			Page.ClientScript.RegisterClientScriptInclude("nw-tabcontrol", Settings.JSPath + "tabcontent.js");
			Page.ClientScript.RegisterStartupScript(typeof(Page), "nw-tabcontrol-init" + ClientID, initScript);
			ScriptManager.RegisterStartupScript(Page, typeof(Page), "nw-tabcontrol-init" + ClientID, initScript, false);

		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<ul id='" + ClientID + "' class='tabs'>");
			int i = 0;
			SelectedTab = 0;
			if (Request.Cookies[ClientID] != null)
				SelectedTab = Request.Cookies[ClientID].Value.ToInt32(0);

			foreach (Control c in Controls)
			{
				if (c is TabPage)
				{
					if (c.Visible)
					{
						TabPage tp = c as TabPage;
						writer.Write("<li><a href='#' rel='");
						writer.Write(c.ClientID);
						writer.Write("' class='" + (i == SelectedTab ? "selected" : "") + "'>");
						writer.Write(tp.Title);
						if (!String.IsNullOrEmpty(tp.Image))
							writer.Write("&nbsp;" + HtmlHelperBase.Instance.Image(tp.Image, ""));
						writer.Write("</a>");
						writer.Write("</li>");
					}
					else
					{
						writer.Write("<li style='display:none'><a>.</a></li>");
					}
					i++;
				}
			}
			writer.Write("</ul>");
			writer.Write("<div style='border-top:1px solid gray; width:100%; margin-bottom: 1em; padding-top: 10px'>");

			i = 0;
			foreach (Control c in Controls)
			{
				if (c is TabPage)
				{
					if (c.Visible)
					{
						TabPage tp = c as TabPage;
						writer.Write("<div id='" + tp.ClientID + "' class='tabcontent' style='display:" + (i == SelectedTab ? "block" : "none") + "'>");
						tp.RenderControl(writer);
						writer.Write("</div>");
					}
					else
					{
						writer.Write("<div class='tabcontent' style='display:none'>.</div>");
					}
					i++;
				}
			}

			//base.Render(writer);

			writer.Write("</div>");
		}
	}
}