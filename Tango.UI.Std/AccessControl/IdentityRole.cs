using System.Collections.Generic;
using System.Security.Claims;

namespace Tango.AccessControl.Std
{
	public class IdentityRole<TKey>
	{
		public TKey Id { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public virtual ICollection<Claim> Claims { get; } = new List<Claim>();
	}
}
