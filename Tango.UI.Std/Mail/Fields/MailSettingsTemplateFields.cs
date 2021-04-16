using System;
using System.Linq;
using Tango.Data;
using Tango.UI;

namespace Tango.Mail
{
    public static class MailSettingsTemplateFields
    {
        public class DefaultGroup : FieldGroup
        {
            public MailTemplateID MailTemplateID { get; set; }
        }
        
        public class MailTemplateID : EntityField<MailSettingsTemplate, int>
        {
            public bool CanRequired { get; set; }
            public override bool IsRequired => CanRequired;
        }
        
    }
}