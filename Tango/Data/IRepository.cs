using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Data
{
	public interface IAbstractDatabase
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; set; }
		IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified);
	}

	public interface IDatabase : IAbstractDatabase
	{
		IRepository<T> Repository<T>();
		IRepository Repository(Type type);
	}

	public interface IRepository
	{
		IDatabase Database { get; }

		string AllObjectsQuery { get; set; }
		string Table { get; }
		IDictionary<string, object> Parameters { get; }

		bool Exists(object id);
		object GetById(object id);
		WhereClauseResult GetByIdWhereClause(object id);
		int Count(Expression predicate = null);
	}

	public interface IRepository<T> : IRepository
	{
		new T GetById(object id);
		IEnumerable<T> List(Expression predicate = null, Func<IDictionary<string, object>, T> selector = null);
		void Create(T entity);
		CreateQueryResult GetCreateQuery(T entity, Dictionary<string, string> replaceProps = null);
		object CreateFrom(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate);

		void Update(T entity);
		string GetUpdateQuery(T entity);
		void Update(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate);
		void Update<TKey>(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids);

		void Delete(Expression<Func<T, bool>> predicate);
		void Delete<TKey>(IEnumerable<TKey> ids);

		bool Any(Expression<Func<T, bool>> predicate);

	}

	public interface IDeleteStrategy<T>
	{
		string GetDeleteQuery<TKey>(EntityInfo entityInfo, IEnumerable<TKey> ids);
		
		string GetDeleteQuery(EntityInfo entityInfo, Expression<Func<T, bool>> predicate);

		int? CommandTimeout { get; }
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

		public static IRepository<T> WithAllObjectsQueryRes<T>(this IRepository<T> rep, string resourceName, object parameters = null)
		{
			return rep.WithAllObjectsQuery(EmbeddedResourceManager.GetString(typeof(T), resourceName), parameters);
		}

		public static TRep GetRepository<TRep, T>(IServiceProvider serviceProvider, IDatabase database)
		{
			var rep = serviceProvider.GetService(typeof(TRep));
			if (rep != null)
				return (TRep) rep;
			
			var baseRep = database.Repository<T>();
			if(baseRep is TRep repository)
				return repository;

			return default;
		}

		public static IEnumerable<T> List<T>(this IRepository<T> rep, Expression<Func<T,bool>> predicate)
		{
			return rep.List(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
		}
	}
}
