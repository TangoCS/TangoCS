using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq.Functions;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Linq;
using NHibernate.Dialect;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Cfg.Loquacious;
using NHibernate.Mapping.ByCode;
using System.Text;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using NHibernate.Transform;
using NHibernate.Event;
using System.Reflection;
using Nephrite.SettingsManager;

using NHibernate.Impl;
using System.Linq.Expressions;
using System.Diagnostics;
using Nephrite.Data;
using Nephrite.Identity;



namespace Nephrite.Web.Hibernate
{

	public abstract class HDataContext : IDisposable, IDataContext
	{
		//[ThreadStatic]
		static Dictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();
		static Dictionary<string, HbmMapping> _mappings = new Dictionary<string, HbmMapping>();
		static Dictionary<string, Configuration> _configs = new Dictionary<string, Configuration>();
		//ISessionFactory _sessionFactory;
		ISession _session;
		//Configuration _cfg = null;

		public List<object> ToInsert { get; private set; }
		public List<object> ToDelete { get; private set; }
		public List<object> ToAttach { get; private set; }
		public List<Action> AfterSaveActions { get; private set; }
		public List<Action> BeforeSaveActions { get; private set; }

		protected bool EnableTableAutoFilter = true;

		public IDataContext All
		{
			get
			{
				EnableTableAutoFilter = false;
				return this;
			}
		}
		public IDataContext Filtered
		{
			get
			{
				EnableTableAutoFilter = true;
				return this;
			}
		}

		//public static DBType DBType
		//{
		//	get
		//	{
		//		return A.DBType;
		//	}
		//	set
		//	{
		//		A.DBType = value;
		//	}
		//}

		public ISession Session
		{
			get
			{
				if (_session == null || !_session.IsOpen)
				{
					_session = SessionFactory.OpenSession();
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
					_cfg = new NHibernate.Cfg.Configuration();
					_cfg = new Configuration();
					_cfg.DataBaseIntegration(_dbConfig);

					//var sw = A.Items["Stopwatch"] as Stopwatch;
					//Log.WriteLine(String.Format("-- {1} create {0} DataBaseIntegration", ID, sw.Elapsed.ToString()));

					_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" } });
					_cfg.SetInterceptor(new HDataContextInterceptor());

					if (_listeners != null)
					{
						_cfg.EventListeners.PreDeleteEventListeners = _listeners.PreDeleteEventListeners.ToArray();
						_cfg.EventListeners.PreInsertEventListeners = _listeners.PreInsertEventListeners.ToArray();
						_cfg.EventListeners.PreUpdateEventListeners = _listeners.PreUpdateEventListeners.ToArray();
						_cfg.EventListeners.PostDeleteEventListeners = _listeners.PostDeleteEventListeners.ToArray();
						_cfg.EventListeners.PostInsertEventListeners = _listeners.PostInsertEventListeners.ToArray();
						_cfg.EventListeners.PostUpdateEventListeners = _listeners.PostUpdateEventListeners.ToArray();

						if (_listeners.SaveOrUpdateEventListeners.Count > 0)
							_cfg.EventListeners.SaveOrUpdateEventListeners = _listeners.SaveOrUpdateEventListeners.ToArray();
					}

					//Log.WriteLine(String.Format("-- {1} create {0} Added Listeners", ID, sw.Elapsed.ToString()));
					_cfg.AddMapping(Mapping);
					//Log.WriteLine(String.Format("-- {1} create {0} Added Mapping", ID, sw.Elapsed.ToString())); 
				}
				return _cfg;
			}
		}

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
					f = Configuration.BuildSessionFactory();
					_sessionFactories.Add(t, f);
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
					mapper.AddMappings(GetEntitiesTypes());
					mapper.AddMappings(GetTableFunctionsTypes());
					f = mapper.CompileMappingForAllExplicitlyAddedEntities();
					_mappings.Add(t, f);
				}
				return f;
			}
		}


		public TextWriter Log 
		{ 
			get 
			{
				if (A.Items["SqlLog"] == null)
				{
					TextWriter log = new StringWriter();
					A.Items["SqlLog"] = log;
					return log;
				}
				return A.Items["SqlLog"] as TextWriter;
			} 
		}
		public string ID { get; set; }

		Action<IDbIntegrationConfigurationProperties> _dbConfig;
		Listeners _listeners;

		public abstract IEnumerable<Type> GetEntitiesTypes();
		public virtual IEnumerable<Type> GetTableFunctionsTypes() { return new List<Type>(); }

		public HDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig, Listeners listeners = null)
		{
			ID = GetType().Name + "-" + Guid.NewGuid().ToString();
			_dbConfig = dbConfig;
			_listeners = listeners;

			//var sw = A.Items["Stopwatch"] as Stopwatch;
			//Log.WriteLine(String.Format("-- {1} create {0}", ID, sw.Elapsed.ToString())); 
			//Log.WriteLine();
			//Log = new StringWriter();
			ToInsert = new List<object>();
			ToDelete = new List<object>();
			ToAttach = new List<object>();
			AfterSaveActions = new List<Action>();
			BeforeSaveActions = new List<Action>();

			

			//_session = SessionFactory.OpenSession();
			//_session.FlushMode = FlushMode.Commit;
		}

		~HDataContext()
		{
			Dispose();
		}

		public void SubmitChanges()
		{

			using (var transaction = Session.BeginTransaction())
			{
				Log.WriteLine("BEGIN TRANSACTION");
				Log.WriteLine();

				foreach (var action in BeforeSaveActions) action();
				foreach (object obj in ToDelete) _session.Delete(obj);
				foreach (object obj in ToInsert) { SetTimeStamp(obj); _session.SaveOrUpdate(obj); }
				foreach (object obj in ToAttach) _session.Merge(obj);

				ToDelete.Clear();
				ToInsert.Clear();
				ToAttach.Clear();

				foreach (var action in AfterSaveActions) action();
				foreach (object obj in ToDelete) _session.Delete(obj);
				foreach (object obj in ToInsert) { SetTimeStamp(obj); _session.SaveOrUpdate(obj); }
				foreach (object obj in ToAttach) _session.Merge(obj);
				
				ToDelete.Clear();
				ToInsert.Clear();
				ToAttach.Clear();

				transaction.Commit();

				Log.WriteLine("COMMIT TRANSACTION");
				Log.WriteLine();

				BeforeSaveActions.Clear();
				AfterSaveActions.Clear();

				//_session.Clear();
				//_session = SessionFactory.OpenSession();
				//_session.FlushMode = FlushMode.Commit;
				//_session.Close();
				//_session.Dispose();
			}
		}

		void SetTimeStamp(object obj)
		{
			int sid = -1;

			if (obj is IWithTimeStamp)
			{
				if (sid == -1) sid = Subject.Current.ID;

				var obj2 = obj as IWithTimeStamp;
				obj2.LastModifiedDate = DateTime.Now;
				obj2.LastModifiedUserID = sid;
			}
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

		abstract public IDataContext NewDataContext();
		abstract public IDataContext NewDataContext(string connectionString);

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

		public ITable<T> GetTable<T>()
		{
			return EnableTableAutoFilter ? 
				new HTable<T>(this, DefaultTableFilters.ApplyFor<T>(Session.Query<T>())) : 
				new HTable<T>(this, Session.Query<T>());
		}

		public ITable GetTable(Type t)
		{
			MethodInfo mi = typeof(LinqExtensionMethods).GetMethods().FirstOrDefault(tp => tp.GetParameters().Any(p => p.ParameterType == typeof(ISession))).MakeGenericMethod(new Type[] { t });
			var q = mi.Invoke(Session, new object[] { Session }) as IQueryable;
			return new HTable(this, q);
		}

		public T Get<T, TKey>(TKey id)
		{
			return Session.Get<T>(id);
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

		public static SqlParameter<T> Set<T>(T value)
		{
			return new SqlParameter<T> { Value = value };
		}

		public void SetQueryParameter(IQuery q, int position)
		{
			q.SetParameter<T>(position, Value);
		}
	}

	public class Listeners
	{
		List<IPreDeleteEventListener> _preDeleteEventListeners = new List<IPreDeleteEventListener>();
		List<IPreInsertEventListener> _preInsertEventListeners = new List<IPreInsertEventListener>();
		List<IPreUpdateEventListener> _preUpdateEventListeners = new List<IPreUpdateEventListener>();
		List<IPostDeleteEventListener> _postDeleteEventListeners = new List<IPostDeleteEventListener>();
		List<IPostInsertEventListener> _postInsertEventListeners = new List<IPostInsertEventListener>();
		List<IPostUpdateEventListener> _postUpdateEventListeners = new List<IPostUpdateEventListener>();

		List<ISaveOrUpdateEventListener> _saveOrUpdateEventListeners = new List<ISaveOrUpdateEventListener>();


		public List<IPreDeleteEventListener> PreDeleteEventListeners { get { return _preDeleteEventListeners; } }
		public List<IPreInsertEventListener> PreInsertEventListeners { get { return _preInsertEventListeners; } }
		public List<IPreUpdateEventListener> PreUpdateEventListeners { get { return _preUpdateEventListeners; } }

		public List<IPostDeleteEventListener> PostDeleteEventListeners { get { return _postDeleteEventListeners; } }
		public List<IPostInsertEventListener> PostInsertEventListeners { get { return _postInsertEventListeners; } }
		public List<IPostUpdateEventListener> PostUpdateEventListeners { get { return _postUpdateEventListeners; } }

		public List<ISaveOrUpdateEventListener> SaveOrUpdateEventListeners { get { return _saveOrUpdateEventListeners; } }
	}

	public class HTable : IQueryable, ITable
	{
		protected HDataContext _dataContext;
		protected IQueryable _query;

		public HTable(HDataContext dataContext, IQueryable query)
		{
			_dataContext = dataContext;
			_query = query;
		}

		public IEnumerator GetEnumerator()
		{
			return _query.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _query.GetEnumerator();
		}
		public Type ElementType
		{
			get { return _query.ElementType; }
		}
		public Expression Expression
		{
			get { return _query.Expression; }
		}
		public IQueryProvider Provider
		{
			get { return _query.Provider; }
		}

		public void InsertOnSubmit(object obj)
		{
			_dataContext.ToInsert.Add(obj);
		}
		public void DeleteOnSubmit(object obj)
		{
			_dataContext.ToDelete.Add(obj);
		}
		public void DeleteAllOnSubmit(IEnumerable objs)
		{
			_dataContext.ToDelete.AddRange(objs.Cast<object>());
		}

		public void AttachOnSubmit(object entity)
		{
			_dataContext.ToAttach.Add(entity);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
	}

	public class HTable<T> : HTable, ITable<T>
	{
		protected IQueryable<T> _query2;

		public HTable(HDataContext dataContext, IQueryable<T> query)
			: base(dataContext, query)
		{
			_query2 = query;
		}

		public new IEnumerator<T> GetEnumerator()
		{
			return _query2.GetEnumerator();
		}

		public void InsertOnSubmit(T obj)
		{
			base.InsertOnSubmit(obj);
		}

		public void DeleteOnSubmit(T obj)
		{
			base.DeleteOnSubmit(obj);
		}

		public void DeleteAllOnSubmit(IEnumerable<T> objs)
		{
			base.DeleteAllOnSubmit(objs);
		}

		public void AttachOnSubmit(T obj)
		{
			base.AttachOnSubmit(obj);
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

	//public class DataContextLogWriter : StringWriter
	//{
	//	public override void Write(char[] buffer, int index, int count)
	//	{
	//		base.Write(buffer, index, count);
	//		if ((new string(buffer)).StartsWith("-- Context:"))
	//		{
	//			char[] buff = ("-- Execute start: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + System.Environment.NewLine).ToCharArray();
	//			base.Write(buff, 0, buff.Length);
	//		}
	//	}
	//}
}