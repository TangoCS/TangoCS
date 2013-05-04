<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Sorter.ascx.cs" Inherits="Nephrite.Web.Controls.Sorter" %>
<asp:HiddenField ID="hfSort" runat="server" />
<asp:LinkButton ID="lbSort" runat="server" OnClick="lbSort_Click" />
<script type="text/javascript">
    function <%=ClientID %>_sort(column)
    {
        document.getElementById('<%=hfSort.ClientID %>').value = column;
        <%=Page.ClientScript.GetPostBackEventReference(lbSort,"") %>;
    }
</script>