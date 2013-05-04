<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="edit.ascx.cs" Inherits="Nephrite.Metamodel.View.Triggers_edit" %>

<asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<%=HtmlHelperWSS.FormTableBegin() %>
	<%=HtmlHelperWSS.FormRowBegin("Таблица")%>
		<asp:DropDownList ID="ddlTableName" runat="server" Width="100%" DataTextField="Name" DataValueField="Name" AutoPostBack="true" OnSelectedIndexChanged="ddlTableName_SelectedIndexChanged" />
	<%=HtmlHelperWSS.FormRowEnd()%>
	<%=HtmlHelperWSS.FormRowBegin("Триггер")%>
		<asp:TextBox ID="tbName" runat="server" Width="100%" />
	<%=HtmlHelperWSS.FormRowEnd()%>
	<%=HtmlHelperWSS.FormRowBegin("Включен")%>
		<asp:CheckBox ID="cbEnabled" runat="server" />
	<%=HtmlHelperWSS.FormRowEnd()%>
	<%=HtmlHelperWSS.FormRowBegin("События")%>
		<asp:CheckBox ID="cbUpdate" runat="server" Text="UPDATE" AutoPostBack="true" OnCheckedChanged="ddlTableName_SelectedIndexChanged" />
		<asp:CheckBox ID="cbInsert" runat="server" Text="INSERT" AutoPostBack="true" OnCheckedChanged="ddlTableName_SelectedIndexChanged" />
		<asp:CheckBox ID="cbDelete" runat="server" Text="DELETE" AutoPostBack="true" OnCheckedChanged="ddlTableName_SelectedIndexChanged" />
	<%=HtmlHelperWSS.FormRowEnd()%>
<%=HtmlHelperWSS.FormTableEnd() %>
<asp:Literal ID="msg" runat="server" />
</ContentTemplate>
</asp:UpdatePanel>

<nw:CodeArea ID="tbTemplate" runat="server" />

<%=HtmlHelperWSS.FormToolbarBegin("100%") %>
<%=HtmlHelperWSS.FormToolbarWhiteSpace() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click" CssClass="ms-ButtonHeightWidth" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin() %>
<nw:BackButton ID="BackButton1" runat="server" />
<%=HtmlHelperWSS.FormToolbarItemEnd() %>
<%=HtmlHelperWSS.FormToolbarItemBegin()%>
<asp:UpdatePanel runat="server" ID="upApply">
<ContentTemplate>
<asp:HiddenField ID="hfModifiedDate" runat="server" />
<asp:Button ID="bApply" runat="server" Text="Применить" onclick="bApply_Click" CssClass="ms-ButtonHeightWidth" />
</ContentTemplate>
</asp:UpdatePanel>
<%=HtmlHelperWSS.FormToolbarItemEnd()%>
<%=HtmlHelperWSS.FormToolbarEnd() %>

