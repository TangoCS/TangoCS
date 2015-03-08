using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Nephrite.Web.Controls;
using System.Text;


namespace Nephrite.Web
{
	public class HtmlHelperWSS : HtmlHelperBase
	{
		public HtmlHelperWSS()
			: base()
		{
			
		}

		public static string CSSClassAlternating = "ms-alternating";

		public static string TH(object title)
		{
			return TH(title, "", false);
		}

		public static string TH(object title, string width)
		{
			return TH(title, width, false);
		}

        public static string TH(object title, string width, bool nowrap)
        {
            return TH(title, width, nowrap, null);
        }

		public static string TH(object title, string width, bool nowrap, int? padding)
		{
            string w = String.IsNullOrEmpty(width) ? "" : ("style='width:" + width + "'");
            string nw = nowrap ? "white-space:nowrap;" : "";
            string p = padding.HasValue ? String.Format("text-align:middle;padding-left:{0};padding-right:{0};", padding.Value) : "";
            return "<th class='ms-vh2' " + w + "><table cellpadding='0' cellspacing='0' class='ms-unselectedtitle' width='100%'><tr><td class='ms-vb' style='height:23px; vertical-align:middle;" + nw + p + "'>" + title.ToString() + "</td></tr></table></th>";
        }

        public static string THBegin()
        {
            return "<th class='ms-vh2'><table cellpadding='0' cellspacing='0' class='ms-unselectedtitle' width='100%'><tr><td class='ms-vb' style='height:23px; vertical-align:middle;'>";
        }
        public static string THEnd()
        {
            return "</td></tr></table></th>";
        }

		public static string RequiredTitleBar()
		{
			return @"<table class='ms-toolbar w700' cellpadding='2' cellspacing='3' border='0'>
<tr>
    <td class='ms-toolbar' nowrap='true' align='right'>
		<span ID='reqdFldTxt' style='white-space: nowrap;padding-right: 3px;' class='ms-descriptiontext'>
		<span class='ms-formvalidation'>*</span> обозначает обязательное поле</span>
	</td>
</tr>
</table><br />";
		}

		public static string ListTableBegin()
        {
			return ListTableBegin(String.Empty, String.Empty);
        }
        public static string ListTableBegin(string width)
		{
			return ListTableBegin(width, String.Empty);
		}
		public static string ListTableBegin(string width, string id)
		{
			return String.Format(@"<table class=""ms-listviewtable"" cellpadding=""0"" cellspacing=""0"" {0} {1}>",
				String.IsNullOrEmpty(width) ? "" : @"style=""width:" + width + @"""",
				String.IsNullOrEmpty(id) ? "" : @"id=""" + id + @"""");
		}
		public static string ListTableEnd()
		{
			return @"</table>";
		}
		public static string ListHeaderBegin()
		{
			return @"<tr class=""ms-viewheadertr"">";
		}
		public static string ListHeaderBegin(string cssClass)
		{
			return @"<tr class=""" + cssClass + @" ms-viewheadertr"">";
		}
		public static string ListHeaderEnd()
		{
			return @"</tr>";
		}
		public static string ListRowBegin(string cssClass)
		{
			return @"<tr class=""" + cssClass + @""">";
		}
		public static string ListRowBegin(string cssClass, string id)
		{
			return @"<tr class=""" + cssClass + @""" id=""" + id + @""">";
		}
		public static string ListRowEnd()
		{
			return @"</tr>";
		}

        public static string FormTableBegin()
        {
            return @"<table class=""ms-formtable"" width=""100%"" cellpadding=""0"" cellspacing=""0"">";
        }
        public static string FormTableBegin(string width)
        {
            return @"<table class=""ms-formtable"" width=""" + width + @""" cellpadding=""0"" cellspacing=""0"">";
        }

        public static string FormTableEnd()
        {
			return @"<tr><td class=""ms-formline"" colspan=""2""><IMG height=""1"" src=""" + Settings.ImagesPath + @"blank.gif"" width=""1"" /></td></tr></table>";
        }

        public static string FormRowBegin(string title)
        {
            return FormRowBegin(title, "", false);
        }
        public static string FormRowBegin(string title, bool required)
        {
            return @"<tr><td class=""ms-formlabel"" width=""190px"" >" + title + (required ? "<span class='ms-formvalidation'> *</span>" : "") + @"</td><td class=""ms-formbody"">";
        }
		public static string FormRowBegin(string title, string comment, bool required)
        {
            return @"<tr><td class=""ms-formlabel"" width=""190px"" >" + title + 
				(required ? "<span class='ms-formvalidation'> *</span>" : "") +
				(String.IsNullOrEmpty(comment) ? "" : ("<div class='ms-descriptiontext' style='font-weight:normal; margin:3px 0 0 1px'>" + comment + "</div>")) +
				@"</td><td class=""ms-formbody"">";
        }
		

		
        public static string FormRowEnd()
        {
            return @"</td></tr>";
        }
        public static string FormRow(string title, object content)
        {
            return String.Format(@"<tr><td class=""ms-formlabel"" width=""190px"" >{0}</td><td class=""ms-formbody"">{1}</td></tr>", title, String.IsNullOrEmpty(Convert.ToString(content)) ? "&nbsp;" : Convert.ToString(content));
        }

		public static string FormToolbarBegin()
		{
			return @"<table class=""ms-formtoolbar"" style=""width:700px""><tr>";
		}
		public static string FormToolbarBegin(string width)
		{
			return @"<table class=""ms-formtoolbar"" style=""width:" + width + @"""><tr>";
		}
		public static string FormToolbarEnd()
		{
			return @"</tr></table>";
		}
		public static string FormToolbarWhiteSpace()
		{
			return @"<td style=""width:100%""></td>";
		}
		public static string FormToolbarItemBegin()
		{
			return "<td style='vertical-align:middle'>";
		}
		public static string FormToolbarItemEnd()
		{
			return "</td>";
		}


		public static string ListCellBegin()
		{
			return @"<td class=""ms-vb2"">";
		}
		public static string ListCellBegin(string align)
		{
			return String.Format(@"<td class=""ms-vb2"" style=""text-align:{0}"">", align);
		}
		public static string ListCellEnd()
		{
			return "</td>";
		}
		public static string ListCell(string content)
		{
			return ListCellBegin() + content + ListCellEnd();
		}

		public static string ListCell(string content, string align)
		{
			return ListCellBegin(align) + content + ListCellEnd();
		}

		public static string ListCellDragHandle(string content)
		{
			return String.Format(@"<td class=""dragHandle ms-vb2"" style=""padding-left:20px"">{0}</td>", content);
		}

		public static string ListGroupTitle(string content, int columns, string cssClass)
		{
			return @"<tr class=""" + cssClass + @"""><td class=""ms-gb"" colspan=""" + columns.ToString() + @""" style=""padding-left:4px;"">" + content + "</td></tr>";
		}

		public static string NavMenuBegin()
		{
			return @"<div class=""ms-quicklaunchouter""><div class=""ms-quickLaunch"" style=""width: 100%;""><table class=""ms-navSubMenu1"" cellspacing=""0"" cellpadding=""0"" border=""0"">";
		}

		public static string NavMenuEnd()
		{
			return "</table></div></div>";
		}

		public static string NavMenuGroup(NavMenuItem group)
		{
			StringBuilder res = new StringBuilder();
			res.Append("<tr><td><table class='ms-navheader' width='100%' cellspacing='0' cellpadding='0' border='0'><tr><td style='width: 100%;'>");
			if (!String.IsNullOrEmpty(group.Url) && group.Url != "#")
			{
				res.Append("<a class='ms-navheader' style='border-style: none; font-size: 8pt;' href='").Append(group.Url).Append("'>").Append(group.Title).Append("</a>");
			}
			else
			{
				res.Append("<b>").Append(group.Title).Append("</b>");
			}
			res.Append("</td></tr></table></td></tr>\r\n");


			if (group.Items.Count > 0)
			{
				res.Append("<tr><td><table class='ms-navSubMenu2' width='100%' cellspacing='0' cellpadding='0' border='0'>\r\n");
			}
			
			foreach (NavMenuItem item in group.Items)
			{
				res.Append("<tr><td><table class='ms-navitem' width='100%' cellspacing='0' cellpadding='0' border='0'><tr><td style='width: 100%;'>");
				if (item.Control != null)
					res.Append(item.Control);
				else
					res.AppendFormat("<a class='ms-navitem' style='border-style: none; font-size: 8pt;{0}' href='{1}'>{2}</a>", item.Selected ? " font-weight:bold;" : "", item.Url, item.Title + (item.Expression.IsEmpty() ? "" : (" " + item.EvaluateExpression())));
				res.Append("</td></tr></table></td></tr>\r\n");
			}
			
			if (group.Items.Count > 0)
			{
				res.Append("</table></td></tr>");
			}

			return res.ToString();
		}

		public static string ChildListTitleBegin()
		{
			return @"<div class=""tabletitle"">";
		}
        
        public static string ChildListTitleBegin(string id)
        {
            return String.Format(@"<div class=""tabletitle"" id=""{0}"">", id);
        }

		public static string ChildListTitleEnd()
		{
			return "</div>";
		}
		public static string ChildListTitle(string title)
		{
			return ChildListTitleBegin() + title + ChildListTitleEnd();
		}

    }
}
