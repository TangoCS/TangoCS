using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nephrite.Identity;
using System.IO;
using System.Text;
using Nephrite.ErrorLog;


namespace Nephrite.Web.TaskManager
{
	public static class TaskManager
	{
		static DateTime lastRun = DateTime.MinValue;

		public static void Run(bool startFromService)
		{
			using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
			{
				// Задачи, которые не успели завершиться, пометить как завершенные
				foreach (var t in 
					from o in dc.ITM_TaskExecution
					from t in dc.ITM_Task
					where o.FinishDate == null && o.TaskID == t.TaskID && t.StartFromService == startFromService
					select o)
				{
					if (t.StartDate.AddMinutes(dc.ITM_Task.Single(o => o.TaskID == t.TaskID).ExecutionTimeout) < DateTime.Now)
					{
						t.ExecutionLog += "\nExecution timed out";
						t.FinishDate = DateTime.Now;
					}
				}
				dc.SubmitChanges();


				var tasks = dc.ITM_Task.Where(o => o.IsActive && o.StartFromService == startFromService && !dc.ITM_TaskExecution.Any(o1 => o1.TaskID == o.TaskID && o1.FinishDate == null)).ToList();
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

		public static void Run(int taskID)
		{
			bool isServiceRun = false;
			string taskName = "";
			try
			{
				List<ITM_TaskParameter> taskparms = new List<ITM_TaskParameter>();

				using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
				{
					//dc.Log = new StringWriter();
					task = dc.ITM_Task.Single(o => o.TaskID == taskID);
					//task.TM_TaskParameters.Count();

					ITM_TaskExecution taskexec = dc.NewITM_TaskExecution();

					taskexec.LastModifiedDate = DateTime.Now;
					taskexec.StartDate = DateTime.Now;
					taskexec.MachineName = Environment.MachineName;
					taskexec.TaskID = taskID;
					taskexec.LastModifiedUserID = Subject.Current.ID;

					dc.ITM_TaskExecution.InsertOnSubmit(taskexec);
					dc.SubmitChanges();
					taskexecid = taskexec.TaskExecutionID;

					taskparms = dc.ITM_TaskParameter.Where(o => o.ParentID == taskID).ToList();
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
					Subject.System.Run(() => mi.Invoke(null, p));

					using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
					{
						try
						{
							//dc.Log = new StringWriter();
							var t = dc.ITM_Task.Single(o => o.TaskID == taskID);
							t.LastStartDate = DateTime.Now;
							t.IsSuccessfull = true;
							var te = dc.ITM_TaskExecution.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
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
					using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
					{
						//dc.Log = new StringWriter();
						var t = dc.ITM_Task.Single(o => o.TaskID == taskID);
						if (isServiceRun)
							LogError(e, taskName, dc.Log.ToString());
						else
							ErrorLogger.Log(e);
						t.IsSuccessfull = false;
						t.LastStartDate = DateTime.Now;
						t.ErrorLogID = errid;
						var te = dc.ITM_TaskExecution.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
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

		public static void RunService(string connectionString)
		{
			ConnectionManager.SetConnectionString(connectionString);
			List<ITM_Task> tasks;
			using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
			{
				// Задачи, которые не успели завершиться, пометить как завершенные
				foreach (var t in
					from o in dc.ITM_TaskExecution
					from t in dc.ITM_Task
					where o.FinishDate == null && o.TaskID == t.TaskID && t.StartFromService
					select o)
				{
					if (t.StartDate.AddMinutes(dc.ITM_Task.Single(o => o.TaskID == t.TaskID).ExecutionTimeout) < DateTime.Now)
					{
						t.ExecutionLog += "\nExecution timed out";
						t.FinishDate = DateTime.Now;
					}
				}
				dc.SubmitChanges();
				tasks = dc.ITM_Task.Where(o => o.IsActive && o.StartFromService && !!dc.ITM_TaskExecution.Any(o1 => o1.TaskID == o.TaskID && o1.FinishDate == null)).ToList();
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
				Subject.System.Run(() => Run(task.TaskID));
			}
		}

		public static void Log(string text)
		{
			using (var dc = (A.Model.NewDataContext()) as IDC_TaskManager)
			{
				var te = dc.ITM_TaskExecution.SingleOrDefault(o => o.TaskExecutionID == taskexecid);
				te.ExecutionLog += text += Environment.NewLine;
				te.LastModifiedDate = DateTime.Now;
				dc.SubmitChanges();
			}
		}
		
		[ThreadStatic]
		static ITM_Task task;

		[ThreadStatic]
		static int taskexecid;

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
