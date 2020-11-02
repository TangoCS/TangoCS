using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Tango.Html;
using Tango.Logger;

namespace Tango.LongOperation
{
	public abstract class ScheduledTaskTicket : LongOperationTicket
	{
		public override int ActionID => Task.ID;
		public override string Title => Task.Title;
		public IScheduledTask Task { get; }
		public Dictionary<string, string> Parameters { get; protected set; }
		public bool IsManualStart { get; }

		public ScheduledTaskTicket(
			IScheduledTask task,
			Dictionary<string, string> parameters = null, bool isManualStart = false) : base(task.ExecutionTimeout)
		{
			Task = task;
			IsManualStart = isManualStart;
			Parameters = parameters;
		}

		void SetMethodParms(TaskExecutionContext context, ParameterInfo[] mp, object[] p)
		{
			for (int i = 0; i < mp.Length; i++)
			{
				if (mp[i].ParameterType.Name == typeof(TaskExecutionContext).Name)
				{
					p[i] = context;
					continue;
				}

				var val = Parameters.Single(o => o.Key == mp[i].Name.ToLower()).Value;

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
		}

		object CreateTask(Type type)
		{
			ConstructorInfo ci = type.GetConstructors().FirstOrDefault();
			if (ci != null)
			{
				ParameterInfo[] pi = ci.GetParameters();
				object[] pr = new object[pi.Length];
				for (int i = 0; i < pi.Length; i++)
				{
					if (pi[i].ParameterType.IsInterface)
						pr[i] = provider.GetService(pi[i].ParameterType);
				}
				return ci.Invoke(pr);
			}
			else
				return FormatterServices.GetUninitializedObject(type);
		}

		void InjectDependences(object obj)
		{
			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute))))
			{
				prop.SetValue(obj, provider.GetService(prop.PropertyType));
			}
		}

		protected abstract int GetTaskExecutionID(IDbConnection connection);
		protected abstract Dictionary<string, string> GetTaskDefaultParameters(IDbConnection connection);

		protected abstract void SetTaskCompleted(IDbConnection connection, TaskExecutionContext context);
		protected abstract void SetTaskError(IDbConnection connection, TaskExecutionContext context, Exception ex);

		public override void Run(IServiceProvider provider, IProgressLogger progressLogger)
		{
			base.Run(provider, progressLogger);

			var connection = provider.GetService(typeof(IDbConnection)) as IDbConnection;

			var taskexecid = GetTaskExecutionID(connection);

			var context = new TaskExecutionContext() {
				ExecutionDetails = new HtmlWriter(),
				IsManual = IsManualStart,
				ExecutionID = taskexecid,
				ProgressLogger = progressLogger,
				Task_ID = Task.ID
			};

			try
			{
				var cls = Task.Class;
				if (!DefaultTaskAssembly.IsEmpty())
					cls += "," + DefaultTaskAssembly;

				Type type = Type.GetType(cls, true);
				var obj = CreateTask(type);
				InjectDependences(obj);

				MethodInfo mi = type.GetMethod(Task.Method);
				ParameterInfo[] mp = mi.GetParameters();
				object[] p = new object[mp.Length];

				Parameters = Parameters?.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase) ??
					GetTaskDefaultParameters(connection) ??
					new Dictionary<string, string>();

				SetMethodParms(context, mp, p);
				mi.Invoke(obj, p);

				SetTaskCompleted(connection, context);
			}
			//catch (ThreadAbortException)
			//{
			//}
			catch (Exception ex)
			{
				if (progressLogger != null)
					progressLogger.WriteMessage(ex.ToString());

				SetTaskError(connection, context, ex);
			}
		}
	}

	public class TaskExecutionContext
	{
		public HtmlWriter ExecutionDetails { get; set; }
		public bool IsManual { get; set; } = false;
		public int ExecutionID { get; set; } = 0;
		public IProgressLogger ProgressLogger { get; set; }
		public int Task_ID { get; set; }
	}
}
