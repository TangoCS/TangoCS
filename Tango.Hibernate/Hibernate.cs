using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Tango.Data;
using Tango.Logger;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Mapping.ByCode.Conformist;
using System.Data.Common;

namespace Tango.Hibernate
{
	public class HDataContext : IDisposable, IDataContext
	{
		static Dictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();
		static Dictionary<string, HbmMapping> _mappings = new Dictionary<string, HbmMapping>();
		static Dictionary<string, Configuration> _configs = new Dictionary<string, Configuration>();
		ISession _session;
		IRequestLoggerProvider _loggerProvider;
		IRequestLogger _logger;
		IClassMappingList _mappingList;

		List<Action<ITransaction>> SaveActions = new List<Action<ITransaction>>();
		public List<Action> AfterSaveActions { get; private set; }
		public List<Action> BeforeSaveActions { get; private set; }

		readonly Func<DbConnection> _connection;
		public DbConnection Connection => Session.Connection;

		public ISession Session
		{
			get
			{
				if (_session == null || !_session.IsOpen)
				{
					var conn = _connection();
					if (_logger.Enabled)
						_session = SessionFactory.OpenSession(conn, new LogInterceptor(_logger));
					else
						_session = SessionFactory.OpenSession(conn);
					_session.FlushMode = FlushMode.Commit;
				}
				return _session;
			}
		}

		public Configuration Configuration
		{
			get 
			{
				string t = this.GetType().Name;
				Configuration _cfg = null;
				if (_configs == null) _configs = new Dictionary<string, Configuration>();

				if (_sessionFactories.ContainsKey(t))
					_cfg = _configs[t];
				else
				{
					_cfg = new Configuration();
					_cfg.DataBaseIntegration(_dbConfig);
					_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" } });
					ConfigurationExtensions?.Invoke(_cfg);
					_cfg.AddMapping(Mapping);
				}
				return _cfg;
			}
		}
		public Action<Configuration> ConfigurationExtensions { get; set; }

		public ISessionFactory SessionFactory
		{
			get
			{
				string t = this.GetType().Name;
				ISessionFactory f = null;
				if (_sessionFactories == null) _sessionFactories = new Dictionary<string, ISessionFactory>();

				if (_sessionFactories.ContainsKey(t))
					f = _sessionFactories[t];
				else
				{
					lock (_sessionFactories)
					{
						if (_sessionFactories.ContainsKey(t))
							f = _sessionFactories[t];
						else
						{
							f = Configuration.BuildSessionFactory();
							_sessionFactories.Add(t, f);
						}
					}
				}
				return f;
			}
		}

		public HbmMapping Mapping
		{
			get
			{
				string t = this.GetType().Name;
				HbmMapping f = null;
				if (_mappings == null) _mappings = new Dictionary<string, HbmMapping>();

				if (_mappings.ContainsKey(t))
					f = _mappings[t];
				else
				{
					var mapper = new ModelMapper();
					mapper.AddMappings(_mappingList.GetTypes());
					f = mapper.CompileMappingForAllExplicitlyAddedEntities();
					_mappings.Add(t, f);
				}
				return f;
			}
		}

		public string ID { get; set; }
		readonly Action<IDbIntegrationConfigurationProperties> _dbConfig;

		public HDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig, Func<DbConnection> connection, IClassMappingList mappingList, IRequestLoggerProvider loggerProvider)
		{
			ID = GetType().Name + "-" + Guid.NewGuid().ToString();
			_dbConfig = dbConfig;
			_connection = connection;
			_loggerProvider = loggerProvider;
			_logger = _loggerProvider.GetLogger("sql");
			_mappingList = mappingList;

			AfterSaveActions = new List<Action>();
			BeforeSaveActions = new List<Action>();
		}

		~HDataContext()
		{
			Dispose();
		}

		public void SubmitChanges()
		{
			using (var transaction = Session.BeginTransaction())
			{
				_logger.Write("BEGIN TRANSACTION");

				foreach (var action in BeforeSaveActions) action();
				foreach (var action in SaveActions) action(transaction);
				foreach (var action in AfterSaveActions) action();

				transaction.Commit();

				_logger.Write("COMMIT TRANSACTION");

				BeforeSaveActions.Clear();
				SaveActions.Clear();
				AfterSaveActions.Clear();
			}
			ClearCache();
			//Session.Clear();
		}

		public void Dispose()
		{
			if (_session != null) _session.Dispose();
		}

		public List<string> TestMappings(ICollection<string> entitiesFilter)
		{
			var invalidUpdates = new List<string>();
			var nop = new NoUpdateInterceptor(invalidUpdates);

			IEnumerable<string> entitiesToCheck;
			if (entitiesFilter == null)
			{
				entitiesToCheck = Configuration.ClassMappings.Select(x => x.EntityName);
			}
			else
			{
				entitiesToCheck = from persistentClass in Configuration.ClassMappings
								  where entitiesFilter.Contains(persistentClass.EntityName)
								  select persistentClass.EntityName;
			}

			foreach (var entityName in entitiesToCheck)
			{
				EntityPersistenceTest(invalidUpdates, entityName, nop);
			}

			return invalidUpdates;
		}

		private void EntityPersistenceTest(ICollection<string> invalidUpdates, string entityName, IInterceptor nop)
		{
			using (var s = SessionFactory.OpenSession(nop))
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("from " + entityName).SetMaxResults(1).SetFirstResult(0).UniqueResult();
				try
				{
					s.Flush();
				}
				catch (Exception ex)
				{
					string emsg = string.Format("EXCEPTION - Flushing entity: {0}", ex.Message);
					invalidUpdates.Add(emsg);
				}
				tx.Rollback();
			}
		}

		public int ExecuteCommand(string command, params object[] parameters)
		{
			var s = Session.CreateSQLQuery(command);
			int i = 0;
			foreach (object p in parameters)
			{
				if (p is ISqlParameter)
					(p as ISqlParameter).SetQueryParameter(s, i);
				else
					s.SetParameter(i, p);
				i++;
			}

			return s.UniqueResult<int>();
		}

		public IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters)
		{
			var s = Session.CreateSQLQuery(query);
			int i = 0;
			foreach (object p in parameters)
			{
				if (p is ISqlParameter)
					(p as ISqlParameter).SetQueryParameter(s, i);
				else
					s.SetParameter(i, p);
				i++;
			}

			Type type = typeof(TResult);
			bool isSimple = type.IsValueType || type.IsPrimitive ||
			new Type[] { 
				typeof(String),
				typeof(Decimal),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(Guid)
			}.Contains(type) || Convert.GetTypeCode(type) != TypeCode.Object;

			if (!isSimple)
				s.SetResultTransformer(Transformers.AliasToBean<TResult>());
			return s.List<TResult>();
		}

		public IDbCommand GetCommand(IQueryable query)
		{
			var sessionImp = (ISessionImplementor)Session;
			var nhLinqExpression = new NhLinqExpression(query.Expression, sessionImp.Factory);
			var translatorFactory = new ASTQueryTranslatorFactory();
			var translators = translatorFactory.CreateQueryTranslators(nhLinqExpression, null, false, sessionImp.EnabledFilters, sessionImp.Factory);

			string sql = translators[0].SQLString;
			var command = Session.Connection.CreateCommand();

			foreach (FilterImpl filter in sessionImp.EnabledFilters.Values)
			{
				foreach (var param in filter.Parameters)
				{
					sql = sql.Replace(":" + filter.Name + "." + param.Key, "?"); 
					
					var p = command.CreateParameter();
					p.ParameterName = param.Key;
					p.Value = param.Value;
					command.Parameters.Add(p);
				}
			}
			foreach (var key in nhLinqExpression.ParameterValuesByName.Keys)
			{
				var param = nhLinqExpression.ParameterValuesByName[key];
				var p = command.CreateParameter();
				p.ParameterName = key;
				p.Value = param.Item1;
				command.Parameters.Add(p);
			}
			command.CommandText = sql;

			return command;
		}

		public IQueryable<T> GetTable<T>()
		{
			return Session.Query<T>();
		}

		public IQueryable GetTable(Type t)
		{
			MethodInfo mi = typeof(LinqExtensionMethods).GetMethods().FirstOrDefault(tp => tp.GetParameters().Any(p => p.ParameterType == typeof(ISession))).MakeGenericMethod(new Type[] { t });
			return mi.Invoke(Session, new object[] { Session }) as IQueryable;
		}

		public T Get<T, TKey>(TKey id)
		{
			return Session.Get<T>(id);
		}

		public void InsertOnSubmit<T>(T obj, int? index = null) where T : class
		{
			if (index == null)
				SaveActions.Add(t => _session.SaveOrUpdate(obj));
			else
				SaveActions.Insert(index.Value, t => _session.SaveOrUpdate(obj));
		}
		public void DeleteOnSubmit<T>(T obj, int? index = null) where T : class
		{
			if (index == null)
				SaveActions.Add(t => _session.Delete(obj));
			else
				SaveActions.Insert(index.Value, t => _session.Delete(obj));
		}
		public void DeleteAllOnSubmit<T>(IEnumerable<T> objs, int? index = null) where T : class
		{
			if (index == null)
				SaveActions.Add(t => {
					foreach (var obj in objs)
						_session.Delete(obj);
				});
			else
				SaveActions.Insert(index.Value, t => {
					foreach (var obj in objs)
						_session.Delete(obj);
				});
		}
		public void AttachOnSubmit<T>(T obj, int? index = null) where T : class
		{
			if (index == null)
				SaveActions.Add(t => _session.Merge(obj));
			else
				SaveActions.Insert(index.Value, t => _session.Merge(obj));
		}

		public void CommandOnSubmit(string query, params object[] parms)
		{
			SaveActions.Add(t => {
				using (var comm = _session.Connection.CreateCommand())
				{
					comm.CommandText = query;
					t.Enlist(comm);
					foreach (var parm in parms)
						comm.Parameters.Add(parm);
					comm.ExecuteNonQuery();
				}
			});
		}

		public void CommandOnSubmit(string query, int index, params object[] parms)
		{
			SaveActions.Insert(index, t => {
				using (var comm = _session.Connection.CreateCommand())
				{
					comm.CommandText = query;
					t.Enlist(comm);
					foreach (var parm in parms)
						comm.Parameters.Add(parm);
					comm.ExecuteNonQuery();
				}
			});
		}

		public void ClearCache()
		{
			Session.GetSessionImplementation().PersistenceContext.Clear();
		}
	}



	public interface ISqlParameter
	{
		void SetQueryParameter(IQuery q, int position);
	}

	public class SqlParameter<T> : ISqlParameter
	{
		public T Value { get; set; }
		public string Name { get; set; }

		public static SqlParameter<T> Set(T value)
		{
			return new SqlParameter<T> { Value = value };
		}

		public void SetQueryParameter(IQuery q, int position)
		{
			q.SetParameter(position, Value);
		}
	}

	public class NoUpdateInterceptor : EmptyInterceptor
	{
		private readonly IList<string> invalidUpdates;

		public NoUpdateInterceptor(IList<string> invalidUpdates)
		{
			this.invalidUpdates = invalidUpdates;
		}

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			string msg = " FlushDirty :" + entity.GetType().FullName;
			invalidUpdates.Add(msg);
			return false;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			string msg = " Save       :" + entity.GetType().FullName;
			invalidUpdates.Add(msg);
			return false;
		}

		public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			string msg = " Delete     :" + entity.GetType().FullName;
			invalidUpdates.Add(msg);
		}
	}


	public interface IClassMappingList
	{
		IEnumerable<Type> GetTypes();
	}

	public class DefaultClassMappingList : IClassMappingList, ITypeObserver
	{
		static List<Type> _mappings = new List<Type>();

		public IEnumerable<Type> GetTypes() => _mappings;

		public void LookOver(Type t)
		{
			if (t.BaseType.IsGenericType)
			{
				var gt = t.BaseType.GetGenericTypeDefinition();
				if (gt == typeof(ClassMapping<>) ||
					gt == typeof(JoinedSubclassMapping<>) ||
					gt == typeof(SubclassMapping<>))
				{
					_mappings.Add(t);
				}
			}
		}
	}
}