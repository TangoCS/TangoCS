<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SingleObject.ascx.cs" Inherits="Nephrite.Web.Controls.SingleObject" %>
<%@ Register TagPrefix="cc1" TagName="SelectObjectHierarchic" Src="SelectObjectHierarchic.ascx" %>
<asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
<ContentTemplate><asp:LinkButton runat="server" ID="lbAutoComplete" OnClick="lbAutoComplete_Click" />
<table width="100%" cellpadding="0" cellspacing="0">
<tr>
    <td width="90%">
        <asp:TextBox runat="server" ID="tbObject" Width="100%" />
		<asp:HiddenField runat="server" ID="hfObjectID" />
    </td>
    <%if (tbObject.Enabled)
	  { %>
    <td>&nbsp;</td>
    <td><%=HtmlHelperBase.Instance.InternalImageLink(RenderRun(), "Выбрать из списка", "list.gif")%></td>
    <td>&nbsp;</td>
    <td><%=HtmlHelperBase.Instance.InternalImageLink("document.getElementById('" + hfObjectID.ClientID + "').value = ''; document.getElementById('" + tbObject.ClientID + "').value = ''; " + Page.ClientScript.GetPostBackEventReference(lbClear, ""), "Очистить", "delete.gif")%></td>
    <%} %>
</tr>
</table>
<asp:LinkButton ID="lbClear" runat="server" OnClick="lbClear_Click" />
</ContentTemplate>
</asp:UpdatePanel>

<cc1:SelectObjectHierarchic
	ID="select"
	runat="server"
	Title="Выбрать объект"
	DataTextField="Title"
	PageSize="20" />
<script type="text/javascript">
	function OnSelect_<%=ClientID %>(title, id) 
	{
		document.getElementById('<%=tbObject.ClientID %>').value = title;
		document.getElementById('<%=hfObjectID.ClientID %>').value = id;
	}
</script>