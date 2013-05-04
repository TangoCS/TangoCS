<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="Nephrite.Web.SPM.edit" %>


<table class="ms-formtoolbar">
<tr>
<td style="width:100%"></td>
<td>
<asp:Button ID="bOK1" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
</td>
<td>
<nw:BackButton runat="server" ID="BackButton1" />
</td>
</tr>
</table>


<table class="ms-formtable w700" cellpadding="0" cellspacing="0">
<tr>
	<td class="ms-formlabel">Название</td>
	<td class="ms-formbody"><asp:TextBox ID="tbTitle" runat="server" Width="100%"></asp:TextBox></td>
</tr>
<tr>
	<td class="ms-formlabel">Системное имя</td>
	<td class="ms-formbody"><asp:TextBox ID="tbSysName" runat="server" Width="100%"></asp:TextBox></td>
</tr>
<tr><td colspan="2" class="ms-formlabel">&nbsp;</td></tr>
</table>

<table class="ms-formtoolbar">
<tr>
<td style="width:100%"></td>
<td>
<asp:Button ID="bOK2" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
</td>
<td>
<nw:BackButton runat="server" ID="BackButton2" />
</td>
</tr>
</table>