using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;
using Nephrite.Web.Model;

namespace Nephrite.Web.Controllers
{
	[ControllerControlsPath("/_controltemplates/Nephrite.Web/")]
	public class MailTemplateController : SimpleClassController<MailTemplate>
	{
		public static string GetMailBody(string templateTitle, object obj)
		{
			MailTemplate t = AppWeb.DataContext.MailTemplates.Single(o => o.Title == templateTitle);
			return Parse(t.TemplateBody, obj);
		}

		public static string GetMailSubject(string templateTitle, object obj)
		{
			MailTemplate t = AppWeb.DataContext.MailTemplates.Single(o => o.Title == templateTitle);
			return Parse(t.TemplateSubject, obj);
		}

		static string Parse(string s, object obj)
		{
			string res = "";

			int i = 0;
			foreach (Match m in Regex.Matches(s, "<#.*?#>"))
			{
				res += s.Substring(i, m.Index - i);
				string macro = m.Value.Substring(2, m.Length - 4).Trim();

				object o = DataBinder.Eval(obj, macro);
				res += o == null ? "" : o.ToString();
				i = m.Index + m.Length;
			}
			if (i < s.Length)
			{
				res += s.Substring(i);
			}

			return res.Trim();
		}
	}


}
