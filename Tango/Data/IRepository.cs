using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Tango.Data
{
	public interface IDatabase
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; }

		IReadRepository<T, TKey> ReadRepository<T, TKey>();
		IListRepository<T> ListRepository<T>();
		IEditRepository<T, TKey> EditRepository<T, TKey>();

		IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified);
	}

	public interface IReadRepository<T, TKey>
	{
		T GetById(TKey id);
	}

	public interface IListRepository<T>
	{
		int Count(Expression predicate = null);
		IEnumerable<T> List(Expression predicate = null);
	}

	public interface IEditRepository<T, TKey> : IReadRepository<T, TKey>
	{
		void Create(T entity);
		void Update(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids);
		void Delete(IEnumerable<TKey> ids);
		void Delete(Expression<Func<T, bool>> predicate);
	}

	public class IdentityAttribute : Attribute
	{
	}

	public class ComputedAttribute : Attribute
	{
	}
}
