using System;
using System.ComponentModel;
using System.Linq;

namespace Tango.Mail.Methods
{
    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Заполнить список адресатов")]
    public class RecipientsMail
    {
        [Description("Заполнение списка")]
        public void Run(MailMessageContext context, [Description("Список адресатов")]string recipients)
        {
            context.MailMessage.Recipients = recipients;
        }
    }
    
    /// <summary>
    /// Получение списка адресатов для постановки в копию
    /// </summary>
    // [TypeCache(MailTypeCacheKeys.PreProcessingMethod)]
    // [Description("Заполнить список копий адресатов")]
    // public class CopyRecipientsMail
    // {
    //     [Description("Заполнение списка")]
    //     public void Run(MailMessageContext context, [Description("Список адресатов")]string recipients)
    //     {
    //         context.MailMessage.CopyRecipients = recipients;
    //     }
    // }
    
    // /// <summary>
    // /// Получение списка известных вложений
    // /// </summary>
    // [TypeCache(MailTypeCacheKeys.PreProcessingMethod)]
    // [Description("Заполнить вложения")]
    // public class ExistAttachmentMail
    // {
    //     [Description("По типу документа")]
    //     public void Run(MailMessageContext context, [Description("Идентификаторы документа")]string documentsIds)
    //     {
    //         if(string.IsNullOrEmpty(documentsIds))
    //             return;
    //         
    //         var ids = documentsIds.Split(';');
    //         var aIds = ids.Select(Guid.Parse);
    //         context.ExistingFileIds = aIds.ToList();
    //     }
    // }

    [TypeCache(MailTypeCacheKeys.PostProcessingMailMethod)]
    [Description("Постобработка")]
    public class PostProcessingMailCls
    {
        [Description("Запуск")]
        public void Run(MailMessageContext context) {}
    }
    
    // public class NewAttachmentMail
    // {
    //     public void Run(MailMessageContext context)
    //     {
    //         //context.MailMessage.Recipients = recipients.Join(";");
    //     }
    // }
}