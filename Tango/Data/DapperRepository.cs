using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tango.Data
{
	public class Store
	{
		public DBType DBType
		{
			get
			{
				switch (Connection.GetType().Name)
				{
					case "NpgsqlConnection":
						return DBType.POSTGRESQL;
					case "SqlConnection":
						return DBType.MSSQL;
					default:
						return DBType.POSTGRESQL;
				}
			}
		}

		public IDbConnection Connection { get; }
		public IDbTransaction Transaction { get; private set; }

		public Store(IDbConnection connection)
		{
			Connection = connection;
		}

		public IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified)
		{
			if (Connection.State != ConnectionState.Open)
				Connection.Open();
			Transaction = Connection.BeginTransaction(il);
			return Transaction;
		}
	}

	public class DapperDatabase : Store, IDatabase
	{
		public DapperDatabase(IDbConnection connection) : base(connection) { }
		public IRepository<T> Repository<T>() => new DapperRepository<T>(this);
	}

	public class DapperRepository<T> : IRepository<T>
	{
		public string AllObjectsQuery { get; set; }
		public object Parameters { get; set; }

		public IDatabase Database { get; }

		protected Dictionary<string, PropertyInfo> keys = new Dictionary<string, PropertyInfo>();
		protected Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();
		protected Dictionary<string, object> parms = new Dictionary<string, object>();

		public DapperRepository(IDatabase database)
		{
			Database = database;
			AllObjectsQuery = "select * from " + typeof(T).Name.ToLower();

			var props = typeof(T).GetProperties();
			foreach (var p in props)
			{
				if (p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
					keys.Add(p.Name, p);
				if (!p.GetCustomAttributes(typeof(ComputedAttribute), false).Any())
					columns.Add(p.Name, p);
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

		public int Count(Expression predicate = null)
		{
			var query = $"select count(1) from ({AllObjectsQuery}) t";
			var args = new DynamicParameters(Parameters);

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
			var query = AllObjectsQuery;
			var args = new DynamicParameters(Parameters);

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

		protected (string clause, Dictionary<string, object> parms) GetByIdWhereClause(object id)
		{
			var parms = new Dictionary<string, object>();
			if (id.GetType() == typeof(Dictionary<string, object>))
			{
				var keys = id as Dictionary<string, object>;
				int i = 0;
				var clause = keys.Select(k => {
					var s = $"{k.Key.ToLower()} = @p{i}";
					parms.Add($"p{i}", k.Value);
					i++;
					return s;
				}).Join(" and ");
				return (clause, parms);
			}
			else
			{
				var cnt = keys.Count();
				if (cnt == 1)
				{
					parms.Add("p0", id);
					return (keys.Keys.First().ToLower() + " = @p0", parms);
				}
				else if (cnt > 1)
				{
					var fields = id.GetType().GetFields();
					int i = 0;
					var clause = keys.Select(k => {
						var s = $"{k.Key.ToLower()} = @p{i}";
						parms.Add($"p{i}", fields[i].GetValue(id));
						i++;
						return s;
					}).Join(" and ");
					return (clause, parms);
				}
				else
					throw new Exception($"Entity {typeof(T).Name} doesn't contain any key property");
			}
		}

		protected (string clause, Dictionary<string, object> parms) GetByIdsWhereClause<TKey>(IEnumerable<TKey> ids)
		{
			var cnt = keys.Count();
			if (cnt == 1)
			{
				var parms = new Dictionary<string, object> {
					{ "p0", ids }
				};
				return (keys.Keys.First().ToLower() + " = any(@p0)", parms);
			}
			else if (cnt > 1)
				throw new Exception($"Composite keys not supported (entity: {typeof(T).Name}).");
			else
				throw new Exception($"Entity {typeof(T).Name} doesn't contain any key property");
		}

		public T GetById(object id)
		{
			var where = GetByIdWhereClause(id);
			var query = $"select * from {typeof(T).Name.ToLower()} where {where.clause}";

			return Database.Connection.QuerySingleOrDefault<T>(query, where.parms);
		}

		public virtual void Create(T entity)
		{
			var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var cols = new List<string>();
			var vals = new List<string>();
			var parms = new Dictionary<string, object>();
			PropertyInfo identity = null;
			var n = 0;

			foreach (var prop in props)
			{
				if (identity == null)
				{
					var hasIdentity = prop.GetCustomAttributes<IdentityAttribute>().Any();
					identity = hasIdentity ? prop : null;
					if (hasIdentity) continue;
				}

				var val = prop.GetValue(entity);
				if (val != null)
				{
					cols.Add(prop.Name.ToLower());
					vals.Add("@i" + n);
					parms.Add("i" + n, val);
					n++;
				}
			}

			var colsClause = cols.Join(", ");
			var valuesClause = vals.Join(", ");
			var returning = identity == null ? "" : $"returning {identity.Name.ToLower()}";

			var query = $"insert into {typeof(T).Name.ToLower()}({colsClause}) values({valuesClause}) {returning}";

			var ret = Database.Connection.ExecuteScalar(query, parms, Database.Transaction);

			if (identity != null)
				identity.SetValue(entity, ret);
		}

		public void Update(T entity)
		{
			var keyCollection = new Dictionary<string, object>();
			var setCollection = new UpdateSetCollection<T>();

			foreach (var col in columns)
				setCollection.Set(col.Key, col.Value.GetValue(entity));

			foreach (var key in keys)
				keyCollection.Add(key.Key, key.Value.GetValue(entity));

			var where = GetByIdWhereClause(keyCollection);
			var query = $"update {typeof(T).Name.ToLower()} set {setCollection.GetClause()} where {where.clause}";

			foreach (var i in setCollection.GetParms())
				where.parms.Add(i.Key, i.Value);

			Database.Connection.ExecuteScalar(query, where.parms, Database.Transaction);
		}

		public void Update(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate)
		{
			var translator = new QueryTranslator();
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
			var args = new DynamicParameters(translator.Parms);

			var collection = new UpdateSetCollection<T>();
			sets(collection);

			var query = $"update {typeof(T).Name.ToLower()} set {collection.GetClause()} where {translator.WhereClause}";

			foreach (var i in collection.GetParms())
				args.Add(i.Key, i.Value);

			Database.Connection.ExecuteScalar(query, args, Database.Transaction);
		}

		public virtual void Update<TKey>(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids)
		{
			var collection = new UpdateSetCollection<T>();
			sets(collection);

			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			var query = $"update {typeof(T).Name.ToLower()} set {collection.GetClause()} where {where.clause}";

			foreach (var i in collection.GetParms())
				where.parms.Add(i.Key, i.Value);

			Database.Connection.ExecuteScalar(query, where.parms, Database.Transaction);
		}

		public virtual void Delete(Expression<Func<T, bool>> predicate)
		{
			var translator = new QueryTranslator();
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
			var args = new DynamicParameters(translator.Parms);
			var query = $"delete from {typeof(T).Name.ToLower()} where {translator.WhereClause}";

			Database.Connection.ExecuteScalar(query, args, Database.Transaction);
		}

		public virtual void Delete<TKey>(IEnumerable<TKey> ids)
		{
			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			var query = $"delete from {typeof(T).Name.ToLower()} where {where.clause}";

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
				return Set(expr.Member.Name, value);
			return this;
		}

		public UpdateSetCollection<TEntity> Set(string property, object value)
		{
			var n = _sets.Count;
			_sets.Add($"{property.ToLower()} = @u{n}");
			_parms.Add("u" + n.ToString(), value);
			return this;
		}

		public string GetClause() => _sets.Join(", ");
		public Dictionary<string, object> GetParms() => _parms;
	}
}
