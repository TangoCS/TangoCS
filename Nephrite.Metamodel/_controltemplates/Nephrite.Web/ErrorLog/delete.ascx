<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="delete.ascx.cs" Inherits="View_ErrorLog_delete" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Data.Linq" %>

<%=Layout.FormTableBegin("700px")%>
<%=Layout.FormRowBegin("Удалить все ошибки ранее", true)%>
<nw:JSCalendar ID="calDate" runat="server" />
<%=Layout.FormRowEnd()%>
<%=Layout.FormTableEnd() %>


<asp:Label ID="lMsg" runat="server" Text="" ForeColor="Red" />
<%=HtmlHelperWSS.FormToolbarBegin() %>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>


<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
	SetTitle("Очистка журнала ошибок");
	lMsg.Text = "";

	if (!IsPostBack)
	{
		calDate.Date = DateTime.Today.AddMonths(-3);
	}


}

protected void bOK_Click(object sender, EventArgs e)
{
	ErrorLogger.DataContext.ExecuteCommand("delete from ErrorLog where ErrorDate < {0}", calDate.Date);
	Query.RedirectBack();
}
</script>
