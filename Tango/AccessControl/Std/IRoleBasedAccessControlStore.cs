using System.Collections.Generic;

namespace Tango.AccessControl.Std
{
	public interface IRoleBasedAccessControlStore<T> : IRoleBasedAccessControlStoreBase
	{
		IEnumerable<T> Roles { get; }
        IEnumerable<T> DenyRoles { get; }
		IEnumerable<T> GetAccessInfo(string securableObjectKey);
	}

	public interface ICacheableRoleBasedAccessControlStore<T> : IRoleBasedAccessControlStoreBase
	{
		IEnumerable<T> Roles { get; }
        IEnumerable<T> DenyRoles { get; }
        IEnumerable<string> GetKeys();
		IEnumerable<string> GetRolesAccess();
        IEnumerable<string> GetRolesDeny();
        IEnumerable<T> RoleAncestors(T id);
	}
}