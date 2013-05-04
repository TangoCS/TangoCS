<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="generatewsdl.ascx.cs" Inherits="Nephrite.Metamodel.View.Utils.generatewsdl" %>
<%=HtmlHelperWSS.FormTableBegin("700px")%>
<%=HtmlHelperWSS.FormRowBegin("URL веб-сервиса", true)%>
<asp:TextBox ID="tbUrl" runat="server" Width="100%" />
<%=HtmlHelperWSS.FormRowEnd()%>
<%=HtmlHelperWSS.FormRowBegin("Пространство имен")%>
<asp:TextBox ID="tbNamespace" runat="server" Width="100%" />
<%=HtmlHelperWSS.FormRowEnd()%>
<%=HtmlHelperWSS.FormTableEnd() %>


<asp:Label ID="lMsg" runat="server" />
<%=HtmlHelperWSS.FormToolbarBegin() %>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>