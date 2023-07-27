using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
    [Table("C_MailCategory")]
	public class MailCategory: IEntity, IWithKey<MailCategory, int>, IWithTitle
    {
        public int ID => MailCategoryID;
        public Expression<Func<MailCategory, bool>> KeySelector(int id)
        {
            return o => o.MailCategoryID == id;
        }

        [Key]
        [Identity]
        [Column]
        public virtual int MailCategoryID { get; set; }
        [Column]
        public virtual int SystemID { get; set; }
        [Column]
        public virtual int MailCategoryTypeID { get; set; }
        public string MailCategoryTypeTitle { get; set; }
        [Column]
        public virtual string Title { get; set; }
        public string SystemName { get; set; }
    }
}