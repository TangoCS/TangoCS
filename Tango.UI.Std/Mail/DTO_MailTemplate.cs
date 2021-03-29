using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    public class DTO_MailTemplate: IEntity, IWithKey<DTO_MailTemplate, int>, IWithTitle
    {
        public int ID => MailTemplateID;
        public Expression<Func<DTO_MailTemplate, bool>> KeySelector(int id)
        {
            return o => o.MailTemplateID == id;
        }
        
        [Key]
        [Column]
        public virtual int MailTemplateID { get; set; }
        public virtual string Title { get; set; }
        public virtual string TemplateSubject { get; set; }
        public virtual string TemplateBody { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual string LastModifiedUser_ID { get; set; }
        public virtual DateTime LastModifiedDate { get; set; }
    }
}