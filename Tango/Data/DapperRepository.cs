using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Tango.Data
{
	public class DapperListRepository<T> : IListRepository<T>
	{
		protected IDbConnection Connection { get; set; }

		protected virtual string Query => "select * from " + typeof(T).Name.ToLower();

		public DapperListRepository(IDbConnection connection)
		{
			Connection = connection;
		}

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

			return Connection.QuerySingle<int>(query, args);
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
				return Connection.Query(query, args).Select(x => {
					var dapperRowProperties = x as IDictionary<string, object>;
					IDictionary<string, object> expando = new ExpandoObject();

					foreach (KeyValuePair<string, object> property in dapperRowProperties)
						expando.Add(property.Key, property.Value);

					return (T)expando;
				});
			else
				return Connection.Query<T>(query, args);
		}
	}

	public class DapperReadRepository<T, TKey> : IReadRepository<T, TKey>
		where T : IWithKey<T, TKey>, new()
	{
		protected IDbConnection Connection { get; set; }

		public DapperReadRepository(IDbConnection connection)
		{
			Connection = connection;
		}

		public T GetById(TKey id)
		{
			var t = new T();

			var translator = new QueryTranslator();
			translator.Translate(t.KeySelector(id));

			var query = "select * from " + typeof(T).Name.ToLower();
			query += " where " + translator.WhereClause;

			var args = new DynamicParameters(translator.Parms);

			return Connection.QuerySingleOrDefault<T>(query, args);
		}
	}
}
