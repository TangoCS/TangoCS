using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Tango.Identity.Std;
using Tango.Logger;
using Tango.Cache;
using System.Collections.Concurrent;

namespace Tango.AccessControl.Std
{
	public abstract class AbstractAccessControl : IRoleBasedAccessControl<int>
	{
		IIdentityManager _identityManager;

		protected IIdentity _identity => _identityManager.CurrentIdentity;
		protected int _userId => _identityManager.CurrentUser.Id;
		protected IPredicateChecker _predicateChecker;
		protected AccessControlOptions _options;
		protected IRequestLogger _logger;

		public AbstractAccessControl(IIdentityManager identityManager, IPredicateChecker predicateChecker, 
			IRequestLoggerProvider loggerProvider, AccessControlOptions options)
		{
			_identityManager = identityManager;
			_predicateChecker = predicateChecker;
			_options = options;
			_logger = loggerProvider.GetLogger("accesscontrol");
		}

		protected abstract IRoleBasedAccessControlStoreBase<int> _baseDataContext { get; }
		public abstract bool Check(string securableObjectKey, bool? defaultAccess = null);
		public abstract bool CheckForRole(int roleID, string securableObjectKey);

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

		IEnumerable<IdentityRole<int>> _roles = null;
		public IEnumerable<IdentityRole<int>> Roles
		{
			get
			{
				if (_roles == null) _roles = _baseDataContext.UserRoles(_identity, _userId);
				return _roles;
			}
		}

		public bool HasRole(params string[] roleName)
		{
			return Roles.Select(o => o.Name.ToLower()).Intersect(roleName.Select(o => o.ToLower())).Count() > 0;
		}

		protected ConcurrentBag<string> AllowItems { get; } = new ConcurrentBag<string>();
		protected ConcurrentBag<string> DisallowItems { get; } = new ConcurrentBag<string>();
	}

	public class DefaultAccessControl : AbstractAccessControl
	{
		
		protected IRoleBasedAccessControlStore<int> _dataContext;
		protected override IRoleBasedAccessControlStoreBase<int> _baseDataContext => _dataContext;

		public DefaultAccessControl(
			IRoleBasedAccessControlStore<int> dataContext,
			IIdentityManager identityManager,
			IPredicateChecker predicateLoader,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(identityManager, predicateLoader, loggerProvider, options)
		{
			_dataContext = dataContext;
		}

		public override bool CheckForRole(int roleID, string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		public override bool Check(string securableObjectKey, bool? defaultAccess = null)
		{
			string key = securableObjectKey.ToUpper();
			if (defaultAccess == null) defaultAccess = _options.DefaultAccess();
			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			List<int> _access = _dataContext.GetAccessInfo(securableObjectKey).ToList();

			if (_access.Count == 0)
			{
				if (defaultAccess.Value || HasRole(_options.AdminRoleName))
				{
					if (!AllowItems.Contains(key))
					{
						if (!AllowItems.Contains(key)) AllowItems.Add(key);
					}
					_logger.Write(key + ": true (default/admin access)");
				}
				else
				{
					if (!DisallowItems.Contains(key))
					{
						if (!DisallowItems.Contains(key)) DisallowItems.Add(key);
					}
					_logger.Write(key + ": false (default/admin access denied)");
				}
				return defaultAccess.Value || HasRole(_options.AdminRoleName);
			}

			if (Roles.Select(o => o.Id).Intersect(_access).Count() > 0)
			{
				if (!AllowItems.Contains(key))
				{
					if (!AllowItems.Contains(key)) AllowItems.Add(key);
				}
				_logger.Write(key + ": true (explicit access)"); 
				return true;
			}
			else
			{
				if (!DisallowItems.Contains(key))
				{
					if (!DisallowItems.Contains(key)) DisallowItems.Add(key);
				}
				_logger.Write(key + ": false (explicit access denied)");
				return false;
			}
		}
	}

	public class CacheableAccessControl : AbstractAccessControl, ICacheable
	{
		protected ICacheableRoleBasedAccessControlStore<int> _dataContext;
		protected override IRoleBasedAccessControlStoreBase<int> _baseDataContext => _dataContext;
		string _cacheName;
		ICache _cache;

		public CacheableAccessControl(
			ICacheableRoleBasedAccessControlStore<int> dataContext,
			IIdentityManager identityManager,
			IPredicateChecker predicateChecker,
			ICache cache,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(identityManager, predicateChecker, loggerProvider, options)
		{
			_dataContext = dataContext;
			_cacheName = GetType().Name;
			_cache = cache;			
		}

		public override bool CheckForRole(int roleID, string securableObjectKey)
		{
			if (!_options.Enabled()) return true;
			var anc = _dataContext.RoleAncestors(roleID);
			string key = securableObjectKey.ToUpper();

			ConcurrentBag<string> _access = null;
			_access = _cache.GetOrAdd(_cacheName + "-access", () => new ConcurrentBag<string>(_dataContext.GetRolesAccess()));

			ConcurrentBag<string> _checking = new ConcurrentBag<string>(anc.Select(o => key + "-" + o.ToString()));
			
			if (_checking.Any(o => _access.Contains(o)))
			{
				_logger.Write("ROLE: " + roleID.ToString() + ", " + key + ": true");
				return true;
			}
			else
			{
				_logger.Write("ROLE: " + roleID.ToString() + ", " + key + ": false");
				return false;
			}
		}

		public override bool Check(string securableObjectKey, bool? defaultAccess = null)
		{
			if (!_options.Enabled()) return true;
			string key = securableObjectKey.ToUpper();
			if (defaultAccess == null) defaultAccess = _options.DefaultAccess();

			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			ConcurrentBag<string> _access = null;
			ConcurrentBag<string> _items = null;

			_access = _cache.GetOrAdd(_cacheName + "-access", () => new ConcurrentBag<string>(_dataContext.GetRolesAccess()));
			_items = _cache.GetOrAdd(_cacheName + "-items", () => new ConcurrentBag<string>(_dataContext.GetKeys()));

			if (!_items.Contains(key))
			{
				if (defaultAccess.Value || HasRole(_options.AdminRoleName))
				{
					if (!AllowItems.Contains(key))
					{
						if (!AllowItems.Contains(key)) AllowItems.Add(key);
					}
					_logger.Write(key + ": true (default/admin access)");
				}
				else
				{
					if (!DisallowItems.Contains(key))
					{
						if (!DisallowItems.Contains(key)) DisallowItems.Add(key);
					}
					_logger.Write(key + ": false (default/admin access denied)");
				}
				return defaultAccess.Value || HasRole(_options.AdminRoleName);
			}

			HashSet<string> _checking = new HashSet<string>(Roles.Select(o => key + "-" + o.Id.ToString()));
			if (_checking.Any(o => _access.Contains(o)))
			{
				if (!AllowItems.Contains(key))
				{
					if (!AllowItems.Contains(key)) AllowItems.Add(key);
				}
				_logger.Write(key + ": true (explicit access)");
				return true;
			}
			else
			{
				if (!DisallowItems.Contains(key))
				{
					if (!DisallowItems.Contains(key)) DisallowItems.Add(key);
				}
				_logger.Write(key + ": false (explicit access denied)");
				return false;
			}
		}

		public void ResetCache()
		{
			_cache.Reset(_cacheName + "-access");
			_cache.Reset(_cacheName + "-items");
		}
	}

	public interface IRoleBasedAccessControl<TKey> : IRoleBasedAccessControl<IdentityRole<TKey>, TKey>
		where TKey : IEquatable<TKey>
	{
	}
}
