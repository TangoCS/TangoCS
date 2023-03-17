using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tango.UI;

namespace Tango.RealTime
{
	public interface IClientProxy
	{
		Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default);
	}

	public interface IHubClients<T>
	{
		T All { get; }

		T AllExcept(IReadOnlyList<string> excludedConnectionIds);
		T Client(string connectionId);
		T Clients(IReadOnlyList<string> connectionIds);
		T Group(string groupName);
		T GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds);
		T Groups(IReadOnlyList<string> groupNames);
		T User(string userId);
		T Users(IReadOnlyList<string> userIds);
	}

	public interface IHubClients : IHubClients<IClientProxy>
	{
	}

	public interface IGroupManager
	{
		Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default);
		Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default);
	}

	public interface IHubContext
	{
		IHubClients Clients { get; }
		IGroupManager Groups { get; }
	}

	public interface IBackgroundWorkerHubContext : IHubContext
	{
	}

	public interface ITangoHubContext
	{
		Task SetElementValue(string service, string action, string key, string id, string value);
		Task SendApiResponse(string service, string action, string key, ActionContext actionContext, Action<ApiResponse> response);
	}
}
