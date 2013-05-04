<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavMenuEx.ascx.cs" Inherits="Nephrite.Web.Controls.NavMenuEx" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>

<table style="width:100%; height:100%;" cellpadding="0" cellspacing="0" border="0" id="navmenuex">
<tr id="navmenutitle"><td class="ms-formbody" style="height:30px;padding:5px;border-bottom:solid 1px #6F9DD9;font-family:Sans-serif;font-weight:bold;font-size: 12pt;">
<%=CurrentGroup != null ? CurrentGroup.Title : String.Empty%>
</td></tr>

<tr><td valign="top" style="border-bottom:solid 1px #6F9DD9;" >
<div class="NavBarList" style="overflow:auto;" id="navmenuexcurrentgrouplist">
<%if (CurrentGroup != null) { %>
<%=RenderMenuGroup(CurrentGroup)%>
<%} %>
</div>
</td></tr>

<%if (Groups != null)
  { %>
<%for (int i = 0; i < Groups.Count; i++) { %>
<tr class="navmenubutton"><td <%=Groups[i] != CurrentGroup ? "onmouseover='navmenuex_hover(this);'onmouseout='navmenuex_unhover(this);'":""%> class="ms-menutoolbar dummy" style="width: 220px; height:<%=Settings.NavMenuButtonsMode == NavMenuButtonsMode.BigButtons ? "32" : "26"%>px;<%=Groups[i] == CurrentGroup ? (" background:#EE9515 url(" + Settings.ImagesPath + "/RadPanelBar/img4.gif) repeat-x scroll 0 0") : "" %>">

        <div class="ms-toolbar" style="white-space: nowrap; padding-left: 3px; border: none; vertical-align:middle">
           <a style="display:block; color:#10110F; line-height:<%=Settings.NavMenuButtonsMode == NavMenuButtonsMode.BigButtons ? "31" : "25"%>px; font-weight:bold; font-size:11px; font-family:Arial,Verdana,Sans-serif; text-decoration:none; width:100%; outline:none" href="<%=Groups[i].Url %>">
                <%if (!String.IsNullOrEmpty(Groups[i].Icon))
                  { %><%=HtmlHelperBase.Instance.Image(Groups[i].Icon, "")%><%} %>
                <span style="color:#10110F; line-height:<%=Settings.NavMenuButtonsMode == NavMenuButtonsMode.BigButtons ? "31" : "25"%>px; font-weight:bold; font-size:11px; font-family:Arial,Verdana,Sans-serif; text-decoration:none" ><%=Groups[i].Title + (Groups[i].Expression.IsEmpty() ? "" : (" " + Groups[i].EvaluateExpression()))%></span>
		   </a>
	</div>

</td></tr>
<%}} %>

</table>

<script type="text/javascript">
	function navmenuex_hover(e) {
		e.className = 'ms-menutoolbar selectedNavMenuExGroup';
	}
	function navmenuex_unhover(e) {
		e.className = 'ms-menutoolbar dummy';
	}
</script>