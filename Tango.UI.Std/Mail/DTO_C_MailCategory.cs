using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    public class DTO_C_MailCategory: IEntity, IWithKey<DTO_C_MailCategory, int>, IWithTitle
    {
        public int ID => MailCategoryID;
        public Expression<Func<DTO_C_MailCategory, bool>> KeySelector(int id)
        {
            return o => o.MailCategoryID == id;
        }

        [Key]
        [Column]
        public virtual int MailCategoryID { get; set; }
        public virtual int SystemID { get; set; }
        public virtual int MailType { get; set; }
        public virtual string Title { get; set; }
        public string SystemName { get; set; }
        
        public IEnumerable<SelectListItem> GetMailTypes() => new List<SelectListItem>
        {
            new SelectListItem("Административная", 0),
            new SelectListItem("Пользовательская", 1)
        };
    }
}