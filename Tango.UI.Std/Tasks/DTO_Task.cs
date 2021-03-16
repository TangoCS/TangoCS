using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
	public partial class DTO_Task : IEntity, IWithKey<DTO_Task, int>, IWithTitle, IScheduledTask
	{
		public virtual Expression<Func<DTO_Task, bool>> KeySelector(int id)
		{
			return o => o.TaskID == id;
		}
		public virtual int ID { get { return TaskID; } }
		[Key]
		[Column]
		public virtual int TaskID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SystemName { get; set; }
		public virtual string Class { get; set; }
		public virtual string Method { get; set; }
		public virtual string Interval { get; set; }
		public virtual int ExecutionTimeout { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual DateTime? LastStartDate { get; set; }
		public virtual bool StartFromService { get; set; }
		public virtual int Status { get; set; }
		public virtual int? TaskGroupID { get; set; }
		public virtual int StartTypeID { get; set; }
	
		public virtual string StartTypeTitle { get; set; }
		public virtual string GroupTitle { get; set; }
	}

	public partial class DTO_TaskParameter : IEntity, IWithKey<DTO_TaskParameter, int>, IWithSeqNo, IWithTitle
	{
		public virtual Expression<Func<DTO_TaskParameter, bool>> KeySelector(int id)
		{
			return o => o.TaskParameterID == id;
		}
		public virtual int ID { get { return TaskParameterID; } }
		[Key]
		[Column] 
		public virtual int TaskParameterID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string Value { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual int ParentID { get; set; }

		public virtual string ParentTitle { get; set; }
		public virtual string ParentClass { get; set; }
		public virtual string ParentMethod { get; set; }
	}

	public partial class DTO_TaskGroup : IEntity, IWithKey<DTO_TaskGroup, int>, IWithTitle
	{
		public virtual Expression<Func<DTO_TaskGroup, bool>> KeySelector(int id)
		{
			return o => o.TaskGroupID == id;
		}
		public virtual int ID { get { return TaskGroupID; } }
		public virtual int TaskGroupID { get; set; }
		public virtual string Title { get; set; }
	}

	public partial class DTO_TaskStartType : IEntity, IWithKey<DTO_TaskStartType, int>, IWithTitle
	{
		public virtual Expression<Func<DTO_TaskStartType, bool>> KeySelector(int id)
		{
			return o => o.TaskStartTypeID == id;
		}
		public virtual int ID { get { return TaskStartTypeID; } }
		public virtual int TaskStartTypeID { get; set; }
		public virtual string Title { get; set; }
	}
	
	public partial class DTO_TaskExecution : IEntity, IWithKey<DTO_TaskExecution, int>
	{
		public virtual Expression<Func<DTO_TaskExecution, bool>> KeySelector(int id)
		{
			return o => o.TaskExecutionID == id;
		}
		public virtual int ID { get { return TaskExecutionID; } }
		[Key]
		[Column] 
		public virtual int TaskExecutionID { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual DateTime? FinishDate { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }
		public virtual bool IsSuccessfull { get; set; }
		public virtual string MachineName { get; set; }
		public virtual string ResultXml { get; set; }
		public virtual string ExecutionLog { get; set; }
		public virtual int TaskID { get; set; }
		public virtual object LastModifiedUserID { get; set; }

		public virtual string TaskName { get; set; }
		public virtual string UserName{ get; set; }
	}
}
