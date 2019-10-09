using Microsoft.AspNetCore.SignalR;

namespace Tango.AspNetCore.SignalR
{
    public class BackgroundWorkerHub : Hub
    {
		public string GetConnectionId()
		{
			return Context.ConnectionId;
		}
	}
}
