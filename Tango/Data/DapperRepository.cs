using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tango.Data
{
	public class Store
	{
		public IDbConnection Connection { get; }
		public IDbTransaction Transaction { get; set; }

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
		public DapperDatabase(IDbConnection connection) : base(connection) { }
		public IRepository<T> Repository<T>() => new DapperRepository<T>(this);
		public IRepository Repository(Type type) => new DapperRepository(this, type);
	}

	public class DapperRepository : IRepository
	{
		public IDatabase Database { get; }
		protected Type Type;
		public string Table { get; }
		protected DBType DBType { get; }
		public string AllObjectsQuery { get; set; }
		protected Dictionary<string, PropertyInfo> keys = new Dictionary<string, PropertyInfo>();
		protected Dictionary<string, PropertyInfo> columns = new Dictionary<string, PropertyInfo>();
		public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

		protected string noKeyMessage => $"Entity {Type.Name} doesn't contain any key property";

		protected IQueryTranslatorDialect Dialect => QueryHelper.CreateDialect(DBType);

		public DapperRepository(IDatabase database, Type type)
		{
			Database = database;
			Type = type;

			var baseNaming = type.GetCustomAttribute(typeof(BaseNamingConventionsAttribute)) as BaseNamingConventionsAttribute;

			Table = Type.GetCustomAttribute<TableAttribute>()?.Name;

			if (Table == null)
			{
				var table = Type.Name.ToLower();
				if (baseNaming != null)
				{
					var prefix = BaseNamingConventions.EntityPrefix[baseNaming.Category];
					if (table.StartsWith(prefix.ToLower()))
					{
						table = DBConventions.EntityPrefix[baseNaming.Category].ToLower() + table.Substring(2);
					}
				}
				Table = table;
			}
			
			if (Table.ToLower().EndsWith(".sql"))
			{
				Table = $"({EmbeddedResourceManager.GetString(Type, Table)})";
				AllObjectsQuery = Table;
			}
			else
				AllObjectsQuery = "select * from " + Table;
			
			DBType = database.GetDBType();

			if (SqlMapper.GetTypeMap(type) is DefaultTypeMap && baseNaming != null)
			{
				var custom = new CustomPropertyTypeMap(
					type,
					(t, columnName) =>
					{
						var name = columnName;
						
						if (name.EndsWith(DBConventions.IDSuffix))
						{
							name = name.Substring(0, name.Length - DBConventions.IDSuffix.Length) + "ID";
							var pid = t.GetProperty(name);
							if (pid != null)
								return pid;
						}
						if (name.EndsWith(DBConventions.GUIDSuffix))
						{
							name = name.Substring(0, name.Length - DBConventions.GUIDSuffix.Length) + "GUID";
							var pguid = t.GetProperty(name);
							if (pguid != null)
								return pguid;
						}

						var p = t.GetProperty(name);
						if (p != null)
							return p;

						throw new Exception();
					});
				
				SqlMapper.SetTypeMap(type, custom);
			}

			var props = Type.GetProperties().Where(o => o.GetCustomAttribute<ColumnAttribute>() != null);
			foreach (var p in props)
			{
				var name = ChangePropertyName(p);

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

		protected string ChangePropertyName(PropertyInfo p)
		{
			var name = p.Name;

			var conventions = p.PropertyType == typeof(Guid) || p.PropertyType == typeof(Guid?)
				? DBConventions.GUIDSuffix
				: DBConventions.IDSuffix;

			if (name.EndsWith(BaseNamingConventions.IDSuffix) && !name.EndsWith(conventions))
			{
				name = name.Substring(0, name.Length - BaseNamingConventions.IDSuffix.Length) + conventions;
			}

			return name;
		}

		public bool Exists(object id)
		{
			var where = GetByIdWhereClause(id);
			var query = $"select 1 from ({AllObjectsQuery}) t where {where.clause}";

			return Database.Connection.QuerySingleOrDefault<int>(query, where.parms, Database.Transaction) == 1;
		}

		protected (string clause, Dictionary<string, object> parms) GetByIdWhereClause(object id)
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
				return (clause, parms);
			}
			else if (idtype == typeof(string) || idtype == typeof(Guid) || idtype == typeof(DateTime) ||
				(idtype.IsValueType && idtype.IsPrimitive))
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				parms.Add("p0", id);
				return (keys.Keys.First().ToLower() + " = @p0", parms);
			}
			else if (idtype.IsValueType)
			{
				if (keys.Count == 0) throw new Exception(noKeyMessage);
				var props = id.GetType().GetProperties();
				int i = 0;
				var clause = keys.Select(k => {
					var s = $"{k.Key.ToLower()} = @p{i}";
					parms.Add($"p{i}", props[i].GetValue(id));
					i++;
					return s;
				}).Join(" and ");
				return (clause, parms);
			}
			else
				throw new Exception(noKeyMessage);
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

			query = QueryHelper.SetNewFieldExpression(query, "count(1)");

			return Database.Connection.QuerySingle<int>(query, args, Database.Transaction);
		}

		public virtual object GetById(object id)
		{
			if (id == null) return null;
			var (clause, parms) = GetByIdWhereClause(id);
			var query = $"select * from ({AllObjectsQuery}) t where {clause}";

			return Database.Connection.QuerySingleOrDefault(Type, query, parms, Database.Transaction);
		}
	}

	public class DapperRepository<T> : DapperRepository, IRepository<T>
	{
		//protected Dictionary<string, object> parms = new Dictionary<string, object>();

		public DapperRepository(IDatabase database) : base(database, typeof(T))
		{
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

		protected (string clause, Dictionary<string, object> parms) GetByIdsWhereClause<TKey>(IEnumerable<TKey> ids)
		{
			var cnt = keys.Count();
			if (cnt == 1)
			{
				var parms = new Dictionary<string, object> {
					{ "p0", ids }
				};
				if (DBType == DBType.POSTGRESQL)
					return (keys.Keys.First().ToLower() + " = any(@p0)", parms);
				else
					return (keys.Keys.First().ToLower() + " in @p0", parms);
			}
			else if (cnt > 1)
				throw new Exception($"Composite keys not supported (entity: {typeof(T).Name}).");
			else
				throw new Exception(noKeyMessage);
		}

		public new T GetById(object id)
		{
			if (id == null) return default;
			return (T)base.GetById(id);
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
					var name = ChangePropertyName(prop);
					cols.Add(name.ToLower());
					vals.Add("@i" + n);
					parms.Add("i" + n, val);
					n++;
				}
			}

			var colsClause = cols.Join(", ");
			var valuesClause =  vals.Join(", ");
			var returning = identity == null ? "" : string.Format(Dialect.ReturningIdentity, identity.Name.ToLower());

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
			var query = $"update {Table} set {setCollection.GetClause()} where {where.clause}";

			foreach (var i in setCollection.GetParms())
				where.parms.Add(i.Key, i.Value);


			Database.Connection.Execute(query, where.parms, Database.Transaction);
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
			var query = $"update {Table} set {collection.GetClause()} where {where.clause}";

			foreach (var i in collection.GetParms())
				where.parms.Add(i.Key, i.Value);

			Database.Connection.Execute(query, where.parms, Database.Transaction);
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
			var translator = new QueryTranslator(Dialect);
			translator.Translate(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression);
			var args = new DynamicParameters(translator.Parms);
			var query = $"delete from {Table} where {translator.WhereClause}";

			Database.Connection.ExecuteScalar(query, args, Database.Transaction);
		}

		public virtual void Delete<TKey>(IEnumerable<TKey> ids)
		{
			var where = ids.Count() == 1 ? GetByIdWhereClause(ids.First()) : GetByIdsWhereClause(ids);
			var query = $"delete from {Table} where {where.clause}";

			Database.Connection.ExecuteScalar(query, where.parms, Database.Transaction);
		}

		public bool Any(Expression<Func<T, bool>> predicate)
		{
			return Count(Enumerable.Empty<T>().AsQueryable().Where(predicate).Expression) > 0;
		}

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
