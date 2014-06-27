using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Linq;
using NHibernate.Dialect;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Cfg.Loquacious;
using NHibernate.Mapping.ByCode;
using System.Text;
using System.Data;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Transform;

namespace Nephrite.Web.Model
{
	public abstract class HDataContext : IDisposable
	{
		static Dictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();
		static Dictionary<string, HbmMapping> _mappings = new Dictionary<string, HbmMapping>();
		//ISessionFactory _sessionFactory;
		ISession _session;
		Configuration _cfg = null;

		public List<object> ToInsert { get; private set; }
		public List<object> ToDelete { get; private set; }
		public List<Action> AfterSaveActions { get; private set; }
		public List<Action> BeforeSaveActions { get; private set; }

		public ISession Session
		{
			get
			{
				if (_session == null)
				{
					_session = SessionFactory.OpenSession();
					_session.FlushMode = FlushMode.Commit;
				}
				return _session;
			}
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
				if (_sessionFactories == null) _sessionFactories = new Dictionary<string, ISessionFactory>();

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
					f = mapper.CompileMappingForAllExplicitlyAddedEntities();
					_mappings.Add(t, f);
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

			_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" } });
			_cfg.SetInterceptor(new HDataContextSqlStatementInterceptor());

			_cfg.AddMapping(Mapping);


			//_session = SessionFactory.OpenSession();
			//_session.FlushMode = FlushMode.Commit;
		}

		~HDataContext()
		{
			Dispose();
		}

		public void SubmitChanges()
		{
			using (var transaction = _session.BeginTransaction())
			{
				Log.WriteLine("BEGIN TRANSACTION");
				Log.WriteLine();

				foreach (var action in BeforeSaveActions)
					action();

				foreach (object obj in ToInsert)
					_session.SaveOrUpdate(obj);

				foreach (object obj in ToDelete)
					_session.Delete(obj);

				foreach (var action in AfterSaveActions)
					action();

				transaction.Commit();

				Log.WriteLine("COMMIT TRANSACTION");
				Log.WriteLine();

				ToDelete.Clear();
				ToInsert.Clear();
				BeforeSaveActions.Clear();
				AfterSaveActions.Clear();

				_session.Close();
				_session.Dispose();
				_session = null;
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
	}

	public class HTable<T> : IQueryable<T>
	{
		HDataContext _dataContext;
		IQueryable<T> _query;

		public HTable(HDataContext dataContext, IQueryable<T> query)
		{
			_dataContext = dataContext;
			_query = query;
		}

		public IEnumerator<T> GetEnumerator()
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

		public void InsertOnSubmit(T obj)
		{
			_dataContext.ToInsert.Add(obj);
			//_session.Persist(obj);
		}

		public void DeleteOnSubmit(T obj)
		{
			_dataContext.ToDelete.Add(obj);
			//_session.Delete(obj);
		}

		public void DeleteAllOnSubmit(IQueryable<T> objs)
		{
			_dataContext.ToDelete.AddRange(objs.Cast<object>());
			//	_session.Delete(obj);
		}
	}

	public class HDataContextSqlStatementInterceptor : EmptyInterceptor
	{
		public override SqlString OnPrepareStatement(SqlString sql)
		{
			AppWeb.DataContext.Log.WriteLine(sql.ToString());
			AppWeb.DataContext.Log.WriteLine();
			return sql;
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