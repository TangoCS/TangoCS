using System;
using System.Collections.Specialized;
using System.Threading;

namespace Tango.TaskManager
{
	public class Task
	{
		public TimeSpan Interval;
		public TaskStartType StartType;
		public string TypeName;
		public string MethodName;
		public NameValueCollection MethodArgs;
		public string TaskName;

		public DateTime LastStartTime = DateTime.MinValue;
		public DateTime LastEndTime = DateTime.MinValue;
		public Thread Thread;
        //public bool IsRunning = false;

        //AppDomain domain;

        //public void Run()
        //{
        //	domain = AppDomain.CreateDomain(TaskName);
        //}
    }

	public enum TaskStartType
	{
		Interval,
		Schedule
	}
}
