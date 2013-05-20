using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
	public class AutoComplete2Options
	{
		

	}

	public class AutoComplete2
	{
		public static void Apply(TextBox textBox, string serviceUrl, AutoComplete2Options options)
		{
			textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-autocomplete", Settings.JSPath + "jquery.autocomplete-min.js");

			string script = @"$('#{0}').autocomplete({
serviceUrl:'{1}',
onSelect: function(value, data){ alert('You selected: ' + value + ', ' + data); }
});";
			ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_autoComplete", script, true);
		}
	}
}