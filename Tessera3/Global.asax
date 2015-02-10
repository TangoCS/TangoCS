<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="Solution" %>
<script RunAt="server">
	protected void Application_Start(object sender, EventArgs e)
	{
		Routes.Register(RouteTable.Routes);
	}
</script>
