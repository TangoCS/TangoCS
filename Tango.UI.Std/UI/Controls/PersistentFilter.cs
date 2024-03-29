﻿using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
//using System.Text.Json;
using Newtonsoft.Json;
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

		public bool Load(TKey? id)
		{
			if (_filter == null || !_filter.ID.Equals(id))
				if (id.HasValue)
					_filter = _storage.GetById(id.Value);

			return _filter != null;
		}

		public bool LoadDefault(string listName, Guid? listName_ID)
		{
			_filter = _storage.GetDefault(listName, listName_ID);
			return _filter != null;
		}

		public IEnumerable<(TKey ID, string Name, bool IsDefault, bool IsShared)> GetViews(string listName)
		{
			if (_views == null) _views = _storage.GetViews(listName);
			return _views.OrderBy(o => o.FilterName).Select(o => (o.ID, o.FilterName, o.IsDefault, o.IsShared));
		}

		

		protected abstract string Serialize(IEnumerable<FilterItem> criteria);

		protected abstract List<FilterItem> Deserialize(string value);


		IEnumerable<FilterItem> _items;
		public IEnumerable<FilterItem> Criteria
		{
			get 
			{
				if (_items == null)
				{
					_items = Filter.FilterValue != null ? 
						Deserialize(Filter.FilterValue) : 
						new List<FilterItem>();
				}
				return _items; 
			}
			set
			{
				Filter.FilterValue = Serialize(value);
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

		bool _isNew = false;

		public string Name
		{
			get { return Filter.FilterName; }
			set { Filter.FilterName = value; }
		}

		public string StringValue
		{
			get { return Filter.FilterValue; }
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

		public void SaveView(string name, bool isShared, bool isDefault, string listName, Guid? listName_ID, string columns)
		{
			if (_isNew) _filter = null;

			Filter.FilterValue = Serialize(Criteria);
			Filter.FilterName = name;
			Filter.ListName = listName;
			Filter.ListNameID = listName_ID;
			Filter.IsDefault = isDefault;
			Filter.Columns = columns;

			_storage.SubmitChanges(Filter, isShared);
		}

		public void DeleteView()
		{
			_storage.Delete(ID);
			_filter = null;
		}

		public void InsertOnSubmit()
		{
			_isNew = true;
		}
	}


	public class PersistentFilterStore : IPersistentFilterStore<int>
	{
		IDatabase _database;
		IUserIdAccessor<long> _users;

		public PersistentFilterStore(IDatabase database, IUserIdAccessor<long> users)
		{
			_database = database;
			_users = users;
		}

		public IPersistentFilterEntity<int> GetById(int id)
		{
			return _database.Repository<N_Filter>().GetById(id);
		}

		public void Delete(int id)
		{
			_database.Repository<N_Filter>().Delete(o => o.FilterID.Equals(id));
		}

		public IPersistentFilterEntity<int> GetDefault(string listName, Guid? listNameID)
		{
			return _database.Connection.QuerySingleOrDefault<N_Filter>(@"
select * 
from n_filter 
where (subjectid is null or subjectid = @subjectid) and isdefault = @isdefault 
and ((listnameid is null and lower(listname) = @listname) or (listnameid = @listnameid))
order by subjectid asc nulls last
limit 1
", new { subjectid = _users.CurrentUserID, listName = listName.ToLower(), isdefault = true, listNameID });
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
			if (entity.IsDefault)
			{
				var views = ((isShared ?? false) ?
					_database.Connection.Query<N_Filter>(@"
select * from n_filter 
where subjectid is null and filtername is not null and lower(listname) = @listname
", new { listName = entity.ListName.ToLower() }) :
					_database.Connection.Query<N_Filter>(@"
select * from n_filter 
where subjectid = @subjectid and filtername is not null and lower(listname) = @listname
", new { subjectid = _users.CurrentUserID, listName = entity.ListName.ToLower() }))
					.Select(o => o as IPersistentFilterEntity<int>).ToList();

				foreach (var view in views.Where(o => !o.ID.Equals(entity.ID)))
				{
					view.IsDefault = false;
					SubmitChanges(view, view.IsShared);
				}
			}

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

		protected override List<FilterItem> Deserialize(string value)
		{
			return XmlHelper.Deserialize<List<FilterItem>>(value);
		}

		protected override string Serialize(IEnumerable<FilterItem> criteria)
		{
			return XmlHelper.Serialize(criteria).ToString();
		}
	}

	public class PersistentFilterJson : AbstractPersistentFilter<int>
	{
		public PersistentFilterJson(IPersistentFilterStore<int> storage) : base(storage)
		{
		}

		protected override List<FilterItem> Deserialize(string value)
		{
			try
			{
				return JsonConvert.DeserializeObject<List<FilterItem>>(value);
			}
			catch (Exception)
			{
				return new List<FilterItem>();
			}
		}

		protected override string Serialize(IEnumerable<FilterItem> criteria)
		{
			return JsonConvert.SerializeObject(criteria);
		}
	}

	public interface IPersistentFilterStore<TKey>
	{
		IPersistentFilterEntity<TKey> GetById(TKey id);
		IPersistentFilterEntity<TKey> GetDefault(string listName, Guid? listName_ID);
		List<IPersistentFilterEntity<TKey>> GetViews(string listName);
		void SubmitChanges(IPersistentFilterEntity<TKey> entity, bool? isShared = null);
		IPersistentFilterEntity<TKey> CreateNew();
		void Delete(TKey id);
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
		public virtual string FilterValue { get; set; }
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
		public virtual long? SubjectID { get; set; }
		[Column]
		public virtual Guid? ListNameID { get; set; }
	}
}
