using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Tasks
{
    [BaseNamingConventions(Category = BaseNamingEntityCategory.Tasks)]
	[Table("tm_taskparameter")]
	public partial class TaskParameter : IEntity, IWithKey<TaskParameter, int>, IWithSeqNo, IWithTitle
	{
		public virtual Expression<Func<TaskParameter, bool>> KeySelector(int id)
		{
			return o => o.TaskParameterID == id;
		}
		public virtual int ID => TaskParameterID;
		[Key]
        [Identity]
        [Column] 
		public virtual int TaskParameterID { get; set; }
        [Column]
        public virtual string Title { get; set; }
        [Column]
        public virtual string SysName { get; set; }
        [Column]
        public virtual string Value { get; set; }
        [Column]
        public virtual int SeqNo { get; set; }
        [Column]
        public virtual int ParentID { get; set; }

		public virtual string ParentTitle { get; set; }
		public virtual string ParentClass { get; set; }
		public virtual string ParentMethod { get; set; }
	}
}
