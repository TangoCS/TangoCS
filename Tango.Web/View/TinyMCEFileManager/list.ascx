<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="list.ascx.cs" Inherits="Nephrite.Web.View.TinyMCEFileManager_list" %>

<nw:Toolbar ID="toolbar" runat="server" />

<% string u = "javascript:window.opener.returnToOpener('{0}');window.close();"; %>
<% string ashx = Query.GetString("type") == "image" ? "data.ashx" : "file.ashx"; %>

<%=Layout.ListTableBegin(new { style = "width:690px" })%>
<%=Layout.ListHeaderBegin()%>
<%=Layout.TH("")%>
<%=Layout.TH(AddSortColumn<IStorageFile, string>("Наименование", o => o.Name))%>
<%=Layout.TH(AddSortColumn<IStorageFile, long>("Размер", o => o.Length))%>
<%=Layout.TH(AddSortColumn<IStorageFile, DateTime>("Дата последнего изменения", o => o.LastModifiedDate))%>
<%=Layout.ListHeaderEnd()%>

<% Html.Repeater(_folders, "", HtmlHelperWSS.CSSClassAlternating, (o, css) =>
   {  %>
<%=Layout.ListRowBegin()%>
<%=Layout.TD(Html.Image("b_mailfolder.gif","Папка"))%>
<%=Layout.TDBegin(new { colspan = "3"})%>
<a href="<%=Query.RemoveParameter("parent", "op").AddQueryParameter("parent", o.ID.ToString())%>"><%=enc(o.Name)%></a>
<%=Layout.TDEnd()%>
<%=Layout.ListRowEnd()%>
<%}); %>


<% Html.Repeater(ApplyPaging(ApplyOrderBy(_files)), "", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>
<%=Layout.ListRowBegin()%>
<%=Layout.TDBegin()%>
<img src="/_layouts/images/<%=FileTypeIcon.GetImageName(o.Extension)%>" />
<%=Layout.TDEnd()%>
<%=Layout.TD("<a href=\"" + String.Format(u, "/" + ashx + "?guid=" + o.ID.ToString()) + "\">" + enc(o.Title) + "</a>")%>
<%=Layout.TD(o.Length.ToString())%>
<%=Layout.TD(o.LastModifiedDate.DateTimeToString())%>
<%=Layout.ListRowEnd()%>
<%}); %>


<%=Layout.ListTableEnd()%>
<%=RenderPager(PageCount) %>