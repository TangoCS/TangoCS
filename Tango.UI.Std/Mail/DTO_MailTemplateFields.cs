using System;
using Tango.UI;

namespace Tango.Mail
{
    public class DTO_MailTemplateFields
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
        
        public class TemplateSubject : EntityField<DTO_MailTemplate, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class TemplateBody : EntityField<DTO_MailTemplate, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class Comment : EntityField<DTO_MailTemplate, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class IsSystem : EntityField<DTO_MailTemplate, bool>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class LastModifiedDate : EntityDateTimeField<DTO_MailTemplate>
        {
            public override string Hint { get; set; } = string.Empty;
        }
    }
}