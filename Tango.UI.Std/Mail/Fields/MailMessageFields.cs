using System;
using System.Linq;
using Newtonsoft.Json;
using Tango.Data;
using Tango.UI;

namespace Tango.Mail
{
    public static class MailMessageFields
    {
        public class DefaultGroup : FieldGroup
        {
            public MailCategoryTitle MailCategoryTitle { get; set; }
            public Subject Subject { get; set; }
            public Body Body { get; set; }
            public CreateDate CreateDate { get; set; }
            public TimeoutValue TimeoutValue { get; set; }
            public StartSendDate StartSendDate { get; set; }
            public FinishSendDate FinishSendDate { get; set; }
            public MaxAttemptsToSendCount MaxAttemptsToSendCount { get; set; }
            public Recipients Recipients { get; set; }
            public CopyRecipients CopyRecipients { get; set; }
            public LastModifiedUserTitle LastModifiedUserTitle { get; set; }
            public MailMessageStatus MailMessageStatus { get; set; }
            public AttemptsToSendCount AttemptsToSendCount { get; set; }
            public LastSendAttemptDate LastSendAttemptDate { get; set; }
            public Error Error { get; set; }
        }
        
        public class Error : EntityField<MailMessage, string>
        {
        }
        public class MailMessageStatus : EntityField<MailMessage, string>
        {
        }
        public class AttemptsToSendCount : EntityField<MailMessage, int>
        {
        }
        public class LastSendAttemptDate : EntityNullableDateTimeField<MailMessage>
        {
        }
        public class MaxAttemptsToSendCount : EntityField<MailMessage, int>
        {
        }
        public class Recipients : EntityField<MailMessage, string>
        {
        }
        public class CopyRecipients : EntityField<MailMessage, string>
        {
        }
        public class LastModifiedUserTitle : EntityField<MailMessage, string>
        {
        }
        public class StartSendDate : EntityNullableDateTimeField<MailMessage>
        {
        }
        public class FinishSendDate : EntityNullableDateTimeField<MailMessage>
        {
        }
        
        public class MailCategoryTitle : EntityField<MailMessage, string>
        {
        }
        public class Subject : EntityField<MailMessage, string>
        {
        }
        public class Body : EntityField<MailMessage, string>
        {
        }
        
        public class CreateDate : EntityDateTimeField<MailMessage>
        {
        }
        
        public class TimeoutValue : EntityField<MailMessage, int>
        {
        }
    }
}