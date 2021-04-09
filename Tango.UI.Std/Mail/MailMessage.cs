using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
    public class MailMessage: IEntity, IWithKey<MailMessage, int>, IWithTitle
    {
        public string Title { get; set; }
        
        public virtual Expression<Func<MailMessage, bool>> KeySelector(int id)
        {
            return o => o.MailMessageID == id;
        }
        public virtual int ID => MailMessageID;
        [Key]
        [Identity]
        [Column]
        public virtual int MailMessageID { get; set; }
        [Column]
        public virtual string Recipients { get; set; }
        [Column]
        public virtual string Subject { get; set; }
        [Column]
        public virtual string Body { get; set; }
        [Column]
        public virtual int MailMessageStatusID { get; set; }
        [Column]
        public virtual string AttachmentName { get; set; }
        [Column]
        public virtual byte[] Attachment { get; set; }
        [Column]
        public virtual string Error { get; set; }
        [Column]
        public virtual string CopyRecipients { get; set; }
        [Column]
        public virtual int TimeoutValue { get; set; }
        [Column]
        public virtual DateTime? LastSendAttemptDate { get; set; }
        [Column]
        public virtual int AttemptsToSendCount { get; set; }
        [Column]
        public virtual DateTime LastModifiedDate { get; set; }
        
        public string MailMessageStatus { get; set; }
    }
}