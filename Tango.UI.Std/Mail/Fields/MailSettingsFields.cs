using System;
using System.Linq;
using Newtonsoft.Json;
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
            public PreProcessingMethod PreProcessingMethod { get; set; }
            public PostProcessingMethod PostProcessingMethod { get; set; }
            public TimeoutValue TimeoutValue { get; set; }
            public SendMailDayInterval SendMailDayInterval { get; set; }
            public SendMailStartInterval SendMailStartInterval { get; set; }
            public SendMailFinishInterval SendMailFinishInterval { get; set; }
            public AttemptsToSendCount AttemptsToSendCount { get; set; }
            public MailCategoryTitle MailCategoryTitle { get; set; }
            public LastModifiedDate LastModifiedDate { get; set; }
            public MailCategoryID MailCategoryID { get; set; }
            public MailTemplateID MailTemplateID { get; set; }
            public SystemName SystemName { get; set; } 
        }
        
        public class MailTemplateTitle : EntityField<MailSettings, string>
        {
        }
        
        public class MailCategoryID : EntityField<MailSettings, int>
        {
        }
        
        public class MailTemplateID : EntityField<MailSettings, int>
        {
            public bool CanRequired { get; set; }
            public override bool IsRequired => CanRequired;
        }
        
        public class PreProcessingMethod : EntityField<MailSettings, MethodSettings>
        {
            public override bool IsRequired { get; set; } = false;
            public override void SubmitProperty(ValidationMessageCollection val)
            {
                //base.SubmitProperty(val);
            }
        }
        
        public class PostProcessingMethod : EntityField<MailSettings, MethodSettings>
        {
            public override bool IsRequired { get; set; } = false;
            public override void SubmitProperty(ValidationMessageCollection val)
            {
                //base.SubmitProperty(val);
            }
        }
        
        public class TimeoutValue : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class SendMailDayInterval : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class SendMailStartInterval : /*EntityTimeField<MailSettings> //*/EntityField<MailSettings, TimeSpan>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class SendMailFinishInterval : EntityField<MailSettings, TimeSpan>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class AttemptsToSendCount : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class MailCategoryTitle : EntityField<MailSettings, string>
        {
        }
        
        public class LastModifiedDate : EntityDateTimeField<MailSettings>
        {
            public override bool IsRequired { get; set; } = false;
        }
        
        public class SystemName : EntityField<MailSettings, string>
        {
            public override bool IsRequired { get; set; } = false;
        }
    }
}