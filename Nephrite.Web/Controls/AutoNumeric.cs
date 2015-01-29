using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;
using Nephrite.Web.SettingsManager;
using Nephrite.Multilanguage;

namespace Nephrite.Web.Controls
{
	public class AutoNumeric
	{
		public static void Apply(TextBox textBox, decimal min, decimal max, int decCount)
        {
            textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-autonumeric", Settings.JSPath + "autoNumeric-1.7.4.js");
			var options = String.Format("{{aSep: '', aDec: '{0}', vMin: '{1}', vMax: '{2}', mDec: '{3}'}}",
				Language.CurrentCulture.NumberFormat.NumberDecimalSeparator, min.ToString(CultureInfo.InvariantCulture),
				max.ToString(CultureInfo.InvariantCulture), decCount);
            string script = "$('#" + textBox.ClientID + "').autoNumeric(" + options + ");";
            ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_autonumeric", script, true);
        }

		public static string GetApplyScript(string selector, decimal min, decimal max, int decCount)
		{
			var options = String.Format("{{aSep: '', aDec: '{0}', vMin: '{1}', vMax: '{2}', mDec: '{3}'}}",
				Language.CurrentCulture.NumberFormat.NumberDecimalSeparator, min.ToString(CultureInfo.InvariantCulture),
				max.ToString(CultureInfo.InvariantCulture), decCount);
			return "$('" + selector + "').autoNumeric(" + options + ");";
		}

		[Obsolete]
		public static void Apply(TextBox textBox, string options)
		{
			textBox.Page.ClientScript.RegisterClientScriptInclude("jquery-autonumeric", Settings.JSPath + "autoNumeric-1.7.4.js");
			string script = "$('#" + textBox.ClientID + "').autoNumeric(" + options + ");";
			ScriptManager.RegisterStartupScript(textBox, textBox.GetType(), textBox.ClientID + "_autonumeric", script, true);
		}
	}
}