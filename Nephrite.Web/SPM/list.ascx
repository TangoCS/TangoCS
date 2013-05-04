<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="list.ascx.cs" Inherits="Nephrite.Web.SPM.list" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.SPM" %>

<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" Width="600px" />


<table class="ms-listviewtable" cellpadding="0" cellspacing="0">
<tr class="ms-viewheadertr">
	<%=HtmlHelperWSS.TH(AddSortColumn<SPM_Role, string>("Название", o => o.Title), "100%")%>
	<%=HtmlHelperWSS.TH(AddSortColumn<SPM_Role, string>("Системное имя", o => o.SysName), "100%")%>
	<%=HtmlHelperWSS.TH("Действие")%>
</tr>
<% if (!IsPostBack) Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(ViewData.OrderBy(o => o.Title)))), "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>">
	<td class="ms-vb2" style="white-space:nowrap"><%=Html.ActionLink<SPMController>(c => c.View(o.RoleID), o.Title)%></td>
	<td class="ms-vb2"><%=o.SysName %></td>
	<td class="ms-vb2" align="center">
	<%=Html.ActionImage<SPMController>(c => c.Edit(o.RoleID), "Редактировать", "edit.png")%>
    <%=Html.ActionImage<SPMController>(c => c.Delete(o.RoleID), "Удалить", "delete.gif")%>
	</td>
</tr>
<%}); %>
</table>