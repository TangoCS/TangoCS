<%@ Control Language="C#" AutoEventWireup="True" Inherits="Nephrite.Web.Controls.ParameterizedLinkManager" Codebehind="ParameterizedLinkManager.ascx.cs" %>
<asp:LinkButton ID="btn" runat="server" Text="" OnClick="OnClick" />
<asp:HiddenField ID="hArg" runat="server" />

<script type="text/javascript">
function <%=this.ClientID %>_click(arg)
{
	document.getElementById("<%= hArg.ClientID %>").value = arg;
	<%= Page.ClientScript.GetPostBackEventReference(btn, "", true) %>;
	return false;
}
</script>