<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<%@ Register Assembly="Nephrite.Metamodel" Namespace="Nephrite.Metamodel" TagPrefix="nm" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web" TagPrefix="cc1" %>
<%@ Import Namespace="Nephrite.Web.App" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><%=HttpUtility.HtmlEncode(AppSettings.Get("meta-title")) %></title>
    <%=RssLinker.RenderLinks() %>
    <link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/RESET.css" />
	<link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/Site.css" />
	<!--<link rel="stylesheet" type="text/css" href="/css/coresite.css" />-->
	<link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/CORE.CSS" />
	<link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/coresite.css" />


	<link rel="stylesheet" type="text/css" href="/css/Tessera.css" />
	<link rel="icon" href="/favicon2.ico" type="image/x-icon"> 
	<link rel="shortcut icon" href="/favicon2.ico" type="image/x-icon">
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
	<meta name="Title" content="<%=HttpUtility.HtmlEncode(AppSettings.Get("meta-title")) %>" />
	<meta name="Description" content="<%=HttpUtility.HtmlEncode(AppSettings.Get("meta-description")) %>" />
	<meta name="Keywords" content="<%=HttpUtility.HtmlEncode(AppSettings.Get("meta-keywords")) %>" />
    <script src="/_controltemplates/Nephrite.Web/js/jquery-1.7.min.js" type="text/javascript"></script>
    <base href="/" />
</head>
<body>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" />
<nw:ModalDialogManager ID="mdm" runat="server" />
<nm:MView ID="MView1" PackageViewFormSysName="MainPage" runat="server" />
<% if (Request.QueryString["showsql"] == "1"){ %>
<div style="padding:10px">
<pre>
	<cc1:sqllog id="SqlLog" runat="server" title="SQL Log" />
</pre>
</div>
<%} %>
</form>
</body>
</html>
