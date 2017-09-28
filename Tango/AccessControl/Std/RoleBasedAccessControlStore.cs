using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using Dapper;

namespace Tango.AccessControl.Std
{
	public abstract class RoleBasedAccessControlStoreBase<T> : IRoleBasedAccessControlStoreBase<T>
	{
		protected IDbConnection _dc;
		protected IIdentity _identity;

		public RoleBasedAccessControlStoreBase(IDbConnection dc, IIdentity identity)
		{
			_dc = dc;
			_identity = identity;
		}

		static List<IdentityRole<T>> _allRoles = null;
		List<IdentityRole<T>> GetAllRoles()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = _dc.Query<IdentityRole<T>>("select RoleID as Id, Title, lower(SysName) as \"Name\" from SPM_Role").ToList();
			return _allRoles;
		}

		public IdentityRole<T> RoleFromID(T id)
		{
			return GetAllRoles().FirstOrDefault(o => o.Id.Equals(id));
		}

		public IEnumerable<IdentityRole<T>> UserRoles(T id)
		{
			WindowsIdentity wi = _identity as WindowsIdentity;
			List<T> r = null;
			if (wi != null && !wi.IsAnonymous)
			{
				var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + groupNames.Join(",") + ")", new { p1 = id }).ToList();
			}
			else
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1", new { p1 = id }).ToList();

			return GetAllRoles().Where(o => r.Contains(o.Id));
		}
	}

	//public class RoleBasedAccessControlStore<T> : RoleBasedAccessControlStoreBase<T>, IRoleBasedAccessControlStore<T>
	//{
	//	public RoleBasedAccessControlStore(IDbConnection dc, IIdentity identity) : base(dc, identity)
	//	{
			
	//	}

	//	public IEnumerable<T> GetAccessInfo(string securableObjectKey)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}

	public class CacheableRoleBasedAccessControlStore<T> : RoleBasedAccessControlStoreBase<T>, ICacheableRoleBasedAccessControlStore<T>
	{
		public CacheableRoleBasedAccessControlStore(IDbConnection dc, IIdentity identity) : base(dc, identity)
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
