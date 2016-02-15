using System;
using System.Security.Principal;

namespace Nephrite.Identity
{
	public interface IIdentityManager<TUser>
		where TUser : class
	{
		IIdentityOptions Options { get; }
		IPasswordHasher PasswordHasher { get; }

		TUser CurrentUser { get; }
		TUser SystemUser { get; }
		IIdentity CurrentIdentity { get; }	

		void RunAs(TUser user, Action action);
	}
}
