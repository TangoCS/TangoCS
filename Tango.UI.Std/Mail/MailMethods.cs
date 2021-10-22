using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tango
{
    [MailUserDataAttribute("mailfiledto")]
    [Description("Почтовая модель для работы с файлами")]
    public class MailFileDto
    {
        [Description("Идентификаторы файлов")]
        public string FileIds { get; set; }
    }
}

namespace Tango.Mail.Methods
{
    public static class EmailValidation
    {
        public static bool IsValid(string email)
        {
            var regex = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            var match = regex.Match(email);
            return match.Success;
        }
    }
    
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Заполнить список адресатов")]
    public class RecipientsMail : IValidatableObject
    {
        [Description("Заполнение списка")]
        public void Run(MailMessageContext context, [EmailAddress][Description("Список адресатов")]string recipients)
        {
            context.MailMessage.Recipients = recipients;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            var delimiters = new[] {',', ';'};

            var items = validationContext.Items.Values.Select(i => i.ToString().Trim());
            if (!items.Any())
                return new List<ValidationResult>
                {
                    new ValidationResult("Не заполнен список адресатов для метода " +
                                         "\"Заполнить список адресатов.Заполнение списка\" ")
                };
            foreach (var item in items)
            {
                var addresses = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                if(addresses.Count() == 1 && addresses.FirstOrDefault().StartsWith("@"))
                    continue;
                    
                foreach (var address in addresses)
                {
                    if(!EmailValidation.IsValid(address))
                        errors.Add(new ValidationResult($"Не удалось распознать электронный адрес {address}"));
                }
            }

            return errors;
        }
    }
    
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Указать отправителя")]
    public class SendersMail : IValidatableObject
    {
        [Description("Email и имя отправителя")]
        public void Run(MailMessageContext context, 
            [EmailAddress][Description("Email отправителя")]string email, 
            [Description("Имя отправителя")]string name)
        {
            context.MailMessage.FromEmail = email;
            context.MailMessage.FromName = name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            var delimiters = new[] {',', ';'};

            var items = validationContext.Items.Values.Select(i => i.ToString().Trim());
            if (!items.Any())
                return new List<ValidationResult>
                {
                    new ValidationResult("Не заполнен список адресатов для метода " +
                                         "\"Указать отправителя.Email и имя отправителя\" ")
                };
            foreach (var item in items)
            {
                var addresses = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                if(addresses.Count() > 1)
                    return new List<ValidationResult>
                    {
                        new ValidationResult("В методе \"Указать отправителя.Email и имя отправителя\" более одного отправителя.")
                    };
                foreach (var address in addresses)
                {
                    if(address.StartsWith("@"))
                        continue;
                    if(!EmailValidation.IsValid(address))
                        errors.Add(new ValidationResult($"Не удалось распознать электронный адрес {address}"));
                }
            }

            return errors;
        }
    }
    /// <summary>
    /// Получение списка адресатов
    /// </summary>
    [TypeCache(MailTypeCacheKeys.PreProcessingMailMethod)]
    [Description("Заполнить список копий адресатов")]
    public class RecipientsCopyMail : IValidatableObject
    {
        [Description("Заполнение списка")]
        public void Run(MailMessageContext context,[EmailAddress][Description("Список адресатов")] string recipients)
        {
            context.MailMessage.CopyRecipients = recipients;
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            var delimiters = new[] {',', ';'};

            var items = validationContext.Items.Values.Select(i => i.ToString().Trim());
            if (!items.Any())
                return new List<ValidationResult>
                {
                    new ValidationResult("Не заполнен список копий адресатов для метода " +
                                         "\"Заполнить список копий адресатов.Заполнение списка\" ")
                };
            foreach (var item in items)
            {
                var addresses = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                if(addresses.Count() == 1 && addresses.FirstOrDefault().StartsWith("@"))
                    continue;
                foreach (var address in addresses)
                {
                    if(!EmailValidation.IsValid(address))
                        errors.Add(new ValidationResult($"Не удалось распознать электронный адрес {address}"));
                }
            }

            return errors;
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