<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="DateLists.ascx.cs" Inherits="Nephrite.Web.Controls.DateLists" %>
<asp:DropDownList EnableViewState="false" ID="Day" runat="server">
</asp:DropDownList>
<asp:DropDownList EnableViewState="false" ID="Month" runat="server">
	<asp:ListItem Value="0">Месяц</asp:ListItem>
	<asp:ListItem Value="1">января</asp:ListItem>
	<asp:ListItem Value="2">февраля</asp:ListItem>
	<asp:ListItem Value="3">марта</asp:ListItem>
	<asp:ListItem Value="4">апреля</asp:ListItem>
	<asp:ListItem Value="5">мая</asp:ListItem>
	<asp:ListItem Value="6">июня</asp:ListItem>
	<asp:ListItem Value="7">июля</asp:ListItem>
	<asp:ListItem Value="8">августа</asp:ListItem>
	<asp:ListItem Value="9">сентября</asp:ListItem>
	<asp:ListItem Value="10">октября</asp:ListItem>
	<asp:ListItem Value="11">ноября</asp:ListItem>
	<asp:ListItem Value="12">декабря</asp:ListItem>
</asp:DropDownList>
<asp:DropDownList EnableViewState="false" ID="Year" runat="server">
</asp:DropDownList>
<%if (ShowTime)
  { %>
<asp:DropDownList EnableViewState="false" ID="Hour" runat="server" />
<asp:DropDownList EnableViewState="false" ID="Minute" runat="server" />
<%} %>