using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nephrite.Identity;
using Nephrite.Identity.Std;
using Nephrite.Logger;
using Nephrite.Cache;
using System.Collections.Concurrent;

namespace Nephrite.AccessControl.Std
{
	public abstract class AbstractAccessControl<TKey> : IRoleBasedAccessControl<TKey>
		where TKey : IEquatable<TKey>
	{	
		protected IIdentity _identity;
		protected TKey _userId;
		protected IPredicateChecker _predicateChecker;
		protected AccessControlOptions _options;
		protected IRequestLogger _logger;

		public AbstractAccessControl(IIdentityManager<IdentityUser<TKey>> identityManager, IPredicateChecker predicateChecker, 
			IRequestLoggerProvider loggerProvider, AccessControlOptions options)
		{
			_identity = identityManager.CurrentIdentity;
			_userId = identityManager.CurrentUser.Id;
			_predicateChecker = predicateChecker;
			_options = options;
			_logger = loggerProvider.GetLogger("accesscontrol");
		}

		protected abstract IRoleBasedAccessControlStoreBase<TKey> _baseDataContext { get; }
		public abstract bool Check(string securableObjectKey, bool defaultAccess = false);
		public abstract bool CheckForRole(TKey roleID, string securableObjectKey);

		public BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			if (!_options.Enabled()) return BoolResult.True;
			return _predicateChecker.Check(securableObjectKey, predicateContext);
		}

		public CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			if (!_options.Enabled()) return new CheckWithPredicateResult(true, CheckWithPredicateResultCode.AccessGranted);

			BoolResult res1 = CheckPredicate(securableObjectKey, predicateContext, defaultAccess);
			if (!res1.Value) return new CheckWithPredicateResult(res1.Value, CheckWithPredicateResultCode.PredicateAccessDenied, res1.Message);

			bool res2 = Check(securableObjectKey, defaultAccess);
			return new CheckWithPredicateResult(res2, res2 ? CheckWithPredicateResultCode.AccessGranted : CheckWithPredicateResultCode.UserAccessDenied);
		}

		IEnumerable<IdentityRole<TKey>> _roles = null;
		public IEnumerable<IdentityRole<TKey>> Roles
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

	public class DefaultAccessControl<TKey> : AbstractAccessControl<TKey>
		where TKey : IEquatable<TKey>
	{
		
		protected IRoleBasedAccessControlStore<TKey> _dataContext;
		protected override IRoleBasedAccessControlStoreBase<TKey> _baseDataContext => _dataContext;

		public DefaultAccessControl(
			IRoleBasedAccessControlStore<TKey> dataContext,
			IIdentityManager<IdentityUser<TKey>> identityManager,
			IPredicateChecker predicateLoader,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(identityManager, predicateLoader, loggerProvider, options)
		{
			_dataContext = dataContext;
		}

		public override bool CheckForRole(TKey roleID, string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		public override bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			string key = securableObjectKey.ToUpper();
			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			List<TKey> _access = _dataContext.GetAccessInfo(securableObjectKey).ToList();

			if (_access.Count == 0)
			{
				if (defaultAccess || HasRole(_options.AdminRoleName))
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
				return defaultAccess || HasRole(_options.AdminRoleName);
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

	public class CacheableAccessControl<TKey> : AbstractAccessControl<TKey>, ICacheable
		where TKey : IEquatable<TKey>
	{
		protected ICacheableRoleBasedAccessControlStore<TKey> _dataContext;
		protected override IRoleBasedAccessControlStoreBase<TKey> _baseDataContext => _dataContext;
		string _cacheName;
		ICache _cache;

		public CacheableAccessControl(
			ICacheableRoleBasedAccessControlStore<TKey> dataContext,
			IIdentityManager<IdentityUser<TKey>> identityManager,
			IPredicateChecker predicateChecker,
			ICache cache,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(identityManager, predicateChecker, loggerProvider, options)
		{
			_dataContext = dataContext;
			_cacheName = GetType().Name;
			_cache = cache;			
		}

		public override bool CheckForRole(TKey roleID, string securableObjectKey)
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

		public override bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			if (!_options.Enabled()) return true;
			string key = securableObjectKey.ToUpper();

			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			ConcurrentBag<string> _access = null;
			ConcurrentBag<string> _items = null;

			_access = _cache.GetOrAdd(_cacheName + "-access", () => new ConcurrentBag<string>(_dataContext.GetRolesAccess()));
			_items = _cache.GetOrAdd(_cacheName + "-items", () => new ConcurrentBag<string>(_dataContext.GetKeys()));

			if (!_items.Contains(key))
			{
				if (defaultAccess || HasRole(_options.AdminRoleName))
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
				return defaultAccess || HasRole(_options.AdminRoleName);
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
