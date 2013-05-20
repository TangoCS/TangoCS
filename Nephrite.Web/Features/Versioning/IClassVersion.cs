using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web.Versioning
{
	public interface IClassVersion
	{
		int ClassVersionID { get; set; }
		DateTime ClassVersionDate { get; set; }
		int ClassVersionNumber { get; set; }
		int CreateUserID { get; set; }
		DateTime VersionStartDate { get; set; }
		DateTime? VersionEndDate { get; set; }
		bool IsCurrent { get; }
	}
}
