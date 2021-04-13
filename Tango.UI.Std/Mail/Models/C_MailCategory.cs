using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
    public class C_MailCategory: IEntity, IWithKey<C_MailCategory, int>, IWithTitle
    {
        public int ID => MailCategoryID;
        public Expression<Func<C_MailCategory, bool>> KeySelector(int id)
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
        public virtual int MailType { get; set; }
        [Column]
        public virtual string Title { get; set; }
        public string SystemName { get; set; }
        
        public IEnumerable<SelectListItem> GetMailTypes() => new List<SelectListItem>
        {
            new SelectListItem("Административная", 0),
            new SelectListItem("Пользовательская", 1)
        };
    }
}