<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectObjectHierarchic.ascx.cs" Inherits="Nephrite.Web.Controls.SelectObjectHierarchic" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<%@ Register src="ModalDialog.ascx" tagname="ModalDialog" tagprefix="uc1" %>

<uc1:ModalDialog Title="Выберите объект" ID="select" runat="server" Width="700px" Top="50px" OnPopulate="select_Populate" HideOnOK="true" >
<NonAJAXContentTemplate>
    <input
        style="width:100%"
        type="text"
        onblur="if(this.value ==''){this.value='<%=SearchText %>';this.className = 'filterInput TextItalic';this.s=''}"
        onfocus="if(this.s!='t' && <%=hfQuickFilter.ClientID %>.value == ''){this.value='';this.className = 'filterInput filterInputActive';this.s='t';}"
        autocomplete="Off"
        value="<%=hfQuickFilter.Value.IsEmpty() ? SearchText : hfQuickFilter.Value %>"
        id="text_<%=ClientID %>"
        <%if(hfQuickFilter.Value.IsEmpty()) { %>class="filterInput TextItalic"<% } %>
        onkeyup="filter_<%=ClientID %>();"/>
</NonAJAXContentTemplate>
<ContentTemplate>
<div style="overflow-y:auto; height: expression( this.scrollHeight > <%=String.IsNullOrEmpty(Height) ? "450px" : Height.Replace("px", "")%> ? '<%=Height ?? "450px"%>' : 'auto' ); max-height:<%=Height ?? "450px"%>; margin-top:5px; margin-bottom:5px">
<asp:Literal ID="sled" runat="server" EnableViewState="false" />
<asp:RadioButtonList runat="server" ID="rblItems" />
<asp:CheckBoxList runat="server" ID="cblItems" />
</div>
<%=AppLayout.Current.Paging.RenderPager("gotopage_" + ClientID, select.PageIndex, select.PageCount)  %>
<asp:LinkButton runat="server" ID="lbRefresh" />
<asp:HiddenField runat="server" ID="hfQuickFilter" />
<asp:HiddenField runat="server" ID="hfParentID" />
<asp:HiddenField runat="server" ID="hfSelectedID" />
<asp:HiddenField runat="server" ID="hfSelectedTitle" />
<asp:HiddenField runat="server" ID="hfSelectedIDs" />
<asp:HiddenField runat="server" ID="hfSelectedTitles" />
<asp:HiddenField runat="server" ID="hfHeight" />
<asp:HiddenField runat="server" ID="hfCheckType" />
<asp:LinkButton runat="server" ID="lbOKClick" OnClick="lbOKClick_Click" />
</ContentTemplate>
<BottomLeftContentTemplate>
<%if (MultipleSelect && AllowSelectAll)
  { %>
<a href="#" onclick="check_<%=ClientID %>(true); return false;">выбрать все</a> | <a href="#" onclick="check_<%=ClientID %>(false); return false;">отменить все</a>
<%} %>
</BottomLeftContentTemplate>
</uc1:ModalDialog>

<script type="text/javascript">
var timer_<%=ClientID %> = 0;
var isIE=document.all;
document.getElementById('<%=hfHeight.ClientID%>').value = getPageSize()[3];
function filter_<%=ClientID %>()
{
    if(timer_<%=ClientID %>)
	{
		window.clearTimeout(timer_<%=ClientID %>);
		timer_<%=ClientID %> = null;
	}
    timer_<%=ClientID %> = window.setTimeout(runfilter_<%=ClientID %>, 400);
}
function runfilter_<%=ClientID %>()
{
    document.getElementById('<%=hfQuickFilter.ClientID%>').value = document.getElementById('text_<%=ClientID %>').value;
    <%=select.SkipDisableFunction %>;
    <%=select.SetPageIndexFunction %>(1);
    <%=Page.ClientScript.GetPostBackEventReference(lbRefresh, "") %>
}
function gotopage_<%=ClientID %>(page)
{
    <%=select.SetPageIndexFunction %>(page);
    <%=Page.ClientScript.GetPostBackEventReference(lbRefresh, "") %>
}
function check_<%=ClientID %>(check)
{
	if (check)
		document.getElementById('<%=hfCheckType.ClientID%>').value = 'check';
	else
		document.getElementById('<%=hfCheckType.ClientID%>').value = 'uncheck';
	<%=Page.ClientScript.GetPostBackEventReference(lbRefresh, "") %>
}
function selectOK_<%=ClientID %>()
{
    <%if(MultipleSelect){ %>
    <%=Page.ClientScript.GetPostBackEventReference(lbOKClick, "") %>
    return true;
    <%}else{ %>
    var tbl = document.getElementById('<%=rblItems.ClientID %>');
	if (tbl != null)
	{
		var items = tbl.getElementsByTagName('input');
		for(var i = 0; i < items.length; i++)
		{
			if (items.item(i).checked)
			{
				var item = items.item(i);
				if (isIE)
				{
					<%=OnSelect %>(item.nextSibling.outerText, item.value);
					document.getElementById('<%=hfSelectedTitle.ClientID %>').value = item.nextSibling.outerText;
				}
				else
				{
					<%=OnSelect %>(item.nextSibling.textContent, item.value);
					document.getElementById('<%=hfSelectedTitle.ClientID %>').value = item.nextSibling.textContent;
				}
				<%if(HasSelectedHandler){ %>
				document.getElementById('<%=hfSelectedID.ClientID %>').value = item.value;
			
				<%=Page.ClientScript.GetPostBackEventReference(lbOKClick, "") %>
				<%} %>
				return true;
			}
		}
	}
    alert('<%=TextResource.Get("Common.Controls.SelectObjectHierarchic.MustSelectAlert", "Выберите объект!") %>');
    return false;
    <%} %>
}
function godeeper_<%=ClientID %>(id)
{
    document.getElementById('text_<%=ClientID %>').value = '';
    document.getElementById('<%=hfParentID.ClientID%>').value = id;
    <%=select.SetPageIndexFunction %>(1);
    <%=Page.ClientScript.GetPostBackEventReference(lbRefresh, "") %>
}
</script>