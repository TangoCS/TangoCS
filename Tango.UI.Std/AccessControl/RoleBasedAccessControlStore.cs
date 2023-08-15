using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Dapper;
using Tango.Identity;
using Tango.Identity.Std;

namespace Tango.AccessControl.Std
{
	public abstract class RoleBasedAccessControlStoreBase<T> : IRoleBasedAccessControlStoreBase
	{
		protected IDbConnection _dc;
		protected IIdentity _identity;
		protected IUserIdAccessor<long> _userIdAccessor;

		public RoleBasedAccessControlStoreBase(IDbConnection dc, IIdentity identity, IUserIdAccessor<long> userIdAccessor)
		{
			_dc = dc;
			_identity = identity;
			_userIdAccessor = userIdAccessor;
		}

		static List<IdentityRole<T>> _allRoles = null;
		protected List<IdentityRole<T>> GetAllRoles()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = _dc.Query<IdentityRole<T>>("select RoleID as Id, Title, lower(SysName) as \"Name\" from SPM_Role").ToList();
			return _allRoles;
		}

		protected virtual IEnumerable<IdentityRole<T>> CurrentUserRoles()
		{
			List<T> r = null;
			if (_identity is WindowsIdentity wi && !wi.IsAnonymous)
			{
				var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1 union select RoleID from SPM_Role where SID in (" + groupNames.Join(",") + ")", 
					new { p1 = _userIdAccessor.CurrentUserID }).ToList();
			}
			else if (_identity is ClaimsIdentity ci)
			{
				var roleClaim = ci.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault();
				if (roleClaim != null)
				{
					if (typeof(T) == typeof(int))
						r = roleClaim.Value.Split(',').Select(x => x.ToInt32(0)).Cast<T>().ToList();
					else if(typeof(T) == typeof(string))
						r = roleClaim.Value.Split(',').Cast<T>().ToList();
					else if (typeof(T) == typeof(Guid))
						r = roleClaim.Value.Split(',').Select(x => Guid.Parse(x)).Cast<T>().ToList();
					else if (typeof(T) == typeof(long))
						r = roleClaim.Value.Split(',').Select(x => x.ToInt64(0)).Cast<T>().ToList();
				}
			}

			if (r == null)
			{
				r = _dc.Query<T>("select RoleID from V_AccessControl_SubjectRole where SubjectID = @p1", 
					new { p1 = _userIdAccessor.CurrentUserID }).ToList();
			}

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
        public IEnumerable<T> DenyRoles => new List<T>();

        public bool CurrentUserHasRoles(string[] roleNames)
		{
			return IdentityRoles.Select(o => o.Name.ToLower()).Intersect(roleNames.Select(o => o.ToLower())).Count() > 0;
		}
	}



	public class CacheableRoleBasedAccessControlStore<T> : RoleBasedAccessControlStoreBase<T>, ICacheableRoleBasedAccessControlStore<T>
	{
		public CacheableRoleBasedAccessControlStore(IDbConnection dc, IIdentity identity, IUserIdAccessor<long> userIdAccessor) : 
			base(dc, identity, userIdAccessor)
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

        public IEnumerable<string> GetRolesDeny()
        {
            return new string[0];
        }

        public IEnumerable<string> GetKeys()
		{
			return _dc.Query<string>(@"select upper(a.SystemName) from SPM_Action a order by a.SystemName");
		}

		public IEnumerable<T> RoleAncestors(T id)
		{
			return AllRoleAsso().Where(o => o.RoleID.Equals(id)).Select(o => o.ParentRoleID).ToList();
		}

		public IEnumerable<IdentityRole<string>> GetUserRoles(string userKey)
		{
			throw new NotImplementedException();
		}
	}

	public class RoleAsso<TKey>
	{
		public TKey ParentRoleID { get; set; }
		public TKey RoleID { get; set; }
	}
}
