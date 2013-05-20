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
using System.Data.Linq;

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
			Log = new StringWriter();

			ToInsert = new List<object>();
			ToDelete = new List<object>();

			_cfg = new Configuration();
			_cfg.DataBaseIntegration(dbConfig);


			_cfg.AddProperties(new Dictionary<string, string>() { { "command_timeout", "300" } });
			_cfg.SetInterceptor(new HDataContextSqlStatementInterceptor(this));

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

	public class HTable<T> : IQueryable<T>, ITable
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

	public class HDataContextSqlStatementInterceptor : EmptyInterceptor
	{
		HDataContext _dataContext;

		public HDataContextSqlStatementInterceptor(HDataContext dc)
		{
			_dataContext = dc;
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			_dataContext.Log.WriteLine(sql.ToString());
			_dataContext.Log.WriteLine();
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