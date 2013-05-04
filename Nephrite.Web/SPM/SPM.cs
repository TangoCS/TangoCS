using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Data.Linq;
using System.DirectoryServices.AccountManagement;

namespace Nephrite.Web.SPM
{
	public class AppSPM
	{
		//List<SPM_RoleAsso> _roleAsso = null;
		//List<SPM_RoleAccess> _roleAccess = null;
		//Dictionary<int, List<int>> _roleAncestors = new Dictionary<int, List<int>>();
		//SortedDictionary<string, int> _methods = new SortedDictionary<string, int>();

		static WebAccessRightsManager _accessRightManager = null;
		public static WebAccessRightsManager AccessRightManager
		{
			get
			{
				if (_accessRightManager == null)
				{
					_accessRightManager = new WebAccessRightsManager();
				}
				if (_lastAccess.AddMinutes(_cacheLifeTime) < DateTime.Now)
				{
					_lastAccess = DateTime.Now;
					AppSPM.Instance.RefreshCache();
				}
				else
				{
					_lastAccess = DateTime.Now;
				}
				return _accessRightManager;
			}
		}
		/*public static SpmModelDataContext DataContext
		{
			get
			{
				if (AccessRightManager == null)
					return new SpmModelDataContext(ConnectionManager.ConnectionString);
				return AccessRightManager.DataContext;
			}
		}*/

		//static Func<SpmModelDataContext, string, SPM_Subject> getSPM_Subject;

		public static int GetCurrentSubjectID()
		{
			return Subject.Current == null ? 0 : Subject.Current.ID;
		}

		/*public static int GetRoleID(string roleName)
		{
			return Role.GetList().Where(o => o.SysName == roleName).Select(o => o.RoleID).FirstOrDefault();
		}*/

		public static string[] CurrentUserRoles
		{
			get
			{
				return Subject.Current.Roles.Select(o => o.SysName).ToArray();
			}
		}

		public static bool IsCurrentUserHasRole(params string[] roleName)
		{
			var s = Subject.Current;
			return s != null ? s.HasRole(roleName) : false;
		}

		/*public static bool IsLoginExists(string login)
		{
			return Subject.FromLogin(login) != null;
		}

		public static SPM_Subject CreateSubject(string SID, string systemName, string title, bool submitChanges)
		{
			SPM_Subject s = new SPM_Subject();
			DataContext.SPM_Subjects.InsertOnSubmit(s);
			s.SID = SID;
			s.SystemName = systemName;
			s.Title = title;
			if (submitChanges) DataContext.SubmitChanges();
			return s;
		}*/

		public static string CheckPass(string password1, string password2)
		{
			string lMess = "";
			char[] pwdChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890~!@#$%^&*()-=\\][{}|+_`';:/?.>,<".ToCharArray();

			if (password1 != password2)
			{
				lMess = "Введенные пароли не совпадают!";
			}

			int lp = ConfigurationManager.AppSettings["MinPassLength"].ToInt32(6);
			if (password1.Length < lp)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Пароль должен быть не короче " + lp.ToString() + " символов!";
			}

			foreach (char c in password1.ToCharArray())
			{
				if (!pwdChars.Contains(c))
				{
					if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
					lMess += "Пароль содержит недопустимые символы!";
					break;
				}
			}

			return lMess;
		}
		public static string CheckUserData(string login, string password1, string password2, string email)
		{
			string lMess = "";
			char[] loginChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890_".ToCharArray();

			if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(login) ||
				String.IsNullOrEmpty(password1) || String.IsNullOrEmpty(password2))
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Необходимо заполнить все поля, помеченные звездочкой!";
			}

			int ll = ConfigurationManager.AppSettings["MaxLoginLength"].ToInt32(12);
			if (login.Length > ll)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Длина имени пользователя не должна превышать " + ll.ToString() + " символов!";
			}
			

			foreach (char c in login.ToCharArray())
			{
				if (!loginChars.Contains(c))
				{
					if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
					lMess += "Имя пользователя содержит недопустимые символы!";
					break;
				}
			}

			if (Subject.FromLogin(login) != null)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "Введенное имя пользователя уже существует в системе!";
			}

			if (Subject.FromEmail(email) != null)
			{
				if (!String.IsNullOrEmpty(lMess)) lMess += "<br />";
				lMess += "В системе уже зарегистрирован пользователь с указанным адресом электронной почты!";
			}

			string s = CheckPass(password1, password2);
			if (!String.IsNullOrEmpty(s)) lMess += "<br />" + s;

			return lMess;
		}

		static AppSPM _instance;
		static DateTime _lastAccess;
		static int _cacheLifeTime = 3;

		public static AppSPM Instance
		{
			get
			{
				if (_instance == null) _instance = new AppSPM();
				if (_lastAccess.AddMinutes(_cacheLifeTime) < DateTime.Now)
				{
					_lastAccess = DateTime.Now;
					AppSPM.Instance.RefreshCache();
				}
				else
				{
					_lastAccess = DateTime.Now;
				}
				return _instance;				
			}			
		}

		/*public bool CheckRoleAccess(int roleID, int actionID)
		{
			List<int> anc = Role.FromID(roleID).Ancestors; //RoleAncestors(roleID);
			return (from ra in _roleAccess
					where anc.Contains(ra.RoleID) && ra.ActionID == actionID
					select ra).Count() > 0;
		}*/

		/*public List<int> RoleAncestors(int roleID)
		{

			if (!_roleAncestors.ContainsKey(roleID))
			{
				lock (_roleAncestors)
				{
					if (!_roleAncestors.ContainsKey(roleID))
					{
						if (_roleAsso == null) RefreshCache();
						List<int> parents = new List<int>();
						parents.Add(roleID);
						AddParents(_roleAsso.Where(o => o.RoleID == roleID), ref parents);
						_roleAncestors.Add(roleID, parents);
						return parents;
					}
					else
						return _roleAncestors[roleID];
				}
			}
			else
				return _roleAncestors[roleID];

		}*/

		/*public List<int> RoleActions(int roleID)
		{
			List<int> anc = Role.FromID(roleID).Ancestors; //RoleAncestors(roleID);
			return (from ra in _roleAccess
					where anc.Contains(ra.RoleID)
					select ra.ActionID).ToList();
		}*/

		/*void AddParents(IEnumerable<SPM_RoleAsso> asso, ref List<int> parents)
		{
			foreach (SPM_RoleAsso ra in asso)
			{
				if (!parents.Contains(ra.ParentRoleID))
				{
					parents.Add(ra.ParentRoleID);
					AddParents(_roleAsso.Where(o => o.RoleID == ra.ParentRoleID), ref parents);
				}
			}
		}*/

		public void RefreshCache()
		{
			//SPM_CasheFlag c = AppSPM.DataContext.SPM_CasheFlags.First();
			//if (c.IsChange)
			//{
				//_roleAsso = DataContext.SPM_RoleAssos.ToList();
				//_roleAccess = DataContext.SPM_RoleAccesses.ToList();
				//_roleAncestors = new Dictionary<int, List<int>>();
				//_methods = new SortedDictionary<string, int>();
			AccessRightManager.RefreshCache();

			//	c.IsChange = false;
			//	AppSPM.DataContext.SubmitChanges();
			//}
		}

		public static void RunWithElevatedPrivileges(Action action)
		{
			Subject.System.Run(action);
			//SPM_Subject system = DataContext.SPM_Subjects.SingleOrDefault(o => o.SystemName == "System");
			//if (system == null)
			//	throw new Exception("Учетная запись System не зарегистрирована в системе");
			//var oldSubject = GetCurrentSubject();
			//var oldSubject2 = Subject.Current;
			//Items["CurrentSubject"] = system;
			//Items["CurrentSubject2"] = Subject.FromLogin("System");
			//action();
			//Items["CurrentSubject"] = oldSubject;
			//Items["CurrentSubject2"] = oldSubject2;
		}

		public static void RunAs(int sid, Action action)
		{
			var s = Subject.FromID(sid);
			if (s == null)
				throw new Exception(String.Format("Учетная запись с ИД {0} не зарегистрирована в системе", sid));
			s.Run(action);
			//SPM_Subject s = DataContext.SPM_Subjects.Single(o => o.SubjectID == sid);
			//var oldSubject = GetCurrentSubject();
			//var oldSubject2 = Subject.Current;
			//Items["CurrentSubject"] = s;
			//Items["CurrentSubject2"] = Subject.FromID(sid);
			//action();
			//Items["CurrentSubject"] = oldSubject;
			//Items["CurrentSubject2"] = oldSubject2;
		}

		static IDictionary Items
		{
			get
			{
				if (HttpContext.Current != null)
					return HttpContext.Current.Items;
				else
				{
					Hashtable ht = (Hashtable)AppDomain.CurrentDomain.GetData("ContextItems");
					if (ht == null)
					{
						ht = new Hashtable();
						AppDomain.CurrentDomain.SetData("ContextItems", ht);
					}
					return ht;
				}
			}
		}

		/// <summary>
		/// Проверка прав на доступ к файлу
		/// </summary>
		/// <param name="guidPath">Полный Guid-путь файла</param>
		/// <param name="folderActionType">Тип действия (код из кодификатора Nephrite.Web.FileStorage.FolderActionType)</param>
		/// <returns>True, если есть доступ или предикаты не заданы</returns>
		public static bool CheckFileAccess(string guidPath, char folderActionType)
		{
			if (guidPath == null)
				return true;
			string[] guids = guidPath.Split('/');
			if (guids.Length < 2)
				return true;
			for (int i = guids.Length - 1; i >= 0; i--)
			{
				var g = new Guid(guids[i]);
				if (predicates.ContainsKey(g))
				{
					var p2 = predicates[g];
					if (p2.ContainsKey(folderActionType))
						return p2[folderActionType](new Guid(guids[guids.Length - 2]));
				}
			}
			return true;
		}

		/// <summary>
		/// Проверка прав на доступ к папке
		/// </summary>
		/// <param name="guidPath">Полный Guid-путь папки</param>
		/// <param name="folderActionType">Тип действия (код из кодификатора Nephrite.Web.FileStorage.FolderActionType)</param>
		/// <returns>True, если есть доступ или предикаты не заданы</returns>
		public static bool CheckFolderAccess(string guidPath, char folderActionType)
		{
			if (guidPath == null)
				return true;
			string[] guids = guidPath.Split('/');
			if (guids.Length < 1)
				return true;
			for (int i = guids.Length - 1; i >= 0; i--)
			{
				var g = new Guid(guids[i]);
				if (predicates.ContainsKey(g))
				{
					var p2 = predicates[g];
					if (p2.ContainsKey(folderActionType))
						return p2[folderActionType](new Guid(guids[guids.Length - 1]));
				}
			}
			return true;
		}

		static Dictionary<Guid, Dictionary<char, Func<Guid, bool>>> predicates = new Dictionary<Guid, Dictionary<char, Func<Guid, bool>>>();
		public static void AddFolderPredicate(Guid folderGuid, char folderActionType, Func<Guid, bool> predicate)
		{
			lock (predicates)
			{
				Dictionary<char, Func<Guid, bool>> p2 = null;
				if (!predicates.ContainsKey(folderGuid))
				{
					p2 = new Dictionary<char, Func<Guid, bool>>();
					predicates.Add(folderGuid, p2);
				}
				else
					p2 = predicates[folderGuid];
				if (!p2.ContainsKey(folderActionType))
					p2.Add(folderActionType, predicate);
			}
		}
	}

	public enum SPMActionType
	{
		BusinessPackage,
		BusinessObject,
		Method
	}

	public class WebAccessRightsManager
	{
		/*public SpmModelDataContext DataContext
		{
			get
			{
				if (HttpContext.Current == null)
					return new SpmModelDataContext(ConnectionManager.Connection);
				if (HttpContext.Current.Items["SPMDataContext"] == null)
				{
					SpmModelDataContext dc = new SpmModelDataContext(ConnectionManager.Connection);
					HttpContext.Current.Items["SPMDataContext"] = dc;
					dc.Log = new DataContextLogWriter();
					dc.CommandTimeout = 300;
				}
				return (SpmModelDataContext)HttpContext.Current.Items["SPMDataContext"];
			}
		}*/

		public bool Check(string businessObject, string method)
		{
			return Check(businessObject, method, false);
		}
		public bool Check(string businessObject, string method, bool defaultResult)
		{
			return ActionSPMContext.Current.Check(businessObject + "." + method, 1, defaultResult);
		}

		public void RefreshCache()
		{
			//_userAccessDenied = new SortedDictionary<string, List<string>>();
			//spmMethods = null;
			ActionSPMContext.ResetCache();
		}
	}

}
