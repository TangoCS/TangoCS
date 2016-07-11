using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;

namespace Tango.UI.Controls
{
	public static class CodeAreaExtension
	{
		public static void CodeArea(this IHtmlWriter c, string name, string value = null, bool enabled = true, string height = "500px")
		{
			//c.Page.RegisterScript("codearea", GlobalSettings.JSPath + "codearea.js");
			//c.Page.RegisterScript("jquery.textarea", GlobalSettings.JSPath + "jquery.textarea.js");

			//c.Page.RegisterStartupScript("textareatab", "$(document).ready(function () { $('textarea').tabby();});");
			//c.Page.RegisterStartupScript("initCodeArea" + name, "createCodeArea('" + name + "','" + height + "');");
		
			c.Div(a => a.Style("padding-left:30px;height:" + height), () =>
			{
				c.TextArea(name, value, a => a.ID(name).Wrap(Wrap.Hard).Class("codearea").Style("height:" + height).Disabled(!enabled));
			});
		}
	}
}