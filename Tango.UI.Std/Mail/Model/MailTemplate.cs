using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
    public class MailTemplate: IEntity, IWithKey<MailTemplate, int>, IWithTitle
    {
        public int ID => MailTemplateID;
        public Expression<Func<MailTemplate, bool>> KeySelector(int id)
        {
            return o => o.MailTemplateID == id;
        }
        
        [Key]
        [Identity]
        [Column]
        public virtual int MailTemplateID { get; set; }
        [Column]
        public virtual string Title { get; set; }
        [Column]
        public virtual string TemplateSubject { get; set; }
        [Column]
        public virtual string TemplateBody { get; set; }
        [Column]
        public virtual string Comment { get; set; }
        [Column]
        public virtual bool IsSystem { get; set; }
        [Column]
        public virtual DateTime LastModifiedDate { get; set; }
        [Column]
        public virtual object LastModifiedUserID { get; set; }
        [Column]
        public virtual DateTime CreateDate { get; set; }
    }
}