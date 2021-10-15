using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tango.AccessControl;
using Tango.Html;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.Notifications
{
	public class NotificationsHelper
	{
		INotificationRepository repository;
		public NotificationsHelper(INotificationRepository notificationRepository)
		{
			repository = notificationRepository;
		}
		public void TopMenuIcon(LayoutWriter w)
		{
			var cnt = repository.GetUnreadCount();

			w.Li(a => a.ID("notifications").Title("Извещения"), () => {
				w.Span(() => {
					w.Icon("notifications", a => a.Style("position:relative"), () => {
						w.B(a => a.ID("notifications_counter").Class("badge").Class(cnt > 0 ? "" : "hide"), cnt.ToString());
					});
				});
			});
		}

		public void NotificationsPopup(LayoutWriter w)
		{
			var url = new ActionLink(w.Context).ToList<Notification>().Url;
			w.PopupForElement("notifications", a => a.Class("notifications").DataHref(url).DataNewContainer(typeof(NotificationPopupContainer), "notification_list"), options: new PopupOptions { CloseOnClick = true, ProxyName = "contextmenuproxy_closeonlink" });
		}
	}
}
