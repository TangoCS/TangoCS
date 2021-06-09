using System;
using System.ComponentModel;
using System.Linq;

namespace Tango
{
    public class MethodValidationAttribute : Attribute
    {
        
    }
}

namespace Tango.Mail.Methods
{
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Заполнить список адресатов")]
    public class RecipientsMail
    {
        [Description("Заполнение списка")]
        public void Run(MailMessageContext context, [Description("Список адресатов")]string recipients)
        {
            context.MailMessage.Recipients = recipients;
        }

        // [MethodValidation]
        // public string Validation(MailMessageContext context)
        // {
        //     
        // }
    }
    
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Указать отправителя")]
    public class SendersMail
    {
        [Description("Email и имя отправителя")]
        public void Run(MailMessageContext context, 
            [Description("Email отправителя")]string email, 
            [Description("Имя отправителя")]string name)
        {
            context.MailMessage.FromEmail = email;
            context.MailMessage.FromName = name;
        }
    }
    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Заполнить список копий адресатов")]
    public class RecipientsCopyMail
    {
        [Description("Заполнение списка")]
        public void Run(MailMessageContext context, [Description("Список адресатов")] string recipients)
        {
            context.MailMessage.CopyRecipients = recipients;
        }
    }

    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Вложить файл из реестра файлов")]
    public class FileAttachmentFromFileData
    {
        [Description("Простое вложение")]
        public void Run(MailMessageContext context, [Description("Список вложений (ID файлов через запятую)")] string fileIDs)
        {
            var ids = fileIDs.Split(',').Select(Guid.Parse);

            context.ExistingFileIds.AddRange(ids);
        }
    }

}