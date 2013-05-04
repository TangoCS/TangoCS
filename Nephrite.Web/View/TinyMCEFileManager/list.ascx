<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="list.ascx.cs" Inherits="Nephrite.Web.View.TinyMCEFileManager_list" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Model" %>
<%@ Import Namespace="Nephrite.Web.Controllers" %>
<%@ Import Namespace="Nephrite.Web.FileStorage" %>
<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" Width="600px" />

<% string u = "javascript:window.opener.returnToOpener('{0}');window.close();"; %>
<% string ashx = Query.GetString("type") == "image" ? "data.ashx" : "file.ashx"; %>

<%=Layout.ListTableBegin(new { style = "width:690px" })%>
<%=Layout.ListHeaderBegin()%>
<%=Layout.TH("")%>
<%=Layout.TH(AddSortColumn<IDbItem, string>("Наименование", o => o.Title))%>
<%=Layout.TH("Размер")%>
<%=Layout.TH(AddSortColumn<IDbItem, DateTime>("Дата последнего изменения", o => o.LastModifiedDate))%>
<%=Layout.TH(AddSortColumn<IDbItem, string>("Последний редактировавший пользователь", o => o.LastModifiedUserName))%>
<%=Layout.ListHeaderEnd()%>
<% Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(_data))), "", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>
<%=Layout.ListRowBegin(o.IsDeleted ? "deletedItem" : css)%>
<%=Layout.TDBegin()%>
<%if(o.Extension == null){%>
<%=Html.Image("b_mailfolder.gif","Папка")%>
<%} else {%>
<img src="/_layouts/images/<%=FileTypeIcon.GetImageName(o.Extension)%>" />
<%}%>
<%=Layout.TDEnd()%>
<%=Layout.TD("<a href=\"" + (o.Type == DbItemType.File ? String.Format(u, "/" + ashx + "?guid=" + o.ID.ToString()) : Query.RemoveParameter("parent", "op").AddQueryParameter("parent", o.ID.ToString())) + "\">" + enc(o.Title) + "</a>")%>
<%=Layout.TDBegin(new { style = "text-align:right" })%>
<%=o.Size%>
<%=Layout.TDEnd()%>
<%=Layout.TD(o.LastModifiedDate.DateTimeToString())%>
<%=Layout.TD(enc(o.LastModifiedUserName))%>
<%=Layout.ListRowEnd()%>
<%}); %>
<%=Layout.ListTableEnd()%>
<%=RenderPager(PageCount) %>