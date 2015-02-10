using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nephrite.Identity
{
	public class IdentityManager<TKey>
	{
		static IdentityManager<TKey> _instanceHolder;
		static object LockObject = new object();

		public Func<IAppContext> AppContext { get; set; }
		public Func<IDC_Identity<TKey>> DataContext { get; set; }
		public IdentityOptions Options { get; set; }

		public IdentityManager(
			Func<IAppContext> appContext,
			Func<IDC_Identity<TKey>> dataContext,
			IdentityOptions options = null)
		{
			AppContext = appContext;
			DataContext = dataContext;
			Options = options ?? new IdentityOptions();
		}

		public static void Init(
			Func<IAppContext> appContext,
			Func<IDC_Identity<TKey>> dataContext,
			IdentityOptions options = null
			)
		{
			if (_instanceHolder == null)
			{
				lock (LockObject)
				{
					if (_instanceHolder == null)
					{
						_instanceHolder = new IdentityManager<TKey>(appContext, dataContext, options);
						return;
					}
				}
			}

			throw new ApplicationException("Initalize() method should be called only once.");
		}

		public static IdentityManager<TKey> Instance
		{
			get
			{
				if (_instanceHolder == null)
				{
					throw new ApplicationException("IdentityConfiguration instance hasn't been initialized.");
				}

				return _instanceHolder;
			}
		}

		public Subject<TKey> CurrentSubject
		{
			get
			{
				var ctx = AppContext();
				if (ctx.Items["CurrentSubject2"] != null)
					return ctx.Items["CurrentSubject2"] as Subject<TKey>;

				Subject<TKey> s = null;
				if (!Options.Enabled)
				{
					s = DataContext().SubjectFromName("anonymous");
				}
				else
				{
					if (ctx.User == null) s = DataContext().SubjectFromName("anonymous");

					WindowsIdentity wi = ctx.User.Identity as WindowsIdentity;
					if (wi != null && !wi.IsAnonymous)
					{
						s = DataContext().SubjectFromSID(wi.User.Value);
						if (s == null) s = DataContext().SubjectFromName("anonymous");
					}
					else
					{
						if (ctx.User.Identity.AuthenticationType == "Forms")
							s = DataContext().SubjectFromName(ctx.User.Identity.Name);
						else
							s = DataContext().SubjectFromName("anonymous");
					}
				}
				ctx.Items["CurrentSubject2"] = s;
				return s;
			}
		}
	}

	public interface IDC_Identity<TKey>
	{
		Subject<TKey> SubjectFromName(string name);
		Subject<TKey> SubjectFromSID(string sid);
		Subject<TKey> SubjectFromID(TKey id);
		Subject<TKey> SubjectFromEmail(string email);

		List<Role<TKey>> GetAllRoles();
		Role<TKey> RoleFromID(TKey id);
		List<TKey> RoleAncestors(TKey id);

		List<TKey> SubjectRoles(TKey id, IEnumerable<string> activeDirectoryGroups = null);

		//public static List<Role> GetList()
		//{
		//	if (_allRoles != null) return _allRoles;
		//	_allRoles = A.Model.ExecuteQuery<Role>("select RoleID as \"RoleID\", Title as \"Title\", lower(SysName) as \"SysName\" from SPM_Role").ToList();
		//	return _allRoles;
		//}

		//public static Role RoleFromID(int id)
		//{
		//	return GetList().FirstOrDefault(o => o.RoleID == id);
		//}

		//static List<RoleAsso> AllRoleAsso()
		//{
		//	if (_allRoleAsso != null) return _allRoleAsso;
		//	_allRoleAsso = A.Model.ExecuteQuery<RoleAsso>(@"select ""ParentRoleID"", ""RoleID"" from dbo.""V_SPM_AllRoleAsso""").ToList();
		//	return _allRoleAsso;
		//}

		//public List<int> RoleAncestors
		//{
		//	get
		//	{
		//		return AllRoleAsso().Where(o => o.RoleID == RoleID).Select(o => o.ParentRoleID).ToList();
		//	}
		//	private set { ;}
		//}

		//string GetRolesAccessQuery { get; }
		//string GetItemsQuery { get; }
	}

	public class IdentityOptions
	{
		[DefaultValue("Administrators")]
		public string AdminRoleName { get; set; }

		[DefaultValue("anonymous")]
		public string AnonymousSubjectName { get; set; }

		[DefaultValue("system")]
		public string SystemSubjectName { get; set; }

		[DefaultValue(true)]
		public bool Enabled { get; set; }
	}
}
