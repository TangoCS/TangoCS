<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="tabledesc.ascx.cs" Inherits="Nephrite.Metamodel.View.Utils.tabledesc" %>
<%@ Import Namespace="Nephrite.Web" %>
<asp:DropDownList runat="server" ID="ddlTable" AutoPostBack="true" OnSelectedIndexChanged="ddlTable_SelectedIndexChanged" DataTextField="Name" DataValueField="Name" />
<br />
<br />
Описание таблицы:<br />
<asp:TextBox runat="server" ID="tbDescription" Width="400px" TextMode="MultiLine" Rows="3" />
<br />
Столбцы:
<%=Layout.FormTableBegin("700px") %>
<%for (int i = 0; i < Columns.Count; i++)
  { %>
<%=Layout.FormRowBegin(Columns[i]) %>
<input name="Col_<%=Columns[i] %>" type="text" style="width:100%" value="<%=Descriptions[i] %>" />
<%=Layout.FormRowEnd()%>
<%} %>
<%=Layout.FormTableEnd() %>
<br />
<asp:Button runat="server" ID="ok" OnClick="ok_Click" Text="Сохранить" />