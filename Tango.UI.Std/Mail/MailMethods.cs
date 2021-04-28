using System;
using System.ComponentModel;
using System.Linq;

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
}