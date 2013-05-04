<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="delete.ascx.cs" Inherits="Nephrite.Metamodel.View.Triggers_delete" %>
Вы действительно хотите удалить триггер <%=name %>?
<br />
<br />
<asp:Button CssClass="ms-ButtonHeightWidth" Text="Удалить" ID="bDelete" runat="server" OnClick="bDelete_Click" />
<nw:BackButton runat="server" ID="bBackButton" />