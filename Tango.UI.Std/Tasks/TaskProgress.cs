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
			Tango.Tasks.BaseTaskController.Progress[taskID] = (percent, description);
			if (percent == 0)
			{
				await tangoHubContext.SendApiResponse("task", "view", taskID.ToString(), null, response => response.HardRedirectTo(null));
			}
			else
				await tangoHubContext.SetElementValue("task", "view", taskID.ToString(), "statusinfo", $"<i class='icon icon-ic_info'><svg class='svgicon-ic_info'><use xlink:href='/data/icons/svg#icon-ic_info'></use></svg></i> В работе: завершено {percent:0.#}%, {description}");
			if (percent == 100)
			{
				await tangoHubContext.SetElementValue("task", "view", taskID.ToString(), "statusinfo", $"<i class='icon icon-ic_info'><svg class='svgicon-ic_info'><use xlink:href='/data/icons/svg#icon-ic_info'></use></svg></i> Завершено 100%, {description}");
				await tangoHubContext.SendApiResponse("task", "view", taskID.ToString(), null, response => response.HardRedirectTo(null));
			}
		}
	}
}
