using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.AccessControl
{
	public interface IAccessControl
	{
		bool Check(string securableObjectKey, bool defaultAccess = false);
		BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false);
		CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false);
		StringBuilder Log { get; }
	}

	public interface IAccessControlForRole<TIdentityKey>
	{
		SubjectWithRoles<TIdentityKey> CurrentSubject { get; }
		bool CheckForRole(TIdentityKey roleID, string securableObjectKey);
	}
}
