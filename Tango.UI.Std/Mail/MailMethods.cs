using System;
using System.ComponentModel;
using System.Linq;

namespace Tango.Mail.Methods
{
    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMethod)]
    [Description("Предварительная обработка")]
    public class RecipientsMail
    {
        [Description("Заполнить список адресатов")]
        public void Run(MailMessageContext context, [Description("Список адресатов")]string recipients, [Description("Список адресатов2")]string recipients2)
        {
            context.MailMessage.Recipients = recipients;
        }
    }
    
    /// <summary>
    /// Получение списка адресатов для постановки в копию
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMethod)]
    [Description("Предварительная обработка")]
    public class CopyRecipientsMail
    {
        [Description("Заполнить список копий адресатов")]
        public void Run(MailMessageContext context, [Description("Список адресатов")]string recipients)
        {
            context.MailMessage.CopyRecipients = recipients;
        }
    }
    
    /// <summary>
    /// Получение списка известных вложений
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMethod)]
    [Description("Предварительная обработка")]
    public class ExistAttachmentMail
    {
        [Description("Заполнить вложения")]
        public void Run(MailMessageContext context, [Description("Список вложений")]string attachmentIds)
        {
            if(string.IsNullOrEmpty(attachmentIds))
                return;
            
            var ids = attachmentIds.Split(';');
            var aIds = ids.Select(Guid.Parse);
            context.ExistingFileIds = aIds.ToList();
        }
    }

    [TypeCache(MailTypeCacheKeys.PostProcessingMethod)]
    [Description("post")]
    public class PostProcessingMailCls
    {
        [Description("run")]
        public void Run() {}
    }
    
    public class NewAttachmentMail
    {
        public void Run(MailMessageContext contexts)
        {
            //context.MailMessage.Recipients = recipients.Join(";");
        }
    }
}