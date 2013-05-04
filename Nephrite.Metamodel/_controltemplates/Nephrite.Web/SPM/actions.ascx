<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="actions.ascx.cs" Inherits="Nephrite.Web.SPM.actions" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.SPM" %>

<nw:Toolbar ID="toolbar" runat="server" />

<nw:TreeView runat="server" ID="atv" />

<nw:ModalDialog ID="action" runat="server" Title="Защищаемый объект" OnPopulate="action_Populate" OnOKClick="action_OKClick">
<ContentTemplate>
<%=HtmlHelperWSS.FormTableBegin() %>
    <%=HtmlHelperWSS.FormRowBegin("Тип")%>
        <asp:Label ID="lType" runat="server" />
    <%=HtmlHelperWSS.FormRowEnd()%>
    <%=HtmlHelperWSS.FormRowBegin("Наименование")%>
        <asp:TextBox ID="tbTitle" runat="server" Width="100%" />
    <%=HtmlHelperWSS.FormRowEnd()%>
    <%=HtmlHelperWSS.FormRowBegin("Системное имя")%>
        <asp:TextBox ID="tbSystemName" runat="server" Width="100%" />
    <%=HtmlHelperWSS.FormRowEnd()%>
<%=HtmlHelperWSS.FormTableEnd() %>
</ContentTemplate>
</nw:ModalDialog>