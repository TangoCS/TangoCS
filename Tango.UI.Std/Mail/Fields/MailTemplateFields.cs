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
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class TemplateBody : EntityField<MailTemplate, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class Comment : EntityField<MailTemplate, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class IsSystem : EntityField<MailTemplate, bool>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class LastModifiedDate : EntityDateTimeField<MailTemplate>
        {
            public override string Hint { get; set; } = string.Empty;
        }
    }
}