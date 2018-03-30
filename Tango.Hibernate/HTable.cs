using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Hibernate
{
	public class HTable<T> : IQueryable<T>
		where T: class
	{
		HDataContext _dataContext;
		IQueryable<T> _query;

		public HTable(HDataContext dataContext, IQueryable<T> query)
		{
			_dataContext = dataContext;
			_query = query;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _query.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _query.GetEnumerator();
		}

		public Type ElementType
		{
			get { return _query.ElementType; }
		}

		public System.Linq.Expressions.Expression Expression
		{
			get { return _query.Expression; }
		}

		public IQueryProvider Provider
		{
			get { return _query.Provider; }
		}

		public void InsertOnSubmit(T obj, int? index = null)
		{
			_dataContext.InsertOnSubmit(obj, index);
		}

		public void DeleteOnSubmit(T obj, int? index = null)
		{
			_dataContext.DeleteOnSubmit(obj, index);
		}

		public void DeleteAllOnSubmit(IQueryable<T> objs, int? index = null)
		{
			_dataContext.DeleteAllOnSubmit(objs, index);
		}
	}
}
