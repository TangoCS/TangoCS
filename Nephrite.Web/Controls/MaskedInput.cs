using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Nephrite.Web
{
	public static class MaskedInput
	{
		public static void Apply(TextBox textBox, string mask)
		{
			textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-maskedinput", Settings.JSPath + "jquery.maskedinput-1.2.2.js");
			string script = "$('#" + textBox.ClientID + "').mask(\"" + mask + "\");";
			ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_maskedinput", script, true);
		}
	}
}
