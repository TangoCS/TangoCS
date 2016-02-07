<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkedObjects.ascx.cs" Inherits="Nephrite.Web.Controls.LinkedObjects" %>

<%foreach(var o in GetLinkedObjects()) { %>
<%=o.MetaClass.Caption %>: <%=o.Title %><br />
<%} %>