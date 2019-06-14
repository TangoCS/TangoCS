using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Tango.Data
{
	public interface IDatabase
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; set; }

		IRepository<T> Repository<T>();

		IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified);
	}

	public interface IRepository<T>
	{
		string AllObjectsQuery { get; set; }
		string Table { get; }
		IDictionary<string, object> Parameters { get; }

		T GetById(object id);
		bool Exists(object id);

		int Count(Expression predicate = null);
		IEnumerable<T> List(Expression predicate = null);

		void Create(T entity);
		object CreateFrom(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate);

		void Update(T entity);
		void Update(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate);
		void Update<TKey>(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids);

		void Delete(Expression<Func<T, bool>> predicate);
		void Delete<TKey>(IEnumerable<TKey> ids);
	}

	public class IdentityAttribute : Attribute
	{
	}

	public class ComputedAttribute : Attribute
	{
	}

	public static class RepositoryExtensions
	{
		public static IRepository<T> WithAllObjectsQuery<T>(this IRepository<T> rep, string allObjectsQuery, object parameters = null)
		{
			rep.AllObjectsQuery = allObjectsQuery;
			if (parameters != null)
				foreach (var p in parameters.GetType().GetProperties())
					rep.Parameters.Add(p.Name, p.GetValue(parameters));
			return rep;
		}
	}
}
