using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.AccessControl
{
	public interface IObjectChangeRequestManager<T>
	{
		bool IsEnabled();
		bool IsCurrentUserModerator();
	}

	public enum ObjectChangeRequestStatus
	{
		New = 1,
		Approved = 2,
		Rejected = 3
	}
}
