<%@ Control Language="C#" AutoEventWireup="true" Inherits="View_Common_linkedobjects" Codebehind="linkedobjects.ascx.cs" %>
<%@ Import Namespace="Nephrite.Web.Controllers" %>


Удаление невозможно, так как имеются связанные объекты:

<table border="0" cellpadding="3" cellspacing="0" style="margin-top: 8px">
<% foreach (LinkedObjectsInfo.RestrictAssociation asso in ViewData.Associations)
   {
	   IModelObject obj = Activator.CreateInstance(asso.LinkedObjectType) as IModelObject;
	   if (obj == null)
	   {
		   %>
<tr>
<td>Тип <%=asso.LinkedObjectType.ToString() %> не приводится к IModelObject</td>		   
<td>&nbsp;</td>		   
<td>&nbsp;</td>		   
</tr>
		   <%
		   continue;
	   }
	   string linkText = obj.GetClassName();
	   if (String.IsNullOrEmpty(linkText)) linkText = asso.LinkedObjectType.Name;
	   %>
<tr>
<td><%=plm.RenderLink(linkText, asso.PropertyName)%></td>
<td style="width:40px">&nbsp;</td>
<td><b><%=asso.LinkedObjectsCount.ToString() %></b></td>
</tr>
<% }%>
</table>

<nw:ParameterizedLinkManager ID="plm" OnClick="detail_Click" runat="server" />


<% if (_list != null) { %>
<table class="ms-listviewtable w700" cellpadding="3" cellspacing="0" style="margin-top: 8px">
<tr class="ms-viewheadertr">
	<th class="ms-vh2">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	ИД
	</td></tr></table>
	</th>
	
	<th class="ms-vh2" width="100%">
	<table class="ms-unselectedtitle"><tr><td class="ms-vb">
	Название
	</td></tr></table>
	</th>
</tr>
<% foreach (object o in _list) { %>
<tr><td class="ms-vb2">
<%=(o as IModelObject).ObjectID%></td><td class="ms-vb2"><%= (o as IModelObject).Title%>
</td></tr>
<% } %>

</table>
<%=RenderPager(PageCount) %>
<% } %>

<br />
<nw:BackButton runat="server" ID="BackButton1" />

