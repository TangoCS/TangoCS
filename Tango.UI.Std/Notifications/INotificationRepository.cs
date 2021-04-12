using System.Collections.Generic;
using Tango.Data;
using Tango.Identity;

namespace Tango.Notifications
{
	public interface INotificationRepository : IRepository<Notification>
	{
		int GetUnreadCount();
		INotificationRepository ReturnAllNotifications();
		void MarkRead(int id);
		void MarkReadAll();
	}
}
