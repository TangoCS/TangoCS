using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nephrite.Web.Controls;
using System.Collections.Generic;
using System.Web.UI;

namespace Nephrite.Web
{
	public class SqlLog : Control
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
            if (Page.Request.QueryString["showsql"] == "1")
            {
				StringBuilder sql = new StringBuilder(20000);

				if (A.Items["SqlLog"] != null)
					sql.Append("\r\n\r\n" + (A.Items["SqlLog"] as TextWriter).ToString());

				string sqlstr = sql.ToString();
				int sqlcount = Regex.Matches(sqlstr, "datacontext").Count;

                writer.Write("<pre>");
				writer.Write("<span style='color:#FF3223'><b>Всего запросов: " + sqlcount.ToString() + "</b></span>");
				writer.Write(sqlstr);
                writer.Write("</pre>");
            }
        }
    }
}
