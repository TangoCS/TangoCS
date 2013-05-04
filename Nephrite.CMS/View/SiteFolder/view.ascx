<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="view.ascx.cs" Inherits="Nephrite.CMS.View.SiteFolder_view" %>
<%@ Register Assembly="Nephrite.Metamodel" Namespace="Nephrite.Metamodel" TagPrefix="nm" %>
<%@ Register Assembly="Nephrite.Web" Namespace="Nephrite.Web.Controls" TagPrefix="nw" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.CMS" %>
<%@ Import Namespace="Nephrite.CMS.Controllers" %>


<nw:Toolbar ID="toolbar" runat="server" />

<table class="ms-formtable wfit" cellpadding="0" cellspacing="0" style="padding:8px">
<tr>
	<td class="ms-formlabel">Родительский объект</td>
	<td class="ms-formbody">
	<% Action<IMMObject> renderLink = null;
    renderLink = (obj) =>
    {
        if (obj == null)
            return;
        var p = GetParentSiteFolder(obj);
        if (p != null)
        {
            renderLink(p);
            %> / <%
        }
           %> <%=Html.ActionLink<SiteFolderController>(c=>c.View(obj.ObjectID, obj.MetaClass.Name), obj.Title)%>
<%  };
    renderLink(ParentFolder);
    %>
	</td>
</tr>
<tr>
    <td class="ms-formlabel">Класс</td>
    <td class="ms-formbody"><%=CurrentMMType.Title%></td>
</tr>
<nm:MView runat="server" ViewFormSysName="view" ID="folder" />
<%=HtmlHelperWSS.FormTableEnd()%>



<asp:Panel runat="server" ID="lists" />

