﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;
using System.Text;
using Nephrite.Web.Controls;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Layout
{
	public class MainWSS2007_FixHeight : ILayoutMain
	{
		public string HeaderBegin()
		{
			return @"<tr><td colspan=""2"" style=""height:1px"">";
		}

		public string HeaderEnd()
		{
			return "</td></tr>";
		}

		public string MainBegin()
		{
			return @"<table cellpadding=""0"" cellspacing=""0"" style=""border-collapse:collapse; margin:0; left:0px; top:0px; width:100%; height:100%;position: fixed;//position: absolute; "">";
		}

		public string MainEnd()
		{
			return "</table>";
		}

		public string ContainerBegin()
		{
			return "<tr>";
		}

		public string ContainerEnd()
		{
			return "</tr>";
		}

		public string ContainerFluidBegin()
		{
			return "<tr>";
		}

		public string ContainerFluidEnd()
		{
			return "</tr>";
		}

		public string SidebarBegin()
		{
			return @"<td class=""ms-leftareacell ms-nav"" style=""width: 150px; padding: 0px 3px 0px 2px"">";
		}

		public string SidebarEnd()
		{
			return "</td>";
		}

		public string ContentBegin()
		{
			return @"<td class=""ms-WPBody"" style=""vertical-align: top; border-bottom: solid 1px #6F9DD9; border-left: solid 1px #6F9DD9; border-right: solid 1px #6F9DD9; width:100%"">";
		}

		public string ContentEnd()
		{
			return "</td>";
		}

		public string ContentHeaderBegin()
		{
			return "";
		}

		public string ContentHeaderEnd()
		{
			return "";
		}

		public string ContentBodyBegin()
		{
			return @"<div class=""ms-bodyareacell"" id=""wsdiv"" style=""overflow-x: auto; overflow-y:auto; position: relative; display:block"">";
		}

		public string ContentBodyEnd()
		{
			return "</div>";
		}

		public string FooterBegin()
		{
			throw new NotImplementedException();
		}

		public string FooterEnd()
		{
			throw new NotImplementedException();
		}

		public string GridRowBegin()
		{
			throw new NotImplementedException();
		}

		public string GridRowEnd()
		{
			throw new NotImplementedException();
		}

		public string GridSpanBegin(int value)
		{
			throw new NotImplementedException();
		}

		public string GridSpanEnd()
		{
			throw new NotImplementedException();
		}
	}

	public class MainWSS2007 : ILayoutMain
	{
		public string HeaderBegin()
		{
			return @"<tr><td colspan=""2"" style=""height:1px"">";
		}

		public string HeaderEnd()
		{
			return "</td></tr>";
		}

		public string MainBegin()
		{
			return @"<table cellpadding=""0"" cellspacing=""0"" style=""border-collapse:collapse; margin:0; width:100%;"">";
		}

		public string MainEnd()
		{
			return "</table>";
		}

		public string ContainerBegin()
		{
			return "<tr>";
		}

		public string ContainerEnd()
		{
			return "</tr>";
		}

		public string ContainerFluidBegin()
		{
			return "<tr>";
		}

		public string ContainerFluidEnd()
		{
			return "</tr>";
		}

		public string SidebarBegin()
		{
			return @"<td class=""ms-leftareacell ms-nav"" style=""vertical-align:top; width:220px"">";
		}

		public string SidebarEnd()
		{
			return "</td>";
		}

		public string ContentBegin()
		{
			return @"<td class=""ms-WPBody"" style=""vertical-align: top;"">";
		}

		public string ContentEnd()
		{
			return "</td>";
		}

		public string ContentHeaderBegin()
		{
			return "";
		}

		public string ContentHeaderEnd()
		{
			return "";
		}

		public string ContentBodyBegin()
		{
			return @"<div class=""ms-bodyareacell"" id=""wsdiv"">";
		}

		public string ContentBodyEnd()
		{
			return "</div>";
		}

		public string FooterBegin()
		{
			throw new NotImplementedException();
		}

		public string FooterEnd()
		{
			throw new NotImplementedException();
		}

		public string GridRowBegin()
		{
			throw new NotImplementedException();
		}

		public string GridRowEnd()
		{
			throw new NotImplementedException();
		}

		public string GridSpanBegin(int value)
		{
			throw new NotImplementedException();
		}

		public string GridSpanEnd()
		{
			throw new NotImplementedException();
		}
	}

	public class ListWSS2007 : ILayoutList
	{
		public string ListTableBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table cellpadding=""0"" cellspacing=""0""");
			sb.AppendAttributes(attributes, "ms-listviewtable");
			sb.Append(">");
			return sb.ToString();
		}

		public string ListHeaderBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append("<tr");
			sb.AppendAttributes(attributes, "ms-viewheadertr nodrop nodrag");
			sb.Append(">");
			return sb.ToString();
		}

		public string THBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append("<th");
			sb.AppendAttributes(attributes, "ms-vh2");
			sb.Append(@"><table cellpadding=""0"" cellspacing=""0"" class=""ms-unselectedtitle"" width=""100%""><tr><td class=""ms-vb"" style=""height:23px; vertical-align:middle;"">");
			return sb.ToString();
		}

		public string THEnd()
		{
			return "</td></tr></table></th>";
		}

		public string ListHeaderEnd()
		{
			return "</tr>";
		}

		public string ListRowBegin(string cssClass, object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append("<tr");
			sb.AppendAttributes(attributes, cssClass);
			sb.Append(">");
			return sb.ToString();
		}

		public string TDBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append("<td");
			sb.AppendAttributes(attributes, "ms-vb2");
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

	public class FormWSS2007 : ILayoutForm
	{
		public string FormTableBegin(object attributes)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table cellpadding=""0"" cellspacing=""0""");
			sb.AppendAttributes(attributes, "ms-formtable");
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
			sb.AppendAttributes(labelAttributes == null ? new {style = "width:190px" } : labelAttributes, "ms-formlabel");
			sb.Append(@">");
			sb.Append(title);
			if (required)
			{
				sb.Append("<span"); 
				sb.AppendAttributes(requiredAttributes, "ms-formvalidation");
				sb.Append(@">&nbsp;*</span>");
			}
			if (!String.IsNullOrEmpty(comment))
			{
				sb.Append(@"<div");
				sb.AppendAttributes(commentAttributes == null ? new { style = "font-weight:normal; margin:3px 0 0 1px" } : commentAttributes, "ms-descriptiontext");
				sb.Append(@">");
				sb.Append(comment);
				sb.Append("</div>");
			}
			sb.Append(@"</td><td");
			sb.AppendAttributes(bodyAttributes, "ms-formbody");
			sb.Append(@">");
			return sb.ToString();
		}

		public string FormRowEnd()
		{
			return @"</td></tr>";
		}

		public string FormTableEnd()
		{
			return @"<tr><td class=""ms-formline"" colspan=""2""><IMG height=""1"" src=""" + Settings.ImagesPath + @"blank.gif"" width=""1"" /></td></tr></table>";
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
			sb.AppendAttributes(attributes, "ms-formtoolbar");
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

	public class ListRowDragWSS2007 : ILayoutListRowDrag
	{
		public string TDDragHandle(string tableid, string content)
		{
			return String.Format(@"<td class=""dragHandle ms-vb2"" style=""padding-left:20px"" id=""{0}DragTD"">{1}</td>", tableid, content);
		}
	}

	public class ToolbarWSS2007 : ILayoutToolbar
	{
		public string ToolbarBegin(string cssClass)
		{
			return @"<table class=""" + (cssClass ?? "ms-menutoolbar") + @""" cellpadding=""2"" cellspacing=""0""	border=""0"" style=""height: 23px;width:100%"" id=""toolbarhead""><tr>";
		}
		public string ToolbarBegin(ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign, ILink titleLink)
		{
			return @"<table class=""" + (position == ToolbarPosition.Float ? "ms-toolbar" : "ms-menutoolbar") + @""" cellpadding=""2"" cellspacing=""0""	border=""0"" style=""height: 23px;width:100%"" id=""toolbarhead""><tr>" + (itemsAlign == ToolbarItemsAlign.Right ? ToolbarWhiteSpace() : "");
		}

		public string ToolbarEnd()
		{
			return "</tr></table>";
		}

		public string ToolbarSeparator()
		{
			return @"<td class=""ms-separator"" style=""white-space: nowrap; padding: 3px; border: none""><img alt="""" src=""" + Settings.ImagesPath + @"blank.gif"" /></td>";
		}

		public string ToolbarWhiteSpace()
		{
			return @"<td class=""ms-toolbar"" style=""width: 100%; padding: 3px""></td>";
		}

		public string ToolbarLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			StringBuilder sb = new StringBuilder(255);

			if (!String.IsNullOrEmpty(image))
				sb.AppendFormat(@"<td class=""ms-toolbar"" style=""white-space: nowrap; padding: 3px; border: none; vertical-align:middle""><a {0} href=""{1}"" onclick=""{2}""><img src=""{3}"" class=""middle"" /></a></td>",
					(targetBlank ? @"target=""_blank""" : String.Empty),
					url,
					onclick,
					Settings.ImagesPath + image);

			sb.AppendFormat(@"<td class=""ms-toolbar"" style=""white-space: nowrap; padding: 3px; border: none; vertical-align:middle""><a {0} href=""{1}"" onclick=""{2}"">{3}</a></td>",
					(targetBlank ? @"target=""_blank""" : String.Empty),
					url,
					onclick,
					title);

			return sb.ToString();
		}

		public string ToolbarImageLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			StringBuilder sb = new StringBuilder(255);

			if (!String.IsNullOrEmpty(image))
				sb.AppendFormat(@"<td class=""ms-toolbar"" style=""white-space: nowrap; padding: 3px; border: none; vertical-align:middle""><a {0} href=""{1}"" onclick=""{2}""><img src=""{3}"" class=""middle"" /></a></td>",
					(targetBlank ? @"target=""_blank""" : String.Empty),
					url,
					onclick,
					Settings.ImagesPath + image);

			return sb.ToString();
		}

		public string ToolbarItem(string content)
		{
			return String.Format(@"<td class=""ms-toolbar"" style=""border: none;"">{0}</td>", content);
		}


		public void ToolbarItem(HtmlTextWriter writer, IBarItem content)
		{
			writer.Write(@"<td class=""ms-toolbar"" style=""border: none;"">");
			content.Layout = AppWeb.Layout.ToolbarButton;
			content.RenderControl(writer);
			writer.Write("</td>");
		}
	}

	public class MessageWSS2007 : ILayoutMessage
	{
		public string ExclamationMessage(string message, object attributes)
		{
			if (String.IsNullOrEmpty(message))
				return "";

			StringBuilder sb = new StringBuilder(150);
			sb.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""0""");
			sb.AppendAttributes(attributes, "ms-informationbar");
			sb.AppendFormat(@"><tr><td width=""10"" valign=""center"" style=""padding: 4px;""><img alt="""" src=""/_layouts/images/exclaim.gif""/></td><td>{0}</td></tr></table>", message);
			return sb.ToString();
		}

		public string CustomMessage(string message, string image, object attributes)
		{
			if (String.IsNullOrEmpty(message))
				return "";

			StringBuilder sb = new StringBuilder(150);
			sb.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""0""");
			sb.AppendAttributes(attributes, "ms-informationbar");
			sb.Append(@"><tr><td width=""10"" valign=""center"" style=""padding: 4px; vertical-align:middle;"">");
			sb.Append(AppWeb.Layout.Image(image, ""));
			sb.AppendFormat(@"</td><td style=""vertical-align:middle;"">{0}</td></tr></table>", message);
			return sb.ToString();
		}

		public string InformationMessage(string message, object attributes)
		{
			throw new NotImplementedException();
		}

		public string ErrorMessage(string message, object attributes)
		{
			throw new NotImplementedException();
		}
	}

	public class PagingWSS2007 : ILayoutPaging
	{
		public string RenderPager(Url baseUrl, int pageIndex, int pageCount, int recordsCount)
		{
			StringBuilder sb = new StringBuilder(1024);

			sb.Append(@"<div style=""padding:3px 0px 8px 7px; vertical-align:middle"">");
			if (pageCount > 1)
			{
				string s = @"<a href=""{0}""><img src=""{1}{2}page.png"" alt=""" + TextResource.Get("Common.Paging.Page", Properties.Resources.PagerPage) + @" {3}"" style=""border:0;"" /></a>&nbsp;";
				if (pageIndex > 2)
				{
					baseUrl = baseUrl.SetParameter("page", "1");
					sb.AppendFormat(s, baseUrl, Settings.ImagesPath, "first", 1);
				}
				if (pageIndex > 1)
				{
					baseUrl = baseUrl.SetParameter("page", (pageIndex - 1).ToString());
					sb.AppendFormat(s, baseUrl, Settings.ImagesPath, "prev", pageIndex - 1);
				}
				sb.AppendFormat(@"{0}&nbsp;<input name=""page"" type=""text"" value=""{1}"" style=""width:40px;"" onkeydown=""javascript:if(event.keyCode==13){{ document.location='{2}&page='+document.forms[0].page.value; return false;}}""/>&nbsp;{4}&nbsp;{3}&nbsp;", TextResource.Get("Common.Paging.Page", Properties.Resources.PagerPage), pageIndex, baseUrl.RemoveParameter("page"), pageCount, TextResource.Get("Common.Paging.From", "из"));
				if (pageIndex < pageCount)
				{
					baseUrl = baseUrl.SetParameter("page", (pageIndex + 1).ToString());
					sb.AppendFormat(s, baseUrl, Settings.ImagesPath, "next", pageIndex + 1);
				}
				if (pageIndex < pageCount - 1)
				{
					baseUrl = baseUrl.SetParameter("page", pageCount.ToString());
					sb.AppendFormat(s, baseUrl, Settings.ImagesPath, "last", pageCount);
				}
			}
			sb.AppendFormat(@"<b>{0}:</b> {1}</div>", TextResource.Get("Common.Paging.TotalRecords", "Всего записей"), recordsCount);
			return sb.ToString();
		}

		public string RenderPager(string gotoPageJSFunction, int pageIndex, int pageCount, int recordsCount)
		{
			StringBuilder sb = new StringBuilder(1024);
			string imgname = "pagerBusy_" + gotoPageJSFunction;
			gotoPageJSFunction = String.Format("{0}.style.visibility = 'visible';{1}({2}); return false;", imgname, gotoPageJSFunction, "{0}");


			sb.Append(@"<div style=""padding:3px 0px 8px 7px; vertical-align:middle"">");
			if (pageCount > 1)
			{
				string s = @"<a href=""#"" onclick=""{0}""><img src=""{1}{2}page.png"" alt=""" + TextResource.Get("Common.Paging.Page", Properties.Resources.PagerPage) + @" {3}"" style=""border:0;"" /></a>&nbsp;";
				if (pageIndex > 2)
				{
					sb.AppendFormat(s, String.Format(gotoPageJSFunction, 1), Settings.ImagesPath, "first", 1);
				}
				if (pageIndex > 1)
				{
					sb.AppendFormat(s, String.Format(gotoPageJSFunction, pageIndex - 1), Settings.ImagesPath, "prev", pageIndex - 1);
				}
				sb.AppendFormat(@"{0}&nbsp;<input name=""page"" type=""text"" value=""{1}"" style=""width:40px;"" onkeydown=""javascript:if(event.keyCode==13){{ {2} }}""/>&nbsp;{4}&nbsp;{3}&nbsp;", TextResource.Get("Common.Paging.Page", Properties.Resources.PagerPage), pageIndex, String.Format(gotoPageJSFunction, "document.forms[0].page.value"), pageCount, TextResource.Get("Common.Paging.From", "из"));
				if (pageIndex < pageCount)
				{
					sb.AppendFormat(s, String.Format(gotoPageJSFunction, pageIndex + 1), Settings.ImagesPath, "next", pageIndex + 1);
				}
				if (pageIndex < pageCount - 1)
				{
					sb.AppendFormat(s, String.Format(gotoPageJSFunction, pageCount), Settings.ImagesPath, "last", pageCount);
				}
			}

			sb.AppendFormat(@"<img src=""{0}Wait_Animate.gif"" name=""{1}"" style=""visibility:hidden;border:0;"" class=""middle"" title=""{2}"" />", Settings.ImagesPath, imgname, TextResource.Get("Common.Paging.Wait", "ждите..."));
			sb.AppendFormat(@"<b>{0}:</b> {1}</div>", TextResource.Get("Common.Paging.TotalRecords", "Всего записей"), recordsCount);
			return sb.ToString();
		}
	}

	public class PopupMenuWSS2007Compact : ILayoutPopupMenu
	{
		public string PopupMenuBegin()
		{
			return @"<div class=""ms-menubuttoninactivehover"" style=""cursor: pointer; white-space: nowrap"" onmouseover=""nt_listtoolbar_mouseover(this)"" onmouseout=""nt_listtoolbar_mouseout(this)"" onclick=""nt_listtoolbar_open(this)"">";
		}

		public string PopupMenuBodyBegin()
		{
			return @"<div class=""ms-MenuUIPopupBody"" style=""float: left; position: absolute; visibility: hidden; display:none; z-index:100"">	<table class=""ms-MenuUI"" cellpadding=""0"" cellspacing=""0"">";
		}

		public string PopupMenuBodyEnd()
		{
			return "</table></div>";
		}

		public string PopupMenuLink(ILink link)
		{
			string onClick = String.IsNullOrEmpty(link.OnClick) ? (link.TargetBlank ? ("javascript:window.open('" + link.Href + "')") : "javascript:window.location = '" + link.Href + "'") : link.OnClick;
			string accessKey = link.AccessKey.IsEmpty() ? "" : String.Format(@"<input style=""position:absolute; top:-300px; left:-300px"" type=""button"" accesskey=""{0}"" onfocus=""{1}"" />", link.AccessKey.ToUpper(), onClick);
			string img = String.IsNullOrEmpty(link.Image) ?
				String.Format(@"<img src=""{0}blank.gif"" style=""width: 16px"" />", Settings.ImagesPath) :
				String.Format(@"<img src=""{0}{1}"" style=""width: 16px"" />", Settings.ImagesPath, link.Image);

			return String.Format(@"<tr><td class=""ms-MenuUIItemTableCellCompact"" style=""padding: 2px"">
				<table cellpadding=""0"" cellspacing=""0"" class=""ms-MenuUIItemTable"" width=""100%"" onmouseover=""nt_listtoolbar_mouseover_tbl(this)"" onmouseout=""nt_listtoolbar_mouseout_tbl(this)"" onclick=""{2}"">
					<tr>
						<td class=""ms-MenuUIIcon"" align=""center"" style=""padding: 0px 6px 0px 2px;"">{0}</td>
						<td class=""ms-MenuUILabelCompact"" style=""padding: 2px 10px 3px 6px;"">
							<label><div style=""white-space: nowrap"">{3}</div></label>
						</td>
						<td class=""ms-MenuUIAccessKey"">{1}</td>
						<td class=""ms-MenuUISubmenuArrow"" style=""width: 16px""></td>
					</tr>
				</table>
			</td></tr>", img, accessKey, onClick, link.Title);
		}

		public string PopupMenuSeparator()
		{
			return @"<tr><td><div class=""ms-MenuUISeparator"">&nbsp;</div></td></tr>";
		}

		public string PopupMenuEnd()
		{
			return String.Format(@"<img src=""{0}menudark.gif"" align=""absmiddle"" border=""0"" /></div>", Settings.ImagesPath);
		}
	}

	public class PopupMenuWSS2007Large : ILayoutPopupMenu
	{
		public string PopupMenuBegin()
		{
			return @"<div class=""ms-menubuttoninactivehover"" style=""cursor: pointer; white-space: nowrap"" onmouseover=""nt_listtoolbar_mouseover(this)"" onmouseout=""nt_listtoolbar_mouseout(this)"" onclick=""nt_listtoolbar_open(this)"">";
		}

		public string PopupMenuBodyBegin()
		{
			return @"<div class=""ms-MenuUIPopupBody"" style=""float: left; position: absolute; visibility: hidden; display:none; z-index:100"">	<table class=""ms-MenuUILarge"" cellpadding=""0"" cellspacing=""0"">";
		}

		public string PopupMenuBodyEnd()
		{
			return "</table></div>";
		}

		public string PopupMenuLink(ILink link)
		{
			string onClick = String.IsNullOrEmpty(link.OnClick) ? (link.TargetBlank ? ("javascript:window.open('" + link.Href + "')") : "javascript:window.location = '" + link.Href + "'") : link.OnClick;
			string accessKey = link.AccessKey.IsEmpty() ? "" : String.Format(@"<input style=""position:absolute; top:-300px; left:-300px"" type=""button"" accesskey=""{0}"" onfocus=""{1}"" />", link.AccessKey.ToUpper(), onClick);
			string img = String.IsNullOrEmpty(link.Image) ?
				String.Format(@"<img src=""{0}blank.gif"" style=""width: 32px"" />", Settings.ImagesPath) :
				String.Format(@"<img src=""{0}{1}"" style=""width: 32px"" />", Settings.ImagesPath, link.Image);

			string style1 = String.IsNullOrEmpty(link.Href) && String.IsNullOrEmpty(link.OnClick) ? "color:gray;" : "cursor:pointer;";
			string style2 = !String.IsNullOrEmpty(link.Href) || !String.IsNullOrEmpty(link.OnClick) ? "style='cursor:pointer;'" : "";

			return String.Format(@"<tr><td class=""ms-MenuUIItemTableCell"" style=""padding: 2px"">
				<table cellpadding=""0"" cellspacing=""0"" class=""ms-MenuUIItemTable"" width=""100%"" onmouseover=""nt_listtoolbar_mouseover_tbl(this)"" onmouseout=""nt_listtoolbar_mouseout_tbl(this)"" onclick=""{2}"">
					<tr>
						<td class=""ms-MenuUIIcon"" align=""center"" style=""padding: 0px 6px 0px 2px;"">{0}</td>
						<td class=""ms-MenuUILabel"" style=""padding:2px 16px 3px 6px;"">
							<label>
                                <b><span style=""{5}white-space:nowrap"">{3}</span></b><br />
                                <span class=""ms-menuitemdescription"" {6}>{4}</span>
                            </label>
						</td>
						<td class=""ms-MenuUIAccessKey"">{1}</td>
						<td class=""ms-MenuUISubmenuArrow"" style=""width: 16px""></td>
					</tr>
				</table>
			</td></tr>", img, accessKey, onClick, link.Title, link.Description, style1, style2);
		}

		public string PopupMenuSeparator()
		{
			return @"<tr><td><div class=""ms-MenuUISeparatorLarge"">&nbsp;</div></td></tr>";
		}

		public string PopupMenuEnd()
		{
			return String.Format(@"<img src=""{0}menudark.gif"" align=""absmiddle"" border=""0"" /></div>", Settings.ImagesPath);
		}
	}

	public class AutoMarginWSS2007 : ILayoutAutoMargin
	{
		public string MarginBegin()
		{
			return "<table style='width:100%'><tr><td style='padding:8px'>";
		}

		public string MarginEnd()
		{
			return "</td></tr></table>";
		}
	}

	public class ButtonsBarWSS2007 : ILayoutToolbar
	{

		public string ToolbarBegin(string cssClass)
		{
			throw new NotImplementedException();
		}

		public string ToolbarBegin(ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign, ILink titleLink)
		{
			StringBuilder sb = new StringBuilder(255);
			sb.Append(@"<table");
			sb.AppendAttributes(null, "ms-formtoolbar");
			sb.Append("><tr>");
			if ((itemsAlign ?? ToolbarItemsAlign.Right) == ToolbarItemsAlign.Right) sb.Append(ToolbarWhiteSpace());
			return sb.ToString();
		}

		public string ToolbarEnd()
		{
			return @"</tr></table>";
		}

		public string ToolbarSeparator()
		{
			return "";
		}

		public string ToolbarWhiteSpace()
		{
			return @"<td style=""width:100%""></td>";
		}

		public string ToolbarLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			throw new NotImplementedException();
		}

		public string ToolbarImageLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			throw new NotImplementedException();
		}

		public string ToolbarItem(string content)
		{
			throw new NotImplementedException();
		}

		public void ToolbarItem(HtmlTextWriter writer, IBarItem content)
		{
			writer.Write(@"<td style=""vertical-align:middle"">");
			content.Layout = AppWeb.Layout.Button;
			content.RenderControl(writer);
			writer.Write("</td>");
		}


		public ILayoutBarItem LayoutBarItem {get; set;}


		public void InitPosition(Control toolbar, ToolbarPosition? position)
		{
			
		}
	}

	public class ButtonWSS2007 : ILayoutBarItem
	{
		public string CssClass
		{
			get { return "ms-ButtonHeightWidth"; }
		}

		public string Style(string image)
		{
			return "";
		}
	}

	public class ToolbarButtonWSS2007 : ILayoutBarItem
	{
		public string CssClass
		{
			get { return "ms-viewtoolbar"; }
		}

		public string Style(string image)
		{
			return "border-style:none;background-color:transparent;cursor:hand;";
		}
	}
	
}