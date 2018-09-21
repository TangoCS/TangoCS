using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tango.Logger;

namespace Tango.AccessControl
{
	public abstract class AbstractAccessControl<TKey> : IRoleBasedAccessControl<TKey>
		where TKey : IEquatable<TKey>
	{
		protected IPredicateChecker _predicateChecker;
		protected AccessControlOptions _options;
		protected IRequestLogger _logger;

		public AbstractAccessControl(IPredicateChecker predicateChecker, IRequestLoggerProvider loggerProvider, AccessControlOptions options)
		{
			_predicateChecker = predicateChecker;
			_options = options;
			_logger = loggerProvider.GetLogger("accesscontrol");
		}

		public abstract bool Check(string securableObjectKey, bool? defaultAccess = null);
		public abstract bool CheckForRole(TKey roleID, string securableObjectKey);

		public BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool? defaultAccess = null)
		{
			if (!_options.Enabled()) return BoolResult.True;
			return _predicateChecker.Check(securableObjectKey, predicateContext);
		}

		public CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool? defaultAccess = null)
		{
			if (!_options.Enabled()) return new CheckWithPredicateResult(true, CheckWithPredicateResultCode.AccessGranted);

			BoolResult res1 = CheckPredicate(securableObjectKey, predicateContext, defaultAccess);
			if (!res1.Value) return new CheckWithPredicateResult(res1.Value, CheckWithPredicateResultCode.PredicateAccessDenied, res1.Message);

			bool res2 = Check(securableObjectKey, defaultAccess);
			return new CheckWithPredicateResult(res2, res2 ? CheckWithPredicateResultCode.AccessGranted : CheckWithPredicateResultCode.UserAccessDenied);
		}

		public abstract bool HasRole(params string[] roleName);
		public abstract IEnumerable<TKey> Roles { get; }

		protected ConcurrentBag<string> AllowItems { get; } = new ConcurrentBag<string>();
		protected ConcurrentBag<string> DisallowItems { get; } = new ConcurrentBag<string>();

		protected bool CheckDefaultAccess(string key, bool? defaultAccess)
		{
			if (defaultAccess.Value || HasRole(_options.AdminRoleName))
			{
				if (!AllowItems.Contains(key))
					AllowItems.Add(key);

				_logger.Write(key + ": true (default/admin access)");
			}
			else
			{
				if (!DisallowItems.Contains(key))
					DisallowItems.Add(key);

				_logger.Write(key + ": false (default/admin access denied)");
			}
			return defaultAccess.Value || HasRole(_options.AdminRoleName);
		}

		protected bool CheckExplicitAccess(string key, bool accessResult)
		{
			if (accessResult)
			{
				if (!AllowItems.Contains(key))
					AllowItems.Add(key);

				_logger.Write(key + ": true (explicit access)");
				return true;
			}
			else
			{
				if (!DisallowItems.Contains(key))
					DisallowItems.Add(key);

				_logger.Write(key + ": false (explicit access denied)");
				return false;
			}
		}
	}

	public interface IRoleBasedAccessControlStoreBase
	{
		bool CurrentUserHasRoles(string[] roleNames);
	}
}
