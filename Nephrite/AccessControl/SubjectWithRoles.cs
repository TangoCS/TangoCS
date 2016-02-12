using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class IdentityUserWithRoles<TKey>
		where TKey : IEquatable<TKey>
	{
		IIdentity _identity;
		AccessControlDataContext<TKey> _dataContext;
		IdentityUser<TKey> _subject;
		AccessControlOptions _options;

		public IdentityUserWithRoles(
			IDbConnection conn,
			IdentityUser<TKey> subject,
			IIdentity identity,
			AccessControlOptions options = null)
		{
			_subject = subject;
			_identity = identity;
			_dataContext = new AccessControlDataContext<TKey>(conn);
			_options = options ?? new AccessControlOptions();
		}

		IEnumerable<Role<TKey>> _roles = null;
		public IEnumerable<Role<TKey>> Roles
		{
			get
			{
				if (_roles == null)
				{
					WindowsIdentity wi = _identity as WindowsIdentity;
					List<TKey> r = null;
					if (wi != null && !wi.IsAnonymous)
					{
						var groupNames = wi.Groups.Select(x => "'" + x.Value + "'");
						r = _dataContext.SubjectRoles(_subject.Id, groupNames);
					}
					else
						r = _dataContext.SubjectRoles(_subject.Id);

					_roles = _dataContext.GetAllRoles().Where(o => r.Contains(o.RoleID));
				}
				return _roles;
			}
		}

		public bool HasRole(params string[] roleName)
		{
			return Roles.Select(o => o.SysName.ToLower()).Intersect(roleName.Select(o => o.ToLower())).Count() > 0;
		}

		bool? isAdministrator = null;
		public bool IsAdministrator
		{
			get
			{
				if (!isAdministrator.HasValue)
				{
					isAdministrator = Roles.Any(o => o.SysName.ToLower() == _options.AdminRoleName.ToLower());
				}

				return isAdministrator.Value;
			}
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
	}

	public class IdentityUserRoles : IdentityUserWithRoles<int>
	{
		public IdentityUserRoles(IDbConnection conn,
			IdentityUser<int> subject,
			IIdentity identity,
			AccessControlOptions options = null) : base(conn, subject, identity, options)
		{

		}
	}
}
