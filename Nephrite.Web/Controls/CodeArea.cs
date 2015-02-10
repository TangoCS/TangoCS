using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;


namespace Nephrite.Web.Controls
{
	public class CodeArea : System.Web.UI.UserControl
	{
		HtmlTextArea _area;
		public HtmlTextArea Area { get { return _area; } }
		public string Text { get { return _area.Value; } set { _area.Value = value; } }
		public bool Enabled { get { return !_area.Disabled; } set { _area.Disabled = !value; } }

		public string Height { get; set; }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Height.IsEmpty()) Height = "500px";

			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Style.Add(HtmlTextWriterStyle.PaddingLeft, "30px");
			div.Style.Add(HtmlTextWriterStyle.Height, Height);
			this.Controls.Add(div);

			_area = new HtmlTextArea();
			_area.EnableViewState = false;
			_area.Attributes.Add("wrap", "off");
			_area.Attributes.Add("class", "CodeArea");
			_area.Style.Add(HtmlTextWriterStyle.Height, Height);
			div.Controls.Add(_area);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptInclude("codearea", Settings.JSPath + "codearea.js");
			Page.ClientScript.RegisterClientScriptInclude("jquery.textarea", Settings.JSPath + "jquery.textarea.js");
			Page.ClientScript.RegisterStartupScript(typeof(Page), "textareatab", "$(document).ready(function () { $('textarea').tabby();});", true);
			Page.ClientScript.RegisterStartupScript(typeof(Page), "initCodeArea" + _area.ClientID, "createCodeArea('" + _area.ClientID + "','" + Height + "');", true);
		}
	}
}
