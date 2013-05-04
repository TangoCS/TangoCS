<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register Assembly="Nephrite.Metamodel" Namespace="Nephrite.Metamodel" TagPrefix="nm" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/CORE.css" />
    <link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/STAND.css" />
    <link rel="stylesheet" type="text/css" href="/Data.ashx?path=CSS/print.css" />
	<meta http-equiv="content-type" content="text/html; charset=utf-8" />
</head>
<body>
    <form id="form1" runat="server">
	<div>
        <cc1:workspace id="Workspace1" RenderTitle="false" runat="server" SkipCreateMdm="true" DisableScriptManager="true" />
    </div>
    </form>
</body>
</html>
