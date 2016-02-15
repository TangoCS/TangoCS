using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nephrite.Identity;
using Nephrite.Identity.Std;

namespace Nephrite.AccessControl.Std
{
	public abstract class AbstractAccessControl<TKey> : IRoleBasedAccessControl<TKey>
		where TKey : IEquatable<TKey>
	{
		public StringBuilder Log { get; set; }
		
		protected static object _lock = new object();
		
		protected IIdentity _identity;
		protected TKey _userId;
		protected IPredicateChecker _predicateChecker;
		protected AccessControlOptions _options;

		public AbstractAccessControl(IIdentityManager<IdentityUser<TKey>> identityManager, IPredicateChecker predicateChecker, AccessControlOptions options)
		{
			_identity = identityManager.CurrentIdentity;
			_userId = identityManager.CurrentUser.Id;
			_predicateChecker = predicateChecker;
			_options = options;
			Log = new StringBuilder();
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

		protected HashSet<string> AllowItems { get; } = new HashSet<string>();
		protected HashSet<string> DisallowItems { get; } = new HashSet<string>();
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
			AccessControlOptions options) : base(identityManager, predicateLoader, options)
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
						lock (_lock) { if (!AllowItems.Contains(key)) AllowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: true (default/admin access)");
				}
				else
				{
					if (!DisallowItems.Contains(key))
					{
						lock (_lock) { if (!DisallowItems.Contains(key)) DisallowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: false (default/admin access denied)");
				}
				return defaultAccess || HasRole(_options.AdminRoleName);
			}

			if (Roles.Select(o => o.Id).Intersect(_access).Count() > 0)
			{
				if (!AllowItems.Contains(key))
				{
					lock (_lock) { if (!AllowItems.Contains(key)) AllowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: true (explicit access)"); 
				return true;
			}
			else
			{
				if (!DisallowItems.Contains(key))
				{
					lock (_lock) { if (!DisallowItems.Contains(key)) DisallowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: false (explicit access denied)");
				return false;
			}
		}
	}

	public class CacheableAccessControl<TKey> : AbstractAccessControl<TKey>
		where TKey : IEquatable<TKey>
	{
		protected ICacheableRoleBasedAccessControlStore<TKey> _dataContext;
		protected override IRoleBasedAccessControlStoreBase<TKey> _baseDataContext => _dataContext;
		string _cacheName;

		public CacheableAccessControl(
			ICacheableRoleBasedAccessControlStore<TKey> dataContext,
			IIdentityManager<IdentityUser<TKey>> identityManager,
			IPredicateChecker predicateChecker,
			AccessControlOptions options) : base(identityManager, predicateChecker, options)
		{
			_dataContext = dataContext;
			_cacheName = GetType().Name;
		}

		public override bool CheckForRole(TKey roleID, string securableObjectKey)
		{
			if (!_options.Enabled()) return true;
			var anc = _dataContext.RoleAncestors(roleID);
			string key = securableObjectKey.ToUpper();

			HashSet<string> _access = null;

			if (!AccessControlCache.AccessCache.ContainsKey(_cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(_dataContext.GetRolesAccess());
					if (!AccessControlCache.AccessCache.ContainsKey(_cacheName)) AccessControlCache.AccessCache.Add(_cacheName, _access);
				}
			}
			else
			{
				_access = AccessControlCache.AccessCache[_cacheName];
			}

			HashSet<string> _checking = new HashSet<string>(anc.Select(o => key + "-" + o.ToString()));
			if (_access.Overlaps(_checking))
			{
				Log.AppendLine("ROLE: " + roleID.ToString() + ", " + key + " v3: true");
				return true;
			}
			else
			{
				Log.AppendLine("ROLE: " + roleID.ToString() + ", " + key + " v3: false");
				return false;
			}
		}

		public override bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			if (!_options.Enabled()) return true;
			string key = securableObjectKey.ToUpper();

			if (AllowItems.Contains(key)) return true;
			if (DisallowItems.Contains(key)) return false;

			HashSet<string> _access = null;
			HashSet<string> _items = null;

			if (!AccessControlCache.AccessCache.ContainsKey(_cacheName) || !AccessControlCache.ItemsCache.ContainsKey(_cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(_dataContext.GetRolesAccess());
					_items = new HashSet<string>(_dataContext.GetKeys());

					if (!AccessControlCache.AccessCache.ContainsKey(_cacheName)) AccessControlCache.AccessCache.Add(_cacheName, _access);
					if (!AccessControlCache.ItemsCache.ContainsKey(_cacheName)) AccessControlCache.ItemsCache.Add(_cacheName, _items);
				}
			}
			else
			{
				_access = AccessControlCache.AccessCache[_cacheName];
				_items = AccessControlCache.ItemsCache[_cacheName];
			}

			if (!_items.Contains(key))
			{
				if (defaultAccess || HasRole(_options.AdminRoleName))
				{
					if (!AllowItems.Contains(key))
					{
						lock (_lock) { if (!AllowItems.Contains(key)) AllowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: true (default/admin access)");
				}
				else
				{
					if (!DisallowItems.Contains(key))
					{
						lock (_lock) { if (!DisallowItems.Contains(key)) DisallowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: false (default/admin access denied)");
				}
				return defaultAccess || HasRole(_options.AdminRoleName);
			}

			HashSet<string> _checking = new HashSet<string>(Roles.Select(o => key + "-" + o.Id.ToString()));
			if (_access.Overlaps(_checking))
			{
				if (!AllowItems.Contains(key))
				{
					lock (_lock) { if (!AllowItems.Contains(key)) AllowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: true (explicit access)");
				return true;
			}
			else
			{
				if (!DisallowItems.Contains(key))
				{
					lock (_lock) { if (!DisallowItems.Contains(key)) DisallowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: false (explicit access denied)");
				return false;
			}
		}
	}

	public class AccessControlCache
	{
		public static Dictionary<string, HashSet<string>> AccessCache = new Dictionary<string, HashSet<string>>();
		public static Dictionary<string, HashSet<string>> ItemsCache = new Dictionary<string, HashSet<string>>();

		public static void ResetCache()
		{
			AccessCache.Clear();
			ItemsCache.Clear();
		}
	}

	public interface IRoleBasedAccessControl<TKey> : IRoleBasedAccessControl<IdentityRole<TKey>, TKey>
		where TKey : IEquatable<TKey>
	{
	}
}
