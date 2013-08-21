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
using Nephrite.Web;
using Nephrite.Web.SettingsManager;

public partial class View_Common_title : System.Web.UI.UserControl
{
	protected void Page_PreRender(object sender, EventArgs e)
	{
		Page.ClientScript.RegisterClientScriptInclude("baloontooltip", Settings.JSPath + "BaloonTooltip/BaloonTooltip.js");
	}
}
