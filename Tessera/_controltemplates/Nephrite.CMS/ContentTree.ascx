<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentTree.ascx.cs" Inherits="Nephrite.CMS.View.ContentTree" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Nephrite.CMS.Controllers" %>
<%@ Import Namespace="Nephrite.Web.SPM" %>

<ul>
<% foreach (MM_ObjectType ot in AppMM.DataContext.MM_ObjectTypes.Where(o => o.BaseObjectType.SysName == "SiteObject").OrderBy(o => o.Title))
   {
	   if (ActionSPMContext.Current.Check(ot.SysName + ".Edit", 1) { %>
   <li><%=HtmlHelperWSS.Instance.ActionLink<ContentController>(c => c.ViewList(ot.ObjectTypeID.ToString()), ot.Title)%></li>
<% }
   } %>
</ul>