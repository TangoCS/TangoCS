<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolbarPopupMenuCompact.ascx.cs"
	Inherits="Nephrite.Web.Controls.ToolbarPopupMenuCompact" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<% 
	if (Items != null && Items.Count > 0)
	{ 
%>
<div class="ms-menubuttoninactivehover" style="cursor: pointer; white-space: nowrap"
	onmouseover="nt_listtoolbar_mouseover(this)" onmouseout="nt_listtoolbar_mouseout(this)"
	onclick="nt_listtoolbar_open(this)">
	<%=Title %><img src="<%=Settings.ImagesPath %>menudark.gif" align="absmiddle" border="0" />
</div>
<div class="ms-MenuUIPopupBody" style="float: left; position: absolute; visibility: hidden; display:none; z-index:100">
	<table class="ms-MenuUI" cellpadding="0" cellspacing="0">
		<% 
			foreach (PopupMenuItem item in Items)
			{
				if (item is PopupMenuMethod)
				{
					PopupMenuMethod pmm = item as PopupMenuMethod;
					if ((!String.IsNullOrEmpty(pmm.URL) && pmm.URL != "#") || !String.IsNullOrEmpty(pmm.OnClick))
					{
		%>
		<tr>
			<td class="ms-MenuUIItemTableCellCompact" style="padding: 2px">
				<table cellpadding="0" cellspacing="0" class="ms-MenuUIItemTable" width="100%" onmouseover="nt_listtoolbar_mouseover_tbl(this)"
					onmouseout="nt_listtoolbar_mouseout_tbl(this)" onclick="<%=String.IsNullOrEmpty(pmm.OnClick) ? (pmm.TargetBlank ? ("javascript:window.open('" + pmm.URL + "')") : "javascript:window.location = '" + pmm.URL + "'") : pmm.OnClick %>">
					<tr>
						<td class="ms-MenuUIIcon" align="center" style="padding: 0px 6px 0px 2px;">
						<% if (String.IsNullOrEmpty(pmm.Image)) { %>
						<img src="<%=Settings.ImagesPath %>blank.gif" style="width: 16px" />
						<% } else { %>
						<img src="<%=Settings.ImagesPath %><%=pmm.Image %>" style="width: 16px" />
						<% } %>
						</td>
						<td class="ms-MenuUILabelCompact" style="padding: 2px 10px 3px 6px;">
							<label>
								<div style="white-space: nowrap">
									<%=pmm.Title%>
								</div>
							</label>
						</td>
						<td class="ms-MenuUIAccessKey">
							<%=pmm.AccessKey.IsEmpty() ? "" : pmm.AccessKey.ToUpper() %>
							<%if (!pmm.AccessKey.IsEmpty())
							{ %>
							<input style="position:absolute; top:-300px; left:-300px" type="button" accesskey="<%=pmm.AccessKey %>" onfocus="<%=String.IsNullOrEmpty(pmm.OnClick) ? (pmm.TargetBlank ? ("javascript:window.open('" + pmm.URL + "')") : "javascript:window.location = '" + pmm.URL + "'") : pmm.OnClick %>" />
							<%} %>
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
				<div class="ms-MenuUISeparator">
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
