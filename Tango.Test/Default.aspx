<%@ Page Language="C#" %>
<form id="form1" runat="server">
<asp:ScriptManager runat="server" ScriptMode="Release" />
<asp:Panel id="p" runat="server"></asp:Panel>
</form>

<script runat="server">
	protected void Page_PreInit(object sender, EventArgs e)
	{
		var control = LoadControl("~/Test.ascx");
		p.Controls.Add(control);
	}
	protected void Page_Load(object sender, EventArgs e)
	{
							
	}
</script>
