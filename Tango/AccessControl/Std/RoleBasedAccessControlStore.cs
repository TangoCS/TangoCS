using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using Dapper;
using Tango.Identity.Std;

namespace Tango.AccessControl.Std
{
	public abstract class RoleBasedAccessControlStoreBase<T> : IRoleBasedAccessControlStoreBase
	{
		protected IDbConnection _dc;
		protected IIdentity _identity;
		protected IIdentityManager _identityManager;

		public RoleBasedAccessControlStoreBase(IDbConnection dc, IIdentity identity, IIdentityManager identityManager)
		{
			_dc = dc;
			_identity = identity;
			_identityManager = identityManager;
		}

		static List<IdentityRole<T>> _allRoles = null;
		protected List<IdentityRole<T>> GetAllRoles()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = _dc.Query<IdentityRole<T>>("select RoleID as Id, Title, lower(SysName) as \"Name\" from SPM_Role").ToList();
			return _allRoles;
		}

		//public IdentityRole<T> RoleFromID(T id)
		//{
		//	return GetAllRoles().FirstOrDefault(o => o.Id.Equals(id));
		//}

		protected virtual IEnumerable<IdentityRole<T>> CurrentUserRoles()
		{
			List<T> r = null;
			var id = _identityManager.CurrentUser.Id;
			if (_identity is WindowsIdentity wi && !wi.IsAnonymous)
			{
				var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + groupNames.Join(",") + ")", new { p1 = id }).ToList();
			}
			else
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1", new { p1 = id }).ToList();

			return GetAllRoles().Where(o => r.Contains(o.Id));
		}

		IEnumerable<IdentityRole<T>> _roles = null;
		IEnumerable<IdentityRole<T>> IdentityRoles
		{
			get
			{
				if (_roles == null) _roles = CurrentUserRoles();
				return _roles;
			}
		}

		public IEnumerable<T> Roles => IdentityRoles.Select(o => o.Id);

		public bool CurrentUserHasRoles(string[] roleNames)
		{
			return IdentityRoles.Select(o => o.Name.ToLower()).Intersect(roleNames.Select(o => o.ToLower())).Count() > 0;
		}
	}



	public class CacheableRoleBasedAccessControlStore<T> : RoleBasedAccessControlStoreBase<T>, ICacheableRoleBasedAccessControlStore<T>
	{
		public CacheableRoleBasedAccessControlStore(IDbConnection dc, IIdentity identity, IIdentityManager identityManager) : 
			base(dc, identity, identityManager)
		{

		}

		static List<RoleAsso<T>> _allRoleAsso;
		List<RoleAsso<T>> AllRoleAsso()
		{
			if (_allRoleAsso != null) return _allRoleAsso;
			_allRoleAsso = _dc.Query<RoleAsso<T>>(@"select ParentRoleID, RoleID from V_AccessControl_RoleAsso").ToList();
			return _allRoleAsso;
		}

		public IEnumerable<string> GetRolesAccess()
		{
			return _dc.Query<string>(@"select ActionRoleAccess from V_AccessControl_ActionRoleAccess order by ActionRoleAccess");
		}

		public IEnumerable<string> GetKeys()
		{
			return _dc.Query<string>(@"select upper(a.SystemName) from SPM_Action a order by a.SystemName");
		}

		public IEnumerable<T> RoleAncestors(T id)
		{
			return AllRoleAsso().Where(o => o.RoleID.Equals(id)).Select(o => o.ParentRoleID).ToList();
		}
	}

	public class RoleAsso<TKey>
	{
		public TKey ParentRoleID { get; set; }
		public TKey RoleID { get; set; }
	}
}
