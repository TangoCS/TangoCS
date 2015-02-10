using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nephrite.Identity
{
	public class Subject<TKey>
	{
		public TKey ID { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Email { get; set; }
		public byte[] PasswordHash { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public string SID { get; set; }
		public bool MustChangePassword { get; set; }

		bool? isAdministrator = null;
		public bool IsAdministrator
		{
			get
			{
				if (!isAdministrator.HasValue)
					isAdministrator = Roles.Any(o => o.SysName.ToLower() == IdentityManager<TKey>.Instance.Options.AdminRoleName.ToLower());
				return isAdministrator.Value;
			}
		}

		public static Subject<TKey> Current
		{
			get
			{
				return IdentityManager<TKey>.Instance.CurrentSubject;
			}
		}

		public static Subject<TKey> System
		{
			get
			{
				return IdentityManager<TKey>.Instance.SystemSubject;
			}
		}

		IEnumerable<Role<TKey>> _roles = null;
		public IEnumerable<Role<TKey>> Roles
		{
			get
			{
				var ctx = IdentityManager<TKey>.Instance.AppContext();
				if (_roles == null)
				{
					WindowsIdentity wi = ctx.User.Identity as WindowsIdentity;
					List<TKey> r = null;
					var dc = IdentityManager<TKey>.Instance.DataContext();

					if (wi != null && !wi.IsAnonymous)
					{
						//IEnumerable<string> groups = ADUser.Current.GetGroups(5);
						var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
						r = dc.SubjectRoles(ID, groupNames);
						//r = A.Model.ExecuteQuery<int>("select \"RoleID\" from DBO.\"V_SPM_AllSubjectRole\" where \"SubjectID\" = ? union select RoleID from SPM_Role where SID in (" + groupNames + ")", sid).ToList();
					}
					else
						r = dc.SubjectRoles(ID); 
						// A.Model.ExecuteQuery<int>("select \"RoleID\" from DBO.\"V_SPM_AllSubjectRole\" where \"SubjectID\" = ?", sid).ToList();

					_roles = dc.GetAllRoles().Where(o => r.Contains(o.RoleID));
				}
				return _roles;
			}
		}

		public bool HasRole(params string[] roleName)
		{
			return Roles.Select(o => o.SysName.ToLower()).Intersect(roleName.Select(o => o.ToLower())).Count() > 0;
		}

		HashSet<string> _allowItems = new HashSet<string>();
		public HashSet<string> AllowItems
		{
			get
			{
				if (_allowItems == null) _allowItems = new HashSet<string>();
				return _allowItems;
			}
		}
		HashSet<string> _disallowItems = new HashSet<string>();
		public HashSet<string> DisallowItems
		{
			get
			{
				if (_disallowItems == null) _disallowItems = new HashSet<string>();
				return _disallowItems;
			}
		}

		public void Run(Action action)
		{
			var oldSubject = Current;
			var ctx = IdentityManager<TKey>.Instance.AppContext();
			ctx.Items["CurrentSubject2"] = this;
			action();
			ctx.Items["CurrentSubject2"] = oldSubject;
		}
	}

	public class Subject : Subject<int>
	{

	}
}
