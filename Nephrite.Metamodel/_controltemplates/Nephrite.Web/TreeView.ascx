<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TreeView.ascx.cs" Inherits="Nephrite.Web.Controls.TreeView" %>

<asp:HiddenField ID="hLoadedTemplates" runat="server" />
<asp:Literal ID="lTv" runat="server" EnableViewState="false"></asp:Literal>
<%if(IsDynamic){ %>
<ul id='<%=ClientID %>_tv' class='treeview-gray treeview' >
</ul>
<%} %>