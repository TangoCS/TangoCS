<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="Nephrite.Web.App.edit" %>
<%@ Import Namespace="Nephrite.Web.App" %>

<table class="ms-formtable wfit" cellpadding="0" cellspacing="0">
<% Html.Repeater<N_Setting>(ViewData, "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>">
	<td class="ms-formlabel"><%=o.Title %></td>
	<td class="ms-formbody">
		<input class="wfit" id="s_<%=o.SystemName %>" name="s_<%=o.SystemName %>" type="text" value="<%=o.Value %>" />
	</td>
</tr>
<%}); %>
</table>

<table class="ms-formtoolbar" style="width:100%">
<tr>
<td style="width:100%"></td>
<td>
<asp:Button ID="bOK1" runat="server" Text="Сохранить" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
</td>
</tr>
</table>
