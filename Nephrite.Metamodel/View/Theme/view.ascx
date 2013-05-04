<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="Nephrite.Metamodel.View.Theme_view" %>
<%@ Import Namespace="Nephrite.Web.App" %>
<%@ Import Namespace="Nephrite.Web" %>
Эта страница служит для изменения шрифтов и цветовой схемы
<%=HtmlHelperWSS.FormTableBegin("700px") %>
<%=HtmlHelperWSS.FormRowBegin("Выбор темы") %>
    <asp:ListBox Rows="30" Width="200px" ID="lbTheme" runat="server" DataTextField="Title" DataValueField="Name" OnSelectedIndexChanged="lbTheme_SelectedIndexChanged" AutoPostBack="true" />
<%=HtmlHelperWSS.FormRowEnd() %>
<%=HtmlHelperWSS.FormRow("Текущая тема", AppSettings.Get("theme"))%>
<%=HtmlHelperWSS.FormTableEnd() %>
<table class="ms-formtoolbar">
<tr>
<td style="width:100%"></td>
<td>
<asp:Button ID="bOK1" runat="server" Text="Применить" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
</td>
<td>
&nbsp;
</td>
</tr>
</table>