using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Tango.RealTime;

namespace Tango.AspNetCore.SignalR
{
	public class TaskService : IHostedService
	{
		readonly TaskCollection _tasks;

		CancellationTokenSource _tokenSource;
		Task _currentTask;

		public TaskService(TaskCollection tasks) => _tasks = tasks;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			while (cancellationToken.IsCancellationRequested == false)
			{
				var taskToRun = await _tasks.DequeueAsync(cancellationToken);

				// We need to save executable task, 
				// so we can gratefully wait for it's completion in Stop method
				_currentTask = taskToRun();
				await _currentTask;
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_tokenSource.Cancel(); // cancel "waiting" for task in blocking collection

			if (_currentTask == null) return;

			// wait when _currentTask is complete
			await Task.WhenAny(_currentTask, Task.Delay(-1, cancellationToken));
		}
	}
}
