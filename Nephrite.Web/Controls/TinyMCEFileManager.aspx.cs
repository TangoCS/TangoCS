using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.MVC;
using Nephrite.Web.View;


namespace Nephrite.Web.Controls
{
	public partial class TinyMCEFileManager : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager sm = new ScriptManager();
			sm.ScriptMode = ScriptMode.Release;
			ph.Controls.Add(sm);

			ModalDialogManager mdm = new ModalDialogManager();
			ph.Controls.Add(mdm);

			var request = HttpContext.Current.Request;
			var Query = new Url(request.Url.PathAndQuery, request.RequestContext.RouteData.Values);

			if (Query.GetString("op").IsEmpty())
			{
				TinyMCEFileManager_list l = Page.LoadControl(Settings.BaseControlsPath + "TinyMCEFileManager/list.ascx") as TinyMCEFileManager_list;
				ph.Controls.Add(l);
			}
			else
			{
				TinyMCEFileManager_upload l = Page.LoadControl(Settings.BaseControlsPath + "TinyMCEFileManager/upload.ascx") as TinyMCEFileManager_upload;
				ph.Controls.Add(l);
			}
		}
	}
}
