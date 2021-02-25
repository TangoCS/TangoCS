using System;
using System.ComponentModel;

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
}
