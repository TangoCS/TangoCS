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

		public static int GetCurrentSubjectID()
		{
			return Subject.Current == null ? 0 : Subject.Current.ID;
		}

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

		public void RefreshCache()
		{
			AccessRightManager.RefreshCache();
		}

		public static void RunWithElevatedPrivileges(Action action)
		{
			Subject.System.Run(action);
		}

		public static void RunAs(int sid, Action action)
		{
			var s = Subject.FromID(sid);
			if (s == null)
				throw new Exception(String.Format("Учетная запись с ИД {0} не зарегистрирована в системе", sid));
			s.Run(action);
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

	public class WebAccessRightsManager
	{
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
			ActionSPMContext.ResetCache();
		}
	}

}
