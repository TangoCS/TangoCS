using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text;
using Tango.Data;

namespace Tango.Notifications
{
	[BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
	public class Notification
	{
        public int ID => NotificationID;
        public Expression<Func<Notification, bool>> KeySelector(int id)
        {
            return o => o.NotificationID == id;
        }

        [Key]
        [Column]
        [Identity]
        public virtual int NotificationID { get; set; }
        [Column]
        public virtual DateTime CreateDate { get; set; }
        [Column]
        public virtual string NotificationText { get; set; }
        [Column]
        public virtual int NotificationCategoryID { get; set; }
        [Column]
        public virtual DateTime? ExpirationDate { get; set; }
        public string NotificationCategoryTitle { get; set; }
        public string NotificationCategoryIcon { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
