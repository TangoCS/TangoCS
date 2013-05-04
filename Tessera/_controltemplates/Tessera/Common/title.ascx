<%@ Control Language="C#" AutoEventWireup="true" Inherits="View_Common_title" Codebehind="title.ascx.cs" %>
<%@ Import Namespace="Nephrite.Web" %>

<table width="100%" cellspacing="0" cellpadding="0" id="titletbl" >
<tr>
 <td width="100%" valign="top" nowrap="" class="ms-areaseparator">
  <table width="100%" cellspacing="0" cellpadding="0" border="0">
   <tbody>
   <tr><td valign="top" class="ms-titlearea" style="text-align:left; padding-right: 8px"><%=Nephrite.Web.Cramb.Render() %>&nbsp;</td></tr>
   <tr>
	<td valign="top" height="100%" class="ms-pagetitle" id="onetidPageTitle">
	  <h2 class="ms-pagetitle" style="font-size:16pt; font-weight:normal">
		<%=HttpContext.Current.Items["title"]%>
	  </h2>
	</td>
   </tr>
  </tbody>
  </table>
 </td>
<%if (Context.Items["helpdata"] != null)
  { %>
  <td valign="middle" class="ms-areaseparator" style="padding:5px; text-align:center; vertical-align:middle;">
  <span style="cursor:pointer; white-space:nowrap" class="BaloonTooltipTrigger" rel="<%=Context.Items["helpdata"] %>" title="Справка">Информация к странице</span>
  </td>
  <td valign="middle" class="ms-areaseparator" style="padding:5px">
  <img style="cursor:pointer" src="<%=Settings.ImagesPath %>help.gif" class="BaloonTooltipTrigger" rel="<%=Context.Items["helpdata"] %>" alt="Справка" title="Справка" />
  </td>
<%} %>
</tr>
</table>