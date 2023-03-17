using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.UI.Std.Tasks
{
	public interface ITaskProgress
	{
		Task SetProgress(int taskID, decimal percent, string description);
	}
}
