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

namespace Nephrite.Web.Model
{
	public abstract class HDataContext : IDisposable
	{
		ISessionFactory _sessionFactory;
		ISession _session;
		Configuration _cfg = null;

		public List<object> ToInsert { get; private set; }
		public List<object> ToDelete { get; private set; }

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
			get { return _sessionFactory; }
		}

		public TextWriter Log { get; set; }
		public abstract IEnumerable<Type> GetEntitiesTypes();

		public HDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
		{

			//ConnectionManager.SetConnectionString("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=servants1;Data Source=TOSHIBA-TOSH\\SQL2008");
			//ConnectionManager.SetConnectionString("Database=servants;UserID=db2admin;Password=q121212;Server=193.233.68.82:50000");

			
			Log = new StringWriter();

			ToInsert = new List<object>();
			ToDelete = new List<object>();

			_cfg = new Configuration();
			_cfg.DataBaseIntegration(dbConfig);


			_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" } });
			_cfg.SetInterceptor(new HDataContextSqlStatementInterceptor());

			var mapper = new ModelMapper();
			mapper.AddMappings(GetEntitiesTypes());
			HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			_cfg.AddMapping(domainMapping);

			_sessionFactory = _cfg.BuildSessionFactory();
			
			_session = _sessionFactory.OpenSession();
			_session.FlushMode = FlushMode.Never;
		}

		~HDataContext()
		{
			Dispose();
		}

		public void SubmitChanges()
		{
			using (var transaction = _session.BeginTransaction())
			{
				foreach (object obj in ToInsert)
					_session.SaveOrUpdate(obj);

				foreach (object obj in ToDelete)
					_session.Delete(obj);

				transaction.Commit();
				_session.Flush();
			}
		}

		public void Dispose()
		{
			_session.Dispose();
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