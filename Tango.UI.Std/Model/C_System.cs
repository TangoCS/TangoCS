using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.Model
{
	[BaseNamingConventions(Category = BaseNamingEntityCategory.Dictionary)]
	[Table("c_system")]
	public partial class C_System : IEntity, IWithKey<C_System, int>, IWithTitle
	{
		public virtual Expression<Func<C_System, bool>> KeySelector(int id)
		{
			return o => o.SystemID == id;
		}
		public virtual int ID => SystemID;
		[Key]
		[Identity]
		[Column]
		public virtual int SystemID { get; set; }
		[Column]
		public virtual string Title { get; set; }
		[Column]
		public virtual bool IsDeleted { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual object LastModifiedUserID { get; set; }
	}
}
