<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="dbbackup.ascx.cs" Inherits="Nephrite.Web.View.Maintance.dbbackup" %>
<%@ Import Namespace="System.IO" %>
<asp:Button ID="bCreateBackup" runat="server" OnClick="bCreateBackup_Click" Text="Создать бэкап" />

<%=HtmlHelperWSS.ChildListTitleBegin()%>
Бэкапы
<%=HtmlHelperWSS.ChildListTitleEnd()%>
<%=HtmlHelperWSS.ListTableBegin() %>
<%=HtmlHelperWSS.ListHeaderBegin() %>
	<%=HtmlHelperWSS.TH("Название")%>
	<%=HtmlHelperWSS.TH("Действие", "50px")%>
<%=HtmlHelperWSS.ListHeaderEnd() %>
<% Html.Repeater((new DirectoryInfo(BackupDir)).GetFiles().OrderBy(o=>o.CreationTime), "", HtmlHelperWSS.CSSClassAlternating, (o, css) =>
   {  %>
<%=HtmlHelperWSS.ListRowBegin(css)%>
	<%=HtmlHelperWSS.ListCellBegin()%> 
		<a href="DbBackup/<%=o.Name %>" title="Скачать бэкап"><%=o.Name %></a>
	<%=HtmlHelperWSS.ListCellEnd()%>
	<%=HtmlHelperWSS.ListCellBegin("center")%> 
		<%=fileDelete.RenderImageConfirm("delete.png", "Удалить", "Удалить файл?", o.Name) %>
	<%=HtmlHelperWSS.ListCellEnd()%>
<%=HtmlHelperWSS.ListRowEnd() %>
<%}); %>
<%=HtmlHelperWSS.ListTableEnd() %>

<nw:ParameterizedLinkManager ID="fileDelete" runat="server" OnClick="fileDelete_Click" />