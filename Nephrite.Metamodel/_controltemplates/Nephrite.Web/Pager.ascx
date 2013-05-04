<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="Nephrite.Web.Controls.Pager" %>
<asp:HiddenField ID="hfPager" runat="server" />
<asp:LinkButton ID="lbRefresh" runat="server" />
<script type="text/javascript">
    function <%=ClientID %>_pager(page)
    {
        document.getElementById('<%=hfPager.ClientID %>').value = page;
        <%=Page.ClientScript.GetPostBackEventReference(lbRefresh,"") %>;
    }
</script>
<%=RenderPager()%>