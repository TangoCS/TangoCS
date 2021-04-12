using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Notifications
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
    public class C_NotificationCategory : IEntity, IWithKey<C_NotificationCategory, int>, IWithTitle
    {
        public int ID => NotificationCategoryID;
        public Expression<Func<C_NotificationCategory, bool>> KeySelector(int id)
        {
            return o => o.NotificationCategoryID == id;
        }

        [Key]
        [Column]
        public virtual int NotificationCategoryID { get; set; }
        
        [Column]
        public virtual string Title { get; set; }
        public string Icon { get; set; }
        
    }
}