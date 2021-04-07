using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cronos;
using Tango.Cache;
using Tango.Identity;
using Tango.Identity.Std;
using Tango.Logger;
using Tango.LongOperation;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.Tasks
{
	public class TaskController : BaseTaskController
	{
        [Inject]
        protected IIdentityManager Identity { get; set; }

        protected override void ExecutingTaskUser(IScheduledTask task)
        {
            Identity.RunAs(Identity.SystemUser, () => Run(task));
        }

        protected override void SetLastModifiedUser(DTO_TaskExecution execution, bool isManual)
        {
            execution.LastModifiedUserID = isManual ? Identity.CurrentUser.Id : Identity.SystemUser.Id;
        }
    }

    public abstract class BaseTaskController : BaseController
    {
        [Inject]
        protected ITaskControllerRepository Repository { get; set; }
        [Inject]
        protected IErrorLogger errorLogger { get; set; }
        [Inject]
        protected ICache Cache { get; set; }

        protected virtual string DefaultTaskAssembly => null;

        /// <summary>
        /// Запуск задачи от имени пользователя
        /// </summary>
        /// <param name="task"></param>
        protected abstract void ExecutingTaskUser(IScheduledTask task);

        /// <summary>
        /// Установка занчения id пользователя последнего запустившего задачу
        /// </summary>
        /// <param name="execution"></param>
        /// <param name="isManual"></param>
        protected abstract void SetLastModifiedUser(DTO_TaskExecution execution, bool isManual);

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RunTasks()
        {
            var running = Repository.TasksRunning();
            foreach (var t in running)
            {
                if (t.StartDate.AddMinutes(t.ExecutionTimeout) < DateTime.Now)
                {
                    Repository.UpdateTaskExecutionTimeOut(t);
                }
            }

            var tasks = Repository.TasksForExecute().Where(o => o.IsActive);

            foreach (var task in tasks)
            {
                if (task.LastStartDate.HasValue)
                {
                    if (task.StartTypeID == 1) // В заданное время суток
                    {
                        // в случае применения для интервалов, работает не совсем корректно
                        var oldTimeUtc = new DateTimeOffset(task.LastStartDate.Value);
                        CronExpression expression = CronExpression.Parse(task.Interval);
                        var next = expression.GetNextOccurrence(oldTimeUtc, TimeZoneInfo.Local);
                        DateTime? nextTime = next?.DateTime;

                        if (nextTime == null || nextTime > DateTime.Now)
                            continue; // Еще не время или время не корректно
                    }
                    else // Задан интервал в минутах
                    {
                        if (DateTime.Now.Subtract(task.LastStartDate.Value).TotalMinutes < task.Interval.ToInt32(0))
                            continue; // Ещё не прошло нужное количество минут
                    }
                }
                ExecutingTaskUser(task);
            }
            return new HttpResult();
        }

        [HttpGet]
        public ActionResult RunTask(int id)
        {
            var task = Repository.GetTask(id);
            ExecutingTaskUser(task);

            return new HttpResult();
        }

        public void Run(IScheduledTask task, bool isManual = false, Dictionary<string, string> param = null, bool withLogger = false)
        {
            DTO_TaskExecution taskexec = new DTO_TaskExecution
            {
                LastModifiedDate = DateTime.Now,
                StartDate = DateTime.Now,
                MachineName = Environment.MachineName,
                TaskID = task.ID,
                IsSuccessfull = false
            };

            SetLastModifiedUser(taskexec, isManual);

            int taskexecid = Repository.CreateTaskExecution(taskexec);
            IRealTimeProgressLogger progressLogger = null;

            try
            {
                var taskclass = task.Class;
                if (taskclass.Split(',').Length == 1 && !DefaultTaskAssembly.IsEmpty())
                    taskclass += "," + DefaultTaskAssembly;

                Type type = Type.GetType(taskclass, true);
                object obj = null;
                ConstructorInfo ci = type.GetConstructors().FirstOrDefault();
                if (ci != null)
                {
                    ParameterInfo[] pi = ci.GetParameters();
                    object[] pr = new object[pi.Length];
                    for (int i = 0; i < pi.Length; i++)
                    {
                        if (pi[i].ParameterType.IsInterface)
                            pr[i] = base.Context.RequestServices.GetService(pi[i].ParameterType);
                    }
                    obj = ci.Invoke(pr);
                }
                foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                            .Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute))))
                    prop.SetValue(obj, base.Context.RequestServices.GetService(prop.PropertyType));

                MethodInfo mi = type.GetMethod(task.Method);
                ParameterInfo[] mp = mi.GetParameters();
                object[] p = new object[mp.Length];

                TaskExecutionContext context = null;
                DTO_TaskParameter[] taskparam = param != null ? null : Repository.GetTaskParameters(task.ID).ToArray();

                if (withLogger)
                {
                    var connid = Context.GetArg("connid");
                    var loggercollection = Cache.GetOrAdd("RealTimeLoggers", () => new ConcurrentDictionary<string, IRealTimeProgressLogger>());

                    if (loggercollection.TryGetValue(connid, out IRealTimeProgressLogger logger))
                        progressLogger = logger;
                    else
                    {
                        progressLogger = base.Context.RequestServices.GetService(typeof(IRealTimeProgressLogger)) as IRealTimeProgressLogger;
                        loggercollection.AddIfNotExists(connid, progressLogger);
                    }
                }

                for (int i = 0; i < mp.Length; i++)
                {
                    if (mp[i].ParameterType.Name == typeof(TaskExecutionContext).Name)
                    {
                        context = new TaskExecutionContext()
                        {
                            ExecutionDetails = new Tango.Html.HtmlWriter(),
                            IsManual = isManual,
                            ExecutionID = taskexecid,
                            ProgressLogger = progressLogger,
                            TaskID = task.ID
                        };
                        p[i] = context;
                        continue;
                    }
                    string val;
                    if (param == null)
                    {
                        val = taskparam.Single(o => o.ParentID == task.ID && o.SysName.ToLower() == mp[i].Name.ToLower()).Value;
                        var defValueAttr = mp[i].GetCustomAttribute<DefaultValueAttribute>(false);
                        if (val.IsEmpty() && defValueAttr != null)
                        {
                            var providerType = defValueAttr.Value as Type;
                            var provider = Activator.CreateInstance(providerType, Context.RequestServices) as ITaskParameterDefaultValueProvider;
                            val = provider.GetValue(task, mp[i]);
                        }
                    }
                    else
                        val = param.Single(o => o.Key.ToLower() == mp[i].Name.ToLower()).Value;

                    var typeConverter = TypeDescriptor.GetConverter(mp[i].ParameterType);

                    if (mp[i].ParameterType == typeof(DateTime) || mp[i].ParameterType == typeof(DateTime?))
                    {
                        if (!DateTime.TryParseExact(val, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                        {
                            if (!DateTime.TryParseExact(val, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                p[i] = null;
                            else
                                p[i] = dt;
                        }
                        else
                            p[i] = dt;

                    }
                    else if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)) && typeConverter.IsValid(val))
                    {
                        p[i] = typeConverter.ConvertFromString(val);
                    }
                    else
                    {
                        switch (mp[i].ParameterType.Name)
                        {
                            case "DateTime":
                                p[i] = DateTime.Today; break;
                            case "Int32":
                                p[i] = 0; break;
                            case "Boolean":
                                p[i] = false; break;
                            default:
                                p[i] = null; break;
                        }
                    }
                }
                mi.Invoke(obj, p);

                taskexec = new DTO_TaskExecution
                {
                    TaskExecutionID = taskexecid,
                    LastModifiedDate = DateTime.Now,
                    FinishDate = DateTime.Now,
                    TaskID = task.ID,
                    IsSuccessfull = true
                };
                if (context != null)
                    taskexec.ResultXml = context.ExecutionDetails.GetStringBuilder().ToString();

                Repository.UpdateTaskExecution(taskexec);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                if (progressLogger != null)
                    progressLogger.WriteExeptionMessage(ex);

                int errorid = errorLogger.Log(ex);
                taskexec = new DTO_TaskExecution
                {
                    TaskExecutionID = taskexecid,
                    LastModifiedDate = DateTime.Now,
                    FinishDate = DateTime.Now,
                    TaskID = task.ID,
                    IsSuccessfull = false
                };
                Repository.UpdateTaskExecutionError(taskexec, errorid);
            }
        }
    }
}
