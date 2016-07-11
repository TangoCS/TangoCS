using System;
using System.Collections.Generic;

namespace Tango.AccessControl
{
	public interface IRoleBasedAccessControl<TRole, TKey> : IAccessControl
		where TKey : IEquatable<TKey>
		where TRole : class
	{
		bool CheckForRole(TKey roleID, string securableObjectKey);
		IEnumerable<TRole> Roles { get; }
		bool HasRole(params string[] roleName);
	}
}
