<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="list.ascx.cs" Inherits="Nephrite.CMS.View.Content_list" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="Nephrite.CMS.Model" %>
<%@ Import Namespace="Nephrite.CMS" %>
<%@ Import Namespace="Nephrite.CMS.Controllers" %>
<%@ Import Namespace="Nephrite.Metamodel.Controllers" %>

<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" Width="600px" />
<nw:QuickFilter ID="qfilter" runat="server" />

<asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
<ContentTemplate>
<%=Layout.ListTableBegin()%>
<%=Layout.ListHeaderBegin()%>
	
	<%=Layout.TH(AddSortColumn<IMM_ModelElement, string>("Название", o => o.Title))%>
	<%=Layout.TH("Кол-во объектов")%>
	<%=Layout.TH("Полный путь")%>
	<%=Layout.TH("Действия")%>
<%=Layout.ListHeaderEnd()%>
<% Nephrite.CMS.Model.MM_ObjectType t = AppCMS.DataContext.MM_ObjectTypes.Single(o => o.ObjectTypeID == Query.GetInt("classid", 0)); %>

<% Html.Repeater(ApplyPaging(filter.ApplyFilter(qfilter.ApplyFilter(ViewData.OrderBy(o => o.Title), SearchExpression))), "", HtmlHelperWSS.CSSClassAlternating, (o, css) =>
   {  %>
<%=Layout.ListRowBegin(css)%>
	
	<%=Layout.TD(Html.ActionLink<ContentController>(cc => cc.ElementsList(o.SiteSectionID, Query.CreateReturnUrl()), o.Title))%>
	<%=Layout.TD(AppCMS.DataContext.SiteObjects.Count(so => so.ParentID == o.SiteSectionID && so.ClassName == t.SysName).ToString())%>
	<%=Layout.TD(enc(o.FullPath))%>
	<%=Layout.TDBegin()%>
	<%=Html.ImageUrl(String.Format("?mode={0}&action=CreateNew&parent={1}&bgroup={2}&returnurl={3}",
		_c.SysName, o.ObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()), "Создать", "add.png")%>
	<%=Html.ImageUrl(String.Format("?mode=SiteSection&action=Edit&oid={2}&bgroup={0}&returnurl={1}",
		Query.GetString("bgroup"), Query.CreateReturnUrl(), o.SiteSectionID.ToString()), "Редактировать", "edit.png")%>
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

