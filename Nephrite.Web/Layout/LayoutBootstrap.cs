using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using System.Text;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Layout
{
	public class ToolbarDropdownBootstrap : ILayoutPopupMenu
	{
		public string PopupMenuBegin()
		{
			return @"<li class=""dropdown""><a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"">";
		}
		public string PopupMenuEnd()
		{
			return @" <b class=""caret""></b></a>";
		}

		public string PopupMenuBodyBegin()
		{
			return @"<ul class=""dropdown-menu"">";
		}

		public string PopupMenuBodyEnd()
		{
			return "</ul></li>";
		}

		public string PopupMenuLink(ILink link)
		{
			string onClick = String.IsNullOrEmpty(link.OnClick) ? (link.TargetBlank ? ("javascript:window.open('" + link.Href + "')") : "javascript:window.location = '" + link.Href + "'") : link.OnClick;
			string accessKey = link.AccessKey.IsEmpty() ? "" : String.Format(@"<input style=""position:absolute; top:-300px; left:-300px"" type=""button"" accesskey=""{0}"" onfocus=""{1}"" />", link.AccessKey.ToUpper(), onClick);
			
			return String.Format(@"<li><a href=""{0}"" {2}>{1}</a>{3}</li>", link.Href, link.Title, onClick, accessKey);
		}

		public string PopupMenuSeparator()
		{
			return @"<li class=""divider""></li>";
		}
	}

	public class ToolbarBootstrap : ILayoutToolbar
	{
		public string ToolbarBegin(string cssClass)
		{
			throw new NotImplementedException();
		}

		public string ToolbarBegin(ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign, ILink titleLink)
		{
			string cssclass = "";
			switch (position)
			{
				case ToolbarPosition.FixedTop:
					cssclass = "navbar-fixed-top";
					break;
				case ToolbarPosition.FixedBottom:
					cssclass = "navbar-fixed-bottom";
					break;
				case ToolbarPosition.StaticTop:
					cssclass = "navbar-static-top";
					break;
				case ToolbarPosition.StaticBottom:
					cssclass = "navbar-static-bottom";
					break;
				case ToolbarPosition.Float:
					break;
				default:
					break;
			}
			if (mode == ToolbarMode.FormsToolbar) cssclass = "navbar-inverse " + cssclass;

			if (titleLink != null)
				return String.Format(@"<div class=""navbar {0}""><div class=""navbar-inner""><a class=""brand"" href=""{2}"">{1}</a><ul class=""nav"">", cssclass, titleLink.Title, titleLink.Href);
			else
				return String.Format(@"<div class=""navbar {0}""><div class=""navbar-inner""><ul class=""nav"">", cssclass);

		}

		public string ToolbarEnd()
		{
			return "</ul></div></div>";
		}

		public string ToolbarSeparator()
		{
			return @"<li class=""divider-vertical""></li>";
		}

		public string ToolbarWhiteSpace()
		{
			return @"</ul><ul class=""nav pull-right"">";
		}

		public string ToolbarLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			string c = onclick.IsEmpty() ? "" : String.Format(@"onclick=""{0}""", onclick);
			string i = image.IsEmpty() ? "" : String.Format(@"<img src=""{0}{1}"" alt=""{2}"" class=""middle"" />", Settings.ImagesPath, image, title);
			return String.Format(@"<li><a href=""{1}"" {2}>{3} {0}</a></li>", title, url, c, i);
		}

		public string ToolbarImageLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			string c = onclick.IsEmpty() ? "" : String.Format(@"onclick=""{0}""", onclick);
			string i = image.IsEmpty() ? "" : String.Format(@"<img src=""{0}{1}"" alt=""{2}"" class=""middle"" />", Settings.ImagesPath, image, title);
			return String.Format(@"<li><a href=""{0}"" {1}>{2}</a></li>", url, c, i);
		}

		public string ToolbarItem(string content)
		{
			return String.Format(@"<li>{0}</li>", content);
		}


		public void ToolbarItem(System.Web.UI.HtmlTextWriter writer, IBarItem content)
		{
			writer.Write(@"<li>");
			content.RenderControl(writer);
			writer.Write("</li>");
		}

	}

	public class LabelsBootstrap : ILayoutLabels
	{
		public string Label(string text)
		{
			return String.Format("<span class='label'>{0}</span>", text);
		}

		public string LabelSuccess(string text)
		{
			return String.Format("<span class='label label-success'>{0}</span>", text);
		}

		public string LabelWarning(string text)
		{
			return String.Format("<span class='label label-warning'>{0}</span>", text);
		}

		public string LabelImportant(string text)
		{
			return String.Format("<span class='label label-important'>{0}</span>", text);
		}

		public string LabelInfo(string text)
		{
			return String.Format("<span class='label label-info'>{0}</span>", text);
		}

		public string LabelInverse(string text)
		{
			return String.Format("<span class='label label-inverse'>{0}</span>", text);
		}
	}

	public class ListBootstrap : ILayoutList
	{
		public string ListTableBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table ");
			sb.AppendAttributes(attributes, "table");
			sb.Append(">");
			return sb.ToString();
		}

		public string ListHeaderBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<tr ");
			sb.AppendAttributes(attributes, "");
			sb.Append(">");
			return sb.ToString();
		}

		public string THBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<th ");
			sb.AppendAttributes(attributes, "");
			sb.Append(">");
			return sb.ToString();
		}

		public string THEnd()
		{
			return "</th>";
		}

		public string ListHeaderEnd()
		{
			return "</tr>";
		}

		public string ListRowBegin(string cssClass, object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<tr ");
			sb.AppendAttributes(attributes, cssClass);
			sb.Append(">");
			return sb.ToString();
		}

		public string TDBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<td ");
			sb.AppendAttributes(attributes, "");
			sb.Append(">");
			return sb.ToString();
		}

		public string TDEnd()
		{
			return "</td>";
		}

		public string ListRowEnd()
		{
			return "</tr>";
		}

		public string ListTableEnd()
		{
			return "</table>";
		}
	}
}