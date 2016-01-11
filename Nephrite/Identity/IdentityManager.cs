using System;
using System.Security.Claims;
using System.Security.Principal;
using Nephrite.Http;

namespace Nephrite.Identity
{
	public class IdentityManager<TKey> : IIdentityManager<TKey>
	{
		//static IIdentityManager<TKey> _instanceHolder;
		//static object LockObject = new object();

		//public IHttpContext HttpContext { get; private set; }
		IDC_Identity<TKey> _dataContext;
		IIdentity _user;
		public IdentityOptions Options { get; private set; }

		public IdentityManager(
			//IHttpContext httpContext,
			IIdentity user,
			IDC_Identity<TKey> dataContext,
			IdentityOptions options = null)
		{
			//HttpContext = httpContext;
			_user = user;
			_dataContext = dataContext;
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
		public IIdentity CurrentIdentity
		{
			get
			{
				return _user;
			}
		}

		Subject<TKey> _currentSubject = null;
		public Subject<TKey> CurrentSubject
		{
			get
			{
				
				if (_currentSubject != null) return _currentSubject;

				Subject<TKey> s = null;
				string name;
				if (!Options.Enabled)
				{
					name = Options.AnonymousSubjectName;
				}
				else
				{
					if (_user == null)
						name = Options.AnonymousSubjectName;
					else
					{
						WindowsIdentity wi = _user as WindowsIdentity;
						if (wi != null && !wi.IsAnonymous)
						{
							name = wi.User.Value;
							if (s == null) name = Options.AnonymousSubjectName;
						}
						else
						{
							name = _user.Name;
						}
					}
				}
				s = _dataContext.SubjectFromName<Subject<TKey>>(name);
				if (s == null) throw new Exception(String.Format("User {0} does not exist in the database", name));
				_currentSubject = s;
				return s;
			}
		}

		public Subject<TKey> SystemSubject
		{
			get
			{
				var name = Options.SystemSubjectName;
				var s = _dataContext.SubjectFromName<Subject<TKey>>(name);
				if (s == null) throw new Exception(String.Format("User {0} does not exist in the database", name));
				return s;
			}
		}

		public void RunAs(TKey sid, Action action)
		{
			var oldSubject = _currentSubject;
			_currentSubject = _dataContext.SubjectFromID<Subject<TKey>>(sid);
			action();
			_currentSubject = oldSubject;
		}
		public void RunAs(Subject<TKey> subject, Action action)
		{
			var oldSubject = _currentSubject;
			_currentSubject = subject;
			action();
			_currentSubject = oldSubject;
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
		public IPasswordHash HashMethod { get; set; }
		public bool AllowRegister { get; set; }
		public bool AllowPasswordReset { get; set; }
		public bool AllowRememberMe { get; set; }
		
		public IdentityOptions()
		{
			AnonymousSubjectName = "anonymous";
			SystemSubjectName = "system";
			Enabled = true;
			HashMethod = new PasswordHash();
			AllowRegister = false;
			AllowPasswordReset = false;
			AllowRememberMe = true;
        }
	}
}
