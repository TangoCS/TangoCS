using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Nephrite.Web;
using Nephrite.Web.Hibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;

namespace Nephrite.Metamodel.Model
{
	public class MMDataContext : HDataContext
	{
		public MMDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
			: base(dbConfig)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(MM_PackageMap));
			l.Add(typeof(MM_ObjectTypeMap));
			l.Add(typeof(MM_ObjectPropertyMap));
			l.Add(typeof(MM_CodifierMap));
			l.Add(typeof(MM_CodifierValueMap));
			l.Add(typeof(MM_MethodMap));
			l.Add(typeof(MM_FormViewMap));
			l.Add(typeof(MM_FormFieldMap));																																	
			l.Add(typeof(N_ReplicationObjectMap));
			l.Add(typeof(N_CacheMap));
			return l;
		}

		public IQueryable<MM_Package> MM_Packages
		{
			get { return new HTable<MM_Package>(this, Session.Query<MM_Package>()); }
		}

		public IQueryable<MM_ObjectType> MM_ObjectTypes
		{
			get { return new HTable<MM_ObjectType>(this, Session.Query<MM_ObjectType>()); }
		}

		public IQueryable<MM_ObjectProperty> MM_ObjectProperties
		{
			get { return new HTable<MM_ObjectProperty>(this, Session.Query<MM_ObjectProperty>()); }
		}

		public IQueryable<MM_Codifier> MM_Codifiers
		{
			get { return new HTable<MM_Codifier>(this, Session.Query<MM_Codifier>()); }
		}

		public IQueryable<MM_CodifierValue> MM_CodifierValues
		{
			get { return new HTable<MM_CodifierValue>(this, Session.Query<MM_CodifierValue>()); }
		}

		public IQueryable<MM_Method> MM_Methods
		{
			get { return new HTable<MM_Method>(this, Session.Query<MM_Method>()); }
		}

		public IQueryable<MM_FormView> MM_FormViews
		{
			get { return new HTable<MM_FormView>(this, Session.Query<MM_FormView>()); }
		}

		public IQueryable<N_ReplicationObject> N_ReplicationObjects
		{
			get { return new HTable<N_ReplicationObject>(this, Session.Query<N_ReplicationObject>()); }
		}

		public IQueryable<N_Cache> N_Caches
		{
			get { return new HTable<N_Cache>(this, Session.Query<N_Cache>()); }
		}

		public XDocument usp_model()
		{
			return Session.CreateSQLQuery("EXEC usp_model").UniqueResult<XDocument>();
		}

		public void MM_FormView_CreateHistoryVersion(int formViewID)
		{
			Session.CreateSQLQuery("EXEC MM_FormView_CreateHistoryVersion @id=:id").SetInt32("id", formViewID).UniqueResult();
		}

		public override IDataContext NewDataContext()
		{
			return new MMDataContext(DBConfig(ConnectionManager.ConnectionString));
		}

		public override IDataContext NewDataContext(string connectionString)
		{
			return new MMDataContext(DBConfig(connectionString));
		}
	}
}