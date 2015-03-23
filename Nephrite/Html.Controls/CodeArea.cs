using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html.Controls
{
	public static class CodeAreaExtension
	{
		public static void CodeArea(this HtmlWriter c, string name, string value = null, bool enabled = true, string height = "500px")
		{
			c.Page.RegisterScript("codearea", GlobalSettings.JSPath + "codearea.js");
			c.Page.RegisterScript("jquery.textarea", GlobalSettings.JSPath + "jquery.textarea.js");

			c.Page.RegisterStartupScript("textareatab", "$(document).ready(function () { $('textarea').tabby();});");
			c.Page.RegisterStartupScript("initCodeArea" + name, "createCodeArea('" + name + "','" + height + "');");
		
			c.Div((a) => a.Style = "padding-left:30px;height:" + height, () =>
			{
				c.TextArea(name, value, (a) =>
				{
					a.ID = name; a.Wrap = Wrap.Hard; a.Class = "codearea"; a.Style = "height:" + height; a.Disabled = !enabled; 
				});
			});
		}
	}
}