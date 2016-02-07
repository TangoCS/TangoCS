<%@ Control Language="C#" AutoEventWireup="true" Inherits="Nephrite.Web.Controls.Filter" Codebehind="ListFilter.ascx.cs" %>
<%@ Register Assembly="Nephrite.Web" TagPrefix="nw" Namespace="Nephrite.Web.Controls" %>
<%@ Register src="ModalDialog.ascx" tagname="ModalDialog" tagprefix="uc1" %>
<%@ Import Namespace="Nephrite.Web" %>


<uc1:ModalDialog ID="filter" runat="server"  Width="650px" OnPopulate="filter_Populate" OnOKClick="filter_OKClick">
<ContentTemplate>

<% int arg = filter.Argument.ToInt32(0); %>

<nw:TabControl runat="server" ID="tcFilter">

<nw:TabPage runat="server" ID="tp0">
<ContentTemplate>

<%=AppLayout.Current.FormTableBegin() %>
<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.Title", "Название"))%>
<asp:TextBox ID="tbTitle" runat="server" Width="100%"></asp:TextBox>
<%=AppLayout.Current.FormRowEnd()%>

<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.PropertiesOfVisibility", "Свойства видимости"))%>
<% cbShared.Enabled = cbPersonal.Enabled = AccessControl.Check("filter.managecommonviews");%>

<asp:RadioButton ID="cbPersonal" GroupName="isshared" runat="server"/><br />
<asp:RadioButton ID="cbShared" GroupName="isshared" runat="server" /><br />
<asp:CheckBox ID="cbDefault" runat="server" /><br />

<%=AppLayout.Current.FormRowEnd()%>

<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.Tabs.Properties.TemplateParametersOfList", "Шаблон параметров списка"),
                              TextResource.Get("System.Filter.Tabs.Properties.TemplateParametersComment", "Строка в формате параметр1=значение,параметр2=значение"), false)%>
<asp:TextBox ID="tbParms" runat="server" Width="100%"></asp:TextBox>
<%=AppLayout.Current.FormRowEnd()%>
<%=AppLayout.Current.FormTableEnd()%>

</ContentTemplate>
</nw:TabPage>

<nw:TabPage runat="server" ID="tp1" >
<ContentTemplate>
<fieldset>
<legend><%=TextResource.Get("System.Filter.Tabs.Filter.Properties", "Параметры фильтра")%></legend>
<table style="width:100%">
<tr>
<td><asp:LinkButton runat="server" ID="lbClear" OnClick="lbClear_Click"><%=TextResource.Get("System.Filter.ClearFilter", "Очистить фильтр")%></asp:LinkButton> </td>
<td style="text-align:right"><asp:LinkButton runat="server" ID="lbSwitchMode" OnClick="lbSwitchMode_Click" /></td>
</tr>
</table>
<asp:HiddenField runat="server" ID="mode" />
<table cellpadding="5" cellspacing="4" width="100%" class="ms-alternating">
<%if (rptFilter.Items.Count == 0)
  { %>
<tr><td><%=TextResource.Get("System.Filter.NotElements", "Фильтр не содержит элементов")%></td></tr>
<%} %>
<%if (mode.Value == "S")
  { %>
<asp:Repeater ID="rptFilter" runat="server" OnItemDataBound="rptFilter_ItemDataBound">
<ItemTemplate>
    <tr>
        <td style="width:100%"><%#Eval("Title") + " " + (Eval("Condition").ToString() == TextResource.Get("System.Filter.LastXDays", "последние x дней") ? String.Format(TextResource.Get("System.Filter.LastDays", "последние &quot;{0}&quot; дней"), Eval("ValueTitle").ToString()) : Eval("Condition") + " &quot;" + HttpUtility.HtmlEncode((string)Eval("ValueTitle")) + "&quot;")%></td>
        <td><asp:ImageButton ID="ImageButton1" runat="server" OnClick="delItem_Click" ImageUrl="/i/delete.gif" /></td>
    </tr>
</ItemTemplate>
</asp:Repeater>
<%}
  else
  {%>
<asp:Repeater ID="rptAdvFilter" runat="server" OnItemDataBound="rptAdvFilter_ItemDataBound">
<ItemTemplate>
    <tr>
		<td>
			<asp:DropDownList runat="server" ID="ddlNot" AutoPostBack="true" OnSelectedIndexChanged="update_items">
				<asp:ListItem Text="" Value="" />
			</asp:DropDownList>
		</td>
		<td>
			<asp:TextBox MaxLength="3" runat="server" ID="lp" Width="30px" AutoPostBack="true" OnTextChanged="update_items" />
		</td>
        <td style="width:100%"><%#Eval("Title") + " " + (Eval("Condition").ToString() == TextResource.Get("System.Filter.LastXDays", "последние x дней") ? String.Format(TextResource.Get("System.Filter.LastDays", "последние &quot;{0}&quot; дней"), Eval("ValueTitle").ToString()) : Eval("Condition") + " &quot;" + HttpUtility.HtmlEncode((string)Eval("ValueTitle")) + "&quot;")%></td>
		<td>
			<asp:TextBox MaxLength="3" runat="server" ID="rp" Width="30px"  AutoPostBack="true" OnTextChanged="update_items" />
		</td>
        <td>
			<asp:DropDownList runat="server" ID="ddlOp" AutoPostBack="true" OnSelectedIndexChanged="update_items">
			</asp:DropDownList>
		</td>
		<td style="white-space:nowrap">
			<span style="visibility:<%#Container.ItemIndex>0?"visible":"hidden" %>">
			<asp:ImageButton ID="up" runat="server" OnClick="upItem_Click" ImageUrl="/i/moveup.gif" />
			</span>
			<span style="visibility:<%#Container.ItemIndex<FilterList.Count - 1?"visible":"hidden" %>">
			<asp:ImageButton ID="down" runat="server" OnClick="downItem_Click" ImageUrl="/i/movedown.gif" />
			</span>
			<asp:ImageButton ID="delItem" runat="server" OnClick="delItem_Click" ImageUrl="/i/delete.gif"/>
		</td>
    </tr>
</ItemTemplate>
</asp:Repeater>
<%} %>
</table>
<%=mode.Value != "S" ? ToText() : "" %>
</fieldset>

<fieldset>
<legend><%=TextResource.Get("System.Filter.Criterion", "Критерий")%></legend>
<div style="height:5px;"></div>
<%=AppLayout.Current.FormTableBegin(new { style = "width:100%"})%>
<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.Field", "Поле"))%>
<asp:DropDownList ID="ddlItem" runat="server" DataTextField="Title" DataValueField="Title" AutoPostBack="true" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" Width="100%" />
<%=AppLayout.Current.FormRowEnd()%>
<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.Condition", "Условие"))%>
<asp:DropDownList ID="ddlCondition" runat="server" OnSelectedIndexChanged="ddlCondition_SelectedIndexChanged" AutoPostBack="true" />
<%=AppLayout.Current.FormRowEnd()%>
<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.Value", "Значение"))%>
<asp:TextBox ID="txtValue" runat="server" Width="100%"/>
<nw:JSCalendar ID="calValue" runat="server"/>
<asp:DropDownList ID="ddlValue" runat="server" Width="100%"/>
<asp:CheckBox ID="cbxValue" runat="server" />
<%=AppLayout.Current.FormTableEnd()%>


<div style="text-align:right">
<%addItem.Enabled = ddlItem.SelectedIndex > 0; %>
<asp:Button ID="addItem" runat="server" OnClick="addItem_Click" />
</div>
</fieldset>


</ContentTemplate>
</nw:TabPage>

<nw:TabPage runat="server" ID="tp2">
<ContentTemplate>

<fieldset style="padding:10px">
<legend><%=TextResource.Get("System.Filter.ColumnsToDisplay", "Отображаемые столбцы")%></legend>
<asp:CheckBoxList runat="server" ID="cblColumns" />
</fieldset>

<%=AppLayout.Current.FormTableBegin() %>
<%=AppLayout.Current.FormRowBegin(TextResource.Get("System.Filter.NumberRowsOnPage","Количество строк на странице"))%>
<asp:TextBox ID="tbItemsOnPage" runat="server" Width="100%"></asp:TextBox>
<%=AppLayout.Current.FormRowEnd()%>
<%=AppLayout.Current.FormTableEnd() %>

</ContentTemplate>
</nw:TabPage>

<nw:TabPage runat="server" ID="tp3">
<ContentTemplate>

<%=AppLayout.Current.ListTableBegin(new { id = "listsort", style = "width:100%" })%>
<%=AppLayout.Current.ListHeaderBegin()%>
	<%=AppLayout.Current.TH(TextResource.Get("System.Filter.Column","Столбец"), new { style = "width:100%" })%>
	<%=AppLayout.Current.TH(TextResource.Get("System.Filter.Mode","Способ"))%>
	<%=AppLayout.Current.TH("")%>
<%=AppLayout.Current.ListHeaderEnd()%>
<%=AppLayout.Current.ListRowBegin("", new { id = "listsort_1" })%>
<%=AppLayout.Current.TDBegin()%>
	<asp:DropDownList ID="ddlSortColumn" runat="server" Width="100%" DataTextField="Name" DataValueField="SeqNo"></asp:DropDownList>
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.TDBegin()%>
	<asp:DropDownList ID="ddlSortOrder" runat="server" >
	</asp:DropDownList>
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.TDBegin()%>
	<a title="Удалить" href="#" onclick="delRow(this.parentNode.parentNode)" id="delBtn" style="display:none">
		<img border="0" style="border: 0pt none; vertical-align: middle;" title="Удалить" alt="Удалить" src="/i/n/delete.gif"/>
	</a>
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.ListRowEnd()%>
<%=AppLayout.Current.ListTableEnd()%>

<a href="#" onclick="addRow('listsort',1); return false;"><%=TextResource.Get("System.Filter.AddRow","Добавить строку")%></a>
<script type="text/JavaScript">
function addRow(id, sourceRow) {
	var table = document.getElementById(id);
	var allRows = table.tBodies[0].rows;
	var cRow = allRows[sourceRow].cloneNode(true)
	var cInp = cRow.getElementsByTagName('input');
	for (var i = 0; i < cInp.length; i++) {
		cInp[i].setAttribute('name', cInp[i].getAttribute('name') + '_' + (allRows.length));
		cInp[i].setAttribute('id', cInp[i].getAttribute('id') + '_' + (allRows.length));
		cInp[i].value = '';
	}
	cInp = cRow.getElementsByTagName('select');
	for (var i = 0; i < cInp.length; i++) {
		cInp[i].setAttribute('name', cInp[i].getAttribute('name') + '_' + (allRows.length));
		cInp[i].setAttribute('id', cInp[i].getAttribute('id') + '_' + (allRows.length));
	}
	cRow.id = id + '_' + (allRows.length);
	var links = cRow.getElementsByTagName('a');
	for (var i = 0; i < links.length; i++) {
		if (links[i].style.display == 'none') links[i].style.display = 'block';
	}
	
	table.tBodies[0].appendChild(cRow);
}
function delRow(row) {
	row.parentNode.removeChild(row);
}

function initsorttab() {
var c1, c2;
<% string[] sort = null; 
if (FilterObject.Sort != null)
{
	sort = FilterObject.Sort.Split(new char[] {','}); %>
<% for (int i = 0; i < sort.Length; i++) { %>
<% string[] sort_val = sort[i].Split(new char[] {'_'}); %>
<% if (i > 0) { %>addRow('listsort',1);<% } %>
c1 = document.getElementById('<%=ddlSortColumn.ClientID %><%=i > 0 ? ("_" + (i+1).ToString()) : ""%>');
c2 = document.getElementById('<%=ddlSortOrder.ClientID %><%=i > 0 ? ("_" + (i+1).ToString()) : ""%>');
c1.value = '<%=sort_val[0]%>';
<% if (sort_val.Length > 1) { %>c2.value = '1';<% } else { %>c2.value = '0';<%} %>
<% } %>
<% } %>
}
</script>

</ContentTemplate>
</nw:TabPage>



<nw:TabPage runat="server" ID="tpGrouping">
<ContentTemplate>
<%=AppLayout.Current.ListTableBegin(new { style = "width:100%" })%>
<%=AppLayout.Current.ListHeaderBegin()%>
    <%=AppLayout.Current.TH(TextResource.Get("System.Filter.Column","Столбец"), new { style = "width:100%" })%>
	<%=AppLayout.Current.TH(TextResource.Get("System.Filter.Mode","Способ"))%>
<%=AppLayout.Current.ListHeaderEnd()%>
<%=AppLayout.Current.ListRowBegin("")%>
<%=AppLayout.Current.TDBegin()%>
<asp:DropDownList ID="ddlGroup1" runat="server" DataTextField="Name" DataValueField="SeqNo"  Width="100%" />
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.TDBegin()%>
	    <asp:DropDownList ID="ddlGroup1Order" runat="server">
	    </asp:DropDownList>
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.ListRowEnd()%>
<%=AppLayout.Current.ListRowBegin("")%>
<%=AppLayout.Current.TDBegin()%>
<asp:DropDownList ID="ddlGroup2" runat="server" DataTextField="Name" DataValueField="SeqNo"  Width="100%" />
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.TDBegin()%>
        <asp:DropDownList ID="ddlGroup2Order" runat="server">
	    </asp:DropDownList>
<%=AppLayout.Current.TDEnd()%>
<%=AppLayout.Current.ListRowEnd()%>
<%=AppLayout.Current.ListTableEnd()%>
</ContentTemplate>
</nw:TabPage>

</nw:TabControl>

<% if (tp0.Visible && arg > 0) { %>
<a href="#" ID="delItem1" onclick="return confirm(document.getElementById('messageDel').textContent);" onserverclick="del_Click" runat="server"><img src="<%=Settings.ImagesPath + "delete.gif"%>" alt="Удалить представление" class="middle" /></a>
<a href="#" ID="delItem2" onclick="return confirm(document.getElementById('messageDel').textContent);" onserverclick="del_Click" runat="server"><%=TextResource.Get("System.Filter.Tabs.Grouping.DeleteCurrentView", "удалить текущее представление")%></a>
<div ID="messageDel" style="display:none;" ><%=TextResource.Get("System.Filter.Tabs.Grouping.MessageDelete", "Вы действительно хотите удалить текущее представление?")%></div>
<% } %>

<asp:Label runat="server" ForeColor="Red" ID="lMsg" />
</ContentTemplate>



</uc1:ModalDialog>

<asp:Label runat="server" ForeColor="Red" ID="lMsg2" />

