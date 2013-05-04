<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="delete.ascx.cs" Inherits="Nephrite.Web.SPM.delete" %>
<%if(lo.GetLinkedObjects().Count > 0){%>
Нельзя удалить роль, так как имеются связанные объекты:
<br /><br />
<nw:LinkedObjects runat="server" ID="lo" />
<br />
<%}%>
<%if(lo.GetLinkedObjects().Count == 0){%>
Вы уверены, что хотите удалить роль "<%=ViewData.Title%>"?
<br /><br />
<asp:Button CssClass="ms-ButtonHeightWidth" Text="Удалить" ID="bDelete" runat="server" OnClick="bDelete_Click" />
<%}%>
<nw:BackButton runat="server" ID="bBackButton" />