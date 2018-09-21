using System;
using Tango.Logger;

namespace Tango.RealTime
{
	public class ProgressLogger : IProgressLogger
	{
		readonly IBackgroundWorkerHubContext _hubContext;
		readonly string _taskid;
		readonly Func<IHubContext, IClientProxy> _clientSelector;

		int _itemsCount;

		public ProgressLogger(IBackgroundWorkerHubContext hubContext, string taskid, Func<IHubContext, IClientProxy> clientSelector)
		{
			_hubContext = hubContext;
			_taskid = taskid.ToLower();
			_clientSelector = clientSelector;
		}

		public void SetItemsCount(int itemsCount)
		{
			_itemsCount = itemsCount;
			_clientSelector(_hubContext).SendCoreAsync("init", new object[] { _taskid, itemsCount });
		}

		public void SetProgress(int itemsCompleted)
		{
			_clientSelector(_hubContext).SendCoreAsync("progress", new object[] { _taskid, itemsCompleted });
			if (itemsCompleted == _itemsCount)
				_clientSelector(_hubContext).SendCoreAsync("complete", new object[] { _taskid, _itemsCount });
		}

		public void WriteMessage(string message, int? itemsCompleted = null)
		{			
			if (itemsCompleted != null)
				SetProgress(itemsCompleted.Value);
			_clientSelector(_hubContext).SendCoreAsync("message", new object[] { _taskid, message });
		}
	}
}
