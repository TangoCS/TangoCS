using System.Collections.Generic;

namespace Tango.AccessControl.Std
{
	public interface IRoleBasedAccessControlStoreBase<T>
	{
		IdentityRole<T> RoleFromID(T id);
		IEnumerable<IdentityRole<T>> UserRoles(T id);
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