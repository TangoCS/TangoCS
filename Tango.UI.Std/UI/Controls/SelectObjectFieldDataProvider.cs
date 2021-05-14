using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.UI.Controls
{
	public interface ISelectObjectFieldDataProvider<TRef>
	{
		int GetCount();
		IEnumerable<TRef> GetData(Paging paging);
		IEnumerable<TRef> GetAllData();
		TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate);
		IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate);
	}

	public class ORMSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		ISelectObjectField<TRef> field;

		public ORMSelectObjectFieldDataProvider(ISelectObjectField<TRef> field)
		{
			this.field = field;
		}

		public IEnumerable<TRef> GetAllData()
		{
			return field.AllObjects;
		}

		public int GetCount()
		{
			return field.ItemsCountQuery().Count();
		}

		public IEnumerable<TRef> GetData(Paging paging)
		{
			return field.DataQuery(paging);
		}

		public TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate)
		{
			return field.AllObjects.Where(predicate).FirstOrDefault();
		}

		public IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate)
		{
			return field.AllObjects.Where(predicate).ToList();
		}
	}

	public class DapperSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		ISelectObjectField<TRef> field;
		IDatabase database;

		public string AllObjectsSql { get; set; }
		public object AllObjectsSqlParms { get; set; }
		public Func<IQueryable<TRef>, IQueryable<TRef>> PostProcessing { get; set; }

		public IRepository<TRef> Repository => database.Repository<TRef>().WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);

		public DapperSelectObjectFieldDataProvider(ISelectObjectField<TRef> field, IDatabase database)
		{
			this.field = field;
			this.database = database;
			if (field.AllObjects == null) field.AllObjects = Enumerable.Empty<TRef>().AsQueryable();
		}

		public IEnumerable<TRef> GetAllData()
		{
			var q = field.AllObjects;
			if (PostProcessing != null) q = PostProcessing(q);
			return Repository.List(q.Expression);
		}

		public int GetCount()
		{
			var q = field.ItemsCountQuery();
			return Repository.Count(q.Expression);
		}

		public IEnumerable<TRef> GetData(Paging paging)
		{
			var q = field.DataQuery(paging);
			if (PostProcessing != null) q = PostProcessing(q);
			return Repository.List(q.Expression);
		}

		public TRef GetObjectByID<T>(T id, Expression<Func<TRef, bool>> predicate)
		{
			var q = field.AllObjects.Where(predicate);
			return Repository.List(q.Expression).FirstOrDefault();
		}

		public IEnumerable<TRef> GetObjectsByID<T>(IEnumerable<T> id, Expression<Func<TRef, bool>> predicate)
		{
			var q = field.AllObjects.Where(predicate);
			return Repository.List(q.Expression);
		}
	}
}
