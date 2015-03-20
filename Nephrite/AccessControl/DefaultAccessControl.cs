using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Http;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class DefaultAccessControl<TIdentityKey> : IAccessControl, IAccessControlForRole<TIdentityKey>
	{
		//string _message = "";

		public Func<IHttpContext> HttpContext { get; private set; }
		public AccessControlOptions Options { get; private set; }
		public Func<IDefaultAccessControlDataContext<TIdentityKey>> DataContext { get; private set; }
		public Func<IIdentityManager<TIdentityKey>> IdentityManager { get; private set; }

		public DefaultAccessControl(
			Func<IHttpContext> httpContext,
			Func<IDefaultAccessControlDataContext<TIdentityKey>> dataContext,
			Func<IIdentityManager<TIdentityKey>> identityManager,
			AccessControlOptions options = null)
		{
			HttpContext = httpContext;
			DataContext = dataContext;
			IdentityManager = identityManager;
			Options = options ?? new AccessControlOptions { Enabled = () => true };
		}


		//public bool Enabled
		//{
		//	get
		//	{
		//		string se = Url.Current.GetString("spmenabled");
		//		if (se == "0" || se.ToLower() == "false")
		//		{
		//			return !Subject.Current.IsAdministrator;
		//		}
		//		return true;
		//	}
		//}

		protected static object _lock = new object();

		public bool CheckForRole(TIdentityKey roleID, string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		public virtual bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			var s = SubjectWithRoles<TIdentityKey>.Current;

			var ctx = HttpContext();
			string key = securableObjectKey.ToUpper();
			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			List<TIdentityKey> _access = DataContext().GetAccessInfo(securableObjectKey).ToList();

			if (_access.Count == 0)
			{
				if (defaultAccess || s.IsAdministrator)
				{
					if (!s.AllowItems.Contains(key))
					{
						lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
					}
					ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: true (default/admin access)" + Environment.NewLine;
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: false (default/admin access)" + Environment.NewLine;
				}
				return defaultAccess || s.IsAdministrator;
			}

			if (s.Roles.Select(o => o.RoleID).Intersect(_access).Count() > 0)
			{
				if (!s.AllowItems.Contains(key))
				{
					lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
				}
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: true (explicit access)" + Environment.NewLine;
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
				}
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: false (explicit access denied)" + Environment.NewLine;
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
	{
		static object _lock = new object();

		public Func<IHttpContext> HttpContext { get; private set; }
		public CacheableAccessControlOptions Options { get; private set; }
		public Func<ICacheableAccessControlDataContext<TIdentityKey>> DataContext { get; private set; }
		public Func<IIdentityManager<TIdentityKey>> IdentityManager { get; private set; }

		public CacheableAccessControl(
			Func<IHttpContext> httpContext,
			Func<ICacheableAccessControlDataContext<TIdentityKey>> dataContext,
			Func<IIdentityManager<TIdentityKey>> identityManager,
			CacheableAccessControlOptions options = null)
		{
			HttpContext = httpContext;
			DataContext = dataContext;
			IdentityManager = identityManager;
			Options = options ?? new CacheableAccessControlOptions { Enabled = () => true };
		}

		public bool CheckForRole(TIdentityKey roleID, string securableObjectKey)
		{
			List<TIdentityKey> anc = DataContext().RoleAncestors(roleID);
			string key = securableObjectKey.ToUpper();

			HashSet<string> _access = null;
			string cacheName = Options.ClassName;
			var ctx = HttpContext();

			if (!AccessControlCache.AccessCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(DataContext().GetRolesAccess());
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
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + "ROLE: " + roleID.ToString() + ", " + key + " v3: true" + Environment.NewLine;
				return true;
			}
			else
			{
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + "ROLE: " + roleID.ToString() + ", " + key + " v3: false" + Environment.NewLine;
				return false;
			}
		}

		public bool Check(string securableObjectKey, bool defaultAccess = false)
		{
			string cacheName = Options.ClassName;
			string key = securableObjectKey.ToUpper();

			var s = SubjectWithRoles<TIdentityKey>.Current;

			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			HashSet<string> _access = null;
			HashSet<string> _items = null;
			var ctx = HttpContext();

			if (!AccessControlCache.AccessCache.ContainsKey(cacheName) || !AccessControlCache.ItemsCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(DataContext().GetRolesAccess());
					_items = new HashSet<string>(DataContext().GetKeys());

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
					ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: true (default/admin access)" + Environment.NewLine;
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: false (default/admin access)" + Environment.NewLine;
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
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: true (explicit access)" + Environment.NewLine;
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
				}
				ctx.Items["SpmLog"] = (string)ctx.Items["SpmLog"] + key + " v3: false (explicit access denied)" + Environment.NewLine;
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
}
