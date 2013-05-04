<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectObject.ascx.cs" Inherits="Nephrite.Web.Controls.SelectObject" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Register src="ModalDialog.ascx" tagname="ModalDialog" tagprefix="uc1" %>

<uc1:ModalDialog Title="Выберите объект" ID="select" runat="server" Width="700px" OnPopulate="select_Populate" OnOKClick="select_OKClick" >
<ContentTemplate>
<div style="overflow-y:auto; height:450px; margin-top:5px; margin-bottom:5px">
<asp:RadioButtonList ID="rbl" runat="server" />
</div>
</ContentTemplate>
</uc1:ModalDialog>


