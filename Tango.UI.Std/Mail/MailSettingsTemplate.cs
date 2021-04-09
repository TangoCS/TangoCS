using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Mail
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Mail)]
    public class MailSettingsTemplate : IEntity, IWithKey<MailSettingsTemplate, int>, IWithTitle
    {
        public int ID => MailSettingsTemplateID;
        public Expression<Func<MailSettingsTemplate, bool>> KeySelector(int id)
        {
            return o => o.MailSettingsTemplateID == id;
        }
        
        [Key]
        [Identity]
        [Column]
        public virtual int MailSettingsTemplateID { get; set; }
        
        [Column]
        public virtual int MailTemplateID { get; set; }
        
        [Column]
        public virtual int MailSettingsID { get; set; }
        
        [Column]
        public virtual DateTime StartDate { get; set; }
        
        [Column]
        public virtual DateTime FinishDate { get; set; }

        public string Title { get; }
    }
}