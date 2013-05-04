<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectObjectArray.ascx.cs" Inherits="Nephrite.Web.Controls.SelectObjectArray" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Register src="ModalDialog.ascx" tagname="ModalDialog" tagprefix="uc1" %>

<uc1:ModalDialog Title="Выберите объект" ID="select" runat="server" Width="700px" OnPopulate="select_Populate" OnOKClick="select_OKClick" >
<ContentTemplate>
<asp:CheckBoxList ID="cbl" runat="server"></asp:CheckBoxList>
</ContentTemplate>
</uc1:ModalDialog>