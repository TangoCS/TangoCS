<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainTree.ascx.cs" Inherits="Nephrite.CMS.View.MainTree" %>
<%@ Import Namespace="Nephrite.Web" %>

<table border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td style="padding: 0px; border: 0px; white-space: nowrap;">
		</td>
		<td style="padding: 0px; border: 0px; width: 100%">
		</td>
		<td style="padding: 0px; border: 0px; text-align: right">
			<nw:ToolbarPopupMenuCompact Title="создать в корне..." runat="server" ID="createMenu" />
		</td>
	</tr>
</table>
<div>
	<nw:TreeView ID="tv" runat="server" />
</div>
