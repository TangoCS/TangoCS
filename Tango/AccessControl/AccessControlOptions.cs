using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.AccessControl
{
	public class AccessControlOptions
	{
		public Func<bool> Enabled { get; set; }
		public Func<IAccessControl, bool> DefaultAccess { get; set; }
		public Func<IAccessControl, bool> DeveloperAccess { get; set; }

		public AccessControlOptions()
		{
			Enabled = () => true;
			DefaultAccess = ac => ac.HasRole("Administrator");
			DeveloperAccess = ac => ac.HasRole("Developer");
		}
	}
}
