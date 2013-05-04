using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
	public class CodeAreaAce : Control
	{
		public CodeAreaAce()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Page.ClientScript.RegisterClientScriptInclude("ace", Settings.JSPath + "Ace/ace.js");
			Page.ClientScript.RegisterClientScriptInclude("acecs", Settings.JSPath + "Ace/theme-csharp.js");
			Page.ClientScript.RegisterClientScriptInclude("acetm", Settings.JSPath + "Ace/theme-textmate.js");
			Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-1.4.2.min.js");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "acevar" + ClientID, "var aceEditor" + ClientID + @";", true);
			Page.ClientScript.RegisterStartupScript(GetType(), "ace-" + ClientID, @"$(document).ready(function () {
	aceEditor" + ClientID + @" = ace.edit(""" + ClientID + @""");
	aceEditor" + ClientID + @".setTheme(""ace/theme/textmate"");
	var CSharpMode = require(""ace/mode/csharp"").Mode;
	aceEditor" + ClientID + @".getSession().setMode(new CSharpMode());
	document.getElementById(""" + ClientID + @""").style.fontSize='14px';
});", true);
			Page.ClientScript.RegisterOnSubmitStatement(GetType(), "acegetdata-" + ClientID, "document.getElementById('" + ClientID + "_data').value = aceEditor" + ClientID + @".getSession().getValue();");
		}

		string _value;
		public string Value { set { _value = value; } get { return Page.Request.Form[ClientID + "_data"]; } }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(@"<div id=""" + ClientID + @""" style=""height: 500px; width: 500px"">");
			writer.Write(HttpUtility.HtmlEncode(_value));
			writer.Write("</div>");
			writer.Write(@"<input type=""hidden"" name=""" + ClientID + @"_data"" id=""" + ClientID + @"_data"">");
		}
	}
}