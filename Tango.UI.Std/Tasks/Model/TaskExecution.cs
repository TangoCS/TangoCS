using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Tasks)]
    public partial class TaskExecution : IEntity, IWithKey<TaskExecution, int>
	{
		public virtual Expression<Func<TaskExecution, bool>> KeySelector(int id)
		{
			return o => o.TaskExecutionID == id;
		}
        public virtual int ID => TaskExecutionID;
		[Key]
        [Identity]
        [Column] 
		public virtual int TaskExecutionID { get; set; }
        [Column]
        public virtual DateTime StartDate { get; set; }
        [Column]
        public virtual DateTime? FinishDate { get; set; }
        [Column]
        public virtual DateTime LastModifiedDate { get; set; }
        [Column]
        public virtual bool IsSuccessfull { get; set; }
        [Column]
        public virtual string MachineName { get; set; }
        [Column]
        public virtual string ResultXml { get; set; }
        [Column]
        public virtual string ExecutionLog { get; set; }
        [Column]
        public virtual int TaskID { get; set; }
        [Column]
        public virtual object LastModifiedUserID { get; set; }

		public virtual string TaskName { get; set; }
		public virtual string UserName{ get; set; }
	}
}
