<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="list.ascx.cs" Inherits="Nephrite.Metamodel.View.Triggers_list" %>
<%@ Import Namespace="Microsoft.SqlServer.Management.Smo" %>
<%@ Import Namespace="Nephrite.Metamodel.Controllers" %>
<nw:Toolbar ID="toolbar" runat="server" />
<nw:Filter ID="filter" runat="server" Width="600px" />

<asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
<ContentTemplate>
<%=Layout.ListTableBegin() %>
<%=Layout.ListHeaderBegin() %>
<%=Layout.TH("№")%>
<%=Layout.TH(AddSortColumn<Trigger, string>("Таблица", o => ((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name))%>
<%=Layout.TH(AddSortColumn<Trigger, string>("Триггер", o => o.Name))%>
<%=Layout.TH(AddSortColumn<Trigger, bool>("А", o => o.IsEnabled))%>
<%=Layout.TH(AddSortColumn<Trigger, bool>("U", o => o.Update))%>
<%=Layout.TH(AddSortColumn<Trigger, bool>("I", o => o.Insert))%>
<%=Layout.TH(AddSortColumn<Trigger, bool>("D", o => o.Delete))%>
<%=Layout.TH("Действия")%>
<%=Layout.ListHeaderEnd() %>
<% int index = (PageIndex - 1) * Nephrite.Web.Settings.PageSize;%>
<% Html.Repeater(ApplyPaging(ApplyOrderBy(filter.ApplyFilter(hfQuickFilter.Value.Trim() != String.Empty ? viewData.Where(SearchExpression(hfQuickFilter.Value.Trim())) : viewData))), "", HtmlHelperWSS.CSSClassAlternating, (o, css) => {  %>
<%=Layout.ListRowBegin(css) %>
<% index++;%>
<%=Layout.TD(index.ToString())%>
<%=Layout.TD(((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name)%>
<%=Layout.TD(Html.ActionLink<TriggersController>(c => c.Edit(((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name, o.Name, Query.CreateReturnUrl()), o.Name))%>
<%=Layout.TD(o.IsEnabled.Icon())%>
<%=Layout.TD(o.Update.Icon())%>
<%=Layout.TD(o.Insert.Icon())%>
<%=Layout.TD(o.Delete.Icon())%>
<%=Layout.TDBegin(new {width="center"})%>
<%=Html.ActionImage<TriggersController>(c => c.Delete(((Microsoft.SqlServer.Management.Smo.Table)o.Parent).Name, o.Name, Query.CreateReturnUrl()), "Удалить", "delete.gif")%>
<%=Layout.TDEnd()%>
<%=Layout.ListRowEnd() %>
<%}); %>
<%=HtmlHelperWSS.ListTableEnd() %>
<%=RenderPager(PageCount) %>

<asp:HiddenField runat="server" ID="hfQuickFilter" />
</ContentTemplate>
<Triggers>
	<asp:AsyncPostBackTrigger ControlID="lbRefresh" />
</Triggers>
</asp:UpdatePanel>
<asp:LinkButton ID="lbRefresh" runat="server" Text="" />

<script type="text/javascript">
var timer = 0;
function filter()
{
	if(timer)
	{
		window.clearTimeout(timer);
		timer = null;
	}
	timer = window.setTimeout(runfilter, 400);
}
function runfilter()
{
	document.getElementById('<%=hfQuickFilter.ClientID%>').value = document.getElementById('qfind').value;
	<%=Page.ClientScript.GetPostBackEventReference(lbRefresh, "") %>
}
</script>