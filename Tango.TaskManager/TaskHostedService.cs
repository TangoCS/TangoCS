﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tango.TaskManager
{
    public class TaskHostedService : IHostedService, IDisposable
    {
        Timer _timer;
        static string taskListFile = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}tasklist.xml";
        static string errorLogFile = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}errorlog.txt";
        static List<Task> tasks = new List<Task>();

        public TaskHostedService()
        {
            Load();
        }

        public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            OnStop();

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
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
                Task t = new Task
                {
                    TaskName = xe.Attribute("Name").Value,
                    MethodName = xe.Attribute("Method").Value,
                    TypeName = xe.Attribute("Type").Value,
                    Interval = new TimeSpan(int.Parse(xe.Attribute("Hours").Value), int.Parse(xe.Attribute("Minutes").Value), 0),
                    StartType = xe.Attribute("StartType").Value == "Interval" ? TaskStartType.Interval : TaskStartType.Schedule,
                    MethodArgs = new NameValueCollection()
                };
                foreach (var arg in xe.Elements())
                {
                    t.MethodArgs.Add(arg.Attribute("Name").Value, arg.Attribute("Value").Value);
                }
                tasks.Add(t);
            }
        }

        static void TimerCallback(object o)
        {
            try
            {
                foreach (var t in tasks)
                {
                    if ((t.StartType == TaskStartType.Interval && t.LastStartTime.Add(t.Interval) < DateTime.Now) ||
                        (t.StartType == TaskStartType.Schedule && t.LastStartTime < DateTime.Today.Add(t.Interval) && DateTime.Today.Add(t.Interval) < DateTime.Now))
                    {
                        if (t.Thread != null && t.Thread.IsAlive)
                            continue;

                        t.LastStartTime = DateTime.Now;
                        t.Thread = new Thread(new ParameterizedThreadStart(RunTask));
                        t.Thread.Start(t);
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
                    string val = task.MethodArgs[mp[i].Name];
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
            catch (Exception e)
            {
                LogError(e, task.TaskName);
            }
            finally
            {
                task.LastEndTime = DateTime.Now;
            }
        }

        static void OnStop()
        {
            try
            {
                foreach (Task t in tasks)
                    if (t.Thread != null && t.Thread.IsAlive)
                        t.Thread.Abort();
            }
            catch (Exception e)
            {
                LogError(e, null);
            }
        }

        static void LogError(Exception e, string taskname)
        {
            string str = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss " + (taskname != null ? "Ошибка при выполнении задачи " + taskname : ""));
            while (e != null)
            {
                str += Environment.NewLine + e.Message;
                str += Environment.NewLine + e.StackTrace + Environment.NewLine + Environment.NewLine;
                e = e.InnerException;
            }

            File.AppendAllText(errorLogFile, str, Encoding.UTF8);
        }
    }
}