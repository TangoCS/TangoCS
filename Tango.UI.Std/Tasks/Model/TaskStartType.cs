using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Tasks)]
	[Table("tm_taskstarttype")]
	public partial class TaskStartType : IEntity, IWithKey<TaskStartType, int>, IWithTitle
	{
		public virtual Expression<Func<TaskStartType, bool>> KeySelector(int id)
		{
			return o => o.TaskStartTypeID == id;
		}
		public virtual int ID => TaskStartTypeID;
        [Key]
        [Identity]
        [Column]
        public virtual int TaskStartTypeID { get; set; }
        [Column]
        public virtual string Title { get; set; }
	}
}
