using System;

namespace Tango.Mail
{
    public static class MailConstants
    {
        public static readonly DateTime StartDate = new DateTime(1900, 1, 1, 0, 0, 0);
        public static readonly DateTime FinishDate = new DateTime(2099, 12, 31, 23, 59, 0);
    }
    public static class MailTypeCacheKeys
    {
        public const string PreProcessingMailMethod = "PreProcessingMailMethod";
        public const string PostProcessingMailMethod = "PostProcessingMailMethod";
        public const string DeleteMailMethod = "DeleteMailMethod";
        public const string AfterSentMethod = "AfterSentMethod";
    }
}