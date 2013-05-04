using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using System.Text;

namespace Nephrite.Web.Layout
{
	public class FormTessera : ILayoutForm
	{
		public string FormTableBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table");
			sb.AppendAttributes(attributes, "t-table");
			sb.Append(">");
			return sb.ToString();
		}

		public string FormRowBegin(string title, string comment, bool required,
			object rowAttributes, object labelAttributes, object requiredAttributes, object commentAttributes, object bodyAttributes)
		{
			StringBuilder sb = new StringBuilder(150);
			sb.Append(@"<tr");
			sb.AppendAttributes(rowAttributes, "");
			sb.Append(@"><td");
			sb.AppendAttributes(labelAttributes, "t-formlabel");
			sb.Append(@">");
			sb.Append(title);
			if (required)
			{
				sb.Append("<span");
				sb.AppendAttributes(requiredAttributes, "t-requiredfield");
				sb.Append(@">&nbsp;*</span>");
			}
			if (!String.IsNullOrEmpty(comment))
			{
				sb.Append(@"<div");
				sb.AppendAttributes(commentAttributes, "t-descriptiontext");
				sb.Append(@">");
				sb.Append(comment);
				sb.Append("</div>");
			}
			sb.Append(@"</td><td");
			sb.AppendAttributes(bodyAttributes, "t-formbody");
			sb.Append(@">");
			return sb.ToString();
		}

		public string FormRowEnd()
		{
			return @"</td></tr>";
		}

		public string FormTableEnd()
		{
			return @"<tr><td class=""t-formline"" colspan=""2""><img height=""1"" src=""" + Settings.ImagesPath + @"blank.gif"" width=""1"" /></td></tr></table>";
		}

		public string GroupTitleBegin(string id)
		{
			return String.Format(@"<div class=""tabletitle"" id=""{0}"">", id);
		}

		public string GroupTitleEnd()
		{
			return "</div>";
		}

		public string ButtonsBarBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table");
			sb.AppendAttributes(attributes, "t-formtoolbar");
			sb.Append("><tr>");
			return sb.ToString();
		}

		public string ButtonsBarEnd()
		{
			return @"</tr></table>";
		}

		public string ButtonsBarWhiteSpace()
		{
			return @"<td style=""width:100%""></td>";
		}

		public string ButtonsBarItemBegin()
		{
			return @"<td style=""vertical-align:middle"">";
		}

		public string ButtonsBarItemEnd()
		{
			return "</td>";
		}
	}

	public class ListRowDragTessera : ILayoutListRowDrag
	{
		public string TDDragHandle(string tableid, string content)
		{
			return String.Format(@"<td class=""dragHandle"" style=""padding-left:20px"" id=""{0}DragTD"">{1}</td>", tableid, content);
		}
	}

	public class NoAutoMargin : ILayoutAutoMargin
	{
		public string MarginBegin()
		{
			return "";
		}

		public string MarginEnd()
		{
			return "";
		}
	}

	public class AutoMarginDiv : ILayoutAutoMargin
	{
		public string MarginBegin()
		{
			return "<div style='padding:8px'>";
		}

		public string MarginEnd()
		{
			return "</div>";
		}
	}
}