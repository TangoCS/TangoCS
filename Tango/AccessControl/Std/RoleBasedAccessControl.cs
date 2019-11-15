using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Logger;
using Tango.Cache;
using System.Collections.Concurrent;

namespace Tango.AccessControl.Std
{
	public class DefaultAccessControl : AbstractAccessControl<int>, IRoleBasedAccessControl<int>
	{	
		protected IRoleBasedAccessControlStore<int> _dataContext;

		public override IEnumerable<int> Roles => _dataContext.Roles;
        public override IEnumerable<int> DenyRoles => _dataContext.DenyRoles;

        public DefaultAccessControl(
			IRoleBasedAccessControlStore<int> dataContext,
			IPredicateChecker predicateLoader,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(predicateLoader, loggerProvider, options)
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
			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			List<int> _access = _dataContext.GetAccessInfo(securableObjectKey).ToList();

			if (_access.Count == 0)
				return CheckDefaultAccess(key, defaultAccess ?? _options.DefaultAccess(this));

			return CheckExplicitAccess(key, Roles.Intersect(_access).Count() > 0);
		}

		public override bool HasRole(params string[] roleNames)
		{
			return _dataContext.CurrentUserHasRoles(roleNames);
		}
	}

	public class CacheableAccessControl : AbstractAccessControl<int>, IRoleBasedAccessControl<int>, ICacheable
	{
		protected ICacheableRoleBasedAccessControlStore<int> _dataContext;

		readonly string _cacheName;
		ICache _cache;

		public override IEnumerable<int> Roles => _dataContext.Roles;
        public override IEnumerable<int> DenyRoles => _dataContext.DenyRoles;

        public CacheableAccessControl(
			ICacheableRoleBasedAccessControlStore<int> dataContext,
			IPredicateChecker predicateChecker,
			ICache cache,
			IRequestLoggerProvider loggerProvider,
			AccessControlOptions options) : base(predicateChecker, loggerProvider, options)
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

			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			ConcurrentBag<string> _access = null;
			ConcurrentBag<string> _items = null;

			_access = _cache.GetOrAdd(_cacheName + "-access", () => new ConcurrentBag<string>(_dataContext.GetRolesAccess()));
			_items = _cache.GetOrAdd(_cacheName + "-items", () => new ConcurrentBag<string>(_dataContext.GetKeys()));

			if (!_items.Contains(key))
				return CheckDefaultAccess(key, defaultAccess ?? _options.DefaultAccess(this));

			HashSet<string> _checking = new HashSet<string>(Roles.Select(o => key + "-" + o.ToString()));
			return CheckExplicitAccess(key, _checking.Any(o => _access.Contains(o)));
		}

		public override bool HasRole(params string[] roleNames)
		{
			return _dataContext.CurrentUserHasRoles(roleNames);
		}

		public void ResetCache()
		{
			_cache.Reset(_cacheName + "-access");
			_cache.Reset(_cacheName + "-items");
		}
	}

	//public interface IRoleBasedAccessControl<TKey> : IRoleBasedAccessControl<IdentityRole<TKey>, TKey>
	//	where TKey : IEquatable<TKey>
	//{
	//}
}
