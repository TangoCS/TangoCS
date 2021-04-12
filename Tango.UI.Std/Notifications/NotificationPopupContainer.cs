using System;
using System.Collections.Generic;
using System.Text;
using Tango.UI;
using Tango.Html;

namespace Tango.Notifications
{
	public class NotificationPopupContainer : ViewContainer
	{
		public NotificationPopupContainer()
		{
			Mapping.Add("contentbody", "body");
			Mapping.Add("buttonsbar", "footer");
			Mapping.Add("contenttoolbar", "toolbar");
		}
		public override void Render(ApiResponse response)
		{
			response.AddWidget(Context.Sender, w => {
				w.Div(a => a.ID("toolbar"));
				w.Div(a => a.ID("body"));
				w.Div(a => a.ID("footer"));
			});
		}
	}
}
