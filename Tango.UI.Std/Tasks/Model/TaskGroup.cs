using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Tasks)]
	[Table("tm_taskgroup")]
	public partial class TaskGroup : IEntity, IWithKey<TaskGroup, int>, IWithTitle
	{
		public virtual Expression<Func<TaskGroup, bool>> KeySelector(int id)
		{
			return o => o.TaskGroupID == id;
		}
		public virtual int ID => TaskGroupID;
        [Key]
        [Identity]
        [Column]
        public virtual int TaskGroupID { get; set; }
        [Column]
        public virtual string Title { get; set; }
        [Column]
        public virtual bool IsDeleted { get; set; }
		[Column]
		public virtual int SeqNo { get; set; }
	}
}
