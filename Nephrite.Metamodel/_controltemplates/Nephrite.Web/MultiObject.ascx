<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiObject.ascx.cs" Inherits="Nephrite.Web.Controls.MultiObject" %>
<%@ Register TagPrefix="cc1" TagName="SelectObjectHierarchic" Src="SelectObjectHierarchic.ascx" %>
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
<ContentTemplate>
<%=ReadOnly ? "" : Html.InternalImageLink(select.RenderRun(), SelectLinkTitle, "list.gif")%>
<% if (ShowMassOperations)
   { %>&nbsp;|&nbsp;<asp:LinkButton ID="lbSelectAll" runat="server" Text="выбрать все" OnClick="lbSelectAll_Click"/>
   <%if (Count() > 0)
	 {%>&nbsp;|&nbsp;<asp:LinkButton ID="lbDeleteAll" runat="server" Text="удалить все" OnClick="lbDeleteAll_Click"/><% }} %>
<br />
<% if (ListSliding) { %>
выбрано <b><%=Count()%></b><%if (Count() > 0)
{%> <a id="sliding<%=ClientID %>showhide" style="text-decoration: none; border-bottom: 1px dashed;" onclick="ShowHide<%=ClientID %>(); return false;" href="#">показать список</a><%} %>
<script>
	function ShowHide<%=ClientID %>() {
        if ($("#sliding<%=ClientID %>").css("display") != "none")
	    {
		    $.cookie("sliding<%=ClientID %>", "0", { path: '/' });
		    $("#sliding<%=ClientID %>").css("display", "none");
	    }
	    else
	    {
		    $.cookie("sliding<%=ClientID %>", "", { path: '/' });
		    $("#sliding<%=ClientID %>").css("display", "block");
	    }
        var lbl = document.getElementById('sliding<%=ClientID %>showhide');
		if (lbl.innerHTML == "показать список")
			lbl.innerHTML = "скрыть список";
		else
			lbl.innerHTML = "показать список";
	}
    function InitSlider<%=ClientID %>() {
        var lbl = document.getElementById('sliding<%=ClientID %>showhide');
	    if ($.cookie("sliding<%=ClientID %>") == "0")
	    {
            lbl.innerHTML = "показать список";
		    $("#sliding<%=ClientID %>").css("display", "none");
	    }
	    else
	    {
            lbl.innerHTML = "скрыть список";
		    $("#sliding<%=ClientID %>").css("display", "block");
	    }
    }
	$(document).ready(InitSlider<%=ClientID %>);
</script>
<div id="sliding<%=ClientID %>" style="display: none;">
<% } %>

<% for (int i = 0; i < Objects.Count; i++)
   {%>
<%=ObjectTitles[i]%> <%=ReadOnly ? "" : plmDelete.RenderImage("delete.gif", "Удалить", Objects[i])%><br />
<%} %>
<% for (int i = 0; i < GuidObjects.Count; i++)
   {%>
<%=ObjectTitles[i]%> <%=ReadOnly ? "" : plmDelete.RenderImage("delete.gif", "Удалить", GuidObjects[i])%><br />
<%} %>
<% if (ListSliding) { %></div><%} %>
<nw:ParameterizedLinkManager ID="plmDelete" runat="server" OnClick="plmDelete_Click" />
</ContentTemplate>
</asp:UpdatePanel>
                
<cc1:SelectObjectHierarchic ID="select" Height="Auto" DataTextField="Title"
runat="server"  MultipleSelect="true" PageSize="20" OnSelected="select_Selected" />

