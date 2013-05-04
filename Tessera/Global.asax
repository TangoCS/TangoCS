<%@ Application Language="C#" %>
<%@ Import Namespace="Nephrite.Web" %>
<%@ Import Namespace="Nephrite.Metamodel" %>
<%@ Import Namespace="Nephrite.Metamodel.Model" %>
<%@ Import Namespace="Nephrite.Web.App" %>

<%@ Import Namespace="System.Web.Hosting" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<%@ Import Namespace="System.Net.Security" %>


<script RunAt="server">
	bool check(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}

	protected void Application_Start(object sender, EventArgs e)
	{
		System.Net.ServicePointManager.ServerCertificateValidationCallback =
			new System.Net.Security.RemoteCertificateValidationCallback(check);

		if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\" + ModelAssemblyGenerator.DllName)))
			AppDomain.CurrentDomain.Load(ModelAssemblyGenerator.DllName.Replace(".dll",""));
		if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Nephrite.CMS.dll")))
			AppDomain.CurrentDomain.Load("Nephrite.CMS");
		MacroManager.Register("sid", () => AppSPM.GetCurrentSubjectID());
	}

	protected void Session_Start(object sender, EventArgs e)
	{

	}

	protected void Application_BeginRequest(object sender, EventArgs e)
	{
		HttpContext.Current.Items["RequestBeginDate"] = DateTime.Now;
		AppBase.DataContext.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
	}

	protected void Application_AuthenticateRequest(object sender, EventArgs e)
	{

	}

	protected void Application_Error(object sender, EventArgs e)
	{
		foreach (var ex in HttpContext.Current.AllErrors)
			ErrorLogger.Log(ex, AppBase.DataContext);
	}

	protected void Session_End(object sender, EventArgs e)
	{

	}

	protected void Application_End(object sender, EventArgs e)
	{

	}
</script>