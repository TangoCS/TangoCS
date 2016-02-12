using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class AbstractAccessControl<TIdentityKey>
		where TIdentityKey : IEquatable<TIdentityKey>
	{
		
	}

	public class DefaultAccessControl<TIdentityKey> : IAccessControl, IAccessControlForRole<TIdentityKey>
		where TIdentityKey : IEquatable<TIdentityKey>
	{
		public StringBuilder Log { get; set; }

		public AccessControlOptions Options { get; private set; }
		AccessControlDataContext<TIdentityKey> _dataContext;
		IIdentityManager<TIdentityKey> _identityManager;
		IDbConnection _conn;
		public IPredicateLoader PredicateLoader { get; private set; }

		public DefaultAccessControl(
			IDbConnection conn,
			IIdentityManager<TIdentityKey> identityManager,
			IPredicateLoader predicateLoader,
			AccessControlOptions options = null)
		{
			_dataContext = new AccessControlDataContext<TIdentityKey>(conn);
			_identityManager = identityManager;
			_conn = conn;
			PredicateLoader = predicateLoader;
			Options = options ?? new AccessControlOptions { Enabled = () => true };
			Log = new StringBuilder();
		}

		IdentityUserWithRoles<TIdentityKey> _current = null;
		public IdentityUserWithRoles<TIdentityKey> CurrentSubject
		{
			get
			{
				if (_current != null) return _current;
				var curSubj = _identityManager.CurrentUser;
				if (curSubj == null) return null;

				_current = new IdentityUserWithRoles<TIdentityKey>(_conn, curSubj, _identityManager.CurrentIdentity);
				return _current;
			}
		}

		protected static object _lock = new object();

		public bool CheckForRole(TIdentityKey roleID, string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		public BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			var pc = new PredicateChecker(PredicateLoader, Options);
			return pc.Check(securableObjectKey, predicateContext);
		}

		public CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			BoolResult res1 = CheckPredicate(securableObjectKey, predicateContext, defaultAccess);
			if (!res1.Value) return new CheckWithPredicateResult(res1.Value, CheckWithPredicateResultCode.PredicateAccessDenied, res1.Message);

			bool res2 = Check(securableObjectKey, defaultAccess);
			return new CheckWithPredicateResult(res2, res2 ? CheckWithPredicateResultCode.AccessGranted : CheckWithPredicateResultCode.SubjectAccessDenied);
		}

		public virtual bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			var s = CurrentSubject;

			string key = securableObjectKey.ToUpper();
			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			List<TIdentityKey> _access = _dataContext.GetAccessInfo(securableObjectKey).ToList();

			if (_access.Count == 0)
			{
				if (defaultAccess || s.IsAdministrator)
				{
					if (!s.AllowItems.Contains(key))
					{
						lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: true (default/admin access)");
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: false (default/admin access denied)");
				}
				return defaultAccess || s.IsAdministrator;
			}

			if (s.Roles.Select(o => o.RoleID).Intersect(_access).Count() > 0)
			{
				if (!s.AllowItems.Contains(key))
				{
					lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: true (explicit access)"); 
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
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

	public class CacheableAccessControl<TIdentityKey>
		: IAccessControl, IAccessControlForRole<TIdentityKey>
		where TIdentityKey : IEquatable<TIdentityKey>
	{
		public StringBuilder Log { get; set; }
		static object _lock = new object();

		public CacheableAccessControlOptions Options { get; private set; }
		AccessControlDataContext<TIdentityKey> _dataContext;
		IIdentityManager<TIdentityKey> _identityManager;
		IDbConnection _conn;
		public IPredicateLoader PredicateLoader { get; private set; }

		public CacheableAccessControl(
			IDbConnection conn,
			IIdentityManager<TIdentityKey> identityManager,
			IPredicateLoader predicateLoader,
			CacheableAccessControlOptions options = null)
		{
			_dataContext = new AccessControlDataContext<TIdentityKey>(conn);
			_identityManager = identityManager;
			_conn = conn;
			PredicateLoader = predicateLoader;
			Options = options ?? new CacheableAccessControlOptions { Enabled = () => true };
			if (Options.ClassName == null) Options.ClassName = GetType().Name;
			Log = new StringBuilder();
		}

		IdentityUserWithRoles<TIdentityKey> _current = null;
		public IdentityUserWithRoles<TIdentityKey> CurrentSubject
		{
			get
			{
				if (_current != null) return _current;
				var curSubj = _identityManager.CurrentUser;
				if (curSubj == null) return null;

				_current = new IdentityUserWithRoles<TIdentityKey>(_conn, curSubj, _identityManager.CurrentIdentity);
				return _current;
			}
		}

		public bool CheckForRole(TIdentityKey roleID, string securableObjectKey)
		{
			if (!Options.Enabled()) return true;
			List<TIdentityKey> anc = _dataContext.RoleAncestors(roleID);
			string key = securableObjectKey.ToUpper();

			HashSet<string> _access = null;
			string cacheName = Options.ClassName;

			if (!AccessControlCache.AccessCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(_dataContext.GetRolesAccess());
					if (!AccessControlCache.AccessCache.ContainsKey(cacheName)) AccessControlCache.AccessCache.Add(cacheName, _access);
				}
			}
			else
			{
				_access = AccessControlCache.AccessCache[cacheName];
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

		public BoolResult CheckPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			if (!Options.Enabled()) return new BoolResult(true);
			var pc = new PredicateChecker(PredicateLoader, Options);
			return pc.Check(securableObjectKey, predicateContext);
		}

		public CheckWithPredicateResult CheckWithPredicate(string securableObjectKey, object predicateContext, bool defaultAccess = false)
		{
			if (!Options.Enabled()) return new CheckWithPredicateResult(true, CheckWithPredicateResultCode.AccessGranted);
			BoolResult res1 = CheckPredicate(securableObjectKey, predicateContext, defaultAccess);
			if (!res1.Value) return new CheckWithPredicateResult(res1.Value, CheckWithPredicateResultCode.PredicateAccessDenied, res1.Message);

			bool res2 = Check(securableObjectKey, defaultAccess);
			return new CheckWithPredicateResult(res2, res2 ? CheckWithPredicateResultCode.AccessGranted : CheckWithPredicateResultCode.SubjectAccessDenied);
		}


		public bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			if (!Options.Enabled()) return true;
			string cacheName = Options.ClassName;
			string key = securableObjectKey.ToUpper();

			var s = CurrentSubject;

			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			HashSet<string> _access = null;
			HashSet<string> _items = null;

			if (!AccessControlCache.AccessCache.ContainsKey(cacheName) || !AccessControlCache.ItemsCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(_dataContext.GetRolesAccess());
					_items = new HashSet<string>(_dataContext.GetKeys());

					if (!AccessControlCache.AccessCache.ContainsKey(cacheName)) AccessControlCache.AccessCache.Add(cacheName, _access);
					if (!AccessControlCache.ItemsCache.ContainsKey(cacheName)) AccessControlCache.ItemsCache.Add(cacheName, _items);
				}
			}
			else
			{
				_access = AccessControlCache.AccessCache[cacheName];
				_items = AccessControlCache.ItemsCache[cacheName];
			}

			if (!_items.Contains(key))
			{
				if (defaultAccess || s.IsAdministrator)
				{
					if (!s.AllowItems.Contains(key))
					{
						lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: true (default/admin access)");
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					Log.AppendLine(key + " v3: false (default/admin access denied)");
				}
				return defaultAccess || s.IsAdministrator;
			}

			HashSet<string> _checking = new HashSet<string>(s.Roles.Select(o => key + "-" + o.RoleID.ToString()));
			if (_access.Overlaps(_checking))
			{
				if (!s.AllowItems.Contains(key))
				{
					lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: true (explicit access)");
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
				}
				Log.AppendLine(key + " v3: false (explicit access denied)");
				return false;
			}
		}
	}

	public class AccessControlOptions
	{
		public string AdminRoleName { get; set; }
		public Func<bool> Enabled { get; set; }

		public AccessControlOptions()
		{
			AdminRoleName = "Administrator";
			Enabled = () => true;
		}
	}

	public class CacheableAccessControlOptions : AccessControlOptions
	{
		public string ClassName { get; set; }
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
		SubjectAccessDenied = 1,
		AccessGranted = 0
	}
}
