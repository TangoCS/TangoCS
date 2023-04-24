using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Tasks
{
	public interface ITaskProgress
	{
		System.Threading.Tasks.Task SetProgress(int taskID, decimal percent, string description);
	}
}
