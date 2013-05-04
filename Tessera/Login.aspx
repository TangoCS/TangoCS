<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Nephrite.Web.App" %>
<%@ Register Assembly="Nephrite.Metamodel" Namespace="Nephrite.Metamodel" TagPrefix="nm" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%=AppSettings.Get("title")%></title>
	<link rel="stylesheet" type="text/css" href="/CSS/RESET.css" />
	<link rel="stylesheet" type="text/css" href="/CSS/Site.css" />
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
</head>
<body>
	<form id="form1" runat="server">
	<nm:MView ID="MView1" PackageViewFormSysName="AdminLogin" runat="server" />
    </form>
</body>
</html>
