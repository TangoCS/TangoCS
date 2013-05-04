<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="view.ascx.cs" Inherits="Nephrite.Web.SPM.view" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.SPM" %>
<%@ Register Assembly="Nephrite.Web" TagPrefix="nw" Namespace="Nephrite.Web.Controls" %>

<table class="ms-formtoolbar" style="width:100%">
<tr>
<td style="width:100%"></td>
<td>
<nw:BackButton runat="server" ID="BackButton1" />
</td>
</tr>
</table>

<nw:Toolbar ID="toolbar" runat="server" TableCssClass="ms-toolbar" />

<table class="ms-formtable" cellpadding="0" cellspacing="0" style="width:100%">
<tr>
	<td class="ms-formlabel">Название</td>
	<td class="ms-formbody"><%=ViewData.Title%></td>
</tr>
<tr>
	<td class="ms-formlabel">Системное имя</td>
	<td class="ms-formbody"><%=ViewData.SysName%></td>
</tr>
</table>


<div class="tabletitle">
Наследует полномочия ролей <a href="#" onclick="<%=roleselect.RenderRun() %>"><%=Html.Image("additem.png", "Добавить роль")%></a>
</div>

<table class="ms-listviewtable" cellpadding="0" cellspacing="0" style="width:100%">
<tr class="ms-viewheadertr">
    <%=HtmlHelperWSS.TH("Название", "100%")%>
    <%=HtmlHelperWSS.TH("Действие")%>
</tr>
<% if (ViewData != null) Html.Repeater<SPM_RoleAsso>(ViewData.SPM_RoleAssos, "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>">
    <td class="ms-vb2"><%=Html.ActionLink<SPMController>(c => c.View(o.ParentRoleID), o.SPM_ParentRole.Title)%></td>
	<td class="ms-vb2" align="center">
	<%=Html.ActionImageConfirm<SPMController>(c => c.RemoveMembership(o.RoleID, o.ParentRoleID), "Удалить", "delete.gif", "Вы уверены, что хотите удалить наследование?")%>
	</td>
</tr>
<%}); %>
</table>
<br />
<div class="tabletitle">
Группы и пользователи
<% if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableADO"])) { %>
<a href="#" onclick="<%=adselect.RenderRun() %>"><%=Html.Image("additem.png", "Добавить")%></a>
<% } %>
</div>
<table class="ms-listviewtable" cellpadding="0" cellspacing="0"  style="width:100%">
<tr class="ms-viewheadertr">
    <%=HtmlHelperWSS.TH("Название", "100%")%>
    <%=HtmlHelperWSS.TH("Действие")%>
</tr>
<% if (ViewData != null) Html.Repeater<SPM_SubjectRole>(ViewData.SPM_SubjectRoles, "", "ms-alternating", (o, css) =>
   {  %>
<tr class="<%=css %>">
    <td class="ms-vb2"><%=o.SPM_Subject.Title%></td>
	<td class="ms-vb2" align="center">
	<%=Html.ActionImageConfirm<SPMController>(c => c.RemoveSubject(o.RoleID, o.SubjectID), "Удалить", "delete.gif", "Вы уверены, что хотите удалить субъект?")%>
	</td>
</tr>
<%}); %>
</table>
<br />
<div class="tabletitle">Полномочия</div>
<div style="color:Gray; padding-bottom: 4px">Роль имеет полномочия на выполнение операции, если у нее или у одной из наследуемых ролей утановлен флаг разрешения</div>

<table class="ms-listviewtable" cellpadding="0" cellspacing="0" style="width:100%">
<% bool alt = false; %>
<% foreach (SPM_Action bp in AppSPM.DataContext.SPM_Actions.Where(o => o.Type == (int)SPMActionType.BusinessPackage))
   { %>
<tr class="ms-viewheadertr">
    <%=HtmlHelperWSS.TH("Название")%>
    <%=HtmlHelperWSS.TH("Просм.", "50px")%>
    <%=HtmlHelperWSS.TH("Ред.", "50px")%>
    <%=HtmlHelperWSS.TH("Созд.", "50px")%>
    <%=HtmlHelperWSS.TH("Удал.", "50px")%>
    <%=HtmlHelperWSS.TH("Спис.", "50px")%>
    <%=HtmlHelperWSS.TH("Вып.", "50px")%>
</tr>
   
<tr >
    <td style="border-bottom: #cccccc 1px solid; font-weight:bold" class="ms-vb2"><%=bp.Title %></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="width: 100%" id='bp_view_<%=bp.ActionID.ToString() %>' name='bp_view_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'View')" />
    </div></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="background-color:#cccccc; width: 100%" id='bp_edit_<%=bp.ActionID.ToString() %>' name='bp_edit_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'Edit')"/>
    </div></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="background-color:#cccccc; width: 100%" id='bp_new_<%=bp.ActionID.ToString() %>' name='bp_new_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'New')"/>
    </div></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="background-color:#cccccc; width: 100%" id='bp_delete_<%=bp.ActionID.ToString() %>' name='bp_delete_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'Delete')"/>
    </div></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="background-color:#cccccc; width: 100%" id='bp_viewlist_<%=bp.ActionID.ToString() %>' name='bp_viewlist_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'ViewList')"/>
    </div></td>
    <td class="ms-vb2" style="border-bottom: #cccccc 1px solid;text-align:center"><div style="background-color:#cccccc; ">
        <input style="background-color:#cccccc; width: 100%" id='bp_exec_<%=bp.ActionID.ToString() %>' name='bp_exec_<%=bp.ActionID.ToString() %>' type="checkbox" onclick="allcheck(this.checked, <%=bp.ActionID %>, 'Execute')"/>
    </div></td>
</tr>
<% foreach (SPM_Action bo in bp.GetChildren().OrderBy(o => o.Title))  {

       bool access = false;
	   bool disable = false;
       List<SPM_Action> allbm = bo.GetChildren().ToList();
       SPM_Action mEdit = allbm.Where(o => o.SystemName == "Edit").FirstOrDefault();
       SPM_Action mView = allbm.Where(o => o.SystemName == "View").FirstOrDefault();
       SPM_Action mViewList = allbm.Where(o => o.SystemName == "ViewList").FirstOrDefault();
       SPM_Action mDelete = allbm.Where(o => o.SystemName == "Delete").FirstOrDefault();
       SPM_Action mNew = allbm.Where(o => o.SystemName == "CreateNew").FirstOrDefault();
	   SPM_Action mExecute = allbm.Where(o => o.SystemName == "Execute").FirstOrDefault();
%>
<tr <%= alt ? "class='ms-alternating'" : "" %>>
    <td class="ms-vb2"><%=bo.Title %></td>
    <td id="<%=bp.ActionID %>View" class="ms-vb2" style="text-align:center">
        <% if (mView != null) {
               access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mView.ActionID);
			   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mView.ActionID);
        %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mView.ActionID.ToString() %>' name='m_<%=mView.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %> />
        <%} %>
    </td>
    <td id="<%=bp.ActionID %>Edit" class="ms-vb2" style="text-align:center">
        <% if (mEdit != null) {
			   access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mEdit.ActionID);
			   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mEdit.ActionID);
        %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mEdit.ActionID.ToString() %>' name='m_<%=mEdit.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %>/>
        <%} %>
    </td>
    <td id="<%=bp.ActionID %>New" class="ms-vb2" style="text-align:center">
        <% if (mNew != null) {
			   access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mNew.ActionID);
			   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mNew.ActionID);
               %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mNew.ActionID.ToString() %>' name='m_<%=mNew.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %>/>
        <%} %>
    </td>
    <td id="<%=bp.ActionID %>Delete" class="ms-vb2" style="text-align:center">
        <% if (mDelete != null) {
			   access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mDelete.ActionID);
			   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mDelete.ActionID);
               %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mDelete.ActionID.ToString() %>' name='m_<%=mDelete.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %>/>
        <%} %>
    </td>
    <td id="<%=bp.ActionID %>ViewList" class="ms-vb2" style="text-align:center">
        <% if (mViewList != null) {
			   access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mViewList.ActionID);
			   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mViewList.ActionID);
               %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mViewList.ActionID.ToString() %>' name='m_<%=mViewList.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %>/>
        <%} %>
    </td>
    <td id="<%=bp.ActionID %>Execute" class="ms-vb2" style="text-align:center">
		<% if (mExecute != null) {
		 access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, mExecute.ActionID);
		 disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == mExecute.ActionID);
               %>
        <input <%=disable ? "disabled='disabled'" : "" %> style="width: 100%" id='m_<%=mExecute.ActionID.ToString() %>' name='m_<%=mExecute.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %>/>
        <%} %>
    </td>
</tr>
<% alt = !alt; %>
<% foreach (SPM_Action bm in allbm.Where(o => o.SystemName != "Edit" &&
       o.SystemName != "View" &&
	   o.SystemName != "ViewList" &&
	   o.SystemName != "Delete" &&
       o.SystemName != "CreateNew" &&
	   o.SystemName != "Execute"
	   )) {
		   access = AppSPM.Instance.CheckRoleAccess(ViewData.RoleID, bm.ActionID);
		   disable = access && !ViewData.SPM_RoleAccesses.Any(o => o.ActionID == bm.ActionID);
       %>
<tr <%= alt ? "class='ms-alternating'" : "" %>>
    <td class="ms-vb2" style="padding-left:40px"><%=bm.Title %></td>
    <td class="ms-vb2"></td>
    <td class="ms-vb2"></td>
    <td class="ms-vb2"></td>
    <td class="ms-vb2"></td>
    <td class="ms-vb2"></td>
    <td id="<%=bp.ActionID %>Execute" class="ms-vb2" style="text-align:center">
        <input <%=disable ? "disabled='disabled'" : "" %> id='m_<%=bm.ActionID.ToString() %>' name='m_<%=bm.ActionID.ToString() %>' type="checkbox" <% if (access) { %>checked="checked"<% } %> />
    </td>
</tr>
<% alt = !alt; %>
<% } %>
<% } %>
<% } %>

</table>

<br />

<table class="ms-formtoolbar" style="width:100%">
<tr>
<td style="width:100%"></td>
<td>
<asp:Button ID="bOK" runat="server" Text="Сохранить полномочия" OnClick="bOK_Click" CssClass="ms-ButtonHeightWidth" Width="150px" />
</td>
<td>
<nw:BackButton runat="server" ID="BackButton2" />
</td>
</tr>
</table>


<nw:ModalDialog ID="adselect" runat="server" Title="Группы и пользователи" OnPopulate="adselect_Populate" OnOKClick="adselect_OKClick">
<ContentTemplate>
<table cellpadding="0" cellspacing="0" style="width: 100%; padding-right:4px">
<tr>
<td style="width:100%"><asp:TextBox ID="tbAdFilter" runat="server" Width="100%"></asp:TextBox></td>
<td><asp:Button ID="bAdFind" runat="server" Text="Найти" OnClick="bAdFind_Click"></asp:Button></td>
</tr>
<tr>
<td colspan="2">
<div style="overflow: auto; width:100%; height: 300; border: 1px gray solid; padding:0; margin: 0px">
<asp:CheckBoxList ID="cblUsers" runat="server" DataTextField="Title" DataValueField="SID"></asp:CheckBoxList>
</div>
</td>
</tr>
</table>
</ContentTemplate>
</nw:ModalDialog>

<nw:ModalDialog ID="roleselect" runat="server" Title="Роли" OnPopulate="roleselect_Populate" OnOKClick="roleselect_OKClick">
<ContentTemplate>
<asp:CheckBoxList ID="cblRoles" runat="server" DataTextField="Title" DataValueField="RoleID"></asp:CheckBoxList>
<br />
<asp:Label ID="lMsg" runat="server" Text="" ForeColor="Red" />
</ContentTemplate>
</nw:ModalDialog>


<script type="text/javascript">
function allcheck (value, bpid, method)
{
    var tds = document.getElementsByTagName("td");
    var cb = null;
    
    for (var i = 0; i < tds.length; i++)
    {
        if (tds[i].id == bpid + method)
        {
            cb = tds[i].getElementsByTagName("input");
            if (cb.length > 0 && !cb[0].disabled) cb[0].checked = value;
        }
    }
}
</script>
