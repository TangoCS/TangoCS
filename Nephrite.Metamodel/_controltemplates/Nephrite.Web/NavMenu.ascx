<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavMenu.ascx.cs" Inherits="Nephrite.Web.Controls.NavMenu" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%= HtmlHelperWSS.NavMenuBegin() %>
<%  foreach (NavMenuItem group in this.Groups)
	{ %>
	<%if (group.Items.Count > 0 || !group.Url.IsEmpty()) { %><%=HtmlHelperWSS.NavMenuGroup(group)%><% } %>
<% } %>
<%= HtmlHelperWSS.NavMenuEnd() %>
