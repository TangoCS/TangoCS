namespace Tango.UI.Std
{
	public class ThemeController : BaseController
	{
		[AllowAnonymous]
		public ActionResult ChangeTheme(string newtheme)
		{
			return new ApiResult(response => {
				response.AddClientAction("domActions", "setCookie", f => new { id = "theme", value = newtheme });
				response.HardRedirectTo("/");
			});
		}
	}
}
