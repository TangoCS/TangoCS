<%@ Page Language="C#" AutoEventWireup="True" ValidateRequest="false" CodeBehind="Default.aspx.cs" Inherits="Tessera._Default" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="Nephrite.Web.App" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web" TagPrefix="cc1" %>

<html>
<head id="Head1" runat="server">
	<title>View test</title>
	<% string js = "\"" + Settings.JSPath; %>
	<link rel="stylesheet" type="text/css" href="/css/calendar.CSS" />
	<link rel="stylesheet" type="text/css" href="/css/datepicker.CSS" />
	<link rel="stylesheet" type="text/css" href="/css/CORE.CSS" />
	<link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/STAND.CSS" />
	<link rel="stylesheet" type="text/css" href="/css/popup.CSS" />
	<link rel="stylesheet" type="text/css" href=<%=js%>Calendar/skins/aqua/theme.css" />
	<link rel="stylesheet" type="text/css" href=<%=js%>jquery-treeview/jquery.treeview.css" />
	<link rel="stylesheet" type="text/css" href=<%=js%>jquery-autocomplete/jquery.autocomplete.css" />
    <link rel="stylesheet" type="text/css" href="/css/Tessera.css" />
	<link rel="stylesheet" type="text/css" href=<%=js%>tree/tree.css" />
<%string themeName = AppSettings.Get("theme");
  if (!String.IsNullOrEmpty(themeName) && Query.GetString("showtheme") == "")
  { %>
    <%="<link rel='stylesheet' type='text/css' href='/THEMES/" + themeName + "/theme.css' />"%>
<%} 
    if (Query.GetString("showtheme") != "")
  { %>
    <link rel="stylesheet" type="text/css" href="/THEMES/<%=Query.GetString("showtheme") %>/theme.css" />
<%} %>
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
</head>
<body style="overflow:hidden;">
	<form id="form1" runat="server">
	<div class="ms-WPBody">
	    <nw:ModalDialogManager ID="mdm" runat="server" />
	</div>

	<table cellpadding="0" cellspacing="0" style="margin:0; left:0px; top:0px; width:100%; height:100%;position: fixed;//position: absolute; ">
        <tr>
            <td colspan="3" class="ms-globalbreadcrumb" style="vertical-align:middle; text-align:right; height:60px; background-color: #acc3e1; background-image: url(_layouts/images/n/head_bg.jpg)">
				<div style="float:left">
				<img src="<%=AppSettings.Get("logo") %>" />
				</div>
				<div style="float: left; text-align: left; font-weight: bold; font-size: 10pt; color:Black; padding-top:10px; padding-left:10px">
                    <%=AppSettings.Get("orgname") %>
                    <br />
                    <%=AppSettings.Get("systemname") %>
                </div>
                <div style="float: right; margin-top: 8px; color: Gray; width:340px; text-align:left">
			    </div>
            </td>
        </tr>
	    <tr>
            <td class="ms-nav" style="padding-left: 5px"></td>
			<td class="ms-WPBody" style="vertical-align: top; border-bottom: solid 1px #6F9DD9; 
				border-left: solid 1px #6F9DD9; border-right: solid 1px #6F9DD9; width:100%; ">
				<div class="ms-bodyareacell" id="wsdiv" style="overflow-x: auto; overflow-y:auto; position: relative; display:block;">
					<cc1:workspace id="Workspace1" runat="server" SkipCreateMdm="true" />
					<% if (Request.QueryString["showsql"] == "1"){ %>
					<cc1:sqllog id="SqlLog" runat="server" title="SQL Log" />
					<%} %>
				</div>
			</td>
			<td class="ms-nav" style="padding-left: 5px"></td>
		</tr>
	</table>
	</form>
</body>
</html>