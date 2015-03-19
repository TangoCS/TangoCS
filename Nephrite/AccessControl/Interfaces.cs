using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.AccessControl
{
	public interface IAccessControl
	{
		bool Check(string securableObjectKey, bool defaultAccess = false);
	}

	public interface IAccessControlForRole<TIdentityKey>
	{
		bool CheckForRole(TIdentityKey roleID, string securableObjectKey);
	}

	public interface IAccessControlDataContext<TIdentityKey>
	{
		List<Role<TIdentityKey>> GetAllRoles();
		Role<TIdentityKey> RoleFromID(TIdentityKey id);
		List<TIdentityKey> RoleAncestors(TIdentityKey id);
		List<TIdentityKey> SubjectRoles(TIdentityKey id, IEnumerable<string> activeDirectoryGroups = null);
	}

	public interface IDefaultAccessControlDataContext<TIdentityKey> : IAccessControlDataContext<TIdentityKey>
	{
		IEnumerable<TIdentityKey> GetAccessInfo(string securableObjectKey);
	}

	public interface ICacheableAccessControlDataContext<TIdentityKey> : IAccessControlDataContext<TIdentityKey>
	{
		IEnumerable<string> GetRolesAccess();
		IEnumerable<string> GetKeys();
	}
}
