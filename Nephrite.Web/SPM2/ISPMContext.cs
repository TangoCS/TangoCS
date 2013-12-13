using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Nephrite.Web.SPM
{
	public interface ISPMContext
	{
		bool Enabled { get; }
		bool CheckPredicate(Guid securableObject, int actionTypeID, object predicateContext);

		bool Check(Guid securableObjectGUID, int actionTypeID);
		bool Check(Guid securableObjectGUID, int actionTypeID, object predicateContext);
		bool Check(string securableObjectName, int actionTypeID);

		bool Check(Guid securableObjectGUID, int actionTypeID, bool defaultAccess);
		bool Check(Guid securableObjectGUID, int actionTypeID, object predicateContext, bool defaultAccess);
		bool Check(string securableObjectName, int actionTypeID, bool defaultAccess);

		bool CheckForRole(int roleID, Guid securableObjectGUID, int actionTypeID);

		string GetLastMessage();
	}

	/*public interface ISecurable
	{
		Guid? GetSecurableObjectGUID(int actionTypeID);
		BoolResult CheckPredicate(int actionTypeID);
		BoolResult Check(int actionTypeID, bool defaultAccess = false);
	}*/

	public abstract class SPMContextBase : ISPMContext
	{
		protected string _message = "";
		protected string _className = "";

		public bool Enabled
		{
			get
			{
				string se = Url.Current.GetString("spmenabled");
				if (se == "0" || se.ToLower() == "false")
				{
					return !Subject.Current.IsAdministrator;
				}
				return true;
			}
		}

		static Dictionary<string, Func<PredicateEvaluationContext, bool>> _predicates = null;
		public static IPredicateLoader PredicateLoader { get; set; }

		public bool CheckPredicate(Guid securableObject, int actionTypeID, object predicateContext)
		{
			if (!Enabled) return true;
			string key = securableObject.ToString() + "-" + actionTypeID.ToString();
			if (_predicates == null)
			{
				_predicates = new Dictionary<string, Func<PredicateEvaluationContext, bool>>();
				if (PredicateLoader == null)
					throw new Exception("Необходимо инициализировать SPMContextBase.PredicateLoader при старте приложения");
				PredicateLoader.Load(_predicates);
			}
			if (!_predicates.ContainsKey(key)) return true;
			var pec = new PredicateEvaluationContext { PredicateContext = predicateContext };
			var result = _predicates[key](pec);
			if (!result)
				_message = pec.Message;
			return result;
		}

		protected static object _lock = new object();

		public virtual bool CheckForRole(int roleID, Guid securableObjectGUID, int actionTypeID)
		{
			throw new NotImplementedException();
		}

		public bool Check(Guid securableObjectGUID, int actionTypeID, object predicateContext)
		{
			if (!CheckPredicate(securableObjectGUID, actionTypeID, predicateContext)) return false;
			return Check(securableObjectGUID, actionTypeID, false);
		}

		public virtual bool Check(Guid securableObjectGUID, int actionTypeID)
		{
			return Check(securableObjectGUID.ToString(), actionTypeID, GetRolesAccessByIdQuery, false);
		}

		public virtual bool Check(string securableObjectName, int actionTypeID)
		{
			return Check(securableObjectName, actionTypeID, GetRolesAccessByNameQuery, false);
		}


		public bool Check(Guid securableObjectGUID, int actionTypeID, object predicateContext, bool defaultAccess)
		{
			if (!CheckPredicate(securableObjectGUID, actionTypeID, predicateContext)) return false;
			return Check(securableObjectGUID, actionTypeID, defaultAccess);
		}

		public virtual bool Check(Guid securableObjectGUID, int actionTypeID, bool defaultAccess)
		{
			return Check(securableObjectGUID.ToString(), actionTypeID, GetRolesAccessByIdQuery, defaultAccess);
		}

		public virtual bool Check(string securableObjectName, int actionTypeID, bool defaultAccess)
		{
			return Check(securableObjectName, actionTypeID, GetRolesAccessByNameQuery, defaultAccess);
		}

		bool Check(string securableObject, int actionTypeID, Func<string> query, bool defaultAccess)
		{
			Subject s = Subject.Current;
			if (s == null) return false;
			
			string key = securableObject.ToUpper() + "-" + actionTypeID.ToString();
			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			string accessQuery = String.Format(query(), securableObject, actionTypeID);
			List<int> _access = A.Model.ExecuteQuery<int>(accessQuery).ToList();

			if (_access.Count == 0)
			{
				if (defaultAccess || s.IsAdministrator)
				{
					if (!s.AllowItems.Contains(key))
					{
						lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
					}
					HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: true (default/admin access)" + Environment.NewLine;
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: false (default/admin access)" + Environment.NewLine;
				} 
				return defaultAccess || s.IsAdministrator;
			}

			if (s.Roles.Select(o => o.RoleID).Intersect(_access).Count() > 0)
			{
				if (!s.AllowItems.Contains(key))
				{
					lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
				}
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: true (explicit access)" + Environment.NewLine;
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
				}
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: false (explicit access denied)" + Environment.NewLine;
				return false;
			}
		}

		public string GetLastMessage()
		{
			return _message;
		}

		protected abstract string GetRolesAccessByIdQuery();
		protected abstract string GetRolesAccessByNameQuery();
		protected abstract string GetItemsIdsQuery();
		protected abstract string GetItemsNamesQuery();
	}

	public abstract class SPMContextBaseCacheable : SPMContextBase
	{
		static Dictionary<string, HashSet<string>> _accessCache = new Dictionary<string, HashSet<string>>();
		static Dictionary<string, HashSet<string>> _itemsCache = new Dictionary<string, HashSet<string>>();

		public static void ResetCache()
		{
			_accessCache.Clear();
			_itemsCache.Clear();
		}

		public override bool Check(Guid securableObjectGUID, int actionTypeID)
		{
			return Check(securableObjectGUID.ToString(), actionTypeID, _className + "-byid", GetRolesAccessByIdQuery, GetItemsIdsQuery, false);
		}

		public override bool Check(string securableObjectName, int actionTypeID)
		{
			return Check(securableObjectName, actionTypeID, _className + "-byname", GetRolesAccessByNameQuery, GetItemsNamesQuery, false);
		}

		public override bool Check(Guid securableObjectGUID, int actionTypeID, bool defaultAccess)
		{
			return Check(securableObjectGUID.ToString(), actionTypeID, _className + "-byid", GetRolesAccessByIdQuery, GetItemsIdsQuery, defaultAccess);
		}

		public override bool Check(string securableObjectName, int actionTypeID, bool defaultAccess)
		{
			return Check(securableObjectName, actionTypeID, _className + "-byname", GetRolesAccessByNameQuery, GetItemsNamesQuery, defaultAccess);
		}

		public override bool CheckForRole(int roleID, Guid securableObjectGUID, int actionTypeID)
		{
			List<int> anc = Role.FromID(roleID).Ancestors;
			string key = securableObjectGUID.ToString().ToUpper() + "-" + actionTypeID.ToString();

			HashSet<string> _access = null;
			string cacheName = _className + "-byid";

			if (!_accessCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(A.Model.ExecuteQuery<string>(GetRolesAccessByIdQuery()));
					if (!_accessCache.ContainsKey(cacheName)) _accessCache.Add(cacheName, _access);
				}
			}
			else
			{
				_access = _accessCache[cacheName];
			}

			HashSet<string> _checking = new HashSet<string>(anc.Select(o => key + "-" + o.ToString()));
			if (_access.Overlaps(_checking))
			{
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + "ROLE: " + roleID.ToString() + ", " + key + " v2.5: true" + Environment.NewLine;
				return true;
			}
			else
			{
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + "ROLE: " + roleID.ToString() + ", " + key + " v2.5: false" + Environment.NewLine;
				return false;
			}
		}

		bool Check(string securableObject, int actionTypeID, string cacheName, Func<string> rolesAccessQuery, Func<string> itemsQuery, bool defaultAccess)
		{
			string key = securableObject.ToUpper() + "-" + actionTypeID.ToString();

			Subject s = Subject.Current;
			if (s == null) return false;

			if (s.AllowItems.Contains(key)) return true;
			if (s.DisallowItems.Contains(key)) return false;

			HashSet<string> _access = null;
			HashSet<string> _items = null;

			if (!_accessCache.ContainsKey(cacheName))
			{
				lock (_lock)
				{
					_access = new HashSet<string>(A.Model.ExecuteQuery<string>(rolesAccessQuery()));
					_items = new HashSet<string>(A.Model.ExecuteQuery<string>(itemsQuery()));

					if (!_accessCache.ContainsKey(cacheName)) _accessCache.Add(cacheName, _access);
					if (!_itemsCache.ContainsKey(cacheName)) _itemsCache.Add(cacheName, _items);
				}
			}
			else
			{
				_access = _accessCache[cacheName];
				_items = _itemsCache[cacheName];
			}

			if (!_items.Contains(key))
			{
				if (defaultAccess || s.IsAdministrator)
				{
					if (!s.AllowItems.Contains(key))
					{
						lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
					}
					HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: true (default/admin access)" + Environment.NewLine;
				}
				else
				{
					if (!s.DisallowItems.Contains(key))
					{
						lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
					}
					HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: false (default/admin access)" + Environment.NewLine;
				}
				return defaultAccess || s.IsAdministrator;
			}

			HashSet<string> _checking = new HashSet<string>(s.Roles.Select(o => key + "-" + o.RoleID.ToString()));
			if (_access.Overlaps(_checking))
			{
				if (!s.AllowItems.Contains(key))
				{
					lock (_lock) { if (!s.AllowItems.Contains(key)) s.AllowItems.Add(key); }
				}
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: true (explicit access)" + Environment.NewLine;
				return true;
			}
			else
			{
				if (!s.DisallowItems.Contains(key))
				{
					lock (_lock) { if (!s.DisallowItems.Contains(key)) s.DisallowItems.Add(key); }
				}
				HttpContext.Current.Items["SpmLog"] = (string)HttpContext.Current.Items["SpmLog"] + key + " v2.5: false (explicit access denied)" + Environment.NewLine;
				return false;
			}
		}
	}

	public class ActionSPMContext : SPMContextBaseCacheable
	{
		private ActionSPMContext()
		{
			_className = "Action";
		}

		static ActionSPMContext _current = new ActionSPMContext();
		public static ActionSPMContext Current
		{
			get { return _current; }
		}

		protected override string GetRolesAccessByIdQuery()
		{
			return @"select convert(varchar(36), a.ItemGUID) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, SPM_RoleAccess ra
				where ra.ActionID = a.ActionID and a.ItemGUID is not null";
		}

		protected override string GetRolesAccessByNameQuery()
		{
			return @"select upper(pa.SystemName + '.' + a.SystemName) + '-1-' + convert(varchar, ra.RoleID)
				from SPM_Action a, 
				SPM_ActionAsso asso,
				SPM_Action pa,
				SPM_ActionAsso asso2,
				SPM_Action roota,
				SPM_RoleAccess ra
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID and
				ra.ActionID = a.ActionID
				order by pa.SystemName + '.' + a.SystemName";
		}

		protected override string GetItemsIdsQuery()
		{
			return @"select convert(varchar(36), a.ItemGUID) + '-1'
				from SPM_Action a
				where a.ItemGUID is not null";
		}

		protected override string GetItemsNamesQuery()
		{
			return @"select upper(pa.SystemName + '.' + a.SystemName) + '-1'
				from SPM_Action a, 
				SPM_ActionAsso asso,
				SPM_Action pa,
				SPM_ActionAsso asso2,
				SPM_Action roota
				where a.ActionID = asso.ActionID and pa.ActionID = asso.ParentActionID and
				pa.ActionID = asso2.ActionID and roota.ActionID = asso2.ParentActionID 
				order by pa.SystemName + '.' + a.SystemName";
		}
	}

	public class N_FolderSPMContext : SPMContextBase
	{
		private N_FolderSPMContext()
		{
			_className = "N_Folder";
		}

		static N_FolderSPMContext _current = new N_FolderSPMContext();
		public static N_FolderSPMContext Current
		{
			get { return _current; }
		}

		protected override string GetRolesAccessByIdQuery()
		{
			return @"select ra.RoleID
				from SPM_Action a, SPM_RoleAccess ra, MM_ObjectType t
				where ra.ActionID = a.ActionID and a.ItemGUID is not null and a.ClassGUID = t.Guid and t.SysName = 'N_Folder'
				and a.ActionTypeID = {1} and a.ItemGUID = '{0}'";
		}

		protected override string GetRolesAccessByNameQuery()
		{
			throw new NotImplementedException();
		}

		protected override string GetItemsIdsQuery()
		{
			throw new NotImplementedException();
		}

		protected override string GetItemsNamesQuery()
		{
			throw new NotImplementedException();
		}
	}
}