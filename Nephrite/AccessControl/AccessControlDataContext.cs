using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Nephrite.AccessControl
{
	public class AccessControlDataContext<T>
	{
		IDbConnection _dc;

		public AccessControlDataContext(IDbConnection dc)
		{
			_dc = dc;
		}

		public IEnumerable<string> GetRolesAccess()
		{
			return _dc.Query<string>(@"select ActionRoleAccess from V_AccessControl_ActionRoleAccess order by ActionRoleAccess");
		}

		public IEnumerable<string> GetKeys()
		{
			return _dc.Query<string>(@"select upper(a.SystemName) from SPM_Action a order by a.SystemName");
		}

		public IEnumerable<T> GetAccessInfo(string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		static List<Role<T>> _allRoles = null;
		public List<Role<T>> GetAllRoles()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = _dc.Query<Role<T>>("select RoleID, Title, lower(SysName) as \"SysName\" from SPM_Role").ToList();
			return _allRoles;
		}

		public Role<T> RoleFromID(T id)
		{
			return GetAllRoles().FirstOrDefault(o => o.RoleID.Equals(id));
		}

		static List<RoleAsso<T>> _allRoleAsso;
		List<RoleAsso<T>> AllRoleAsso()
		{
			if (_allRoleAsso != null) return _allRoleAsso;
			_allRoleAsso = _dc.Query<RoleAsso<T>>(@"select ParentRoleID, RoleID from dbo.V_AccessControl_RoleAsso").ToList();
			return _allRoleAsso;
		}

		public List<T> RoleAncestors(T id)
		{
			return AllRoleAsso().Where(o => o.RoleID.Equals(id)).Select(o => o.ParentRoleID).ToList();
		}


		public List<T> SubjectRoles(T id, IEnumerable<string> activeDirectoryGroups = null)
		{
			List<T> r = null;
			if (activeDirectoryGroups != null)
			{
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + activeDirectoryGroups.Join(",") + ")", new { p1 = id }).ToList();
			}
			else
			{
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1", new { p1 = id }).ToList();
			}
			return r;
		}
	}
}
