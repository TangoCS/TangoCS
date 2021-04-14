using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tango.Data;
using Tango.Identity;

namespace Tango
{
    public class MethodSettingsCollection
    {
        public MethodSettings[] MethodSettings { get; set; }
    }
    
    public class MethodSettings
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, object> Params { get; set; }
    }

    public class MethodHelper
    {
        public void ExecuteMethodCollection<TContext>(MethodSettingsCollection methodSettingsCollection, TContext context)
        {
            foreach (var methodSetting in methodSettingsCollection.MethodSettings)
            {
                ExecuteMethod(methodSetting, context);
            }
        }
        
        public void ExecuteMethod<TContext>(MethodSettings methodSettings, TContext context)
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
                var parms = methodSettings.Params.ToDictionary(i => i.Key.ToLower(), i => i.Value);
                var count = parms.Count;
                if (ps.Any(p => p.ParameterType == typeof(TContext)))
                    count += 1;
                values = new object[count];
                for (var i = 0; i < ps.Length; i++)
                {
                    var key = ps[i].Name.ToLower();
                    if (parms.TryGetValue(key, out var value))
                    {
                        if (ps[i].ParameterType == typeof(Guid))
                            values[i] = (Guid) value; // value.ToGuid();
                        else if (ps[i].ParameterType == typeof(DateTime?))
                            values[i] = (DateTime?) value; //value.ToDateTime();
                        else if (ps[i].ParameterType == typeof(DateTime))
                            values[i] = (DateTime) value; //value.ToDateTime(DateTime.MinValue);
                        else if (ps[i].ParameterType == typeof(int?))
                            values[i] = (int?) value; //value.ToInt32();
                        else if (ps[i].ParameterType == typeof(int))
                            values[i] = (int) value; // value.ToInt32(0);
                        else if (ps[i].ParameterType == typeof(long?))
                            values[i] = (long?) value; //value.ToInt64();
                        else if (ps[i].ParameterType == typeof(long))
                            values[i] = (long) value; // value.ToInt64(0);
                        else if (ps[i].ParameterType == typeof(bool?))
                            values[i] = (bool?) value; // value.ToBoolean();
                        else if (ps[i].ParameterType == typeof(bool))
                            values[i] = (bool) value; // value.ToBoolean(false);
                        else
                            values[i] = value;
                    }
                    else
                    {
                        if (ps[i].ParameterType == typeof(TContext))
                            values[i] = context;
                    }
                }
            }
            else
            {
                values = new object[1];
                values[0] = context;
            }

            if (method != null)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(obj, values);
            }
        }
        
        public TRet ExecuteMethod<TRet, TContext>(MethodSettings methodSettings, TContext context)
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
                        // if (ps[i].ParameterType == typeof(Guid))
                        //     values[i] = value.ToGuid();
                        // else if (ps[i].ParameterType == typeof(DateTime?))
                        //     values[i] = value.ToDateTime();
                        // else if (ps[i].ParameterType == typeof(DateTime))
                        //     values[i] = value.ToDateTime(DateTime.MinValue);
                        // else if (ps[i].ParameterType == typeof(int?))
                        //     values[i] = value.ToInt32();
                        // else if (ps[i].ParameterType == typeof(int))
                        //     values[i] = value.ToInt32(0);
                        // else if (ps[i].ParameterType == typeof(long?))
                        //     values[i] = value.ToInt64();
                        // else if (ps[i].ParameterType == typeof(long))
                        //     values[i] = value.ToInt64(0);
                        // else if (ps[i].ParameterType == typeof(bool?))
                        //     values[i] = value.ToBoolean();
                        // else if (ps[i].ParameterType == typeof(bool))
                        //     values[i] = value.ToBoolean(false);
                        // else
                            values[i] = value;
                    }
                    else
                    {
                        if (ps[i].ParameterType == typeof(TContext))
                        {
                            values[i] = context;
                        }
                    }
                }
            }

            if (method != null)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                return (TRet)method.Invoke(obj, values);
            }

            return default;
        }
    }
}

namespace Tango.Mail
{
    public class MailMessageContext
    {
        public MailMessage MailMessage { get; set; }
        public List<Guid> ExistingFileIds { get; set; } = new List<Guid>();
        public List<FileData> NewFiles { get; set; } = new List<FileData>();
    }

    public class MailHelper
    {
        private readonly IDatabase _database;
        private readonly IUserIdAccessor<object> _userIdAccessor;
        private readonly MethodHelper _methodHelper;
        
        public MailHelper(IDatabase database, IUserIdAccessor<object> userIdAccessor, MethodHelper methodHelper)
        {
            _database = database;
            _userIdAccessor = userIdAccessor;
            _methodHelper = methodHelper;
        }

        public void CreateMailMessageTest<TEntity>(string systemName, TEntity viewData)
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

            const string preProcessingJson = @"
{
'MethodSettings':
[
    {
        'ClassName':'Tango.Mail.RecipientsMail',
        'MethodName':'Run',
        'Params': {
            'recipients': 'aa@aa.ru;bb@bb.ru'
        }
    },
    {
        'ClassName':'Tango.Mail.CopyRecipientsMail',
        'MethodName':'Run',
        'Params': {
            'recipients': 'tt@tt.ru;cc@cc.ru'
        }
    },
    {
        'ClassName':'Tango.Mail.ExistAttachmentMail',
        'MethodName':'Run',
        'Params':
        {
            'attachmentIds': 'fb1cd464-8c1d-4016-930a-a9c540eae9e5;d44d0b21-3fb2-4b01-b9ea-e47f5072c42c;44c253a6-4002-4d2b-810e-59b895eec6be;f946232b-02f6-4599-a8c4-a165ac234009;878310ad-7b17-498c-a072-fb59bab8eb08;6ba33718-52f8-4256-b8db-087f801b3bf0'            
        }
    },
    {
        'ClassName':'Tango.Mail.NewAttachmentMail',
        'MethodName':'Run',
        'Params':null
    }
]}
";
            
            var (subject, body) = ParseTemplate(templateSubj, templateBody, viewData);

            var context = new MailMessageContext
            {
                MailMessage = new MailMessage
                {
                    MailMessageStatusID = (int) MailMessageStatus.New,
                    AttemptsToSendCount = 0,
                    LastSendAttemptDate = null,
                    CreateDate = DateTime.Now,
                    Subject = subject,
                    Body = body,
                    LastModifiedUserID = _userIdAccessor.CurrentUserID ?? _userIdAccessor.SystemUserID
                }
            };
            
            var settings = JsonConvert.DeserializeObject<MethodSettingsCollection>(preProcessingJson);
            
            _methodHelper.ExecuteMethodCollection(settings, context);

            Trace.Write(context);
        }

        public void CreateMailMessage<TEntity>(string systemName, TEntity viewData)
        {
            var mailSettings = _database.Repository<MailSettings>().List()
                .FirstOrDefault(item => item.SystemName != null && item.SystemName.ToLower().Equals(systemName.ToLower()));
            if (mailSettings != null)
            {
                var mailTemplate = GetMailTemplate(mailSettings);
                if (mailTemplate != null)
                {
                    var (subject, body) = ParseTemplate(mailTemplate.TemplateSubject, mailTemplate.TemplateBody, viewData);

                    var mailMessageContext = new MailMessageContext
                    {
                        MailMessage = new MailMessage
                        {
                            MailMessageStatusID = (int) MailMessageStatus.New,
                            AttemptsToSendCount = mailSettings.AttemptsToSendCount ?? 0,
                            LastSendAttemptDate = null,
                            CreateDate = DateTime.Now,
                            Subject = subject,
                            Body = body,
                            TimeoutValue = mailSettings.TimeoutValue,
                            LastModifiedUserID = _userIdAccessor.CurrentUserID ?? _userIdAccessor.SystemUserID
                        }
                    };
                    
                    if (!string.IsNullOrEmpty(mailSettings.PreProcessingMethod))
                    {
                        var mailMethod = JsonConvert.DeserializeObject<MethodSettings>(mailSettings.PreProcessingMethod);
                        
                        _methodHelper.ExecuteMethod(mailMethod, mailMessageContext);
                    }
                    
                    if (!string.IsNullOrEmpty(mailSettings.PostProcessingMethod))
                    {
                        var mailMethod = JsonConvert.DeserializeObject<MethodSettings>(mailSettings.PostProcessingMethod);
                        
                        _methodHelper.ExecuteMethod(mailMethod, mailMessageContext);
                    }

                    _database.Repository<MailMessage>().Create(mailMessageContext.MailMessage);

                    foreach (var existFileId in mailMessageContext.ExistingFileIds)
                    {
                        var mailMessageAttachment = new MailMessageAttachment
                        {
                            MailMessageID = mailMessageContext.MailMessage.MailMessageID,
                            FileID = existFileId
                        };
                        _database.Repository<MailMessageAttachment>().Create(mailMessageAttachment);
                    }
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