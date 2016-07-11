using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using Dapper;

namespace Tango.AccessControl.Std
{
	public class Tessera2AccessControlStoreBase<T> : IRoleBasedAccessControlStoreBase<T>, ICacheableRoleBasedAccessControlStore<T>
	{
		protected IDbConnection _dc;

		public Tessera2AccessControlStoreBase(IDbConnection dc)
		{
			_dc = dc;
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

		public IEnumerable<IdentityRole<T>> UserRoles(IIdentity identity, T id)
		{
			WindowsIdentity wi = identity as WindowsIdentity;
			List<T> r = null;
			if (wi != null && !wi.IsAnonymous)
			{
				var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
				r = _dc.Query<T>("select RoleID from V_SPM_AllSubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + groupNames.Join(",") + ")", new { p1 = id }).ToList();
			}
			else
				r = _dc.Query<T>("select RoleID from V_SPM_AllSubjectRole where SubjectID = @p1", new { p1 = id }).ToList();

			return GetAllRoles().Where(o => r.Contains(o.Id));
		}

		static List<RoleAsso<T>> _allRoleAsso;
		List<RoleAsso<T>> AllRoleAsso()
		{
			if (_allRoleAsso != null) return _allRoleAsso;
			_allRoleAsso = _dc.Query<RoleAsso<T>>(@"select ParentRoleID, RoleID from V_SPM_AllRoleAsso").ToList();
			return _allRoleAsso;
		}

		public IEnumerable<string> GetRolesAccess()
		{
			return _dc.Query<string>(@"select upper(a1.SystemName + '.' + a2.SystemName) + '-' + cast(ra.roleid as varchar) AS ActionRoleAccess 
FROM spm_action a1 
JOIN spm_actionasso asso ON a1.ActionID = asso.ParentActionID
JOIN spm_action a2 ON asso.ActionID = a2.ActionID  
JOIN spm_roleaccess ra ON ra.actionid = a2.actionid
where a2.Type = 2 and a1.Type = 1 
order by ActionRoleAccess");
		}

		public IEnumerable<string> GetKeys()
		{
			return _dc.Query<string>(@"select upper(a1.SystemName + '.' + a2.SystemName) as SystemName
from SPM_Action a1, SPM_Action a2, SPM_ActionAsso asso
where asso.ParentActionID = a1.ActionID and asso.ActionID = a2.ActionID and a2.Type = 2 and a1.Type = 1  
order by a2.SystemName");
		}

		public IEnumerable<T> RoleAncestors(T id)
		{
			return AllRoleAsso().Where(o => o.RoleID.Equals(id)).Select(o => o.ParentRoleID).ToList();
		}
	}
}
