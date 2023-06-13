using Tango.Html;

namespace Tango.UI.Std
{
	[OnAction("utils", "notfoundresources")]
	public class utils_notfoundresources : ViewPagePart
	{
		public override void OnLoad(ApiResponse response)
		{
			var title = Resources.Get("abc.model.utils.notfoundresources");

			response.AddWidget("contentbody", w => {
				var res = Tango.Localization.DefaultResourceManager.Options.NotFoundResources;
				w.Pre(() => {
					foreach (var r in res)
						w.Div(r);
				});
			});
			response.AddWidget("contenttitle", title);
			response.AddWidget("title", title);		 
		}
	}
}
