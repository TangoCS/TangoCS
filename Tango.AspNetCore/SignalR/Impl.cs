using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tango.Html;
using Tango.UI;

namespace Tango.AspNetCore.SignalR
{
	public abstract class HubContext : RealTime.IHubContext
	{
		protected HubClients hubCallerClients;
		protected GroupManager groupManager;

		public RealTime.IHubClients Clients { get { return hubCallerClients;  } }
		public RealTime.IGroupManager Groups { get { return groupManager; } }
	}

	public class BackgroundWorkerHubContext : HubContext, RealTime.IBackgroundWorkerHubContext
	{
		public BackgroundWorkerHubContext(IHubContext<BackgroundWorkerHub> context)
		{
			hubCallerClients = new HubClients(context.Clients);
			groupManager = new GroupManager(context.Groups);
		}
	}

	public class HubClients : RealTime.IHubClients
	{
		readonly IHubClients hubCallerClients;

		public HubClients(IHubClients hubCallerClients)
		{
			this.hubCallerClients = hubCallerClients;
		}

		public RealTime.IClientProxy All => new ClientProxy(hubCallerClients.All);

		public RealTime.IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds)
			=> new ClientProxy(hubCallerClients.AllExcept(excludedConnectionIds));

		public RealTime.IClientProxy Client(string connectionId)
			=> new ClientProxy(hubCallerClients.Client(connectionId));

		public RealTime.IClientProxy Clients(IReadOnlyList<string> connectionIds)
			=> new ClientProxy(hubCallerClients.Clients(connectionIds));

		public RealTime.IClientProxy Group(string groupName)
			=> new ClientProxy(hubCallerClients.Group(groupName));

		public RealTime.IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
			=> new ClientProxy(hubCallerClients.GroupExcept(groupName, excludedConnectionIds));

		public RealTime.IClientProxy Groups(IReadOnlyList<string> groupNames)
			=> new ClientProxy(hubCallerClients.Groups(groupNames));

		public RealTime.IClientProxy User(string userId)
			=> new ClientProxy(hubCallerClients.User(userId));

		public RealTime.IClientProxy Users(IReadOnlyList<string> userIds)
			=> new ClientProxy(hubCallerClients.Users(userIds));
	}

	public class ClientProxy : RealTime.IClientProxy
	{
		readonly IClientProxy clientProxy;

		public ClientProxy(IClientProxy clientProxy)
		{
			this.clientProxy = clientProxy;
		}

		public Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default)
			=> clientProxy.SendCoreAsync(method, args, cancellationToken);
	}

	public class GroupManager : RealTime.IGroupManager
	{
		readonly IGroupManager groupManager;

		public GroupManager(IGroupManager groupManager)
		{
			this.groupManager = groupManager;
		}

		public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
			=> groupManager.AddToGroupAsync(connectionId, groupName, cancellationToken);

		public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
			=> groupManager.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
	}

	public class TangoHub : Hub
	{
		private static readonly ConcurrentDictionary<string, string> groupsMap = new ConcurrentDictionary<string, string>();

		public async Task SetServiceAction(string service, string action, string key = null)
		{
			var groupName = (service + "." + action + "." + key).ToLower();

			if (groupsMap.TryGetValue(Context.ConnectionId, out string currentGroupName))
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentGroupName);

			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			groupsMap[Context.ConnectionId] = groupName;
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			groupsMap.Remove(Context.ConnectionId, out string currentGroupName);
			return base.OnDisconnectedAsync(exception);
		}
	}

	public class TangoHubContext : RealTime.ITangoHubContext
	{
		IHubContext<TangoHub> hubContext;
		public TangoHubContext(IHubContext<TangoHub> hubContext)
		{
			this.hubContext = hubContext;
		}

		public async Task SetElementValue(string service, string action, string key, string id, string value)
		{
			await hubContext.Clients.Group((service + "." + action + "." + key).ToLower()).SendAsync("SetElementValue", id, value);
		}

		public async Task SendApiResponse(string service, string action, string key, ActionContext actionContext, Action<ApiResponse> response)
		{
			ApiResponse data = new ApiResponse();
			response(data);
			var content = data.Serialize(actionContext);
			await hubContext.Clients.Group((service + "." + action + "." + key).ToLower()).SendAsync("ProcessApiResponse", content);
		}
	}
}
