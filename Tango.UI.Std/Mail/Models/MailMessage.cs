using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    public enum MailMessageStatus
    {
        /// <summary>
        /// Новое
        /// </summary>
        New = 1,
        /// <summary>
        /// Отправлено
        /// </summary>
        Sent = 2,
        /// <summary>
        /// Ошибка отправки
        /// </summary>
        SendError = 3,
        /// <summary>
        /// Остановка отправки
        /// </summary>
        StopSending = 4
    }
    
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
        public virtual DateTime CreateDate { get; set; }
        [Column]
        public virtual object LastModifiedUserID { get; set; }
        
        public string LastModifiedUserTitle { get; set; }
        
        public string MailMessageStatus { get; set; }
        
        [Column]
        public virtual string FromName { get; set; }
        [Column]
        public virtual string FromEmail { get; set; }
        [Column]
        public virtual int MaxAttemptsToSendCount { get; set; }
        [Column]
        public virtual int MailCategoryID { get; set; }
        public string MailCategoryTitle { get; set; }
        
        [Column]
        public virtual DateTime? StartSendDate { get; set; }
        [Column]
        public virtual DateTime? FinishSendDate { get; set; }
    }
}