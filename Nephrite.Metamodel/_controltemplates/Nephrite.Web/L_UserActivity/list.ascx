<%@ Control Language="C#" AutoEventWireup="True" Inherits="View_L_UserActivity_list" Codebehind="list.ascx.cs" %>
<%@ Import Namespace="System.Linq" %>


<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" />

<table class="ms-listviewtable" cellpadding="0" cellspacing="0">
<tr class="ms-viewheadertr">
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	<%=AddSortColumn<L_UserActivity, string>("Пользователь", o => o.UserID)%>
	</td></tr></table>
	</th>
	
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	<%=AddSortColumn<L_UserActivity, DateTime?>("Дата операции", o => o.OperationDate)%>
	</td></tr></table>
	</th>
	
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	<%=AddSortColumn<L_UserActivity, string>("Операция", o => o.Operation)%>
	</td></tr></table>
	</th>
	
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	<%=AddSortColumn<L_UserActivity, string>("Тип объекта", o => o.ClassName)%>
	</td></tr></table>
	</th>
	
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	<%=AddSortColumn<L_UserActivity, int>("Идентификатор объекта", o => o.ObjectID.Value)%>
	</td></tr></table>
	</th>
	
	
	
	
</tr>

<% Html.Repeater<L_UserActivity>(ApplyPaging<L_UserActivity>(ApplyOrderBy<L_UserActivity>(filter.ApplyFilter<L_UserActivity>(ViewData))), "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>">
	<td class="ms-vb2"><%=o.UserID%></td>
	<td class="ms-vb2"><%=o.OperationDate.DateToString() + " " + o.OperationDate.TimeToString()%>
	<td class="ms-vb2"><%=o.Operation%>
	<td class="ms-vb2"><%=o.ClassName%>
	<td class="ms-vb2"><%=o.ObjectID%>
</tr>
<%}); %>
</table>
<%=RenderPager(PageCount) %>