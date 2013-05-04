<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageUploadSimple.ascx.cs" Inherits="Nephrite.Web.Controls.ImageUploadSimple" %>

<img id="img" runat="server" alt="" />
<% if (cbDel.Visible) { %><br /><% } %>
<asp:CheckBox ID="cbDel" runat="server" Text="удалить изображение" />
<% if (cbDel.Visible) { %><br /><% } %>
<% if (Enabled) { %>
<asp:FileUpload ID="fuFile" runat="server" Width="100%" />
<% } %>
<asp:Label ID="lMess" runat="server" ForeColor="Red" />