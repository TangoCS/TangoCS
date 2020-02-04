using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Tango.Data;
using Tango.Identity;

namespace Tango.UI.Controls
{
	public abstract class AbstractPersistentFilter<TKey> : IPersistentFilter<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		IPersistentFilterStore<TKey> _storage;
		IPersistentFilterEntity<TKey> _filter;
		List<IPersistentFilterEntity<TKey>> _views;

		//public bool editMode = false;

		public AbstractPersistentFilter(IPersistentFilterStore<TKey> storage)
		{
			_storage = storage;
		}

		public bool Load(TKey? id, string listName, string listParms)
		{
			if (_filter == null || !_filter.ID.Equals(id))
			{
				if (id.HasValue)
					_filter = _storage.GetById(id.Value);
				else
					_filter = _storage.GetDefault(listName, listParms);
			}
			return _filter != null;
		}

		public IEnumerable<(TKey ID, string Name, bool IsDefault)> GetViews(string listName, IReadOnlyDictionary<string, object> listParms)
		{
			if (_views == null) LoadViews(listName, listParms);
			return _views.OrderBy(o => o.FilterName).Select(o => (o.ID, o.FilterName, o.IsDefault));
		}

		void LoadViews(string listName, IReadOnlyDictionary<string, object> listParms)
		{
			_views = _storage.GetViews(listName);

			int i = 0;
			while (i < _views.Count())
			{
				bool del = false;
				var v = _views.ElementAt(i);
				if (!v.ListParms.IsEmpty())
				{
					string[] pa = v.ListParms.Split(new char[] { ',' });
					foreach (string p in pa)
					{
						string[] cv = p.Split(new char[] { '=' });
						if (cv.Length == 2)
						{
							if (listParms.Parse<string>(cv[0]).ToLower() != cv[1].ToLower())
							{
								_views.RemoveAt(i);
								del = true;
							}
						}
					}
				}
				if (!del) i++;
			}
		}

		List<FilterItem> _items;
		public List<FilterItem> Criteria
		{
			get 
			{
				if (_items == null)
				{
					_items = Filter.FilterValue != null ? 
						XmlHelper.Deserialize<List<FilterItem>>(Filter.FilterValue.Root) : 
						new List<FilterItem>();
				}
				return _items; 
			}
			set
			{
				_items = value;
			}
		}

		IPersistentFilterEntity<TKey> Filter
		{
			get
			{
				if (_filter == null)
					_filter = _storage.CreateNew();

				return _filter;
			}
		}

		public TKey ID => Filter.ID;
		public bool IsShared => Filter.IsShared;

		public string Name
		{
			get { return Filter.FilterName; }
			set { Filter.FilterName = value; }
		}

		public string ListParms
		{
			get { return Filter.ListParms; }
			set { Filter.ListParms = value; }
		}

		//public string Columns
		//{
		//	get { return Filter.Columns; }
		//	set { Filter.Columns = value; }
		//}

		//public string Sort
		//{
		//	get { return Filter.Sort; }
		//	set { Filter.Sort = value; }
		//}

		//public int ItemsOnPage
		//{
		//	get { return Filter.ItemsOnPage < 1 ? 50 : Filter.ItemsOnPage; }
		//	set { Filter.ItemsOnPage = value; }
		//}

		public string ListName
		{
			get { return Filter.ListName; }
			set { Filter.ListName = value; }
		}

		public bool IsDefault
		{
			get { return Filter.IsDefault; }
			set { Filter.IsDefault = value; }
		}

		//public int? SubjectID
		//{
		//	get { return Filter.SubjectID; }
		//	set { Filter.SubjectID = value; }
		//}

		//#region Группировки
		//public string Group1Sort
		//{
		//	get { return Filter.Group1Sort; }
		//	set { Filter.Group1Sort = value; }
		//}

		//public string Group2Sort
		//{
		//	get { return Filter.Group2Sort; }
		//	set { Filter.Group2Sort = value; }
		//}

		//public int? Group1Column
		//{
		//	get { return Filter.Group1Column; }
		//	set { Filter.Group1Column = value; }
		//}

		//public int? Group2Column
		//{
		//	get { return Filter.Group2Column; }
		//	set { Filter.Group2Column = value; }
		//}
		//#endregion

		public void SaveCriteria()
		{
			Filter.FilterValue = new XDocument(XmlHelper.Serialize(Criteria));
			_storage.SubmitChanges(Filter);
		}

		public void SaveView(string name, bool isShared, bool isDefault, string listName, IReadOnlyDictionary<string, object> listParms)
		{
			Filter.FilterValue = new XDocument(XmlHelper.Serialize(Criteria));
			Filter.FilterName = name;
			Filter.ListName = listName;
			Filter.ListParms = listParms?.Select(kv => kv.Key + "=" + kv.Value).Join("&");
			Filter.IsDefault = isDefault;

			if (isDefault)
			{
				if (_views == null)
					LoadViews(listName, listParms);

				foreach (var view in _views.Where(o => !o.ID.Equals(_filter.ID)))
					view.IsDefault = false;
			}

			_storage.SubmitChanges(Filter, isShared);
		}

		public void InsertOnSubmit()
		{
			var f = _storage.CreateNew();
			f.FilterValue = _filter.FilterValue;
			_filter = f;
		}
	}

	public class PersistentFilterStore : IPersistentFilterStore<int>
	{
		IDatabase _database;
		IUserIdAccessor<int> _users;

		public PersistentFilterStore(IDatabase database, IUserIdAccessor<int> users)
		{
			_database = database;
			_users = users;
		}

		public IPersistentFilterEntity<int> GetById(int id)
		{
			return _database.Repository<N_Filter>().GetById(id);
		}


		public IPersistentFilterEntity<int> GetDefault(string listName, string listParms)
		{
			if (!listParms.IsEmpty())
				return _database.Connection.QuerySingleOrDefault<N_Filter>(@"
select * 
from n_filter 
where (subjectid is null or subjectid = @subjectid) and isdefault = @isdefault 
and lower(listname) = @listname and listparms = @listparms
", new { subjectid = _users.CurrentUserID, listName = listName.ToLower(), listParms, isdefault = true });
			else
				return _database.Connection.QuerySingleOrDefault<N_Filter>(@"
select * 
from n_filter 
where (subjectid is null or subjectid = @subjectid) and isdefault = @isdefault 
and lower(listname) = @listname
", new { subjectid = _users.CurrentUserID, listName = listName.ToLower(), isdefault = true });
		}

		public List<IPersistentFilterEntity<int>> GetViews(string listName)
		{
			return _database.Connection.Query<N_Filter>(@"
select * 
from n_filter 
where (subjectid is null or subjectid = @subjectid) and filtername is not null
and lower(listname) = @listname
", new { subjectid = _users.CurrentUserID, listName = listName.ToLower() }).Select(o => o as IPersistentFilterEntity<int>).ToList();
		}

		public void SubmitChanges(IPersistentFilterEntity<int> entity, bool? isShared = null)
		{
			if (!(entity is N_Filter)) return;
			var filter = entity as N_Filter;

			if (isShared.HasValue && !isShared.Value)
				filter.SubjectID = _users.CurrentUserID;

			if (entity.ID.Equals(default))
				_database.Repository<N_Filter>().Create(filter);
			else
				_database.Repository<N_Filter>().Update(filter);
		}

		public IPersistentFilterEntity<int> CreateNew()
		{
			return new N_Filter();
		}
	}

	public class PersistentFilter : AbstractPersistentFilter<int>
	{
		public PersistentFilter(IPersistentFilterStore<int> storage) : base(storage)
		{
		}
	}

	public interface IPersistentFilterStore<TKey>
	{
		IPersistentFilterEntity<TKey> GetById(TKey id);
		IPersistentFilterEntity<TKey> GetDefault(string listName, string listParms);
		List<IPersistentFilterEntity<TKey>> GetViews(string listName);
		void SubmitChanges(IPersistentFilterEntity<TKey> entity, bool? isShared = null);
		IPersistentFilterEntity<TKey> CreateNew();
	}

	[Table("N_Filter")]
	public partial class N_Filter : IEntity, IWithKey<N_Filter, int>, IPersistentFilterEntity<int>
	{
		public virtual Expression<Func<N_Filter, bool>> KeySelector(int id)
		{
			return o => o.FilterID == id;
		}
		public virtual int ID { get { return FilterID; } }
		public virtual bool IsShared => SubjectID == null;

		[Key]
		[Identity]
		[Column]
		public virtual int FilterID { get; set; }
		[Column]
		public virtual string ListName { get; set; }
		[Column]
		public virtual XDocument FilterValue { get; set; }
		[Column]
		public virtual string FilterName { get; set; }
		[Column]
		public virtual bool IsDefault { get; set; }
		[Column]
		public virtual int? Group1Column { get; set; }
		[Column]
		public virtual string Group1Sort { get; set; }
		[Column]
		public virtual int? Group2Column { get; set; }
		[Column]
		public virtual string Group2Sort { get; set; }
		[Column]
		public virtual string ListParms { get; set; }
		[Column]
		public virtual string Columns { get; set; }
		[Column]
		public virtual string Sort { get; set; }
		[Column]
		public virtual int ItemsOnPage { get; set; }
		[Column]
		public virtual int? SubjectID { get; set; }
	}
}
