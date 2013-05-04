using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
	public static class QuickFilterUrlExtension
	{
		public static Url SetQuickSearchQuery(this Url url)
		{
			return QuickFilter.SetSearchQuery(url);
		}
	}

	[ParseChildren(true)]
	[PersistChildren(false)]
	public class QuickFilter : Control, INamingContainer
	{
		HiddenField hQuickFilter = new HiddenField { ID = "hQuickFilter" };
		LinkButton go = new LinkButton { ID = "Go" };

		public static string SearchQuery
		{
			get
			{
				if (HttpContext.Current.Request.Form["qfind"] != null)
					return HttpContext.Current.Request.Form["qfind"];

				return HttpUtility.UrlDecode(Query.GetString("qfind"));
			}
		}
		public static Url SetSearchQuery()
		{
			return Url.Current.SetParameter("qfind", HttpUtility.UrlEncode(QuickFilter.SearchQuery));
		}
		public static Url SetSearchQuery(Url url)
		{
			return url.SetParameter("qfind", HttpUtility.UrlEncode(QuickFilter.SearchQuery));
		}
		

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Controls.Add(hQuickFilter);
			Controls.Add(go);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Page.IsPostBack) hQuickFilter.Value = SearchQuery;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			string script = @"
<script type='text/javascript'>
var timer = 0;
function " + ClientID + @"filter()
{
	if(timer)
	{
		window.clearTimeout(timer);
		timer = null;
	}
	timer = window.setTimeout(run" + ClientID + @"filter, 400);
}
function run" + ClientID + @"filter()
{
	document.getElementById('" + hQuickFilter.ClientID + @"').value = document.getElementById('qfind').value;
	" + Page.ClientScript.GetPostBackEventReference(go, "") + @"
}
</script>
";
			base.Render(writer);
			writer.Write(script);
		}

		public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Func<string, Expression<Func<T, bool>>> SearchExpression)
			where T : class
		{
			return hQuickFilter.Value.Trim() != String.Empty ? query.Where(SearchExpression(hQuickFilter.Value.Trim())) : query;
		}

		public string SearchTextBox
		{
			get
			{
				string s = TextResource.Get("Common.Toolbar.QFind", "Поиск");
				return SearchQuery.IsEmpty() ? "<input type=\"text\" onblur=\"if(this.value ==''){this.value='" + s + "';this.className = 'filterInput TextItalic';}\"" +
		" onfocus=\"if(this.className!='filterInput filterInputActive'){this.value='';this.className = 'filterInput filterInputActive';}\" name=\"qfind\" autocomplete=\"Off\"" +
		" value=\"" + s + "\" class=\"filterInput TextItalic\" id=\"qfind\" onkeydown=\"return event.keyCode != 13;\" onkeyup=\"" + ClientID + "filter();\"/>" :
		"<input type=\"text\" onblur=\"if(this.value ==''){this.value='" + s + "';this.className = 'filterInput TextItalic';}\"" +
		" onfocus=\"if(this.className!='filterInput filterInputActive'){this.value='';this.className = 'filterInput filterInputActive';}\" name=\"qfind\" autocomplete=\"Off\"" +
		" value=\"" + SearchQuery + "\" class=\"filterInput filterInputActive\" id=\"qfind\" onkeydown=\"return event.keyCode != 13;\" onkeyup=\"" + ClientID + "filter();\"/>";
			}
		}
	}
}
