using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Web.SettingsManager;

namespace Tessera
{
	public partial class _Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Page.ClientScript.RegisterClientScriptInclude("mainwindow", Settings.JSPath + "homewindow.js");
		}
	}
}
