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
}
