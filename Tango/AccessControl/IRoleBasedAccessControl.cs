using System;
using System.Collections.Generic;

namespace Tango.AccessControl
{
	public interface IRoleBasedAccessControl<TKey> : IAccessControl
		where TKey : IEquatable<TKey>
	{
		bool CheckForRole(TKey roleID, string securableObjectKey);
		IEnumerable<TKey> Roles { get; }
		bool HasRole(params string[] roleName);
	}
}
