namespace Tango.UI.Std
{
	public class ConnectionController : BaseController
	{
		[AllowAnonymous]
		public ActionResult ChangeConnection(string newconn)
		{
			return new ApiResult(response => {
				response.AddClientAction("domActions", "setCookie", f => new { id = "conn", value = newconn });
				response.HardRedirectTo("/");
			});
		}
	}
}
