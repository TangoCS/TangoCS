using System;
using System.Security.Principal;

namespace Nephrite.Identity
{
	public interface IIdentityManager<TKey>
		where TKey : IEquatable<TKey>
	{
		IIdentityOptions Options { get; }
		IPasswordHasher PasswordHasher { get; }

		IdentityUser<TKey> CurrentUser { get; }
		IdentityUser<TKey> SystemUser { get; }
		IIdentity CurrentIdentity { get; }	

		void RunAs(TKey sid, Action action);
		void RunAs(IdentityUser<TKey> user, Action action);
	}
}
