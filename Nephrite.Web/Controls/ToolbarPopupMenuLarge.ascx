<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolbarPopupMenuLarge.ascx.cs" Inherits="Nephrite.Web.Controls.ToolbarPopupMenuLarge" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<% 
	if (Items.Count > 0)
	{ 
%>
<div class="ms-menubuttoninactivehover" style="cursor: pointer; white-space: nowrap"
	onmouseover="nt_listtoolbar_mouseover(this)" onmouseout="nt_listtoolbar_mouseout(this)"
	onclick="nt_listtoolbar_open(this)">
	<%=Title %><img src="<%=Settings.ImagesPath %>menudark.gif" align="absmiddle" border="0" />
</div>
<div class="ms-MenuUIPopupBody" style="float: left; position: absolute; visibility: hidden; z-index:500;">
	<table class="ms-MenuUILarge" cellpadding="0" cellspacing="0">
		<% 
			foreach (PopupMenuItem item in Items)
			{
				if (item is PopupMenuMethod)
				{
					PopupMenuMethod pmm = item as PopupMenuMethod;
					if (pmm.ShowDisabled || (!String.IsNullOrEmpty(pmm.URL) && pmm.URL != "#") || !String.IsNullOrEmpty(pmm.OnClick))
					{
		%>
		<tr>
			<td class="ms-MenuUIItemTableCell" style="padding: 2px">
				<table cellpadding="0" cellspacing="0" class="ms-MenuUIItemTable" width="100%" onmouseover="<%=pmm.URL == "" ? "" : "nt_listtoolbar_mouseover_tbl(this)" %>"
					onmouseout="nt_listtoolbar_mouseout_tbl(this)" onclick="<%=String.IsNullOrEmpty(pmm.OnClick) ? (pmm.TargetBlank ? ("javascript:window.open('" + pmm.URL + "')") : (pmm.URL == "" ? "" : "javascript:window.location = '" + pmm.URL + "'")) : pmm.OnClick %>">
					<tr>
						<td class="ms-MenuUIIcon" align="center" style="padding: 0px 6px 0px 2px;">
						<% if (String.IsNullOrEmpty(pmm.Image)) { %>
						<img src="<%=Settings.ImagesPath %>blank.gif" style="width: 32px" />
						<% } else { %>
						<img src="<%=Settings.ImagesPath %><%=pmm.Image %>" style="width: 32px" />
						<% } %>

						</td>
						<td class="ms-MenuUILabel" style="padding:2px 16px 3px 6px;">
							<label>
                                <b><span style="<%=String.IsNullOrEmpty(pmm.URL) && String.IsNullOrEmpty(pmm.OnClick) ? "color:gray;" : "cursor:pointer;"%>white-space:nowrap"><%=pmm.Title%></span></b><br />
                                <span class="ms-menuitemdescription" <%=!String.IsNullOrEmpty(pmm.URL) || !String.IsNullOrEmpty(pmm.OnClick) ? "style='cursor:pointer;'" : ""%>><%=pmm.Description%></span>
                            </label>
						</td>
						<td class="ms-MenuUIAccessKey">
						</td>
						<td class="ms-MenuUISubmenuArrow" style="width: 16px">
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<% 
					}
				}
				if (item is PopupMenuSeparator)
				{
		%>
		<tr>
			<td>
				<div class="ms-MenuUISeparatorLarge">
					&nbsp;</div>
			</td>
		</tr>
		<%
				
				}
			}
		%>
	</table>
</div>
<% 
	}
%>
