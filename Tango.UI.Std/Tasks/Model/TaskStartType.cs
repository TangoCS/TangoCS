using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
	[Table("tm_taskstarttype")]
	public partial class TaskStartType : IEntity, IWithKey<TaskStartType, int>, IWithTitle
	{
		public virtual Expression<Func<TaskStartType, bool>> KeySelector(int id)
		{
			return o => o.TaskTypeID == id;
		}
		public virtual int ID => TaskTypeID;
        [Key]
        [Identity]
        [Column]
        public virtual int TaskTypeID { get; set; }
        [Column]
        public virtual string Title { get; set; }
	}
}
