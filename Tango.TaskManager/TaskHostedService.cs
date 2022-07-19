using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Sys = System.Threading.Tasks;

namespace Tango.TaskManager
{
    public class TaskHostedService : IHostedService, IDisposable
    {
        Timer _timer;
        static string taskListFile = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}tasklist.xml";
        static string errorLogFile = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}errorlog_{{0}}.txt";
        static List<Task> tasks = new List<Task>();
        private readonly CancellationTokenSource _cancelling = new CancellationTokenSource();

        public TaskHostedService()
        {
            Load();
        }

        public Sys.Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerCallback, _cancelling.Token, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return Sys.Task.CompletedTask;
        }

        public Sys.Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            try
            {
                _cancelling.Cancel();
                OnStop();
            }
            catch (Exception e)
            {
                LogError(e, null);
            }
        
            return Sys.Task.CompletedTask;
        }

        static void OnStop()
        {
            bool isinterrupt = false;
            foreach (Task t in tasks)
            {
                if (t.Thread != null && t.Thread.IsAlive)
                {
                    t.Thread.Interrupt();
                    isinterrupt = true;
                }
            }
            if (isinterrupt)
                Thread.CurrentThread.Join(TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _cancelling.Dispose();
        }

        static void Load()
        {
            if (tasks.Count > 0)
                return;
            if (!File.Exists(taskListFile))
                return;

            var doc = XDocument.Load(taskListFile);

            foreach (var xe in doc.Root.Elements("Task"))
            {
                if (xe.Attribute("ThreadCount") == null || !int.TryParse(xe.Attribute("ThreadCount").Value, out int threadCount))
                    threadCount = 1;

                var TaskName = xe.Attribute("Name").Value;
                var MethodName = xe.Attribute("Method").Value;
                var TypeName = xe.Attribute("Type").Value;
                var Interval = new TimeSpan(int.Parse(xe.Attribute("Hours").Value), int.Parse(xe.Attribute("Minutes").Value), 0);
                var StartType = xe.Attribute("StartType").Value == "Interval" ? TaskStartType.Interval : TaskStartType.Schedule;
                var MethodArgs = new Dictionary<string, object>();
                foreach (var arg in xe.Elements())
                {
                    if (arg.Name == "Argument")
                        MethodArgs.Add(arg.Attribute("Name").Value, arg.Attribute("Value").Value);
                    else
					{
                        string dictionaryName = arg.Attribute("Name").Value;
                        var dictionaryArgs = new Dictionary<string, string>();
                        foreach (var a in arg.Elements())
                        {
                            dictionaryArgs.Add(a.Attribute("Name").Value, a.Attribute("Value").Value);
                        }
                        if (dictionaryArgs.Count > 0)
                            MethodArgs.Add(dictionaryName, dictionaryArgs);
                    }
                }
                for (int i = 0; i < threadCount; i++)
                {
                    Task t = new Task
                    {
                        TaskName = TaskName,
                        MethodName = MethodName,
                        TypeName = TypeName,
                        Interval = Interval,
                        StartType = StartType,
                        MethodArgs = new Dictionary<string, object>(MethodArgs)
                    };
                    tasks.Add(t);
                }
            }
        }

        static void TimerCallback(object o)
        {
            var _cancelling = (CancellationToken)o;

            try
            {
                foreach (var t in tasks)
                {
                    if (_cancelling.IsCancellationRequested)
                        break;

                    if ((t.StartType == TaskStartType.Interval && t.LastStartTime.Add(t.Interval) < DateTime.Now) ||
                        (t.StartType == TaskStartType.Schedule && t.LastStartTime < DateTime.Today.Add(t.Interval) && DateTime.Today.Add(t.Interval) < DateTime.Now))
                    {
                        if (t.Thread != null && t.Thread.IsAlive)
                            continue;

                        t.LastStartTime = DateTime.Now;
                        t.Thread = new Thread(new ParameterizedThreadStart(RunTask));
                        t.Thread.Start(t);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e, null);
            }
        }

        static void RunTask(object obj)
        {
            var task = (Task)obj;

            try
            {
                Type type = Type.GetType(task.TypeName, true, true);
                MethodInfo mi = type.GetMethod(task.MethodName);

                ParameterInfo[] mp = mi.GetParameters();
                object[] p = new object[mp.Length];
                for (int i = 0; i < mp.Length; i++)
                {
                    object val = task.MethodArgs.GetValueOrDefault(mp[i].Name);
                    try
                    {
                        p[i] = Convert.ChangeType(val, mp[i].ParameterType);
                    }
                    catch
                    {
                        switch (mp[i].ParameterType.Name)
                        {
                            case "DateTime":
                                p[i] = DateTime.Today;
                                break;
                            case "Int32":
                                p[i] = 0;
                                break;
                            case "Boolean":
                                p[i] = false;
                                break;
                            default:
                                throw;
                        }
                    }
                }
                mi.Invoke(null, p);
            }
            catch (ThreadAbortException)
            {
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (Exception e)
            {
                LogError(e, task.TaskName);
            }
            finally
            {
                task.LastEndTime = DateTime.Now;
            }
        }

        public static void LogError(Exception e, string taskname)
        {
            string str = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss " + (taskname != null ? "Ошибка при выполнении задачи " + taskname : ""));
            while (e != null)
            {
                str += Environment.NewLine + e.Message;
                str += Environment.NewLine + e.StackTrace + Environment.NewLine + Environment.NewLine;
                e = e.InnerException;
            }

            File.AppendAllText(string.Format(errorLogFile, taskname?.ToLower() ?? "main"), str, Encoding.UTF8);
        }
    }
}
