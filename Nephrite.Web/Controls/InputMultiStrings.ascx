<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InputMultiStrings.ascx.cs" Inherits="Nephrite.Web.Controls.InputMultiStrings" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="System.Linq" %>

<asp:TextBox ID="tbInput" runat="server" Width="95%"></asp:TextBox>
<asp:ImageButton ID="bAddString" runat="server" OnClick="bAddString_Click" />
<br />

<% if (_data != null)
   { %>
<table class="ms-listviewtable" cellpadding="0" cellspacing="0">
<% int i = 0;
   HtmlHelperBase.Instance.Repeater(_data, "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>" >
	<td class="ms-vb2" width="100%"><%=o%></td>
	<td class="ms-vb2" align="center" style="white-space:nowrap">
	<%=plm.RenderImageConfirm("delete.gif", "Удалить", "Вы уверены, что хотите удалить запись?", i)%>
	<%=plm_up.RenderImage("arrow_up.png", "Вверх", i)%>
	<%=plm_down.RenderImage("arrow_down.png", "Вниз", i)%>
	</td>
</tr>
<%     i++;
   }); %>
</table>
<% } %>

<nw:ParameterizedLinkManager ID="plm" OnClick="bDeleteString_Click" runat="server" />
<nw:ParameterizedLinkManager ID="plm_up" OnClick="bUpString_Click" runat="server" />
<nw:ParameterizedLinkManager ID="plm_down" OnClick="bDownString_Click" runat="server" />
