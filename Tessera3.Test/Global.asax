<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="Solution" %>
<%@ Import Namespace="Solution.Configuration" %>
<script RunAt="server">
	protected void Application_Start(object sender, EventArgs e)
	{
		Nephrite.Razor.RazorFormRenderer.Configure("Views");
		Routes.Register(RouteTable.Routes);
	}
</script>
