using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tango.Data
{
	public interface IReadRepository<T, TKey>
		where T : IWithKey<T, TKey>
	{
		T GetById(TKey id);
	}

	public interface IListRepository<T>
	{
		int Count(Expression predicate = null);
		IEnumerable<T> List(Expression predicate = null);
	}

	public interface IEditRepository<T, TKey> : IReadRepository<T, TKey>
		where T : IWithKey<T, TKey>
	{
		void Create(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}
