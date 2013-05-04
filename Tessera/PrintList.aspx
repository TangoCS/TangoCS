<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=HttpContext.Current.Items["title"] %></title>
	<link rel="Stylesheet" type="text/css" href="/css/PrintList.css" />
</head>
<body onload="window.print()">
    <form id="form1" runat="server">
	<asp:ScriptManager runat="server" ScriptMode="Release" />
	<div class="ms-WPBody">
		<nw:ModalDialogManager ID="mdm" runat="server" />
	</div>
	<h1><%=HttpContext.Current.Items["title"] %></h1>
	<h3>Дата и время печати: <%=DateTime.Now.DateTimeToString() %></h3>
<%	if (filter != null && filter.FilterList.Count() > 0)
	{
%>
<h3>Параметры фильтра:</h3>
<%=filter.ToText()%>
<br />
<br />
<%} %>
    <div>
    <cc1:Workspace ID="workspace" runat="server" DisablePaging="true" RenderTitle="false" LayoutClass="PrintListLayout" DisableScriptManager="true" SkipCreateMdm="true"  />
    </div>
    </form>
</body>
</html>
<script runat="server">
Nephrite.Web.Controls.Filter filter;
void Page_Init(object sender, EventArgs e)
{
	Nephrite.Web.AppWeb.Page = Page;
	Nephrite.Web.Controls.Paging.DefaultPageSize = 0;
}

void Page_LoadComplete(object sender, EventArgs e)
{
	filter = workspace.Controls[1].FindControl("filter") as Nephrite.Web.Controls.Filter;
	if (filter != null)
		filter.Visible = false;
}
</script>