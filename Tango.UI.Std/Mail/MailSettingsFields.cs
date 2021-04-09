using System;
using Tango.UI;

namespace Tango.Mail
{
    public static class MailSettingsFields
    {
        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public MailTemplateTitle MailTemplateTitle { get; set; }
            public CreateMailMethod CreateMailMethod { get; set; }
            public PostProcessingMethod PostProcessingMethod { get; set; }
            public RecipientsMethod RecipientsMethod { get; set; }
            public TimeoutValue TimeoutValue { get; set; }
            public SendMailStartInterval SendMailStartInterval { get; set; }
            public SendMailFinishInterval SendMailFinishInterval { get; set; }
            public AttemptsToSendCount AttemptsToSendCount { get; set; }
            public MailCategoryTitle MailCategoryTitle { get; set; }
            public LastModifiedDate LastModifiedDate { get; set; }
            public MailTemplate MailTemplate { get; set; }
        }
        
        public class MailTemplateTitle : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class MailTemplate : EntityField<MailSettings, int>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class CreateMailMethod : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class PostProcessingMethod : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class RecipientsMethod : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class TimeoutValue : EntityField<MailSettings, int>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SendMailStartInterval : EntityField<MailSettings, TimeSpan>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SendMailFinishInterval : EntityField<MailSettings, TimeSpan>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class AttemptsToSendCount : EntityField<MailSettings, int>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class MailCategoryTitle : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class LastModifiedDate : EntityDateTimeField<MailSettings>
        {
            public override string Hint { get; set; } = string.Empty;
        }
    }
}