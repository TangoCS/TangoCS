using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Identity
{
	public class Role<TKey>
	{
		public TKey RoleID { get; set; }
		public string Title { get; set; }
		public string SysName { get; set; }
	}

	public class RoleAsso<TKey>
	{
		public TKey ParentRoleID { get; set; }
		public TKey RoleID { get; set; }
	}
}
