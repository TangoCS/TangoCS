using System;
using System.Linq;

namespace Tango.Mail
{
    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    public class RecipientsMail
    {
        public void Run(MailMessageContext context, string recipients)
        {
            context.MailMessage.Recipients = recipients;
        }
    }
    
    /// <summary>
    /// Получение списка известных вложений
    /// </summary>
    public class ExistAttachmentMail
    {
        public void Run(MailMessageContext context, string attachmentIds)
        {
            if(string.IsNullOrEmpty(attachmentIds))
                return;
            
            var ids = attachmentIds.Split(';');
            var aIds = ids.Select(Guid.Parse);
            context.ExistingFileIds = aIds.ToList();
        }
    }
    
    public class NewAttachmentMail
    {
        public void Run(MailMessageContext contexts)
        {
            //context.MailMessage.Recipients = recipients.Join(";");
        }
    }
}