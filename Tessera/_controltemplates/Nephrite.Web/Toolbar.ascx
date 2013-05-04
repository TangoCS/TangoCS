<%@ Control Language="C#" AutoEventWireup="true" Inherits="Nephrite.Web.Controls.Toolbar"
	CodeBehind="Toolbar.ascx.cs" %>
<%@ Import Namespace="System.IO" %>

<%@ Import Namespace="Nephrite.Web" %>
<% if (items.Count > 0 && items[0] is ToolbarItemSeparator) items.RemoveAt(0); %>
<% if (items.Count > 0 && items[items.Count - 1] is ToolbarItemSeparator) items.RemoveAt(items.Count - 1); %>
<% 
	if (items.Count > 0 || rightItems.Count > 0)
	{ 
%>
<%
	if (TableCssClass == null)
	{
%>
<table class="ms-listdescription" style="border-collapse: collapse; width:100%" id="toolbarhead">
	<tr>
		<td>
		</td>
	</tr>
</table>
<%
	}
%>
<table class="<%=TableCssClass??"ms-menutoolbar" %>" cellpadding="2" cellspacing="0"
	border="0" style="height: 23px;">
	<tr>
		<% 
			foreach (ToolbarItem item in items)
			{
				if (item is ToolbarItemSeparator)
				{ 
		%>
		<td class="ms-separator" style="white-space: nowrap; padding: 3px; border: none">
			<img alt="" src="/_layouts/images/blank.gif" />
		</td>
		<% 
			    }
		%>
		
		<% 
			    if (item is ToolbarItemMethod)
			    {
				    ToolbarItemMethod im = item as ToolbarItemMethod;
		%>
		<% if (!String.IsNullOrEmpty(im.Image)) { %>
		<td class="ms-toolbar" style="white-space: nowrap; padding: 3px; border: none; vertical-align:middle">
			<a <%=im.TargetBlank ? @"target=""_blank""" : String.Empty %> href="<%=im.Url %>"
				onclick="<%=im.OnClick%>"><img src="<%=Settings.ImagesPath %><%=im.Image %>" class="middle" /></a>
		</td>
		<% } %>
		<td class="ms-toolbar" style="white-space: nowrap; padding: 3px; border: none; vertical-align:middle">
			<a <%=im.TargetBlank ? @"target=""_blank""" : String.Empty %> href="<%=im.Url %>"
				onclick="<%=im.OnClick%>"><%=im.Title%></a>
		</td>
		<%  
			    }

			    if (item is ToolbarItemPopupMenu)
			    {
				    ToolbarItemPopupMenu ipm = item as ToolbarItemPopupMenu;

				    HtmlTextWriter hw = new HtmlTextWriter(new StringWriter());
				    ipm.Control.RenderControl(hw);
		%>
		<td class="ms-toolbar">
			<%=hw.InnerWriter.ToString() %>
		</td>
		<%
			    }
		    }
		%>
		<td class="ms-toolbar" style="width: 100%; padding: 3px">
		</td>
		<% 
			foreach (ToolbarItem item in rightItems)
			{ %>
		<td class="ms-toolbar" style="width: 100%;">
			<%
				if (item is ToolbarItemText)
				{
					ToolbarItemText it = item as ToolbarItemText;
			%>
			<%=it.Title %>
			<%
				} 
			%>
			<%  if (item is ToolbarItemSeparator)
				{ 
		%>
		<td class="ms-separator" style="white-space: nowrap; padding: 3px; border: none">
			<img alt="" src="/_layouts/images/blank.gif" />
		</td>
		<% 
			}
		%>
			<%
				if (item is ToolbarItemPopupMenu)
				{
					ToolbarItemPopupMenu ipm = item as ToolbarItemPopupMenu;

					HtmlTextWriter hw = new HtmlTextWriter(new StringWriter());
					ipm.Control.RenderControl(hw);
			%>
			<td class="ms-toolbar">
				<%=hw.InnerWriter.ToString() %>
			</td>
			<%
				} 
			%>
		</td>
		<% 
			}
		%>
	</tr>
</table>
<% 
	}
%>