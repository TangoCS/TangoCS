﻿using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tango.Data
{
	public class Store
	{
		public virtual IDbConnection Connection { get; }
		public virtual IDbTransaction Transaction { get; set; }

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

	public static class StoreExtensions
	{
		public static DBType GetDBType(this IAbstractDatabase db)
		{
			return GetDBTypeByConnectionName(db.Connection.GetType().Name);
		}

		public static DBType GetDBTypeByConnectionName(string cName)
		{
			switch (cName)
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

	public class DapperDatabase : Store, IDatabase
	{
		private IServiceProvider provider;
		
		public DapperDatabase(IDbConnection connection, IServiceProvider provider) : base(connection)
		{
			this.provider = provider;
		}
		public IRepository<T> Repository<T>() => new DapperRepository<T>(this, this.provider);
		public IRepository Repository(Type type) => new DapperRepository(this, type);
	}

	public static class ConnectionExtensions
	{
		public static void InitDbConventions(this IDbConnection dbConnection, Type type)
		{
			var baseNaming = type.GetCustomAttribute<BaseNamingConventionsAttribute>();
			if (SqlMapper.GetTypeMap(type) is DefaultTypeMap && baseNaming != null)
			{
				var custom = new CustomPropertyTypeMap(type, QueryHelper.GetPropertyByName);
				SqlMapper.SetTypeMap(type, custom);
			}
		} 
		public static void InitDbConventions<T>(this IDbConnection dbConnection)
		{
			var type = typeof(T);
			dbConnection.InitDbConventions(type);
		}
	}

	public class DapperRepository : IRepository
	{
		public IDatabase Database { get; }
		protected Type Type;
		public string Table { get; }
		protected DBType DBType { get; }
		public virtual string AllObjectsQuery { get; set; }
		protected Dictionary<string, PropertyInfo> keys = new Dictionary<string, PropertyInfo>();
		protected Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();
		public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

		protected string noKeyMessage => $"Entity {Type.Name} doesn't contain any key property";
		protected string incorrectKeyMessage => $"Incorrect key format";

		protected IQueryTranslatorDialect Dialect => QueryHelper.CreateDialect(DBType);

		public DapperRepository(IDatabase database, Type type)
		{
			Database = database;
			Type = type;
			Table = Type.GetCustomAttribute<TableAttribute>()?.Name ?? Type.Name;

			var baseNaming = type.GetCustomAttribute<BaseNamingConventionsAttribute>();
			if (baseNaming != null)
			{
				var basePrefix = BaseNamingConventions.EntityPrefix[baseNaming.Category]?.ToLower() ?? "";
				var dbPrefix = DBConventions.EntityPrefix[baseNaming.Category] ?? "";

				if (Table.ToLower().StartsWith(basePrefix))
					Table = $"{dbPrefix}{Table.Substring(basePrefix.Length)}";
			}

			if (Table.ToLower().EndsWith(".sql"))
			{
				Table = $"({EmbeddedResourceManager.GetString(Type, Table)})";
				AllObjectsQuery = Table;
			}
			else
				AllObjectsQuery = "select * from " + Table;
			
			DBType = database.GetDBType();
			
			database.Connection.InitDbConventions(type);

			var props = Type.GetProperties().Where(o => o.GetCustomAttribute<ColumnAttribute>() != null);
			foreach (var p in props)
			{
				var name = baseNaming != null ? QueryHelper.GetPropertyName(p) : p.Name;

				if (p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
					keys.Add(name, p);
				if (!p.GetCustomAttributes(typeof(ComputedAttribute), false).Any() &&
				    !p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
				{
					//if(p.GetCustomAttributes(typeof())) - если поле содержит NonID - игнорировать
					
					columns.Add(name, p);
				}
			}
		}

		

		public bool Exists(object id)
		{
			var where = GetByIdWhereClause(id);
			var query = $"select 1 from ({AllObjectsQuery}) t where {where.Clause}";

			return Database.Connection.QuerySingleOrDefault<int>(query, where.Parms, Database.Transaction) == 1;
		}

		public WhereClauseResult GetByIdWhereClause(object id)
		{
			var parms = new Dictionary<string, object>();
			var idtype = id.GetType();

			if (idtype == typeof(Dictionary<string, object>))
			{
				var ids = id as Dictionary<string, object>;
				int i = 0;
				var clause = ids.Select(k => {
					var s = $"{k.Key.ToLower()} = @p{i}";
					parms.Add($"p{i}", k.Value);
					i++;
					return s;
				}).Join(" and ");
				return new WhereClauseResult(clause, parms);
			}
			else if (idtype == typeof(string) || idtype == typeof(Guid) || idtype == typeof(DateTime) ||
				(idtype.IsValueType && idtype.IsPrimitive))
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				parms.Add("p0", id);
				return new WhereClauseResult(keys.Keys.First().ToLower() + " = @p0", parms);
			}
			#if NET
			else if (id is ITuple)
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				var fields = idtype.GetFields();
				int i = 0;
				var clause = keys.Select(k => {
					var s = $"{k.Key.ToLower()} = @p{i}";
					parms.Add($"p{i}", fields[i].GetValue(id));
					i++;
					return s;
				}).Join(" and ");
				return new WhereClauseResult(clause, parms);
			}
			#endif
			else if (idtype.IsValueType)
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				var props = idtype.GetProperties();
				int i = 0;
				var clause = keys.Select(k => {
					var s = $"{k.Key.ToLower()} = @p{i}";
					parms.Add($"p{i}", props[i].GetValue(id));
					i++;
					return s;
				}).Join(" and ");
				return new WhereClauseResult(clause, parms);
			}
			else
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				var props = idtype.GetProperties();
				if (props.Count() == 0) throw new Exception(incorrectKeyMessage);
				int i = 0;
				var clause = props.Where(p => keys.ContainsKey(p.Name)).Select(p => {
					var s = $"{p.Name.ToLower()} = @p{i}";
					parms.Add($"p{i}", p.GetValue(id));
					i++;
					return s;
				}).Join(" and ");
				return new WhereClauseResult(clause, parms);
			}
		}



		public virtual int Count(Expression predicate = null)
		{
			var query = "";
			var args = new DynamicParameters();

			foreach (var pair in Parameters)
				args.Add(pair.Key, pair.Value);

			if (predicate != null)
			{
				var (q, a) = QueryHelper.ApplyExpressionToQuery(QueryHelper.SetNewFieldExpression(AllObjectsQuery, "*"), predicate, Dialect);
				query = q;

				foreach (var pair in a)
					args.Add(pair.Key, pair.Value);
			}
			else
				query = AllObjectsQuery;

			query = QueryHelper.SetNewFieldExpression(query, "count(1)");

			return Database.Connection.QuerySingle<int>(query, args, Database.Transaction);
		}

		public virtual object GetById(object id)
		{
			if (id == null) return null;
			var where = GetByIdWhereClause(id);
			var query = $"select * from ({AllObjectsQuery}) t where {where.Clause}";

			foreach (var pair in Parameters)
				where.Parms.Add(pair.Key, pair.Value);

			return Database.Connection.QuerySingleOrDefault(Type, query, where.Parms, Database.Transaction);
		}
	}

	public class DapperRepository<T> : DapperRepository, IRepository<T>
	{
		//protected Dictionary<string, object> parms = new Dictionary<string, object>();
		private IServiceProvider provider;
		public DapperRepository(IDatabase database, IServiceProvider provider) : base(database, typeof(T))
		{
			this.provider = provider;
		}

		public virtual IEnumerable<T> List(Expression predicate = null, Func<IDictionary<string, object>, T> selector = null)
		{
			var query = AllObjectsQuery;
			var args = new DynamicParameters();

			foreach (var pair in Parameters)
				args.Add(pair.Key, pair.Value);

			if (predicate != null)
			{
				var (q, a) = QueryHelper.ApplyExpressionToQuery(QueryHelper.SetNewFieldExpression(AllObjectsQuery, "*"), predicate, Dialect);
				query = q;

				foreach (var pair in a)
					args.Add(pair.Key, pair.Value);
			}

			if (typeof(T) == typeof(ExpandoObject) && selector == null)
			{
				selector = x => {
					var expando = new ExpandoObject() as IDictionary<string, object>;

					foreach (KeyValuePair<string, object> property in x)
						expando.Add(property.Key, property.Value);

					return (T)expando;
				};
			}

			if (selector != null)
				return Database.Connection.Query(query, args, Database.Transaction).Select(x => selector(x as IDictionary<string, object>));
			else
				return Database.Connection.Query<T>(query, args, Database.Transaction);
		}

		protected WhereClauseResult GetByIdsWhereClause<TKey>(IEnumerable<TKey> ids)
		{
			var cnt = keys.Count();
			if (cnt == 1)
			{
				var parms = new Dictionary<string, object> {
					{ "p0", ids }
				};
				if (DBType == DBType.POSTGRESQL)
					return new WhereClauseResult(keys.Keys.First().ToLower() + " = any(@p0)", parms);
				else
					return new WhereClauseResult(keys.Keys.First().ToLower() + " in @p0", parms);
			}
			else if (cnt > 1)
				throw new Exception($"Composite keys are not supported (entity: {typeof(T).Name}).");
			else
				throw new Exception(noKeyMessage);
		}

		public new T GetById(object id)
		{
			if (id == null) return default;
			return (T)base.GetById(id);
		}

		public virtual CreateQueryResult GetCreateQuery(T entity, Dictionary<string, string> replaceProps = null)
		{
			var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(o => o.GetCustomAttribute<ColumnAttribute>() != null);

			var cols = new List<string>();
			var vals = new List<string>();
			PropertyInfo identity = null;
			
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
					var propName = QueryHelper.GetPropertyName(prop).ToLower();
					cols.Add(propName);
					
					if(replaceProps != null && replaceProps.TryGetValue(propName, out string value))
						vals.Add($"{Dialect.VariablePrefix}{value.ToLower()}"); 
					else
						vals.Add(QueryHelper.GetStringValue(val));
				}
			}

			var colsClause = cols.Join(", ");
			var valuesClause = vals.Join(", ");

			var returning = string.Empty;
			var declareVariable = string.Empty;
			var returningIDVariable = string.Empty;
			if (identity != null)
			{
				returningIDVariable = $"{identity.Name.ToLower()}_{Guid.NewGuid()}".Replace("-", "_");
				returning = Dialect.ReturningIdentity(returningIDVariable, returningIDVariable);
				declareVariable = string.Format(Dialect.DeclareVariable, Dialect.VariablePrefix, returningIDVariable, Dialect.GetDBType(identity.PropertyType));
			}

			var query = cols.Count > 1 ? $"{declareVariable} insert into {Table}({colsClause}) values({valuesClause}) {returning}" : string.Format(Dialect.InsertDefault, Table) + " " + returning;
			return new CreateQueryResult { ReturningIDVariable = returningIDVariable, Query = query };
		}

		public virtual void Create(T entity)
		{
			var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(o => o.GetCustomAttribute<ColumnAttribute>() != null);
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
					var name = QueryHelper.GetPropertyName(prop);
					cols.Add(name.ToLower());
					vals.Add("@i" + n);
					parms.Add("i" + n, val);
					n++;
				}
			}

			var colsClause = cols.Join(", ");
			var valuesClause =  vals.Join(", ");
			var returning = identity == null ? "" : Dialect.ReturningIdentity(identity.Name.ToLower(), string.Empty);

			var query = props.Count() > 1 ? $"insert into {Table}({colsClause}) values({valuesClause}) {returning}" : string.Format(Dialect.InsertDefault, Table) + " " + returning;

			var ret = Database.Connection.ExecuteScalar(query, parms, Database.Transaction);

			if (identity != null)
				identity.SetValue(entity, identity.PropertyType == typeof(Int32) ? Convert.ToInt32(ret) : ret);
		}

		public virtual void Update(T entity)
		{
			var keyCollection = new Dictionary<string, object>();
			var setCollection = new UpdateSetCollection<T>();

			foreach (var col in columns)
				setCollection.Set(col.Key, col.Value.GetValue(entity));

			foreach (var key in keys)
				keyCollection.Add(key.Key, key.Value.GetValue(entity));

			var where = GetByIdWhereClause(keyCollection);
			var query = $"update {Table} set {setCollection.GetClause()} where {where.Clause}";

			foreach (var i in setCollection.GetParms())
				where.Parms.Add(i.Key, i.Value);


			Database.Connection.Execute(query, where.Parms, Database.Transaction);
		}

		public string GetUpdateQuery(T entity)
		{
			throw new NotImplementedException();
		}

		public void Update(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate)
		{
			var translator = new QueryTranslator(Dialect);
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
			var args = new DynamicParameters(translator.Parms);

			var collection = new UpdateSetCollection<T>();
			sets(collection);

			var query = $"update {Table} set {collection.GetClause()} where {translator.WhereClause}";

			foreach (var i in collection.GetParms())
				args.Add(i.Key, i.Value);

			Database.Connection.Execute(query, args, Database.Transaction);
		}

		public virtual void Update<TKey>(Action<UpdateSetCollection<T>> sets, IEnumerable<TKey> ids)
		{
			var collection = new UpdateSetCollection<T>();
			sets(collection);

			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			var query = $"update {Table} set {collection.GetClause()} where {where.Clause}";

			foreach (var i in collection.GetParms())
				where.Parms.Add(i.Key, i.Value);

			Database.Connection.Execute(query, where.Parms, Database.Transaction);
		}

		public virtual object CreateFrom(Action<UpdateSetCollection<T>> sets, Expression<Func<T, bool>> predicate)
		{
			var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(o => o.GetCustomAttribute<ColumnAttribute>() != null);
			var insCols = new List<string>();
			var selCols = new List<string>();
			var vals = new List<string>();
			var parms = new Dictionary<string, object>();
			PropertyInfo identity = null;
			var n = 0;

			var collection = new UpdateSetCollection<T>();
			sets(collection);
			var setsCols = collection.GetColumnsWithValues();

			foreach (var prop in props)
			{
				if (identity == null)
				{
					var hasIdentity = prop.GetCustomAttributes<IdentityAttribute>().Any();
					identity = hasIdentity ? prop : null;
					if (hasIdentity) continue;
				}

				var key = prop.Name.ToLower();
				if (setsCols.ContainsKey(key))
				{
					selCols.Add("@i" + n);
					parms.Add("i" + n, setsCols[key]);
					n++;
				}
				else
				{
					selCols.Add(key);
				}
				insCols.Add(key);
			}

			var selColsClause = selCols.Join(", ");
			var insColsClause = insCols.Join(", ");
			var returning = identity == null ? "" : $"returning {identity.Name.ToLower()}";

			var translator = new QueryTranslator(Dialect);
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);

			foreach (var p in translator.Parms)
				parms.Add(p.Key, p.Value);

			var query = $"insert into {Table}({insColsClause}) select {selColsClause} from {Table} where {translator.WhereClause} {returning}";

			var ret = Database.Connection.ExecuteScalar(query, parms, Database.Transaction);

			return identity != null ? ret : null;
		}

		public virtual void Delete(Expression<Func<T, bool>> predicate)
		{
			var strategy = provider.GetService(typeof(IDeleteStrategy<T>)) as IDeleteStrategy<T>;

			var query = string.Empty;
			
			if (strategy != null)
			{
				query = strategy.GetDeleteQuery(new EntityInfo
				{
					Table = Table,
					Dialect = Dialect,
					Keys = keys
					
				}, predicate);
				
				Database.Connection.ExecuteScalar(query, transaction: Database.Transaction);
			}
			else
			{
				var translator = new QueryTranslator(Dialect);
				translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
				var args = new DynamicParameters(translator.Parms);
				query = $"delete from {Table} where {translator.WhereClause}";

				Database.Connection.ExecuteScalar(query, args, Database.Transaction);
			}
			
		}

		public virtual void Delete<TKey>(IEnumerable<TKey> ids)
		{
			var strategy = provider.GetService(typeof(IDeleteStrategy<T>)) as IDeleteStrategy<T>;

			var query = string.Empty;

			if (strategy != null)
			{
				query = strategy.GetDeleteQuery(new EntityInfo
				{
					Table = Table,
					Dialect = Dialect,
					Keys = keys
					
				},ids);

				Database.Connection.ExecuteScalar(query, transaction: Database.Transaction, commandTimeout: strategy.CommandTimeout);
			}
			else
			{
				var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids); 
			
				query = $"delete from {Table} where {where.Clause}";

				Database.Connection.ExecuteScalar(query, where.Parms, Database.Transaction);
			}
		}

		public bool Any(Expression<Func<T, bool>> predicate)
		{
			return Count(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression) > 0;
		}
    }

	public class CreateQueryResult
	{
		public string Query { get; set; } = string.Empty;
		public string ReturningIDVariable { get; set; } = string.Empty;

		public static implicit operator string(CreateQueryResult result) => result.Query;
	}

	public class WhereClauseResult
	{
		public string Clause { get; private set; }
		public Dictionary<string, object> Parms { get; private set; }

		public WhereClauseResult(string clause, Dictionary<string, object> parms)
		{
			Clause = clause;
			Parms = parms;
		}
	}


	public class EntityInfo
	{
		public string Table { get; set; }
		public Dictionary<string, PropertyInfo> Keys { get; set; }
		public IQueryTranslatorDialect Dialect { get; set; }
	}
	public class UpdateSetCollection<TEntity>
	{
		List<string> _columns = new List<string>();
		Dictionary<string, object> _parms = new Dictionary<string, object>();

		public UpdateSetCollection<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> property, TValue value)
		{
			if (property.Body is MemberExpression expr)
				return Set(expr.Member.Name, value);
			return this;
		}

		public UpdateSetCollection<TEntity> Set(string property, object value)
		{
			var n = _columns.Count;
			_columns.Add(property.ToLower());
			_parms.Add("u" + n.ToString(), value);
			return this;
		}

		public string GetClause() => _columns.Select((s, n) => $"{s} = @u{n}").Join(", ");

		public Dictionary<string, object> GetParms() => _parms;
		public IEnumerable<string> GetColumns() => _columns;
		public Dictionary<string, object> GetColumnsWithValues()
		{
			var res = new Dictionary<string, object>();
			for (int i = 0; i < _columns.Count; i++)
				res.Add(_columns[i], _parms[$"u{i}"]);
			return res;
		}
	}
}
