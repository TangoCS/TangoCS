using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tango.Data;
using Tango.Identity;

namespace Tango
{
    public class MethodValidationHelper
    {
        static ConcurrentDictionary<string, Type> _types = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string fullnameclass)
        {
            var cls = fullnameclass.Split(',')[0];

            if (!_types.TryGetValue(cls, out var type))
            {
                type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.FullName.ToLower() == cls.ToLower());

                _types.AddIfNotExists(cls, type);
            }

            return type;
        }
        
        public IEnumerable<ValidationResult> ValidateMethodSettings(MethodSettings methodSettings)
        {
            var objectType = GetType(methodSettings.ClassName);
            if (objectType == null) return new List<ValidationResult>();
            
            var obj = Activator.CreateInstance(objectType);
            if (obj is IValidatableObject)
            {
                var props = objectType.GetMethod(methodSettings.MethodName)?.GetParameters()
                    .Where(prop => Attribute.IsDefined(prop, typeof(ValidationAttribute)))
                    .Select(i => i.Name?.ToLower());

                if (props == null)
                    return new List<ValidationResult>();

                var context = new ValidationContext(obj);
                foreach (var prop in props)
                {
                    if (methodSettings.Params.TryGetValue(prop, out var value))
                    {
                        context.Items.Add(prop, value);
                    }
                }

                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(obj, context, results))
                {
                    
                }

                return results;
            }
            
            return new List<ValidationResult>();
        }
    }
    
    public interface IMethodContext
    {
        IDatabase Database { get; }
    }

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
        public void ExecuteMethodCollection<TContext>(MethodSettingsCollection methodSettingsCollection, TContext context) where TContext: IMethodContext
        {
            foreach (var methodSetting in methodSettingsCollection.MethodSettings)
            {
                ExecuteMethod(methodSetting, context);
            }
        }

        public object ExecuteMethod(MethodInfo method, object[] vals)
        {
            var ps = method?.GetParameters();
            if (ps == null) return null;

            if (ps.Length != vals.Length)
                throw new Exception($"Количество значений метода валидации ({method.Name}) не соответствует с переданными значениями");
                    
            var obj = Activator.CreateInstance(method.DeclaringType);
            return method.Invoke(obj, vals);
        }

        public void ExecuteMethod<TContext>(MethodSettings methodSettings, TContext context) where TContext: IMethodContext
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

        public TRet ExecuteMethod<TRet, TContext>(MethodSettings methodSettings, TContext context) where TContext: IMethodContext
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
    
    public class MailUserDataAttribute : Attribute
    {
        public string Key { get; }

        public MailUserDataAttribute(string key)
        {
            Key = key;
        }
    }
    
    /// <summary>
    /// Необходимо помечать классы, которые используются в моделях для почтовых отправок, атрибутом MailUserData
    /// </summary>
    public class MailUserDataCache : ITypeObserver
    {
        static readonly Dictionary<string, List<Type>> _typeCache = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);

        public List<Type> Get(string key)
        {
            if (_typeCache.TryGetValue(key, out var list))
                return list;
            else
                return null;
        }

        void ITypeObserver.LookOver(IServiceProvider provider, Type t)
        {
            var attr = t.GetCustomAttribute<MailUserDataAttribute>();
            if (attr == null) return;

            if (_typeCache.TryGetValue(attr.Key, out var list))
                list.Add(t);
            else
                _typeCache.Add(attr.Key, new List<Type> { t });
        }
    }
}

namespace Tango.Mail
{
    
    
    public interface IMailMethodContext : IMethodContext
    {
        MailMessage MailMessage { get; set; }
    }
    
    public class AfterSentContext : IMailMethodContext
    {
        public AfterSentContext(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; }
        public MailMessage MailMessage { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
    }

    public class MailMessageContext : IMailMethodContext
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

        public MailHelper(IDatabase database, IUserIdAccessor<object> userIdAccessor, MethodHelper methodHelper,
            IMailHelperRepository mailHelperRepository)
        {
            _database = database;
            _userIdAccessor = userIdAccessor;
            _methodHelper = methodHelper;
            _mailHelperRepository = mailHelperRepository;
        }

        private object GetValue<TEntity>(KeyValuePair<string, object> param, TEntity viewData)
        {
            var value = param.Value;
            if (value != null && value.ToString().StartsWith("@"))
            {
                var propInfo = typeof(TEntity).GetProperty(value.ToString().Replace("@", "").Trim(),
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.IgnoreCase);
                return propInfo != null ? propInfo.GetValue(viewData) : value;
            }

            return value;
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
                            LastModifiedUserID = _userIdAccessor.CurrentUserID ?? _userIdAccessor.SystemUserID,
                            DeleteMethod = mailSettings.DeleteMethod,
                            AfterSentMethod = mailSettings.AfterSentMethod,
                            MailSettingsID = mailSettings.MailSettingsID
                        }
                    };

                    var date = mailMessageContext.MailMessage.CreateDate.Date;
                    if (mailSettings.SendMailStartInterval != TimeSpan.Zero ||
                        mailSettings.SendMailFinishInterval != TimeSpan.Zero)
                    {
                        mailMessageContext.MailMessage.StartSendDate = date
                            .AddDays(mailSettings.SendMailDayInterval)
                            .Add(mailSettings.SendMailStartInterval);

                        mailMessageContext.MailMessage.FinishSendDate = date
                            .AddDays(mailSettings.SendMailDayInterval)
                            .Add(mailSettings.SendMailFinishInterval);
                    }
                    else if (mailSettings.SendMailDayInterval > 0)
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
                        throw new MailHelperException($"Не заполнены адреса получателей в настройке {mailSettings.Title}");

                    if (string.IsNullOrEmpty(mailMessageContext.MailMessage.FromEmail))
                        throw new MailHelperException($"Не заполнен Email отправителя в настройке {mailSettings.Title}");

                    _database.Repository<MailMessage>().Create(mailMessageContext.MailMessage);

                    foreach (var existFileId in mailMessageContext.ExistingFileIds)
                    {
                        var mailMessageAttachment = new MailMessageAttachment
                        {
                            MailMessageID = mailMessageContext.MailMessage.MailMessageID,
                            FileGUID = existFileId
                        };
                        _database.Repository<MailMessageAttachment>().Create(mailMessageAttachment);
                    }

					if (!string.IsNullOrEmpty(mailSettings.PostProcessingMethod))
					{
						using (var transaction = _database.BeginTransaction())
                        {
						
							var mailMethods = JsonConvert.DeserializeObject<MethodSettingsCollection>(mailSettings.PostProcessingMethod);
							foreach (var methodSetting in mailMethods.MethodSettings)
							{
								foreach (var param in methodSetting.Params)
								{
									methodSetting.Params[param.Key] = GetValue(param, viewData);
								}

								_methodHelper.ExecuteMethod(methodSetting, mailMessageContext);
							}

							transaction.Commit();
						}
					}
                }
            }
        }

        public void ExecuteAfterSentMethods(IMailMethodContext context)
        {
            if (!string.IsNullOrEmpty(context.MailMessage.AfterSentMethod))
            {
                using (var transaction = _database.BeginTransaction())
                {
					ExecuteMethod(context, context.MailMessage.AfterSentMethod);
					transaction.Commit();
				}
            }
        }

        public void DeleteMailMessage(MailMessage mailMessage)
        {
            if (!string.IsNullOrEmpty(mailMessage.DeleteMethod))
            {
                var mailMessageContext = new MailMessageContext(_database) {MailMessage = mailMessage};
                ExecuteMethod(mailMessageContext, mailMessage.DeleteMethod);
            }
        }

        private void ExecuteMethod(IMailMethodContext context, string method)
        {
            var mailMethods = JsonConvert.DeserializeObject<MethodSettingsCollection>(method);
            foreach (var methodSetting in mailMethods.MethodSettings)
            {
                _methodHelper.ExecuteMethod(methodSetting, context);
            }
        }

        private MailTemplate GetMailTemplate(MailSettings mailSettings)
        {
            var mailSettingsTemplate = _mailHelperRepository.GetMailSettingsTemplateByMailSettingsId(mailSettings.MailSettingsID);
            return mailSettingsTemplate != null
                ? _database.Repository<MailTemplate>().GetById(mailSettingsTemplate.MailTemplateID)
                : null;
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

    public class MailHelperException : Exception
    {
        public MailHelperException(string message) : base(message)
        {
        }

        public MailHelperException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
