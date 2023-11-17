using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.AccessControl
{
	public interface IObjectChangeRequestManager
	{
		bool IsEnabled(Type entity);
		bool IsCurrentUserModerator();
	}
}
