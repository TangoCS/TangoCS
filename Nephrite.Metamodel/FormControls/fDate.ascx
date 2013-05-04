<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="fDate.ascx.cs" Inherits="Nephrite.Metamodel.FormControls.fDate" %>
<asp:DropDownList ID="Day" runat="server">
</asp:DropDownList>
<asp:DropDownList ID="Month" runat="server">
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
<asp:DropDownList ID="Year" runat="server">
</asp:DropDownList>
