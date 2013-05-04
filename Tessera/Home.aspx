<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="Nephrite.Web.App" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web" TagPrefix="cc1" %>
<%@ Register Src="~/_controltemplates/Tessera/Common/title.ascx" TagPrefix="cc2" TagName="title" %>
<html>
<head id="Head1" runat="server">
	<title></title>
	<asp:PlaceHolder runat="server" ID="phCSS"></asp:PlaceHolder>
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
</head>
<body style="overflow:hidden;">
	<form id="form1" runat="server">
	<asp:ScriptManager runat="server" ScriptMode="Release" />
	<div class="ms-WPBody">
		<nw:ModalDialogManager ID="mdm" runat="server" />
	</div>
	<table cellpadding="0" cellspacing="0" style="border-collapse:collapse; margin:0; left:0px; top:0px; width:100%; height:100%;position: fixed;//position: absolute; ">
		<tr>
			<td colspan="2" style="height:1px">
				<nm:MView PackageViewFormSysName="homeheader" runat="server" ID="homeheader" />
			</td>
		</tr>
		<tr>
			<td class="ms-leftareacell ms-nav" style="width: 150px; padding: 0px 3px 0px 2px">
				<cc1:mainmenu id="MainMenu1" runat="server" title="Меню" CssClass="mainmenu" Width="220px" AscxFileName="mainmenu2.ascx" />
			</td>
			<td class="ms-WPBody" style="vertical-align: top; border-bottom: solid 1px #6F9DD9;
				border-left: solid 1px #6F9DD9; border-right: solid 1px #6F9DD9; width:100%">
				<cc2:title runat="server" /><asp:PlaceHolder runat="server" ID="toolbarPlace" />
				<div class="ms-bodyareacell" id="wsdiv" style="overflow-x: auto; overflow-y:auto; position: relative; display:block">
					<cc1:workspace id="Workspace1" RenderTitle="false" runat="server" SkipCreateMdm="true" DisableScriptManager="true" />
					<% if (Request.QueryString["showsql"] == "1"){ %>
					<pre>
					<cc1:sqllog id="SqlLog" runat="server" title="SQL Log" />
					<%} %>
					<pre id="tracelog"></pre>
				</div>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>

<script runat="server">
protected void Page_Init(object sender, EventArgs e)
{
	HttpContext.Current.Items["Toolbar"] = toolbarPlace;
	HttpContext.Current.Items["Trace"] = Trace;
	if (AppSPM.GetCurrentSubject() == null)
		Response.Redirect("/Login.aspx");
	if (AppSPM.GetCurrentSubject().MustChangePassword)
		Response.Redirect("/ChangePassword.aspx?returnurl=" + Query.CreateReturnUrl());
	
	StringBuilder css = new StringBuilder();
	css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/css/CORE.CSS"" />");
	string themeName = AppSettings.Get("theme");
	if (!String.IsNullOrEmpty(themeName) && Query.GetString("showtheme") == "")
	{
		css.Append(@"<link rel='stylesheet' type='text/css' href='/THEMES/" + themeName + "/theme.css' />");
	} 
    if (Query.GetString("showtheme") != "")
	{ 
		css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/THEMES/" + Query.GetString("showtheme") + @"/theme.css"" />");
	}
	css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/css/calendar.CSS"" />");
	css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/css/datepicker.CSS"" />");
	css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/CSS/STAND.CSS"" />");
    css.Append(@"<link rel=""stylesheet"" type=""text/css"" href=""/css/Tessera.css"" />");
	LiteralControl l = new LiteralControl(css.ToString());
	phCSS.Controls.Add(l);
}
protected void Page_Load(object sender, EventArgs e)
{
	Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");
	Page.ClientScript.RegisterClientScriptInclude("jquery", Settings.JSPath + "jquery-1.7.min.js");
	Page.ClientScript.RegisterClientScriptInclude("mainwindow", Settings.JSPath + "homewindow.js");
}
</script>