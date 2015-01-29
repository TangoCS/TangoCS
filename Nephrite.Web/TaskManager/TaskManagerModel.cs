using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.TaskManager
{
	public interface IDC_TaskManager : IDataContext
	{
		ITable<ITM_Task> ITM_Task { get; }
		ITable<ITM_TaskExecution> ITM_TaskExecution { get; }
		ITable<ITM_TaskParameter> ITM_TaskParameter { get; }

		ITM_TaskExecution NewITM_TaskExecution();
	}

	public interface ITM_Task : IEntity
	{
		int TaskID { get; set; }
		string Title { get; set; }
		string Class { get; set; }
		bool StartType { get; set; }
		string Method { get; set; }
		int Interval { get; set; }
		System.Nullable<System.DateTime> LastStartDate { get; set; }
		bool IsSuccessfull { get; set; }
		bool IsActive { get; set; }
		bool StartFromService { get; set; }
		System.Nullable<int> ErrorLogID { get; set; }
		int ExecutionTimeout { get; set; }
	}

	public interface ITM_TaskExecution : IEntity
	{
		int TaskExecutionID { get; set; }
		int TaskID { get; set; }
		int LastModifiedUserID { get; set; }
		System.DateTime StartDate { get; set; }
		System.Nullable<System.DateTime> FinishDate { get; set; }
		bool IsSuccessfull { get; set; }
		string MachineName { get; set; }
		string ResultXml { get; set; }
		string ExecutionLog { get; set; }
		System.DateTime LastModifiedDate { get; set; }
	}

	public interface ITM_TaskParameter : IEntity
	{
		int TaskParameterID { get; set; }
		int ParentID { get; set; }
		string Title { get; set; }
		string SysName { get; set; }
		string Value { get; set; }
		int SeqNo { get; set; }
	}
}