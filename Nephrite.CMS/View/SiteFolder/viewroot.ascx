<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="viewroot.ascx.cs" Inherits="Nephrite.CMS.View.SiteFolder_viewroot" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web.Controls" TagPrefix="nw" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.CMS" %>
<%@ Import Namespace="Nephrite.CMS.Controllers" %>
<%@ Import Namespace="System.Collections.Generic" %>

<nw:Toolbar ID="toolbar" runat="server" />

<asp:Panel runat="server" ID="lists" />

<% foreach (var cls in OtherClasses)
   { %>
<div class="tabletitle" style="padding-left:8px"><%=cls.TitlePlural %></div>
<div style="padding:8px">
<%=HtmlHelperWSS.ListTableBegin("100%")%>
<%=HtmlHelperWSS.ListHeaderBegin() %>
<%=HtmlHelperWSS.TH("Название") %>
<%=HtmlHelperWSS.TH("Действие", "50px")%>
<%=HtmlHelperWSS.ListHeaderEnd() %>
<%  List<IMMObject> rootitems = new List<IMMObject>();
    Repository r = new Repository();
    var ot = Nephrite.Metamodel.Model.ObjectTypeRepository.Get(cls.SysName);
    var list = r.GetList(ot).Where(o => !o.IsDeleted);
    
    Html.Repeater(list.OrderBy(o => o.Title).ToList(), "", HtmlHelperWSS.CSSClassAlternating, (o, css) =>
    {%>
<%=HtmlHelperWSS.ListRowBegin(css) %>
<%=HtmlHelperWSS.ListCellBegin() %>
    <a href="<%=o.GetMethodUrl("Edit") %>"><%=o.Title %></a>
<%=HtmlHelperWSS.ListCellEnd() %>
<%=HtmlHelperWSS.ListCellBegin("center")%>
    <a href="<%=o.GetMethodUrl("Delete") %>"><%=Html.Image("delete.png", "Удалить") %></a>
<%=HtmlHelperWSS.ListCellEnd() %>
<%=HtmlHelperWSS.ListRowEnd() %>
<%});%>

<%=HtmlHelperWSS.ListTableEnd() %>
</div>
<%} %>