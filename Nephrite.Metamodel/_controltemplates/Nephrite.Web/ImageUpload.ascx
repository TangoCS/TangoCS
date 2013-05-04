<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageUpload.ascx.cs" Inherits="Nephrite.Web.Controls.ImageUpload" %>
<img id="img" runat="server" alt="" />
 
<% if (!String.IsNullOrEmpty(savedID.Value)) { %><asp:LinkButton ID="lbDelNewFile" runat="server" OnClick="lbDelNewFile_Click"><img src="<%=Settings.ImagesPath %>delete.gif" alt="удалить" class="middle" /></asp:LinkButton><% } %>
<% if (cbDel.Visible) { %><br /><% } %>
<asp:CheckBox ID="cbDel" runat="server" Text="удалить изображение" />
<% if (cbDel.Visible) { %><br /><% } %>
<asp:FileUpload ID="fuFile" runat="server" Width="100%" />
<asp:HiddenField ID="savedID" runat="server" />
<div style="display:none">
<asp:Button ID="lbLoad" runat="server" Text="Button" UseSubmitBehavior="true" OnClick="lbLoad_Click" />
</div>

<script type="text/javascript">
	function fileSelected<%=ClientID %>() {
		<%= Page.ClientScript.GetPostBackEventReference(lbLoad, "", true) %>;
	}
</script>