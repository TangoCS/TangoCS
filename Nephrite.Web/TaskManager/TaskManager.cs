using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Threading;
using Nephrite.Web.App;
using Nephrite.Web.SPM;
using Nephrite.Web.Model;
using System.IO;
using System.Text;

namespace Nephrite.Web.TaskManager
{
	public static class TaskManager
	{
		static DateTime lastRun = DateTime.MinValue;
		public static void Run()
		{
			using (var dc = new HCoreDataContext(AppWeb.DBConfig))
			{
				// Задачи, которые не успели завершиться, пометить как завершенные
				foreach (var t in from o in dc.TM_TaskExecutions
								  from t in dc.TM_Tasks
								  where o.FinishDate == null && o.TaskID == t.TaskID && t.StartFromService == (HttpContext.Current == null)
								  select o)
				{
					if (t.StartDate.AddMinutes(dc.TM_Tasks.Single(o => o.TaskID == t.TaskID).ExecutionTimeout) < DateTime.Now)
					{
						t.ExecutionLog += "\nExecution timed out";
						t.FinishDate = DateTime.Now;
					}
				}
				dc.SubmitChanges();


				var tasks = dc.TM_Tasks.Where(o => o.IsActive && o.StartFromService == (HttpContext.Current == null) && !dc.TM_TaskExecutions.Any(o1 => o1.TaskID == o.TaskID && o1.FinishDate == null)).ToList();
				foreach (var task in tasks)
				{
					if (task.LastStartDate.HasValue)
					{
						if (task.StartType) // В заданное время суток
						{
							if (task.LastStartDate.Value.Date == DateTime.Today)
								continue; // Сегодня уже запускали
							if (DateTime.Today.AddMinutes(task.Interval) > DateTime.Now)
								continue; // Еще не время
						}
						else // Задан интервал в минутах
						{
							if (DateTime.Now.Subtract(task.LastStartDate.Value).TotalMinutes < task.Interval)
								continue; // Ещё не прошло нужное количество минут
						}
					}
					int taskID = task.TaskID;
					Run(taskID);
				}
			}
		}

		public static void RunService(string connectionString)
		{
			ConnectionManager.SetConnectionString(connectionString);
			List<TM_Task> tasks;
			using (var dc = new HCoreDataContext(AppWeb.DBConfig))
			{
				// Задачи, которые не успели завершиться, пометить как завершенные
				foreach (var t in
					from o in dc.TM_TaskExecutions
					from t in dc.TM_Tasks
					where o.FinishDate == null && o.TaskID == t.TaskID && t.StartFromService
					select o)
				{
					if (t.StartDate.AddMinutes(dc.TM_Tasks.Single(o => o.TaskID == t.TaskID).ExecutionTimeout) < DateTime.Now)
					{
						t.ExecutionLog += "\nExecution timed out";
						t.FinishDate = DateTime.Now;
					}
				}
				dc.SubmitChanges();
				tasks = dc.TM_Tasks.Where(o => o.IsActive && o.StartFromService && !!dc.TM_TaskExecutions.Any(o1 => o1.TaskID == o.TaskID && o1.FinishDate == null)).ToList();
			}
			foreach (var task in tasks)
			{
				if (task.LastStartDate.HasValue)
				{
					if (task.StartType) // В заданное время суток
					{
						if (task.LastStartDate.Value.Date == DateTime.Today)
							continue; // Сегодня уже запускали
						if (DateTime.Today.AddMinutes(task.Interval) > DateTime.Now)
							continue; // Еще не время
					}
					else // Задан интервал в минутах
					{
						if (DateTime.Now.Subtract(task.LastStartDate.Value).TotalMinutes < task.Interval)
							continue; // Ещё не прошло нужное количество минут
					}
				}
				AppSPM.RunWithElevatedPrivileges(() => Run(task.TaskID));
			}
		}


		[ThreadStatic]
		static TM_Task task;

		[ThreadStatic]
		static int taskexecid;

		public static void Log(string text)
		{
			using (var dc = new HCoreDataContext(AppWeb.DBConfig))
			{
				var te = dc.TM_TaskExecutions.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
				te.ExecutionLog += text += Environment.NewLine;
				te.LastModifiedDate = DateTime.Now;
				dc.SubmitChanges();
			}
		}
				
		public static void Run(int taskID)
		{
			bool isServiceRun = false;
			string taskName = "";
			try
			{
				List<TM_TaskParameter> taskparms = new List<TM_TaskParameter>();

				using (var dc = new HCoreDataContext(AppWeb.DBConfig))
				{
					dc.Log = new StringWriter();
					task = dc.TM_Tasks.Single(o => o.TaskID == taskID);
					//task.TM_TaskParameters.Count();

					var taskexec = new TM_TaskExecution
					{
						LastModifiedDate = DateTime.Now,
						StartDate = DateTime.Now,
						MachineName = Environment.MachineName,
						TaskID = taskID,
						LastModifiedUserID = AppSPM.GetCurrentSubjectID()
					};
					dc.TM_TaskExecutions.InsertOnSubmit(taskexec);
					dc.SubmitChanges();
					taskexecid = taskexec.TaskExecutionID;

					taskparms = dc.TM_TaskParameters.Where(o => o.ParentID == taskID).ToList();
				}

				isServiceRun = task.StartFromService;
				taskName = task.Title;

				try
				{
					Type type = Type.GetType(task.Class, true, true);
					MethodInfo mi = type.GetMethod(task.Method);

					ParameterInfo[] mp = mi.GetParameters();
					object[] p = new object[mp.Length];
					for (int i = 0; i < mp.Length; i++)
					{
						try
						{
							string val = taskparms.Single(o => o.SysName.ToLower() == mp[i].Name.ToLower()).Value;
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
					AppSPM.RunWithElevatedPrivileges(() => mi.Invoke(null, p));

					using (var dc = new HCoreDataContext(AppWeb.DBConfig))
					{
						try
						{
							dc.Log = new StringWriter();
							var t = dc.TM_Tasks.Single(o => o.TaskID == taskID);
							t.LastStartDate = DateTime.Now;
							t.IsSuccessfull = true;
							var te = dc.TM_TaskExecutions.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
							te.LastModifiedDate = DateTime.Now;
							te.FinishDate = DateTime.Now;
							te.IsSuccessfull = true;
							dc.SubmitChanges();
						}
						catch (Exception e)
						{
							throw new Exception(dc.Log.ToString(), e);
						}
					}
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception e)
				{
					int errid = ErrorLogger.Log(e);
					using (var dc = new HCoreDataContext(AppWeb.DBConfig))
					{
						dc.Log = new StringWriter();
						var t = dc.TM_Tasks.Single(o => o.TaskID == taskID);
						if (isServiceRun)
							LogError(e, taskName, dc.Log.ToString());
						else
							ErrorLogger.Log(e);
						t.IsSuccessfull = false;
						t.LastStartDate = DateTime.Now;
						t.ErrorLogID = errid;
						var te = dc.TM_TaskExecutions.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
						te.LastModifiedDate = DateTime.Now;
						te.FinishDate = DateTime.Now;
						te.IsSuccessfull = false;
						dc.SubmitChanges();
					}
				}
			}
			catch (Exception e)
			{
				if (isServiceRun)
					LogError(e, taskName, "");
				ErrorLogger.Log(e);
			}
		}

		static void LogError(Exception e, string taskname, string sql)
		{
			string str = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss " + (taskname != null ? "Ошибка при выполнении задачи " + taskname : ""));
			while (e != null)
			{
				str += Environment.NewLine + e.Message;
				str += Environment.NewLine + e.StackTrace + Environment.NewLine + Environment.NewLine;
				e = e.InnerException;
			}

			File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\errorlog.txt", str, Encoding.UTF8);
			File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\errorlog.txt", "------SQL------\r\n" + sql, Encoding.UTF8);
		}
	}
}
