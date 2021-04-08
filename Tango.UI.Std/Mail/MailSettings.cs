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
        [Column]
        public virtual string Title { get; set; }
        [Column]
        public virtual int MailTemplateID { get; set; }
        [Column]
        public virtual string CreateMailMethod { get; set; }
        [Column]
        public virtual string PostProcessingMethod { get; set; }
        [Column]
        public virtual string RecipientsMethod { get; set; }
        [Column]
        public virtual int TimeoutValue { get; set; }
        [Column]
        public virtual TimeSpan SendMailStartInterval { get; set; }
        [Column]
        public virtual TimeSpan SendMailFinishInterval { get; set; }
        [Column]
        public virtual int? AttemptsToSendCount { get; set; }
        [Column]
        public virtual int? MailCategoryID { get; set; }
        
        public string MailTemplateTitle { get; set; }
        public string MailCategoryTitle { get; set; }
    }
}