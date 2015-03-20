using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Http;
using Nephrite.Identity;

namespace Nephrite.AccessControl
{
	public class SubjectWithRoles<TKey>
	{
		IIdentity _identity;
		IAccessControlDataContext<TKey> _dataContext;
		Subject<TKey> _subject;
		AccessControlOptions _options;

		public SubjectWithRoles(
			Subject<TKey> subject,
			IIdentity identity,
			IAccessControlDataContext<TKey> dataContext,
			AccessControlOptions options = null)
		{
			_subject = subject;
			_identity = identity;
			_dataContext = dataContext;
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
						r = _dataContext.SubjectRoles(_subject.ID, groupNames);
					}
					else
						r = _dataContext.SubjectRoles(_subject.ID);

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

		public static SubjectWithRoles<TKey> Current
		{
			get
			{
				var ctx = IdentityManager<TKey>.Instance.HttpContext();
				if (ctx.Items["CurrentSubjectWithRoles"] != null)
					return ctx.Items["CurrentSubjectWithRoles"] as SubjectWithRoles<TKey>;

				var curSubj = IdentityManager<TKey>.Instance.CurrentSubject;
				if (curSubj == null) return null;

				var s = new SubjectWithRoles<TKey>(
					curSubj, 
					ctx.User.Identity, 
					ActionAccessControl<TKey>.Instance.DataContext());
				ctx.Items["CurrentSubjectWithRoles"] = s;
				return s;
			}
		}
	}

	public class SubjectWithRoles : SubjectWithRoles<int>
	{
		public SubjectWithRoles(Subject<int> subject,
			IIdentity identity,
			IAccessControlDataContext<int> dataContext,
			AccessControlOptions options = null) : base(subject, identity, dataContext, options)
		{

		}
	}
}
