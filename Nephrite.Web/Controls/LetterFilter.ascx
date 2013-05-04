<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LetterFilter.ascx.cs" Inherits="Nephrite.Web.Controls.LetterFilter" %>
<%@ Import Namespace="Nephrite.Web" %>
<div style="padding:5px">
<a title="Без фильтра" style='text-decoration:none;<%=Query.GetString("l") == "" ? "background-color:Black; color:White;font-weight:bold;" : ""%>' href="<%=Query.RemoveParameter("l") %>"><span style="padding:3px">Все</span></a>
<asp:Repeater ID="rptLetters" runat="server">
<ItemTemplate>
<a title="Отфильтровать по первой букве" style='text-decoration:none;<%#Query.GetString("l") == HttpUtility.UrlEncode(Container.DataItem.ToString()).ToUpper() ? "background-color:Black; color:White; font-weight:bold;" : ""%>' href="<%#Query.RemoveParameter("l") + "&l=" + Container.DataItem.ToString() %>"><span style="padding:3px"><%#Container.DataItem%></span></a>
</ItemTemplate>
</asp:Repeater>
</div>