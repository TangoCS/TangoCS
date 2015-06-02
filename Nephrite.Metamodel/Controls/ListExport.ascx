<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListExport.ascx.cs" Inherits="Nephrite.Metamodel.Controls.ListExport" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>

<a href='#' onclick='selectAll()'>Выбрать все</a>&nbsp;|&nbsp;<a href='#' onclick='unselectAll()'>Очистить все</a>
<br /><br />
<div id='fields' style='width:100%'>
<%	var ot = AppMM.DataContext.MM_ObjectTypes.Single(o => o.SysName == ObjectTypeSysName);
	foreach (var prop1 in ot.MM_ObjectProperties.Where(o => o.MM_FormField != null && o.MM_FormField.ShowInView && o.UpperBound == 1).OrderBy(o => o.SeqNo))
	{ 
%>
<input onclick='this.blur();' onchange='toggleProps(checked, "<%=prop1.ObjectPropertyID %>")' type='checkbox' id='ExportProperty<%=prop1.ObjectPropertyID %>' name='ExportProperty<%=prop1.ObjectPropertyID %>' <%=(prop1.UpperBound == 1 && !requiredProperties.Contains(prop1.SysName) ? "" : "disabled='disabled' ") %> <%=((Page.Request.Form["ExportProperty" + prop1.ObjectPropertyID.ToString()] == "on" || !Page.IsPostBack || requiredProperties.Contains(prop1.SysName)) && prop1.UpperBound == 1 ? "checked='checked'" : "") %> />
<%=prop1.Title %><br />
<%	if (prop1.RefObjectType != null)
	{%>
<div id='props<%=prop1.ObjectPropertyID %>' style='margin-left:25px'>
<%	foreach (var prop2 in prop1.RefObjectType.MM_ObjectProperties.Where(o => !o.IsPrimaryKey && o.MM_FormField != null && o.MM_FormField.ShowInView && o.UpperBound == 1).OrderBy(o => o.SeqNo))
	{%>
<input onclick='this.blur();' onchange='toggleParent(checked, "<%=prop1.ObjectPropertyID %>")' type='checkbox' name='ExportProperty<%=prop1.ObjectPropertyID %>_<%=prop2.ObjectPropertyID %>' <%=(prop1.UpperBound == 1 && !requiredProperties.Contains(prop1.SysName + "." + prop2.SysName )? "" : "disabled='disabled' ") %> <%=((Page.Request.Form["ExportProperty" + prop1.ObjectPropertyID.ToString() + "_" + prop2.ObjectPropertyID.ToString()] == "on" || !Page.IsPostBack) && prop1.UpperBound == 1 ? "checked='checked'" : "") %> />
<%=prop2.Title%><br />
<%	} %>
</div>
<%
		}
	}
%>
</div>
<p>Всего записей: <%=ObjectCount%></p>
<script type='text/javascript'>
function selectAll()
{
	var f = document.getElementById('fields');
	var items = f.getElementsByTagName('input');
	for (var i = 0; i < items.length; i++)
		if (!items.item(i).disabled)
			items.item(i).checked = true;
}

function unselectAll()
{
	var f = document.getElementById('fields');
	var items = f.getElementsByTagName('input');
	for (var i = 0; i < items.length; i++)
		if (!items.item(i).disabled)
			items.item(i).checked = false;
}

function toggleProps(checked, id)
{
	var f = document.getElementById('props' + id);
	if (f != null) {
		var items = f.getElementsByTagName('input');
		for (var i = 0; i < items.length; i++)
			items.item(i).checked = checked;
	}
}

function toggleParent(checked, id) {
	var f = document.getElementById('props' + id);
	var items = f.getElementsByTagName('input');
	var ischecked = false;
	for (var i = 0; i < items.length; i++)
		if (items.item(i).checked)
			ischecked = true;
	if (checked)
		ischecked = true;
	document.getElementById('ExportProperty' + id).checked = ischecked;
}
</script>
<br />

<%if (ObjectCount > MaxCount)
  { %>
<span style="color: red">Количество экспортируемых записей превышает <%=MaxCount %>. Пожалуйста, уточните перечень с помощью фильтра.</span>
<%} %>
<asp:UpdatePanel runat="server" ID="up">
<ContentTemplate>
<%=HtmlHelperWSS.FormToolbarBegin("120px") %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton runat="server" ID="BackButton" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarEnd() %>
<img src="<%=Settings.ImagesPath%>loading.gif" id="listexportimgloading" <%=bOK.Enabled ? "style='display:none'" : ""%> alt='Выполняется экспорт' />
<br />
<asp:Label runat="server" ID="lbProgress" />
<asp:HiddenField runat="server" ID="hfGuid" />
</ContentTemplate>
</asp:UpdatePanel>
<asp:LinkButton runat="server" ID="lbDownload" OnClick="lbDownload_Click" />