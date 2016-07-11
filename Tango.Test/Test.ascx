<%@ Control Language="C#" %>
<div>Ascx</div>
<div><%=_title %></div>
<asp:Button ID="bOK" runat="server" Text="ОК" onclick="bOK_Click"  />

<asp:UpdatePanel ID="up1" runat="server">
<ContentTemplate>
	<asp:Label ID="lMess" runat="server" /> 
	<asp:Button ID="bUpdatePanel" runat="server" Text="UpdatePanel" onclick="bOK2_Click"  />
</ContentTemplate>
</asp:UpdatePanel>
<script runat="server">
	string _title = "Page_Load was not called";

	protected void Page_Load(object sender, EventArgs e)
	{
		_title = "Page_Load was called";
	}

	protected void bOK_Click(object sender, EventArgs e)
	{
		_title = "OK";
	}

	protected void bOK2_Click(object sender, EventArgs e)
	{
		lMess.Text = "UpdatePanel is working well!";
	}
</script>