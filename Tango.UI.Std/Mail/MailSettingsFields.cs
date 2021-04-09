using System;
using System.Linq;
using Tango.Data;
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
        
        public class MailTemplate : EntityField<MailSettings, Mail.MailTemplate, int>
        {
            public override string Hint { get; set; } = string.Empty;
            
            [Inject] public IDatabase Database { get; set; }

            public override string StringValue => Database.Repository<Mail.MailTemplate>().List()
                .Where(o => o.MailTemplateID == ViewData.MailTemplateID)
                .Select(o => o.Title).FirstOrDefault();

            public override void SubmitProperty(ValidationMessageCollection val)
            {
                ViewData.MailTemplateID = FormValue;
            }

            public override Mail.MailTemplate PropertyValue => Database.Repository<Mail.MailTemplate>().List()
                .FirstOrDefault(o => o.MailTemplateID == ViewData.MailTemplateID);

            public override Func<ValidationBuilder<int>, ValidationBuilder<int>> ValidationFunc => vb => vb.NotNull();
            //public override bool IsRequired => true;
            //public override bool ReadOnly => ViewData.ConsumerContract_SAP_ID != null || (bool)Args["editSAPLinkOnly"];
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