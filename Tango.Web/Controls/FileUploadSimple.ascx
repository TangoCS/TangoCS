<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadSimple.ascx.cs" Inherits="Nephrite.Web.Controls.FileUploadSimple" %>

<% if (cbDel.Visible && Enabled) { %><asp:RadioButton ID="rbNoChanges" runat="server" Checked="true" /><% } %>
<asp:Label ID="lLink" runat="server" />


<% if (Enabled) { %>
<table style="width:100%; border-collapse:collapse" cellpadding="0" cellspacing="0">
<tr>
<% if (cbDel.Visible) { %>
<td style="padding-right:5px; width:110px"><asp:RadioButton ID="rbChange" runat="server" Text="заменить&nbsp;на" /></td>
<% } %>
<td>
<asp:FileUpload ID="fuFile" runat="server" Width="100%" />
</td>
</tr>

<% if (cbDel.Visible) { %>
<tr><td colspan="2">
<asp:RadioButton ID="cbDel" runat="server" Text="удалить файл" />
</td></tr>
<% } %>

</table>
<% } %>

<asp:Label ID="lMess" runat="server" ForeColor="Red" />
