﻿using System;
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
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class MailCategoryID : EntityField<MailSettings, int>
        {
            public override string Hint { get; set; } = string.Empty;
            public override bool IsRequired => true;
        }
        
        public class MailTemplateID : EntityField<MailSettings, int>
        {
            public bool CanRequired { get; set; }
            public override string Hint { get; set; } = string.Empty;
            public override bool IsRequired => CanRequired;
        }
        
        public class PreProcessingMethod : EntityField<MailSettings, string>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class PostProcessingMethod : EntityField<MailSettings, string>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class TimeoutValue : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SendMailDayInterval : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SendMailStartInterval : /*EntityTimeField<MailSettings> //*/EntityField<MailSettings, TimeSpan>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SendMailFinishInterval : EntityField<MailSettings, TimeSpan>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class AttemptsToSendCount : EntityField<MailSettings, int>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class MailCategoryTitle : EntityField<MailSettings, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class LastModifiedDate : EntityDateTimeField<MailSettings>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
        
        public class SystemName : EntityField<MailSettings, string>
        {
            public override bool IsRequired { get; set; } = false;
            public override string Hint { get; set; } = string.Empty;
        }
    }
}