using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.UI.Std.Tasks
{
	public class TaskProgress : ITaskProgress
	{
		RealTime.ITangoHubContext tangoHubContext;
		public TaskProgress(RealTime.ITangoHubContext tangoHubContext)
		{
			this.tangoHubContext = tangoHubContext;
		}
		public async Task SetProgress(int taskID, decimal percent, string description)
		{
			string status = percent < 100 ? $"В работе: завершено {percent:0.00}%, {description}" : $"Исполнено: {description}";
			await tangoHubContext.SetElementValue("task", "view", taskID.ToString(), "status", status);
			await tangoHubContext.SetElementValue("task", "viewlist", null, $"status_{taskID}", status);
		}
	}
}
