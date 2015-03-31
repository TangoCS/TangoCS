<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavMenuEx.ascx.cs" Inherits="Nephrite.Web.Controls.NavMenuEx" %>
<%@ Import Namespace="Nephrite" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Web.Controls" %>
<nav id="sidebar" class="nav">
<div class="nav-header ms-formbody">
    <%=CurrentGroup != null ? CurrentGroup.Title : String.Empty%>
</div>
<div class="nav-body">
    <%=CurrentGroup != null ? RenderMenuGroup(CurrentGroup) : ""%>
</div>
<%if (Groups != null)
  { %>
<div class="nav-buttonsbar">
<%for (int i = 0; i < Groups.Count; i++)
  {
      var cls = Groups[i] == CurrentGroup ? "ms-MenuUIItemTableHover" : "";
      var evnts = Groups[i] != CurrentGroup ? "onmouseover='navmenuex_hover(this);'onmouseout='navmenuex_unhover(this);'" : "";
      var h = Settings.NavMenuButtonsMode == NavMenuButtonsMode.BigButtons ? "30" : "24";
      %>
<div class="nav-button <%=cls %>" <%=evnts %> style="height:<%=h%>px;">
<a style="line-height:<%=h%>px;" href="<%=Groups[i].Url%>">
<%=Groups[i].Icon.IsEmpty() ? "" : HtmlHelperBase.Instance.Image(Groups[i].Icon, "")%>
<span style="line-height:<%=h%>px;">
    <%=Groups[i].Title + (Groups[i].Expression.IsEmpty() ? "" : (" " + Groups[i].EvaluateExpression()))%>
</span></a>
</div>
<%}%>
</div>
<%}%>
</nav>
<script type="text/javascript">
	function navmenuex_hover(e) {
		e.className = 'nav-button ms-MenuUIItemTableHover';
	}
	function navmenuex_unhover(e) {
		e.className = 'nav-button';
	}
</script>