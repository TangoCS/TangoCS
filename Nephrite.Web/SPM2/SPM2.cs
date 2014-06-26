using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Principal;
using System.Collections;
using Nephrite.Meta;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Threading;
using Nephrite.Web.Controls.Scripts;

namespace Nephrite.Web.SPM
{
	[Cache("SPM")]
	public class Role
	{
		public int RoleID { get; set; }
		public string Title { get; set; }
		public string SysName { get; set; }

		static List<Role> _allRoles = null;
		static List<RoleAsso> _allRoleAsso = null;
	

		public static List<Role> GetList()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = A.Model.ExecuteQuery<Role>("select RoleID as \"RoleID\", Title as \"Title\", lower(SysName) as \"SysName\" from SPM_Role").ToList();
			return _allRoles;
		}

		public static Role FromID(int id)
		{
			return GetList().FirstOrDefault(o => o.RoleID == id);
		}

		public List<int> Ancestors
		{
			get
			{
				return AllRoleAsso().Where(o => o.RoleID == RoleID).Select(o => o.ParentRoleID).ToList();
			}
			private set { ;}
		}

		public static void ResetCache()
		{
			_allRoles = null;
			_allRoleAsso = null;
		}

		static List<RoleAsso> AllRoleAsso()
		{
			if (_allRoleAsso != null) return _allRoleAsso;
			_allRoleAsso = A.Model.ExecuteQuery<RoleAsso>("select ParentRoleID as \"ParentRoleID\", RoleID as \"RoleID\" from V_SPM_AllRoleAsso").ToList();
			return _allRoleAsso;
		}

		class RoleAsso
		{
			public int ParentRoleID { get; set; }
			public int RoleID { get; set; }
		}
	}

	public class Subject
	{
		private Subject() { }
        private static SPM2Scripts sPM2Scripts =  new SPM2Scripts();
			
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

		public int ID { get; private set; }
		public string Login { get; private set; }
		public string Title { get; private set; }
		public string Email { get; private set; }
		public byte[] PasswordHash { get; private set; }
		internal int _IsActive { get; private set; }
		public bool IsActive
		{
			get { return _IsActive == 1; }
		}
		internal int _IsDeleted { get; private set; }
		public bool IsDeleted
		{
			get { return _IsDeleted == 1; }
		}
		public string SID { get; set; }
		internal int _MustChangePassword { get; private set; }
		public bool MustChangePassword
		{
			get { return _MustChangePassword == 1; }
		}
		bool? isAdministrator = null;
		public bool IsAdministrator
		{
			get
			{
				if (!isAdministrator.HasValue)
					isAdministrator = Roles.Any(o => o.SysName == ConfigurationManager.AppSettings["AdministratorsRole"].ToLower());
				return isAdministrator.Value;
			}
			private set { ;}
		}

		public IEnumerable<Role> Roles
		{
			get
			{
				int sid = Subject.Current.ID;
				IEnumerable<Role> roles = Items["SubjectRoles2_" + sid.ToString()] as IEnumerable<Role>;
				if (roles == null)
				{
					WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
					List<int> r = null;

					if (wi != null && !wi.IsAnonymous)
					{
						//IEnumerable<string> groups = ADUser.Current.GetGroups(5);
						string groupNames = wi.Groups.Select(x => "'" + x.Value + "'").Join(",");
                        r = A.Model.ExecuteQuery<int>("select \"RoleID\" from DBO.\"V_SPM_AllSubjectRole\" where \"SubjectID\" = ? union select RoleID from SPM_Role where SID in (" + groupNames + ")", sid).ToList();
					}
					else
                        r = A.Model.ExecuteQuery<int>("select  \"RoleID\" from DBO.\"V_SPM_AllSubjectRole\" where \"SubjectID\" = ?", sid).ToList();

					roles = Role.GetList().Where(o => r.Contains(o.RoleID));
					Items["SubjectRoles2_" + sid.ToString()] = roles;
				}
				return roles;
			}
			private set { ;}
		}
		HashSet<string> _allowItems = new HashSet<string>();
		public HashSet<string> AllowItems
		{
			get
			{
				if (_allowItems == null) _allowItems = new HashSet<string>();
				return _allowItems;
			}
			private set { ;}
		}
		HashSet<string> _disallowItems = new HashSet<string>();
		public HashSet<string> DisallowItems
		{
			get
			{
				if (_disallowItems == null) _disallowItems = new HashSet<string>();
				return _disallowItems;
			}
			private set { ;}
		}

		public bool HasRole(params string[] roleName)
		{
			return Roles.Select(o => o.SysName).Intersect(roleName.Select(o => o.ToLower())).Count() > 0;
		}

		public void Run(Action action)
		{
			Subject oldSubject = Subject.Current;
			Items["CurrentSubject2"] = this;
			action();
			Items["CurrentSubject2"] = oldSubject;
		}

		public static Subject Current
		{
			get
			{
				if (Items["CurrentSubject2"] != null)
					return (Subject)Items["CurrentSubject2"];

				Subject s = null;
				if (ConfigurationManager.AppSettings["DisableSPM"] != null || HttpContext.Current == null)
				{
					s = Subject.FromLogin("anonymous");
				}
				else
				{
					if (HttpContext.Current.User == null) s = Subject.FromLogin("anonymous");

					WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
					if (wi != null && !wi.IsAnonymous)
					{
						s = Subject.FromSID(wi.User.Value, wi.Name.ToLower());
						if (s == null)
							s = Subject.FromLogin("anonymous");
					}
					else
					{
						if (HttpContext.Current.User.Identity.AuthenticationType == "Forms")
							s = Subject.FromLogin(HttpContext.Current.User.Identity.Name);
						else
							s = Subject.FromLogin("anonymous");
					}
				}
				Items["CurrentSubject2"] = s;
				return s;
			}
		}
		public static Subject FromLogin(string login)
		{
            return A.Model.ExecuteQuery<Subject>(sPM2Scripts.GetFromLoginScript, login.ToLower()).SingleOrDefault();
		}
		public static Subject FromSID(string sid, string login)
		{
            return A.Model.ExecuteQuery<Subject>(sPM2Scripts.GetFromSIDScript, sid, login).SingleOrDefault();
		}
		public static Subject FromID(int id)
		{
            return A.Model.ExecuteQuery<Subject>(sPM2Scripts.GetFromIDScript, id).SingleOrDefault();
		}
		public static Subject FromEmail(string email)
		{
            return A.Model.ExecuteQuery<Subject>(sPM2Scripts.GetFromEmailScript, email.ToLower()).SingleOrDefault();
		}
		public static Subject System
		{
			get
			{
				Subject s = Subject.FromLogin("System");
				if (s == null) throw new Exception("Учетная запись System не зарегистрирована в системе");
				return s;
			}
		}
	}

	public interface IPredicateLoader
	{
		void Load(Dictionary<string, Func<PredicateEvaluationContext, bool>> list);
	}

	[Cache("SPM")]
	public class SPM2
	{
		static HashSet<string> _accessCache = null;
		static HashSet<string> _itemsCache = null;


		public static bool Check<T>(int actionTypeID, Guid itemGUID, T predicateContext)
		{
			return Check(actionTypeID, itemGUID, predicateContext, false);
		}

		public static bool Check(int actionTypeID, Guid itemGUID)
		{
			return Check(actionTypeID, itemGUID, null, false);
		}

		static object _lock = new object();
		public static bool Check(int actionTypeID, Guid itemGUID, object predicateContext, bool defaultAccess)
		{
			Subject s = Subject.Current;
			string key = itemGUID.ToString() + "-" + actionTypeID.ToString();
			if (s == null) return false;
			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			if (_accessCache == null)
			{
				lock (_lock)
				{
					if (_accessCache == null)
					{
						string allAccessQuery = "select DISTINCT convert(varchar(36),ItemGUID) + '-' + convert(varchar(14),ActionTypeID) + '-' + convert(varchar, RoleID) as \"Value\" "+
				  "from SPM_RoleAccess ra, SPM_Action a "+
				  "where a.Type = 2 and a.ItemGUID is not null and a.ActionID = ra.ActionID "+
				  "order by Value";

						string allActionsQuery = "SELECT convert(varchar(36),ItemGUID) + '-' + convert(varchar(14),ActionTypeID) as \"Value\" "+
				"FROM SPM_Action "+
				"WHERE Type = 2 and ItemGUID is not null "+
				"ORDER BY Value";

						_accessCache = new HashSet<string>(A.Model.ExecuteQuery<string>(allAccessQuery));
						_itemsCache = new HashSet<string>(A.Model.ExecuteQuery<string>(allActionsQuery));
					}
				}
			}

			if (_itemsCache.Contains(key.ToUpper())) // есть ЗО
			{
				HashSet<string> _checking = new HashSet<string>(s.Roles.Select(o => key.ToUpper() + "-" + o.RoleID.ToString()));

				if (_accessCache.Overlaps(_checking)) // есть права у одной из ролей пользователя
				{
					if (predicateContext == null || CheckPredicate(key, predicateContext))
					{
						if (!s.AllowItems.Contains(key))
						{
							lock (_lock)
							{
								if (!s.AllowItems.Contains(key))
									s.AllowItems.Add(key);
							}
						}
						return true;
					}
					else
					{
						if (!s.DisallowItems.Contains(key))
						{
							lock (_lock)
							{
								if (!s.DisallowItems.Contains(key))
									s.DisallowItems.Add(key);
							}
						}
						return false;
					}
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock)
						{
							if (!s.DisallowItems.Contains(key))
								s.DisallowItems.Add(key);
						}
					}
					return false;
				}
			}
			else // ЗО нет
			{
				if (defaultAccess || s.IsAdministrator) return true;
			}

			return false;
		}

		public static void ResetCache()
		{
			_accessCache = null;
			_itemsCache = null;
			_predicates = null;
		}

		static Dictionary<string, Func<PredicateEvaluationContext, bool>> _predicates = null;

		public static bool CheckPredicate<T>(string key, T predicateContext)
		{
			return CheckPredicate(key, predicateContext);
		}

		public static bool CheckPredicate(string key, object predicateContext)
		{
			if (_predicates == null)
			{
				_predicates = new Dictionary<string, Func<PredicateEvaluationContext, bool>>();
				if (PredicateLoader == null)
					throw new Exception("Необходимо инициализировать SPM2.PredicateLoader при старте приложения");
				PredicateLoader.Load(_predicates);
			}
			if (!_predicates.ContainsKey(key)) return true;
			var pec = new PredicateEvaluationContext { PredicateContext = predicateContext };
			var result = _predicates[key](pec);
			if (!result)
				SetLastMessage(pec.Message);
			return result;
		}

		public static IPredicateLoader PredicateLoader { get; set; }

		public static void SetLastMessage(string msg)
		{
			if (HttpContext.Current != null)
			{
				HttpContext.Current.Items["LastSPMMessage"] = msg;
			}
		}

		public static string GetLastMessage()
		{
			if (HttpContext.Current != null)
			{
				if (HttpContext.Current.Items["LastSPMMessage"] != null)
					return (string)HttpContext.Current.Items["LastSPMMessage"];
			}
			return "";
		}

		public static bool Enabled
		{
			get
			{
				string se = Url.Current.GetString("spmenabled");
				if (se == "0" || se.ToLower() == "false")
				{
					return !AppSPM.IsCurrentUserHasRole(ConfigurationManager.AppSettings["AdministratorsRole"]);
				}
				return true;
			}
		}
	}

	public class PredicateEvaluationContext
	{
		public object PredicateContext { get; set; }
		public string Message { get; set; }
	}
}