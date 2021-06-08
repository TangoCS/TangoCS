using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
    public class MailSettings: IEntity, IWithKey<MailSettings, int>, IWithTitle
    {
        public int ID => MailSettingsID;
        public Expression<Func<MailSettings, bool>> KeySelector(int id)
        {
            return o => o.MailSettingsID == id;
        }
        
        [Key]
        [Identity]
        [Column]
        public virtual int MailSettingsID { get; set; }
        public int MailTemplateID { get; set; }
        [Column]
        public virtual string Title { get; set; }
        [Column]
        public virtual string PreProcessingMethod { get; set; }
        [Column]
        public virtual string PostProcessingMethod { get; set; }
        [Column]
        public virtual string AfterSentMethod { get; set; }
        [Column]
        public virtual string DeleteMethod { get; set; }
        [Column]
        public virtual int TimeoutValue { get; set; }
        [Column]
        public virtual int SendMailDayInterval { get; set; }
        [Column]
        public virtual TimeSpan SendMailStartInterval { get; set; }
        [Column]
        public virtual TimeSpan SendMailFinishInterval { get; set; }
        [Column]
        public virtual int? AttemptsToSendCount { get; set; }
        [Column]
        public virtual int MailCategoryID { get; set; }
        [Column]
        public virtual DateTime LastModifiedDate { get; set; }
        [Column]
        public virtual object LastModifiedUserID { get; set; }
        [Column]
        public virtual DateTime CreateDate { get; set; }
        [Column]
        public virtual string SystemName { get; set; }
        
        public string MailTemplateTitle { get; set; }
        public string MailCategoryTitle { get; set; }
        public bool HasTemplate { get; set; }
        public string Recipients { get; set; }
    }
}