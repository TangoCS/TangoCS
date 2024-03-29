﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cronos;
using Tango.Cache;
using Tango.Html;
using Tango.Identity;
using Tango.Identity.Std;
using Tango.Localization;
using Tango.Logger;
using Tango.LongOperation;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.Tasks
{
	public class TaskController<TUser> : BaseTaskController where TUser : class
	{
        [Inject]
        protected IIdentityManager<TUser> Identity { get; set; }

		[Inject]
        protected IUserIdAccessor<object> UserIdAccessor { get; set; }

        protected override void ExecutingTaskUser(IScheduledTask task)
        {
            Identity.RunAs(Identity.SystemUser, () => Run(task));
        }

        protected override void SetLastModifiedUser(TaskExecution execution, bool isManual)
        {
            execution.LastModifiedUserID = isManual ? UserIdAccessor.CurrentUserID : UserIdAccessor.SystemUserID;
        }
    }

    public abstract class BaseTaskController : BaseController
    {
        [Inject]
        protected ITaskControllerRepository Repository { get; set; }
        [Inject]
        protected IErrorLogger ErrorLogger { get; set; }
        [Inject]
        protected ICache Cache { get; set; }
		[Inject]
        protected ITaskProgress TaskProgress { get; set; }

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
        protected abstract void SetLastModifiedUser(TaskExecution execution, bool isManual);

        [Obsolete]
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

        //[HttpGet]
        //public ActionResult RunTask(int id)
        //{
        //    var task = Repository.GetTask(id);
        //    ExecutingTaskUser(task);
        //    return new HttpResult();
        //}
                
        static Dictionary<int, (decimal percent, string description)> progress = new Dictionary<int, (decimal percent, string description)>();
        public static IDictionary<int, (decimal percent, string description)> Progress => progress;

		[Obsolete]
		public void Run(IScheduledTask task, bool isManual = false, Dictionary<string, string> param = null, bool withLogger = false)
        {
            progress[task.ID] = (0, "");

            var taskexec = new TaskExecution
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

			TaskExecutionContext context = new TaskExecutionContext()
			{
				ExecutionDetails = new Tango.Html.HtmlWriter(),
				IsManual = isManual,
				ExecutionID = taskexecid,
				ProgressLogger = null,
				TaskID = task.ID
			};

			try
			{
                var type = TaskTypeCollection.GetType(task.Class);
                var obj = CreateTaskInstance(type);

                if (withLogger)
                {
                    var connid = Context.GetArg("connid");
                    var loggercollection = Cache.GetOrAdd("RealTimeLoggers", () => new ConcurrentDictionary<string, IRealTimeProgressLogger>());

                    if (!loggercollection.TryGetValue(connid, out IRealTimeProgressLogger logger))
                    {
						logger = base.Context.RequestServices.GetService(typeof(IRealTimeProgressLogger)) as IRealTimeProgressLogger;
                        loggercollection.AddIfNotExists(connid, logger);
                    }
					progressLogger = logger;
					context.ProgressLogger = logger;
				}

                MethodInfo mi = type.GetMethod(task.Method);
                ParameterInfo[] mp = mi.GetParameters();
                if (param == null) param = new Dictionary<string, string>();

                InitTaskParms(task, mp, param);

                var p = GetTaskParmValues(context, mp, param);

                mi.Invoke(obj, p);

                taskexec = new TaskExecution
                {
                    TaskExecutionID = taskexecid,
                    LastModifiedDate = DateTime.Now,
                    FinishDate = DateTime.Now,
                    TaskID = task.ID,
                    IsSuccessfull = true,
                    ResultXml = ""
                };
                if (context != null)
                {
                    taskexec.ResultXml = context.ExecutionDetails.ToString();
					taskexec.ResultCode = context.ResultCode;
				}
                Repository.UpdateTaskExecution(taskexec);
                TaskProgress.SetProgress(task.ID, 100, Repository.resourceManager.GetExt<Task>("completed"));
            }
            //catch (ThreadAbortException)
            //{
            //}
            catch (Exception ex)
            {
                if (progressLogger != null)
                    progressLogger.WriteExeptionMessage(ex);

                int errorid = ErrorLogger.Log(ex);
                taskexec = new TaskExecution
                {
                    TaskExecutionID = taskexecid,
                    LastModifiedDate = DateTime.Now,
                    FinishDate = DateTime.Now,
                    TaskID = task.ID,
                    IsSuccessfull = false,
					ResultXml = "",
					ResultCode = 3
                };
                if (context != null)
                {
					if (!context.ExecutionDetails.IsEmpty())
						context.ExecutionDetails.Br();
                    taskexec.ResultXml = context.ExecutionDetails.ToString();
                }
				Repository.UpdateTaskExecutionError(taskexec, errorid);
            }
        }

		[Obsolete]
		public HtmlWriter CustomRun(IScheduledTask task, Dictionary<string, string> param = null)
        {
            var type = TaskTypeCollection.GetType(task.Class);
            var obj = CreateTaskInstance(type);

            TaskExecutionContext context = new TaskExecutionContext() {
                ExecutionDetails = new Tango.Html.HtmlWriter(),
                IsManual = true,
                TaskID = task.ID
            };

            MethodInfo mi = type.GetMethod(task.Method);
            ParameterInfo[] mp = mi.GetParameters();
            if (param == null) param = new Dictionary<string, string>();

            InitTaskParms(task, mp, param);

            var p = GetTaskParmValues(context, mp, param);

            mi.Invoke(obj, p);

            return context.ExecutionDetails;
        }

        protected object[] GetTaskParmValues(TaskExecutionContext context, ParameterInfo[] mp, Dictionary<string, string> param)
        {
            object[] p = new object[mp.Length];

            for (int i = 0; i < mp.Length; i++)
            {
                if (mp[i].ParameterType.Name == typeof(TaskExecutionContext).Name)
                {
                    p[i] = context;
                    continue;
                }
                else if (mp[i].ParameterType.IsInterface)
                {
                    p[i] = Context.RequestServices.GetService(mp[i].ParameterType);
                    continue;
                }
                string val = param[mp[i].Name];

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

            return p;
        }

        protected void InitTaskParms(IScheduledTask task, ParameterInfo[] mp, Dictionary<string, string> param)
        {
            var taskparam = Repository.GetTaskParameters(task.ID).ToDictionary(x => x.SysName.ToLower(), x => x.Value);

            for (int i = 0; i < mp.Length; i++)
            {
                if (!param.ContainsKey(mp[i].Name) && taskparam.ContainsKey(mp[i].Name.ToLower()))
                {
                    string val = taskparam[mp[i].Name.ToLower()];
                    var defValueAttr = mp[i].GetCustomAttribute<DefaultValueAttribute>(false);
                    if (val.IsEmpty() && defValueAttr != null)
                    {
                        var providerType = defValueAttr.Value as Type;
                        var provider = Activator.CreateInstance(providerType, Context.RequestServices) as ITaskParameterDefaultValueProvider;
                        val = provider.GetValue(task, mp[i]);
                    }
                    param.Add(mp[i].Name, val);
                }
            }
        }

        protected object CreateTaskInstance(Type type)
        {
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

            return obj;
        }

		[Obsolete]
		public void RunWithTimeOut(IScheduledTask task, bool isManual = false, Dictionary<string, string> param = null, bool withLogger = false)
		{
			CancellationTokenSource source = new CancellationTokenSource();
			source.Token.Register(OnTaskCancel);
			source.CancelAfter(TimeSpan.FromMinutes(task.ExecutionTimeout));
			
            try
			{
				var thread = System.Threading.Tasks.Task.Run(() => Run(task, isManual, param, withLogger), source.Token);
				thread.Wait();
			}
			finally
			{
				source?.Dispose();
			}

			void OnTaskCancel()
			{
				source?.Dispose();
			}
		}
	}
}
