using System;

namespace Tango.Identity
{
	public interface IIdentityManager<TUser>
		where TUser : class
	{
		IIdentityOptions Options { get; }

		TUser CurrentUser { get; }
		TUser SystemUser { get; }

		void RunAs(TUser user, Action action);
	}

	public interface IIdentityStore<TUser, TKey>
		where TKey : IEquatable<TKey>
	{
		TUser UserFromName(string name);
		TUser UserFromProviderKey(string providerName, string providerKey);
		TUser UserFromID(TKey id);
		TUser UserFromEmail(string email);
	}
}
