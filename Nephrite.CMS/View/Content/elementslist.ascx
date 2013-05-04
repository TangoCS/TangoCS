<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="elementslist.ascx.cs" Inherits="Nephrite.CMS.View.Content_elementslist" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="Nephrite.CMS.Model" %>
<%@ Import Namespace="Nephrite.CMS" %>
<%@ Import Namespace="Nephrite.Metamodel.Controllers" %>


<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" Width="600px" />
<nw:QuickFilter ID="qfilter" runat="server" />

<asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
<ContentTemplate>
<%=Layout.ListTableBegin(new { id = "tList" })%>
<%=Layout.ListHeaderBegin()%>
	<%=Layout.TH("")%>
	<%=Layout.TH("Название")%>
	<%=Layout.TH("Дата создания")%>
	<%=Layout.TH("Дата публикации")%>
	<%=Layout.TH("Действия", new { Style = "width:110px" })%>
<%=Layout.ListHeaderEnd()%>
<%
	bool p = true;
	bool unp = true;
	if (ConfigurationManager.AppSettings["DisableSPM"] == null)
	{
        p = ActionSPMContext.Current.Check("SiteObject.Publish", 1);
        unp = ActionSPMContext.Current.Check("SiteObject.Unpublish", 1);
	}

	 %>
<% Html.Repeater(ApplyPaging(filter.ApplyFilter(qfilter.ApplyFilter(ViewData, SearchExpression))), "", HtmlHelperWSS.CSSClassAlternating, (o, css) =>
   {  %>
<%=Layout.ListRowBegin(css, new {id = o.SiteObjectID.ToString()})%>
	<%=Layout.TDDragHandle("tList")%>
	<%=Layout.TD(Html.ActionUrl(String.Format("?mode={0}&action=Edit&oid={1}&bgroup={2}&returnurl={3}",
	   o.ClassName, o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), enc(o.Title)))%>
	<%=Layout.TD(o.CreateDate.DateToString())%>
	<%=Layout.TD(o.PublishDate.DateToString())%>
	<%=Layout.TDBegin()%>
		<% if (!o.IsPublished) { %>
	   <%=p ? Html.ImageUrl(String.Format("?mode=SiteObject&action=Publish&oid={1}&bgroup={2}&returnurl={3}",
	o.ClassName, o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Опубликовать", "task.png") : ""%>
		<% } else { %>
	   <%=unp ? Html.ImageUrl(String.Format("?mode=SiteObject&action=Unpublish&oid={1}&bgroup={2}&returnurl={3}",
	o.ClassName, o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Снять с публикации", "unapprove.GIF") : ""%>
		<% } %>
	
	   <%=Html.ImageUrl(String.Format("?mode=SiteObject&action=MoveDown&oid={0}&bgroup={1}&returnurl={2}",
	   o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Вверх","arrow_up.png")%>
	   <%=Html.ImageUrl(String.Format("?mode=SiteObject&action=MoveUp&oid={0}&bgroup={1}&returnurl={2}",
	   o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Вниз","arrow_down.png")%>
	   <%=Html.ImageUrl(String.Format("?mode=SiteObject&action=Delete&oid={0}&bgroup={1}&returnurl={2}",
	   o.SiteObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Удалить","delete.gif")%>
	<%=Layout.TDEnd()%>
<%=Layout.ListRowEnd()%>
<%}); %>
<%=Layout.ListTableEnd()%>
<%=RenderPager(PageCount) %>
</ContentTemplate>
<Triggers>
	<asp:AsyncPostBackTrigger ControlID="qfilter" />
</Triggers>
</asp:UpdatePanel>


<div style="margin:10px 0 0 10px">
<nw:BackButton ID="BackButton" runat="server" />
</div>

<nw:TableDnD Type="Nephrite.CMS.Model.SiteObject, Nephrite.CMS" SortDirection="Desc" TableID="tList" runat="server" ID="tList" />