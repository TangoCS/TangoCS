using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    //[BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
    public partial class TaskStartType : IEntity, IWithKey<TaskStartType, int>, IWithTitle
	{
		public virtual Expression<Func<TaskStartType, bool>> KeySelector(int id)
		{
			return o => o.TaskStartTypeID == id;
		}
		public virtual int ID { get { return TaskStartTypeID; } }
        [Column]
        public virtual int TaskStartTypeID { get; set; }
        [Column]
        public virtual string Title { get; set; }
	}
}
