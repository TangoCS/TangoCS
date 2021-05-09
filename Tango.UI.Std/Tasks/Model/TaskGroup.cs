using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    //[BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
    public partial class TaskGroup : IEntity, IWithKey<TaskGroup, int>, IWithTitle
	{
		public virtual Expression<Func<TaskGroup, bool>> KeySelector(int id)
		{
			return o => o.TaskGroupID == id;
		}
		public virtual int ID { get { return TaskGroupID; } }
        [Column]
        public virtual int TaskGroupID { get; set; }
        [Column]
        public virtual string Title { get; set; }
	}
}
