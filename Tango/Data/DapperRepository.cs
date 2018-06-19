using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Data
{
	public class DapperDatabase : IDatabase
	{
		public DapperDatabase(IDbConnection connection)
		{
			Connection = connection;
		}

		public IDbConnection Connection { get; }
		public IDbTransaction Transaction { get; private set; }
		
		public IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified)
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();
			Transaction = Connection.BeginTransaction(il);
			return Transaction;
		}

		public IEditRepository<T, TKey> EditRepository<T, TKey>() => new DapperEditRepository<T, TKey> { Database = this };
		public IListRepository<T> ListRepository<T>() => new DapperListRepository<T> { Database = this };
		public IReadRepository<T, TKey> ReadRepository<T, TKey>() => new DapperReadRepository<T, TKey> { Database = this };
	}

	public class DapperListRepository<T> : IListRepository<T>
	{
		protected virtual string Query => "select * from " + typeof(T).Name.ToLower();

		public IDatabase Database { get; set; }

		public int Count(Expression predicate = null)
		{
			var query = $"select count(1) from ({Query}) t";
			var args = new DynamicParameters();

			if (predicate != null)
			{
				var translator = new QueryTranslator();
				translator.Translate(predicate);
				if (!translator.WhereClause.IsEmpty()) query += " where " + translator.WhereClause;

				foreach (var pair in translator.Parms)
					args.Add(pair.Key, pair.Value);
			}

			return Database.Connection.QuerySingle<int>(query, args);
		}

		public IEnumerable<T> List(Expression predicate = null)
		{
			var query = Query;
			var args = new DynamicParameters();
			

			if (predicate != null)
			{
				query = $"select * from ({query}) t";
				var translator = new QueryTranslator();
				translator.Translate(predicate);
				if (!translator.WhereClause.IsEmpty()) query += " where " + translator.WhereClause;
				if (!translator.OrderBy.IsEmpty()) query += " order by " + translator.OrderBy;
				if (translator.Parms.ContainsKey("take")) query += " limit @take";
				if (translator.Parms.ContainsKey("skip")) query += " offset @skip";

				foreach (var pair in translator.Parms)
					args.Add(pair.Key, pair.Value);
			}

			if (typeof(T) == typeof(ExpandoObject))
				return Database.Connection.Query(query, args).Select(x => {
					var dapperRowProperties = x as IDictionary<string, object>;
					IDictionary<string, object> expando = new ExpandoObject();

					foreach (KeyValuePair<string, object> property in dapperRowProperties)
						expando.Add(property.Key, property.Value);

					return (T)expando;
				});
			else
				return Database.Connection.Query<T>(query, args);
		}
	}

	public class DapperReadRepository<T, TKey> : IReadRepository<T, TKey>
	{
		public IDatabase Database { get; set; }

		protected List<string> keys = new List<string>();
		protected List<string> columns = new List<string>();
		protected Dictionary<string, object> parms = new Dictionary<string, object>();
		//protected TKey keyValue = default(TKey);
		//Expression<Func<T, bool>> keySelector = null;

		public DapperReadRepository()
		{
			var props = typeof(T).GetProperties();
			foreach (var p in props)
			{
				if (p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
					keys.Add(p.Name);
				if (!p.GetCustomAttributes(typeof(ComputedAttribute), false).Any())
					columns.Add(p.Name);
			}

			//var paramExp = Expression.Parameter(typeof(T), "o");

			//Expression whereExpr = null;
			//foreach (var key in keys)
			//{
			//	var leftExp = Expression.Property(paramExp, key);
			//	var rightExp = Expression.Field(Expression.Constant(this), "keyValue");
			//	if (whereExpr == null)
			//		whereExpr = Expression.Equal(leftExp, rightExp);
			//	else
			//		whereExpr = Expression.AndAlso(whereExpr, Expression.Equal(leftExp, rightExp));
			//}

			//keySelector = Expression.Lambda<Func<T, bool>>(whereExpr, paramExp);
		}

		protected (string clause, Dictionary<string, object> parms) GetByIdWhereClause(TKey id)
		{
			var parms = new Dictionary<string, object>();
			var cnt = keys.Count();
			if (cnt == 1)
			{
				parms.Add("p0", id);
				return (keys.First().ToLower() + " = @p0", parms);
			}
			else if (cnt > 1)
			{
				var fields = typeof(TKey).GetFields();
				int i = 0;
				var clause = keys.Select(k => {
					var s = $"{k.ToLower()} = p{i}";
					parms.Add($"p{i}", fields[i].GetValue(id));
					i++;
					return s;
				}).Join(" and ");
				return (clause, parms);
			}
			else
				throw new Exception($"Entity {typeof(T).Name} doesn't contain any key property");
		}

		protected (string clause, Dictionary<string, object> parms) GetByIdsWhereClause(IEnumerable<TKey> ids)
		{
			var cnt = keys.Count();
			if (cnt == 1)
			{
				var parms = new Dictionary<string, object> {
					{ "p0", ids }
				};
				return (keys.First().ToLower() + " = any(@p0)", parms);
			}
			else if (cnt > 1)
				throw new Exception($"Composite keys not supported (entity: {typeof(T).Name}).");
			else
				throw new Exception($"Entity {typeof(T).Name} doesn't contain any key property");
		}

		public T GetById(TKey id)
		{
			//SqlProperties = props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any()).Select(p => new SqlPropertyMetadata(p)).ToArray();
			//var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
			//IdentitySqlProperty = identityProperty != null ? new SqlPropertyMetadata(identityProperty) : null;

			//keyValue = id;

			//var translator = new QueryTranslator();
			//var predicate = Enumerable.Empty<T>().AsQueryable().Where(keySelector).Expression;
			//translator.Translate(predicate);

			var query = "select * from " + typeof(T).Name.ToLower();
			var where = GetByIdWhereClause(id);
			query += " where " + where.clause; //translator.WhereClause;

			//var args = new DynamicParameters(translator.Parms);

			return Database.Connection.QuerySingleOrDefault<T>(query, where.parms);
		}
	}

	public class DapperEditRepository<T, TKey> : DapperReadRepository<T, TKey>, IEditRepository<T, TKey>
	{
		public virtual void Create(T entity)
		{
			throw new NotImplementedException();
		}

		public virtual void Delete(IEnumerable<TKey> ids)
		{
			var query = "delete from " + typeof(T).Name.ToLower();
			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			query += " where " + where.clause;

			Database.Connection.ExecuteScalar(query, where.parms, Database.Transaction);
		}

		public virtual void Delete(Expression<Func<T, bool>> predicate)
		{
			var query = "delete from " + typeof(T).Name.ToLower();
			var translator = new QueryTranslator();
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
			var args = new DynamicParameters(translator.Parms);
			query += " where " + translator.WhereClause;

			Database.Connection.ExecuteScalar(query, args, Database.Transaction);
		}

		public virtual void Update(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids)
		{
			var collection = new UpdateSetCollection<T>();
			sets(collection);

			var query = "update " + typeof(T).Name.ToLower() + " set " + collection.GetClause();
			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			query += " where " + where.clause;

			foreach (var i in collection.GetParms())
				where.parms.Add(i.Key, i.Value);

			Database.Connection.ExecuteScalar(query, where.parms, Database.Transaction);
		}
	}

	public class UpdateSetCollection<TEntity>
	{
		List<string> _sets = new List<string>();
		Dictionary<string, object> _parms = new Dictionary<string, object>();

		public UpdateSetCollection<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> property, TValue value)
		{
			if (property.Body is MemberExpression expr)
			{
				var n = _sets.Count;
				_sets.Add($"{expr.Member.Name.ToLower()} = @u{n}");
				_parms.Add("u" + n.ToString(), value);
			}
			return this;
		}

		public string GetClause() => _sets.Join(", ");
		public Dictionary<string, object> GetParms() => _parms;
	}
}
