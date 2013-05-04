<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="Nephrite.Web.Controls.FileUpload" %>

<asp:Label ID="lLink" runat="server" />
<% if (!String.IsNullOrEmpty(savedID.Value)) { %><asp:LinkButton ID="lbDelNewFile" runat="server" OnClick="lbDelNewFile_Click"><img src="<%=Settings.ImagesPath %>delete.gif" alt="удалить" class="middle" /></asp:LinkButton><% } %>
<% if (cbDel.Visible) { %><br /><% } %>
<asp:CheckBox ID="cbDel" runat="server" Text="удалить файл" />
<% if (cbDel.Visible) { %><br /><% } %>
<asp:FileUpload ID="fuFile" runat="server" Width="100%" />
<asp:HiddenField ID="savedID" runat="server" />
<div style="display:none">
<asp:Button ID="lbLoad" runat="server" Text="Button" UseSubmitBehavior="true" OnClick="lbLoad_Click" />
</div>
<asp:Label ID="lMess" runat="server" ForeColor="Red" />
<script type="text/javascript">
	function fileSelected<%=ClientID %>() {
		initModalPopup();
		showModalPopup();
		<%= Page.ClientScript.GetPostBackEventReference(lbLoad, "", true) %>;
	}
</script>