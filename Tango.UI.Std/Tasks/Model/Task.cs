using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Tasks)]
	[Table("tm_task")]
    public partial class Task : IEntity, IWithKey<Task, int>, IWithTitle, IWithTimeStamp, IScheduledTask
	{
		public virtual Expression<Func<Task, bool>> KeySelector(int id)
		{
			return o => o.TaskID == id;
		}
        public virtual int ID => TaskID;
		[Key]
        [Identity]
        [Column]
		public virtual int TaskID { get; set; }
        [Column]
        public virtual string Title { get; set; }
        [Column] 
        public virtual string SystemName { get; set; }
        [Column]
        public virtual string Class { get; set; }
        [Column]
        public virtual string Method { get; set; }
        [Column]
        public virtual string Interval { get; set; }
        [Column]
        public virtual int ExecutionTimeout { get; set; }
        [Column]
        public virtual bool IsActive { get; set; }
        [Column]
        public virtual DateTime? LastStartDate { get; set; }
        [Column]
        public virtual bool StartFromService { get; set; }
        [Column]
        public virtual int Status { get; set; }
        [Column]
        public virtual int? TaskGroupID { get; set; }
        [Column]
        public virtual int StartTypeID { get; set; }
        [Column]
        public virtual int Priority { get; set; }
        [Column]
        public virtual int SystemID { get; set; }
        [Column]
        public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual string Description { get; set; }
		[Column]
		public virtual bool OneThread { get; set; }

		public virtual DateTime? LastCheckDate { get; set; }

		public virtual string StartTypeTitle { get; set; }
		public virtual string GroupTitle { get; set; }
        public virtual string SystemTitle { get; set; }
    }
}
