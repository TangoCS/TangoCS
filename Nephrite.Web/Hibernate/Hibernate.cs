using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
using System.Data.Linq;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using NHibernate.Transform;
using NHibernate.Event;
using System.Reflection;
using Nephrite.Web.SettingsManager;
using Nhibernate.Extensions;
using Nephrite.Web.SPM;


namespace Nephrite.Web.Hibernate
{
	
	public abstract class HDataContext : IDisposable, IDataContext
	{
		static Dictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();
		//ISessionFactory _sessionFactory;
		ISession _session;
		Configuration _cfg = null;

		public List<object> ToInsert { get; private set; }
		public List<object> ToDelete { get; private set; }
		public List<Action> AfterSaveActions { get; private set; }
		public List<Action> BeforeSaveActions { get; private set; }

		public static string DBType
		{
			get
			{
				return System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper();
			}
		}

		public static Action<IDbIntegrationConfigurationProperties> DBConfig(string connectionString)
		{
			return c =>
			{
				switch (DBType)
				{
					case "MSSQL": c.Dialect<MsSql2008Dialect>(); break;
					case "DB2": c.Dialect<DB2Dialect>(); break;
					case "ORACLE": c.Dialect<Oracle10gDialect>(); break;
					case "POSTGRESQL": c.Dialect<PostgreSQLDialect>(); break;
					default: c.Dialect<MsSql2008Dialect>(); break;
				}
				
				c.ConnectionString = connectionString;
				c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
				c.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
				//c.LogFormattedSql = true;
				if (!System.Configuration.ConfigurationManager.AppSettings["ValidateSchema"].IsEmpty())
					c.SchemaAction = SchemaAutoAction.Validate;
			};
		}

		public ISession Session
		{
			get { return _session; }
		}

		public Configuration Configuration
		{
			get { return _cfg; }
		}

		public ISessionFactory SessionFactory
		{
			get 
			{
				string t = this.GetType().Name;
				ISessionFactory f = null;

				if (_sessionFactories.ContainsKey(t))
					f = _sessionFactories[t];
				else
				{
					f = _cfg.BuildSessionFactory();
					_sessionFactories.Add(t, f);
				}
				return f; 
			}
		}

		public TextWriter Log { get; set; }
		public abstract IEnumerable<Type> GetEntitiesTypes();

		public HDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
		{
			Log = new StringWriter();

			ToInsert = new List<object>();
			ToDelete = new List<object>();
			AfterSaveActions = new List<Action>();
			BeforeSaveActions = new List<Action>();

			_cfg = new Configuration();
			_cfg.DataBaseIntegration(dbConfig);
	

			_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" }});
			_cfg.SetInterceptor(new HDataContextSqlStatementInterceptor(this));
			//_cfg.EventListeners.PreDeleteEventListeners = new IPreDeleteEventListener[] { new TrackChangesListener() };
			//_cfg.EventListeners.PreInsertEventListeners = new IPreInsertEventListener[] { new TrackChangesListener() };
			//_cfg.EventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[] { new TrackChangesListener() };

			var mapper = new ModelMapper();
			mapper.AddMappings(GetEntitiesTypes());
			HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			_cfg.AddMapping(domainMapping);


			_session = SessionFactory.OpenSession();
			_session.FlushMode = FlushMode.Commit;
		}

		~HDataContext()
		{
			Dispose();
		}

		public void SubmitChanges()
		{

			using (var transaction = _session.BeginTransaction())
			{
				foreach (var action in BeforeSaveActions)
					action();

				foreach (object obj in ToInsert)
				{
					_session.SaveOrUpdate(obj);
				}

				foreach (object obj in ToDelete)
					_session.Delete(obj);

				foreach (var action in AfterSaveActions)
					action();

				transaction.Commit();
				//_session.Flush();
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
				entitiesToCheck = _cfg.ClassMappings.Select(x => x.EntityName);
			}
			else
			{
				entitiesToCheck = from persistentClass in _cfg.ClassMappings
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
			command.CommandText = sql;

			return command;
		}

		public IQueryable<T> GetTable<T>() //where T : class
		{  
			return Session.Query<T>();
		}

		public ITable GetTable(Type t)
		{

			MethodInfo mi = typeof(LinqExtensionMethods).GetMethods().FirstOrDefault(tp => tp.GetParameters().Any(p => p.ParameterType == typeof(ISession))).MakeGenericMethod(new Type[] { t }); ;
			var q = mi.Invoke(Session, new object[] { Session }) as IQueryable;
			return new HTable(this, q);
		
		}

		public T Get<T, TKey>(TKey id) where T : IEntity, IWithKey<T, TKey>, new()
		{
			return Session.Get<T>(id);
		}
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
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _query.GetEnumerator();
		}
		public Type ElementType
		{
			get { return _query.ElementType; }
		}
		public System.Linq.Expressions.Expression Expression
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
		public void InsertAllOnSubmit(IEnumerable objs)
		{
			_dataContext.ToDelete.AddRange(objs.Cast<object>());
		}
		public void DeleteOnSubmit(object obj)
		{
			_dataContext.ToDelete.Add(obj);
		}
		public void DeleteAllOnSubmit(IEnumerable objs)
		{
			_dataContext.ToDelete.AddRange(objs.Cast<object>());
		}

		public void Attach(object entity, object original)
		{
			throw new NotImplementedException();
		}
		public void Attach(object entity, bool asModified)
		{
			throw new NotImplementedException();
		}
		public void Attach(object entity)
		{
			throw new NotImplementedException();
		}
		public void AttachAll(IEnumerable entities, bool asModified)
		{
			throw new NotImplementedException();
		}
		public void AttachAll(IEnumerable entities)
		{
			throw new NotImplementedException();
		}
		public DataContext Context
		{
			get { throw new NotImplementedException(); }
		}
		public ModifiedMemberInfo[] GetModifiedMembers(object entity)
		{
			throw new NotImplementedException();
		}

		public object GetOriginalEntityState(object entity)
		{
			throw new NotImplementedException();
		}
		public bool IsReadOnly
		{
			get { return false; }
		}
	}

	public class HTable<T> : HTable, IQueryable<T>
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
}