using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tango.Data;

namespace Tango
{
    public class MethodSettings
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
    
    public class MethodSettingsReturnTypeAttribute : Attribute
    {
        public Type ReturnType { get; set; }
    }
}

namespace Tango.Mail
{
    [MethodSettingsReturnType(ReturnType = typeof(List<string>))]
    public interface IAttachmentMail
    {
    }

    public interface IPostProcessingMail
    {
    }

    public interface IRecipientsMail
    {
    }
    
    public class CreateMailMessageContext
    {
        public AttachmentMailResult AttachmentMailResult { get; set; }
        public PostProcessingMailResult PostProcessingMailResult { get; set; }
        public RecipientsMailResult RecipientsMailResult { get; set; }
    }

    public class AttachmentMailResult
    {
        
    }
    
    public class PostProcessingMailResult
    {
        
    }
    
    public class RecipientsMailResult
    {
        
    }

    public class MailHelper
    {
        private readonly IDatabase _database;
        public MailHelper(IDatabase database)
        {
            _database = database;
        }

        public void CreateMailMessage<TEntity>(string systemName, TEntity viewData)
        {
            const string templateSubj = "Техническая ошибка в журнале загрузки: [IntegrationLogRecord_ID]";
            const string templateBody = @"Ошибка в журнале загрузки:
            Дата/время: [CreateDate]
            Направление: [Direction]
            Тип интеграционного процесса: [Process_ID]
            Тип шага интеграционного процесса: [ProcessStep_ID]
            XML: [Xml]
            External_ID: [External_ID]
            Result: [Result]
            Код ошибки: [ErrorCode]
            Текст ошибки: [ErrorText]
            UpdateInfo: [UpdateInfo]
            LastInfo: [LastInfo]
            ";
            const string json = @"
{
    className: 'Askue2.MGLEP.Views.Test',
    methodName: 'Hello',
    params: {
        'a': 'user'
    }
}
";
            var (s, t) = ParseTemplate(templateSubj, templateBody, viewData);

            var json1 = JsonConvert.DeserializeObject<MethodSettings>(json);
            var v = ParseAndExecuteMethod<AttachmentMailResult>(json1);

            Trace.Write(s);


            var mailSettings = _database.Repository<MailSettings>().List()
                .FirstOrDefault(item => item.SystemName != null && item.SystemName.ToLower().Equals(systemName.ToLower()));
            if (mailSettings != null)
            {
                var mailTemplate = GetMailTemplate(mailSettings);

                if (mailTemplate != null)
                {
                    var (subject, body) = ParseTemplate(mailTemplate.TemplateSubject, mailTemplate.TemplateBody, viewData);

                    var context = new CreateMailMessageContext();
                    if (!string.IsNullOrEmpty(mailSettings.AttachmentMethod))
                    {
                        var mailMethod = JsonConvert.DeserializeObject<MethodSettings>(mailSettings.AttachmentMethod);
                        
                        context.AttachmentMailResult = ParseAndExecuteMethod<AttachmentMailResult>(mailMethod);
                    }
                    
                    if (!string.IsNullOrEmpty(mailSettings.PostProcessingMethod))
                    {
                        var mailMethod = JsonConvert.DeserializeObject<MethodSettings>(mailSettings.PostProcessingMethod);
                        
                        context.PostProcessingMailResult = ParseAndExecuteMethod<PostProcessingMailResult>(mailMethod);
                    }
                    
                    if (!string.IsNullOrEmpty(mailSettings.RecipientsMethod))
                    {
                        var mailMethod = JsonConvert.DeserializeObject<MethodSettings>(mailSettings.RecipientsMethod);
                        
                        context.RecipientsMailResult = ParseAndExecuteMethod<RecipientsMailResult>(mailMethod);
                    }
                    
                    // Запись в реестр почты.
                    var mailMessage = new MailMessage
                    {
                        MailMessageStatusID = (int) MailMessageStatus.New,
                        AttemptsToSendCount = 0,
                        LastSendAttemptDate = null,
                        CreateDate = DateTime.Now,
                        Subject = subject,
                        Body = body
                    };
                    
                    _database.Repository<MailMessage>().Create(mailMessage);
                }
            }
        }

        private MailTemplate GetMailTemplate(MailSettings mailSettings)
        {
            var mailSettingsTemplate = _database.Repository<MailSettingsTemplate>().List()
                .FirstOrDefault(item => item.MailSettingsID == mailSettings.MailSettingsID);
            if (mailSettingsTemplate != null)
            {
                return _database.Repository<MailTemplate>().List()
                    .FirstOrDefault(item => item.MailTemplateID == mailSettingsTemplate.MailTemplateID);
            }

            return null;
        }

        private T ParseAndExecuteMethod<T>(MethodSettings methodSettings)
        {
            var objectType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(i => i.FullName == methodSettings.ClassName);

            var method = objectType?.GetMethod(methodSettings.MethodName);
            var ps = method?.GetParameters();
            
            object[] values = null;
            if (methodSettings.Params != null && methodSettings.Params.Any() && ps != null)
            {
                values = new object[methodSettings.Params.Count];
                for (int i = 0; i < ps.Length; i++)
                {
                    var key = ps[i].Name.ToLower();
                    if (methodSettings.Params.TryGetValue(key, out var value))
                    {
                        if (ps[i].ParameterType == typeof(Guid))
                            values[i] = value.ToGuid();
                        else if (ps[i].ParameterType == typeof(DateTime?))
                            values[i] = value.ToDateTime();
                        else if (ps[i].ParameterType == typeof(DateTime))
                            values[i] = value.ToDateTime(DateTime.MinValue);
                        else if (ps[i].ParameterType == typeof(int?))
                            values[i] = value.ToInt32();
                        else if (ps[i].ParameterType == typeof(int))
                            values[i] = value.ToInt32(0);
                        else if (ps[i].ParameterType == typeof(long?))
                            values[i] = value.ToInt64();
                        else if (ps[i].ParameterType == typeof(long))
                            values[i] = value.ToInt64(0);
                        else if (ps[i].ParameterType == typeof(bool?))
                            values[i] = value.ToBoolean();
                        else if (ps[i].ParameterType == typeof(bool))
                            values[i] = value.ToBoolean(false);
                        else
                            values[i] = value;
                    }
                }
            }

            if (method != null)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                //obj.InjectProperties(serviceProvider);
                return (T)method.Invoke(obj, values);
            }

            return default;
        }

        private (string subject, string body) ParseTemplate<T>(string templateSubject, string templateBody, T viewData)
        {
            var type = viewData.GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                templateSubject = templateSubject.Replace($"[{propertyInfo.Name}]", propertyInfo.GetValue(viewData)?.ToString());
                templateBody = templateBody.Replace($"[{propertyInfo.Name}]", propertyInfo.GetValue(viewData)?.ToString());
            }

            return (templateSubject, templateBody);
        }
    }
}