using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Nephrite.Web.Controls
{
	public partial class LetterFilter : BaseUserControl
	{
		object selector;

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public void SetLetters<T>(IQueryable<T> allitems, Expression<Func<T, string>> selector)
		{
			rptLetters.DataSource = allitems.Select(selector).Distinct();
			rptLetters.DataBind();
			this.selector = selector;
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> allitems)
		{
			if (Query.GetString("l") == "")
				return allitems;
			else
			{
				var expr = Expression.Lambda<Func<T, bool>>(Expression.Equal(((Expression<Func<T, string>>)selector).Body, Expression.Constant(HttpUtility.UrlDecode(Query.GetString("l")))), ((Expression<Func<T, string>>)selector).Parameters);
				return allitems.Where(expr);
			}
		}
	}
}