<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TinyMCEImageManager.aspx.cs" Inherits="Nephrite.Web.Controls.TinyMCEImageManager" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Register src="ParameterizedLinkManager.ascx" tagname="ParameterizedLinkManager" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Image Manager</title>
    <link rel="stylesheet" type="text/css" href="/css/RESET.CSS" />
    <link rel="stylesheet" href="/_controltemplates/Nephrite.Web/js/tiny_mce/themes/advanced/skins/default/dialog.css">
    
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <!--<div id="showimage"></div>-->
    <div>
    <table cellpadding="0" cellspacing="0" style="width:100%">
		<tr>
			<td><asp:Literal runat="server" ID="litFolderDisplay"></asp:Literal></td>
			<td style="text-align:right"><asp:Literal runat="server" ID="litFolderNav"></asp:Literal></td>
        </tr>
    </table>
    <div style="height:200px; overflow-y:scroll">
    <table cellpadding="0" cellspacing="0" style="width:482px;">
		<tr style="background-color:#D8D8D8; padding:3px">
            <td style="width:100%">Название</td>
            <td style="white-space:nowrap">Размер</td>
            <td style="text-align:center;white-space:nowrap">Дата</td>
            <td style="width: 20px"></td>
        </tr>
        <% foreach (string d in _dirs)
		   { %>
        <tr>
            <td><img src="<%=Settings.ImagesPath %>folder.gif" alt="f" />&nbsp;<a href="<%=Request.Path %>?dir=<%=Server.UrlEncode(storage.CurrentDirectory + (storage.CurrentDirectory.Length > 0 ? "/" : "") + d)%>"><%=d%></a></td>
            <td>&nbsp;</td>
            <td style="white-space:nowrap"><%=storage.GetModifiedDate(d).ToString()%></td>
            <td><%=plm.RenderImageConfirm("delete.gif", "удалить", "Удалить каталог со всем содержимым?", "folder," + d)%></td>
        </tr>
        <% } %>
        <% foreach (string f in _files)
		   { %>
        <tr>
            <td><img src="<%=Settings.ImagesPath %>jpeg16.gif" alt="i" />&nbsp;<a href="javascript:window.opener.returnToOpener('/<%=storage.GetUrl(f) %>');window.close();"><%=f%></a></td>
            <td><%=storage.GetFileSize(f) %></td>
            <td style="white-space:nowrap"><%=storage.GetModifiedDate(f).ToString()%></td>
            <td><%=plm.RenderImageConfirm("delete.gif", "удалить", "Удалить файл?", "file," + f) %></td>
        </tr>
        <% } %>
    </table>
    </div>
    
    <p>Добавить файл: <asp:FileUpload ID="uploadedFile" runat="server" />&nbsp;<asp:Button ID="upload" runat="server" Text="Загрузить" /></p>
    <p>Создать каталог: 
	<asp:TextBox ID="tbFolderName" runat="server"></asp:TextBox>
	<asp:Button ID="bCreateFolder" runat="server" Text="Создать" OnClick="bCreateFolder_Click" /></p>
	
    <asp:Label id="dirContent" runat="server"/>
    <asp:Label id="message" runat="server"/>
    
    </div>
    
    <uc1:ParameterizedLinkManager ID="plm" runat="server" OnClick="lbDel_Click" />
    </form>
</body>
</html>
