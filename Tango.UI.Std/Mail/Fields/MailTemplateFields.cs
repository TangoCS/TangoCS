using System;
using Tango.UI;

namespace Tango.Mail
{
    public static class MailTemplateFields
    {
        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public TemplateSubject TemplateSubject { get; set; }
            public TemplateBody TemplateBody { get; set; }
            public Comment Comment { get; set; }
            public IsSystem IsSystem { get; set; }
            public LastModifiedDate LastModifiedDate { get; set; }
        }
        
        public class TemplateSubject : EntityField<MailTemplate, string>
        {
        }
        
        public class TemplateBody : EntityField<MailTemplate, string>
        {
        }
        
        public class Comment : EntityField<MailTemplate, string>
        {
        }
        
        public class IsSystem : EntityField<MailTemplate, bool>
        {
        }
        
        public class LastModifiedDate : EntityDateTimeField<MailTemplate>
        {
        }
    }
}