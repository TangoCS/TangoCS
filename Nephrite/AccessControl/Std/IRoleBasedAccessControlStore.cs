using System.Collections.Generic;
using System.Security.Principal;

namespace Nephrite.AccessControl.Std
{
	public interface IRoleBasedAccessControlStoreBase<T>
	{
		IdentityRole<T> RoleFromID(T id);
		IEnumerable<IdentityRole<T>> UserRoles(IIdentity identity, T id);
	}

	public interface IRoleBasedAccessControlStore<T> : IRoleBasedAccessControlStoreBase<T>
	{
		IEnumerable<T> GetAccessInfo(string securableObjectKey);
	}

	public interface ICacheableRoleBasedAccessControlStore<T> : IRoleBasedAccessControlStoreBase<T>
	{
		IEnumerable<string> GetKeys();
		IEnumerable<string> GetRolesAccess();
		IEnumerable<T> RoleAncestors(T id);
	}
}