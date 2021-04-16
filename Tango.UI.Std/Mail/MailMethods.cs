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
}