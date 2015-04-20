using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nephrite.Http;

namespace Nephrite.Identity
{
	public class IdentityManager<TKey> : IIdentityManager<TKey>
	{
		//static IIdentityManager<TKey> _instanceHolder;
		//static object LockObject = new object();

		public IHttpContext HttpContext { get; private set; }
		public IDC_Identity<TKey> DataContext { get; private set; }
		public IdentityOptions Options { get; private set; }

		public IdentityManager(
			IHttpContext httpContext,
			IDC_Identity<TKey> dataContext,
			IdentityOptions options = null)
		{
			HttpContext = httpContext;
			DataContext = dataContext;
			Options = options ?? new IdentityOptions();
		}

		//public static void Init(
		//	Func<IHttpContext> httpContext,
		//	Func<IDC_Identity<TKey>> dataContext,
		//	IdentityOptions options = null
		//	)
		//{
		//	if (_instanceHolder == null)
		//	{
		//		lock (LockObject)
		//		{
		//			_instanceHolder = new IdentityManager<TKey>(httpContext, dataContext, options);
		//			return;
		//		}
		//	}

		//	throw new ApplicationException("Initalize() method should be called only once.");
		//}

		//public static IIdentityManager<TKey> Instance
		//{
		//	get
		//	{
		//		if (_instanceHolder == null)
		//		{
		//			throw new ApplicationException("IdentityManager instance hasn't been initialized.");
		//		}

		//		return _instanceHolder;
		//	}
		//}

		public Subject<TKey> CurrentSubject
		{
			get
			{
				var ctx = HttpContext;
				if (ctx.Items["CurrentSubject2"] != null)
					return ctx.Items["CurrentSubject2"] as Subject<TKey>;

				Subject<TKey> s = null;
				if (!Options.Enabled)
				{
					s = DataContext.SubjectFromName<Subject<TKey>>("anonymous");
				}
				else
				{
					if (ctx.User == null) s = DataContext.SubjectFromName<Subject<TKey>>("anonymous");

					WindowsIdentity wi = ctx.User.Identity as WindowsIdentity;
					if (wi != null && !wi.IsAnonymous)
					{
						s = DataContext.SubjectFromSID<Subject<TKey>>(wi.User.Value);
						if (s == null) s = DataContext.SubjectFromName<Subject<TKey>>("anonymous");
					}
					else
					{
						if (ctx.User.Identity.AuthenticationType == "Forms")
							s = DataContext.SubjectFromName<Subject<TKey>>(ctx.User.Identity.Name);
						else
							s = DataContext.SubjectFromName<Subject<TKey>>("anonymous");
					}
				}
				ctx.Items["CurrentSubject2"] = s;
				return s;
			}
		}

		public Subject<TKey> SystemSubject
		{
			get
			{
				var name = Options.SystemSubjectName;
				var s = DataContext.SubjectFromName<Subject<TKey>>(name);
				if (s == null) throw new Exception(String.Format("Учетная запись {0} не зарегистрирована в системе", name));
				return s;
			}
		}

		public void RunAs(TKey sid, Action action)
		{
			var oldSubject2 = Subject.Current;
			HttpContext.Items["CurrentSubject2"] = DataContext.SubjectFromID<Subject<TKey>>(sid);
			action();
			HttpContext.Items["CurrentSubject2"] = oldSubject2;
		}
	}

	public interface IDC_Identity<TKey>
	{
		TSubject SubjectFromName<TSubject>(string name);
		TSubject SubjectFromSID<TSubject>(string sid);
		TSubject SubjectFromID<TSubject>(TKey id);
		TSubject SubjectFromEmail<TSubject>(string email);

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
		public string AnonymousSubjectName { get; set; }
		public string SystemSubjectName { get; set; }
		public bool Enabled { get; set; }

		public IdentityOptions()
		{
			AnonymousSubjectName = "anonymous";
			SystemSubjectName = "system";
			Enabled = true;
		}
	}
}
