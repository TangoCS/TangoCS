using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.AccessControl
{
	public class AccessControlOptions
	{
		public string AdminRoleName { get; set; }
		public Func<bool> Enabled { get; set; }

		public AccessControlOptions()
		{
			AdminRoleName = "Administrator";
			Enabled = () => true;
		}
	}
}
