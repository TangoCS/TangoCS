using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.AccessControl
{
	public interface IAccessControl<TIdentityKey>
	{
		bool Check(string securableObjectKey, bool defaultAccess = false);
		bool CheckForRole(TIdentityKey roleID, string securableObjectKey);
	}
}
