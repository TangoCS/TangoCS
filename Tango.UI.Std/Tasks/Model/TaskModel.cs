using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;

namespace Tango.Tasks
{
	public enum TaskStatusType
	{
		[Description("Не исполнено")]
		None = 0,
		[Description("В работе")]
		Progress = 1,
		[Description("Исполнено успешно")]
		Complete = 2,
		[Description("Ошибка исполнения")]
		Error = 3,
	}

	public class TaskRunning
	{
		public int TaskID { get; set; }
		public int TaskExecutionID { get; set; }
		public DateTime StartDate { get; set; }
		public int ExecutionTimeout { get; set; }
	}

	public static class TaskTypeCollection
	{
		static ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();
	
		public static Type GetType(string fullnameclass)
        {
            if (fullnameclass.IsEmpty())
                return null;

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
}
