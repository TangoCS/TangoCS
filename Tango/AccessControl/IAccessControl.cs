using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tango.AccessControl
{
	public interface IAccessControl
	{
		bool Check(string securableObjectKey, bool defaultAccess = false);
		BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false);
		CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false);
	}

	public class CheckWithPredicateResult : BoolResult
	{
		public CheckWithPredicateResultCode Code { get; private set; }
		public CheckWithPredicateResult(bool value, CheckWithPredicateResultCode code, string message = "")
			: base(value, message)
		{
			Code = code;
		}
	}

	public enum CheckWithPredicateResultCode
	{
		PredicateAccessDenied = 2,
		UserAccessDenied = 1,
		AccessGranted = 0
	}
}
