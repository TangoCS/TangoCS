using System;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using Nephrite.Web.App;
using System.Web;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nephrite.Web.Controls;
using System.Collections.Generic;
using Nephrite.Web.Model;

namespace Nephrite.Web
{
    public class SqlLog : WebPart
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);

            if (Page.Request.QueryString["showsql"] == "1")
            {
				StringBuilder sql = new StringBuilder(20000);
				List<DataContext> dcl = new List<DataContext>(); 
				foreach (var key in HttpContext.Current.Items.Keys)
				{
					if ((string)key == "BaseDataContext")
						continue;

					var item = HttpContext.Current.Items[key] as DataContext;
					if (item != null && item.Log != null)
					{
						if (dcl.Contains(item))
							continue;
						sql.Append("\r\n\r\n<b>" + item.GetType().FullName + "</b> - " + key + "\r\n" + item.Log.ToString());
						dcl.Add(item);
						continue;
					}
					var item2 = HttpContext.Current.Items[key] as HDataContext;
					if (item2 != null && item2.Log != null)
					{
						sql.Append("\r\n\r\n<b>" + item2.GetType().FullName + "</b> - " + key + "\r\n" + item2.Log.ToString());
						continue;
					}
				}
				string sqlstr = sql.ToString();
				int sqlcount = Regex.Matches(sqlstr, "-- Context:").Count;

                writer.Write("<pre>");
				writer.Write("<span style='color:#FF3223'><b>Всего запросов: " + sqlcount.ToString() + "</b></span>");
				writer.Write(sqlstr);
                writer.Write("</pre>");
				writer.Write("<b>Журнал проверок прав доступа</b>");
				if(HttpContext.Current.Items["SpmLog"] != null)
					writer.Write("<pre>" + (string)HttpContext.Current.Items["SpmLog"] + "</pre>");
				else
					writer.Write("<pre>нет записей</pre>");
				if (HttpContext.Current.Items["RequestBeginDate"] != null)
				{
					TimeSpan ts = DateTime.Now.Subtract((DateTime)HttpContext.Current.Items["RequestBeginDate"]);
					writer.Write("<br /><b>Время формирования страцицы: {0}.{1}</b>", Math.Floor(ts.TotalSeconds), ts.Milliseconds);
				}
            }
        }
    }
}
