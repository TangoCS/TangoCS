using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
	public class CodeMirror : System.Web.UI.UserControl
	{
		public string BeforeControlID { get; set; }
		public string AfterControlID { get; set; }

		HtmlTextArea area = new HtmlTextArea();
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			area = new HtmlTextArea();
			area.EnableViewState = false;
			area.ID = ID + "Area";
			Controls.Add(new LiteralControl("<style>.CodeMirror {border: 2px inset #dee; font-size:14px; font-family: Consolas,Courier New Cyr;}</style>"));
			Controls.Add(area);
			
			Page.ClientScript.RegisterClientScriptInclude("codemirror", Settings.JSPath + "CodeMirror/lib/codemirror.js");
			Page.ClientScript.RegisterClientScriptInclude("codemirror-clike", Settings.JSPath + "CodeMirror/mode/clike/clike.js");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "codemirror" + ID, @"var editor" + ClientID + @";
$(document).ready(function () {
		var beforeTop = $('#" + BeforeControlID + @"').position().top;
		var h = $(window).height() - beforeTop;" + (AfterControlID.IsEmpty() ? "" : @"
		h = h - $('#" + AfterControlID + @"').height();") + @"
		editor" + ClientID + @" = CodeMirror.fromTextArea(document.getElementById(""" + area.ClientID + @"""), {
		lineNumbers: true,
		matchBrackets: true,
		mode: ""text/x-csharp"",
		indentWithTabs: true,
		indentUnit: 4,
		tabSize: 4,
		onBlur: function() {document.getElementById(""" + area.ClientID + @""").value = editor" + ClientID + @".getValue();}
	});
	$("".CodeMirror"").css(""height"", h + ""px"");
	$("".CodeMirror-scroll"").css(""height"", h + ""px"");
	$("".CodeMirror"").css(""width"", ""100%"");
	$("".CodeMirror-scroll"").css(""width"", ""100%"");
});", true);
			PlaceHolder ph = Page.Header.FindControl("phCSS") as PlaceHolder;
			if (ph != null)
			{
				ph.Controls.Add(new LiteralControl(@"<link href=""" + Settings.JSPath + @"CodeMirror/lib/codemirror.css"" rel=""stylesheet"" type=""text/css"" />"));
			}
		}

		public string Text { get { return area.Value; } set { area.Value = value; } }
	}
}