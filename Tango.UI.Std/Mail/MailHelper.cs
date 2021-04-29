using System;
using System.Collections.Concurrent;
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
                return (TRet) method.Invoke(obj, values);
            }

            return default;
        }
    }
}

namespace Tango.Mail
{
    public class MailMessageContext
    {
        public MailMessageContext(IDatabase database)
        {
            Database = database;
        }

        public MailMessage MailMessage { get; set; }
        public List<Guid> ExistingFileIds { get; set; } = new List<Guid>();
        public List<FileData> NewFiles { get; set; } = new List<FileData>();
        public IDatabase Database { get; }
    }

    public class MailHelper
    {
        private readonly IDatabase _database;
        private readonly IUserIdAccessor<object> _userIdAccessor;
        private readonly MethodHelper _methodHelper;
        private readonly IMailHelperRepository _mailHelperRepository;

        public MailHelper(IDatabase database, IUserIdAccessor<object> userIdAccessor, MethodHelper methodHelper, IMailHelperRepository mailHelperRepository)
        {
            _database = database;
            _userIdAccessor = userIdAccessor;
            _methodHelper = methodHelper;
            _mailHelperRepository = mailHelperRepository;
        }

        private object GetValue<TEntity>(KeyValuePair<string, object> param, TEntity viewData)
        {
            if (param.Value.ToString().StartsWith("@"))
            {
                var propInfo = typeof(TEntity).GetProperty(param.Value.ToString().Replace("@", ""),
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.IgnoreCase);
                return propInfo != null ? propInfo.GetValue(viewData) : param.Value;
            }

            return param.Value;
        }

        public void CreateMailMessage<TEntity>(string systemName, TEntity viewData)
        {
            var mailSettings = _mailHelperRepository.GetMailSettingsBySystemName(systemName);
            if (mailSettings != null)
            {
                var mailTemplate = GetMailTemplate(mailSettings);
                if (mailTemplate != null)
                {
                    var (subject, body) = ParseTemplate(mailTemplate.TemplateSubject, mailTemplate.TemplateBody, viewData);

                    var mailMessageContext = new MailMessageContext(_database)
                    {
                        MailMessage = new MailMessage
                        {
                            MailMessageStatusID = (int) MailMessageStatus.New,
                            LastSendAttemptDate = null,
                            CreateDate = DateTime.Now,
                            Subject = subject,
                            Body = body,
                            MaxAttemptsToSendCount = mailSettings.AttemptsToSendCount ?? 5,
                            MailCategoryID = mailSettings.MailCategoryID,
                            TimeoutValue = mailSettings.TimeoutValue,
                            LastModifiedUserID = _userIdAccessor.CurrentUserID ?? _userIdAccessor.SystemUserID
                        }
                    };

                    var date = mailMessageContext.MailMessage.CreateDate.Date;
                    if (mailSettings.SendMailStartInterval != TimeSpan.Zero || mailSettings.SendMailFinishInterval != TimeSpan.Zero)
                    {
                        mailMessageContext.MailMessage.StartSendDate = date
                            .AddDays(mailSettings.SendMailDayInterval)
                            .Add(mailSettings.SendMailStartInterval);
                        
                        mailMessageContext.MailMessage.FinishSendDate = date
                            .AddDays(mailSettings.SendMailDayInterval)
                            .Add(mailSettings.SendMailFinishInterval);
                    }
                    else if(mailSettings.SendMailDayInterval > 0)
                    {
                        mailMessageContext.MailMessage.StartSendDate = date.AddDays(mailSettings.SendMailDayInterval);
                        mailMessageContext.MailMessage.FinishSendDate = date.AddDays(mailSettings.SendMailDayInterval).EndDay();
                    }

                    if (!string.IsNullOrEmpty(mailSettings.PreProcessingMethod))
                    {
                        var mailMethods = JsonConvert.DeserializeObject<MethodSettingsCollection>(mailSettings.PreProcessingMethod);
                        foreach (var methodSetting in mailMethods.MethodSettings)
                        {
                            foreach (var param in methodSetting.Params)
                            {
                                methodSetting.Params[param.Key] = GetValue(param, viewData);
                            }

                            _methodHelper.ExecuteMethod(methodSetting, mailMessageContext);
                        }
                    }
                    
                    // Валидация на обязательные поля: Email отправителя, Email получателя
                    if (string.IsNullOrEmpty(mailMessageContext.MailMessage.Recipients))
                        throw new MailHelperValidateException($"Не заполнены адреса получателей в настройке {mailSettings.Title}");
                    
                    if (string.IsNullOrEmpty(mailMessageContext.MailMessage.FromEmail))
                        throw new MailHelperValidateException($"Не заполнен Email отправителя в настройке {mailSettings.Title}");

                    using (var transaction = _database.BeginTransaction())
                    {
                        try
                        {
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

                            if (!string.IsNullOrEmpty(mailSettings.PostProcessingMethod))
                            {
                                var mailMethods = JsonConvert.DeserializeObject<MethodSettingsCollection>(mailSettings.PostProcessingMethod);
                                foreach (var methodSetting in mailMethods.MethodSettings)
                                {
                                    foreach (var param in methodSetting.Params)
                                    {
                                        if (param.Value.ToString().StartsWith("@"))
                                        {
                                            methodSetting.Params[param.Key] = GetValue(param, viewData);
                                        }
                                    }

                                    _methodHelper.ExecuteMethod(methodSetting, mailMessageContext);
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
        }

        private MailTemplate GetMailTemplate(MailSettings mailSettings)
        {
            var mailSettingsTemplate = _mailHelperRepository.GetMailSettingsTemplateByMailSettingsId(mailSettings.MailSettingsID);
            return mailSettingsTemplate != null ? _database.Repository<MailTemplate>().GetById(mailSettingsTemplate.MailTemplateID) : null;
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

    public static class MailTypeCollection
    {
        static ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string fullnameclass)
        {
            string taskclass = fullnameclass.Split(',')[0];

            if (!types.TryGetValue(taskclass, out var type))
            {
                type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.FullName.ToLower() == taskclass.ToLower());

                types.AddIfNotExists(taskclass, type);
            }

            return type;
        }
    }

    public class MailHelperValidateException : Exception
    {
        public MailHelperValidateException(string message) : base(message)
        {
        }

        public MailHelperValidateException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}