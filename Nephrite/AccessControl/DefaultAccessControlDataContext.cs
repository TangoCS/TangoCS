using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Nephrite.AccessControl
{
	public class DefaultAccessControlDataContext : ICacheableAccessControlDataContext<int>, IAccessControlDataContext<int>
	{
		IDbConnection _dc;

		public DefaultAccessControlDataContext(IDbConnection dc)
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

		public IEnumerable<int> GetAccessInfo(string securableObjectKey)
		{
			throw new NotImplementedException();
		}

		static List<Role<int>> _allRoles = null;
		public List<Role<int>> GetAllRoles()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = _dc.Query<Role<int>>("select RoleID, Title, lower(SysName) as \"SysName\" from SPM_Role").ToList();
			return _allRoles;
		}

		public Role<int> RoleFromID(int id)
		{
			return GetAllRoles().FirstOrDefault(o => o.RoleID == id);
		}

		static List<RoleAsso<int>> _allRoleAsso;
		List<RoleAsso<int>> AllRoleAsso()
		{
			if (_allRoleAsso != null) return _allRoleAsso;
			_allRoleAsso = _dc.Query<RoleAsso<int>>(@"select ParentRoleID, RoleID from dbo.V_AccessControl_RoleAsso").ToList();
			return _allRoleAsso;
		}

		public List<int> RoleAncestors(int id)
		{
			return AllRoleAsso().Where(o => o.RoleID == id).Select(o => o.ParentRoleID).ToList();
		}


		public List<int> SubjectRoles(int id, IEnumerable<string> activeDirectoryGroups = null)
		{
			List<int> r = null;
			if (activeDirectoryGroups != null)
			{
				r = _dc.Query<int>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + activeDirectoryGroups.Join(",") + ")", new { p1 = id }).ToList();
			}
			else
			{
				r = _dc.Query<int>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1", new { p1 = id }).ToList();
			}
			return r;
		}
	}
}
