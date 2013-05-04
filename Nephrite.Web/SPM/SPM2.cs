using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Principal;
using System.Collections;

namespace Nephrite.Web.SPM
{
	public interface ISPMActionCategory
	{
		int ID {get;}
		string Title {get;}
		IQueryable<SPMActionData> GetActions();
		//SystemMapItem GetSystemMapItem(Guid objectGuid);
	}

	public class SPMActionData : IModelObject
	{
		public int ActionID { get; set; }
		public int CategoryID { get; set; }
		public Guid? SubCategoryGUID { get; set; }
		public Guid ItemGUID { get; set; }

		public string SubCategory { get; set; }
		public string Item { get; set; }

		public string Title { get; set; }

		public int ObjectID
		{
			get { return ActionID; }
		}
		public string GetClassName()
		{
			return "Защищаемая операция";
		}
		public Guid ObjectGUID
		{
			get { return Guid.Empty; }
		}
	}

	[Cache("SPM")]
	public class Role
	{
		public int RoleID { get; set; }
		public string Title { get; set; }
		public string SysName { get; set; }

		static IEnumerable<Role> _allRoles = null;

		public static IEnumerable<Role> GetList()
		{
			if (_allRoles != null) return _allRoles;
			_allRoles = Base.Model.ExecuteQuery<Role>("select RoleID, Title, SysName from SPM_Role");
			return _allRoles;
		}

		public static void ResetCache()
		{
			_allRoles = null;
		}
	}

	public class Subject
	{
		private Subject() { }

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
		public byte[] PasswordHash { get; private set; }
		public bool IsActive { get; private set; }
		public bool IsDeleted { get; private set; }
		public string SID { get; set; }

		bool? isAdministrator = null;
		public bool IsAdministrator 
		{ 
			get 
			{ 
				if (!isAdministrator.HasValue) 
					isAdministrator = Roles.Any(o => o.SysName == ConfigurationManager.AppSettings["AdministratorsRole"]);
				return isAdministrator.Value;
			}
		}

		public IEnumerable<Role> Roles 
		{
			get
			{
				int sid = Subject.Current.ID;
				IEnumerable<Role> roles = Items["SubjectRoles_" + sid.ToString()] as IEnumerable<Role>;
				if (roles == null)
				{
					IEnumerable<int> r = Base.Model.ExecuteQuery<int>("select RoleID from V_SPM_AllSubjectRoles where SubjectID = {0}", sid);
					roles = Role.GetList().Where(o => r.Contains(o.RoleID));
					Items["SubjectRoles_" + sid.ToString()] = roles;
				}
				return roles;
			}
		}
		HashSet<Guid> _allowItems = new HashSet<Guid>();
		public HashSet<Guid> AllowItems { get { return _allowItems; } }
		HashSet<Guid> _disallowItems = new HashSet<Guid>();
		public HashSet<Guid> DisallowItems { get { return _disallowItems; } }

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
					WindowsIdentity wi = HttpContext.Current.User.Identity as WindowsIdentity;
					if (wi != null && !wi.IsAnonymous)
					{
						s = Subject.FromSID(wi.User.Value);
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
			return Base.Model.ExecuteQuery<Subject>("select SubjectID, Login, Title, PasswordHash, IsActive, IsDeleted, SID from SPM_Subject where lower(SystemName) = {0}", login.ToLower()).SingleOrDefault();
		}
		public static Subject FromSID(string sid)
		{
			return Base.Model.ExecuteQuery<Subject>("select SubjectID, Login, Title, PasswordHash, IsActive, IsDeleted, SID from SPM_Subject where SID = {0}", sid).SingleOrDefault();
		}
		public static Subject FromID(int id)
		{
			return Base.Model.ExecuteQuery<Subject>("select SubjectID, Login, Title, PasswordHash, IsActive, IsDeleted, SID from SPM_Subject where SubjectID = {0}", id).SingleOrDefault();
		}
		public static Subject System()
		{
			Subject s = Subject.FromLogin("System");
			if (s == null) throw new Exception("Учетная запись System не зарегистрирована в системе");
			return s;
		}
	}

	[Cache("SPM")]
	public class SPM2
	{
		static Dictionary<int, ISPMActionCategory> _categories = new Dictionary<int, ISPMActionCategory>();
		public static void RegisterActionCategory(ISPMActionCategory category)
		{
			_categories.Add(category.ID, category);
		}
		public static Dictionary<int, ISPMActionCategory> Categories
		{
			get
			{
				return _categories;
			}
		}
		

		public static void DeleteAction(int id)
		{
			Base.Model.ExecuteCommand("delete from SPM_Action where ActionID = {0}", id);
		}

		public static int CreateAction(int CategoryID, Guid? SubCategoryGUID, Guid ItemGUID)
		{
			return Base.Model.ExecuteQuery<int>("insert into SPM_Action (CategoryID, SubCategoryGUID, ItemGUID) values ({0},{1},{2}); select scope_identity()", CategoryID, SubCategoryGUID, ItemGUID).First();
		}

		static HashSet<string> _accessCache = null;
		static HashSet<Guid> _itemsCache = null;

		public static bool Check(Guid itemGUID)
		{
			return Check(itemGUID);
		}

		public static bool Check(Guid itemGUID, bool defaultAccess)
		{
			Subject s = Subject.Current;
			if (s == null) return false;
			if (s.AllowItems.Contains(itemGUID)) return true;
			if (s.DisallowItems.Contains(itemGUID)) return false;

			if (_accessCache == null)
			{
				string allAccessQuery = @"
				WITH FullRoleAsso(ActionID, ItemGUID, RoleID) 
				AS (SELECT a.ActionID, a.ItemGUID, RoleID
				FROM dbo.SPM_RoleAccess ra, dbo.SPM_Action a
				WHERE ra.ActionID = a.ActionID and a.Type = 2 and a.ItemGUID is not null
				UNION ALL
				SELECT FullRoleAsso.ActionID, a.ItemGUID, ra.ParentRoleID
				FROM FullRoleAsso, dbo.SPM_RoleAsso ra, dbo.SPM_Action a
				WHERE FullRoleAsso.RoleID = ra.RoleID and FullRoleAsso.ActionID = a.ActionID and 
				a.Type = 2 and a.ItemGUID is not null)

				SELECT DISTINCT convert(varchar(36),ItemGUID) + '-' + convert(varchar, RoleID) as Value
				FROM FullRoleAsso
				ORDER BY Value";

				string allActionsQuery = @"
				SELECT ItemGUID as Value 
				FROM SPM_Action 
				WHERE Type = 2 and ItemGUID is not null
				ORDER BY Value";

				_accessCache = new HashSet<string>(Base.Model.ExecuteQuery<string>(allAccessQuery, null));
				_itemsCache = new HashSet<Guid>(Base.Model.ExecuteQuery<Guid>(allActionsQuery, null));
			}

			if (_itemsCache.Contains(itemGUID)) // есть ЗО
			{
				string sitem = itemGUID.ToString();
				HashSet<string> _checking = new HashSet<string>(s.Roles.Select(o => sitem + "-" + o.RoleID.ToString()));

				if (_accessCache.Overlaps(_checking)) // есть права у одной из ролей пользователя
				{
					s.AllowItems.Add(itemGUID);
					return true;
				}
				else
				{
					s.DisallowItems.Add(itemGUID);
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
		}
	}
}