using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nephrite.Metamodel;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.Hibernate;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace Tessera.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			A.Model = new HCoreDataContext(HDataContext.DBConfig(ConnectionManager.ConnectionString));
			Func<string, Expression<Func<SPM_Subject, bool>>> SearchExpression = s => (o => SqlMethods.Like(o.SystemName, "%" + s + "%"));

			bool val = false;
			Expression<Func<SPM_Subject, bool>> column = o => o.IsActive;
			var expr = Expression.Lambda<Func<SPM_Subject, bool>>(Expression.Equal(column.Body, Expression.Constant(val)), column.Parameters);

			IQueryable<SPM_Subject> r = App.DataContext.SPM_Subject.Where(expr);
			//r = ApplyFilter(r, SearchExpression, "anonymous");
			var r2 = r.First();
			//var r = App.DataContext.V_OrgUnit.Where(o => (o.ParentOrgUnitGUID ?? Guid.Empty) == new Guid("00000000-0000-0000-0000-000000000000")).ToList();
			r2.LastModifiedDate = DateTime.Now;
			App.DataContext.SubmitChanges();

			Console.WriteLine(App.DataContext.Log.ToString());
			Console.WriteLine(A.Model.Log.ToString());
			Console.ReadKey();
		}

		public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Func<string, Expression<Func<T, bool>>> SearchExpression, string val)
			where T : class
		{
			return query.Where(SearchExpression(val));
		}


	}

	public class ViewData
	{
		public string UserName { get; set; }
	}


	public class SPM_Subject : IEntity, IWithTitle, IWithKey<SPM_Subject, int>, IWithTimeStamp
	{
		public virtual int SubjectID { get; set; }
		public virtual string SystemName { get; set; }
		public virtual string Title { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual DateTime LastModifiedDate { get; set; }

		//int _LastModifiedUserID = 0;
		public virtual int LastModifiedUserID
		{
			get { return LastModifiedUser.SubjectID; }
			set { LastModifiedUser = new SPM_Subject { SubjectID = value }; }
		}
		public virtual SPM_Subject LastModifiedUser { get; set; }

		public virtual System.Linq.Expressions.Expression<Func<SPM_Subject, bool>> KeySelector(int id)
		{
			return o => o.SubjectID == id;
		}

		public virtual string GetTitle()
		{
			return Title;
		}
	}

	public class Employee
	{
		public virtual Guid EmployeeGUID { get; set; }
		public virtual Guid OrgUnitGUID { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			return true;
		}
		public override int GetHashCode()
		{
			return EmployeeGUID.GetHashCode();
		} 


	}

	public class Appendix
	{
		public virtual int AppendixID { get; set; }
		public virtual Guid? FileGUID { get; set; }
	}

	public class V_OrgUnit
	{
		public virtual Guid OrgUnitGUID { get; set; }
		public virtual Guid? ParentOrgUnitGUID { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			return true;
		}
		public override int GetHashCode()
		{
			return OrgUnitGUID.GetHashCode();
		} 
	}




	public class SPM_SubjectMap : ClassMapping<SPM_Subject>
	{
		public SPM_SubjectMap()
		{
			Id(x => x.SubjectID, map => map.Generator(Generators.Identity));
			Property(x => x.SystemName);
			Property(x => x.Title);
			Property(x => x.IsActive, map => map.Type<IntBackedBoolUserType>());
			Property(x => x.LastModifiedDate);

			Property(x => x.LastModifiedUserID, map => map.Formula("LastModifiedUserID"));
			ManyToOne(x => x.LastModifiedUser, map => { map.Column("LastModifiedUserID"); map.Cascade(Cascade.None); });
		}
	}

	public class AppendixMap : ClassMapping<Appendix>
	{
		public AppendixMap()
		{
			Id(x => x.AppendixID, map => map.Generator(Generators.Identity));
			Property(x => x.FileGUID, map => map.Type<StringBackedGuidUserType>());
		}
	}

	public class V_OrgUnitMap : ClassMapping<V_OrgUnit>
	{
		public V_OrgUnitMap()
		{
			ComposedId(i => i.Property(p => p.OrgUnitGUID, map =>
			{
				map.Column("OrgUnitGUID");
				map.Type<StringBackedGuidUserType>();
			}));

			Property(x => x.ParentOrgUnitGUID, map => { map.Type<StringBackedGuidUserType>(); });
		}
	}
	public class EmployeeMap : ClassMapping<Employee>
	{
		public EmployeeMap()
		{
			ComposedId(i => i.Property(p => p.EmployeeGUID, map =>
			{
				map.Column("EmployeeGUID");
				map.Type<StringBackedGuidUserType>();
			}));
			Property(x => x.OrgUnitGUID, map => map.Type<StringBackedGuidUserType>());
		}
	}

	public class App
	{
		static HibernateDataContext _dataContext = new HibernateDataContext(HDataContext.DBConfig(ConnectionManager.ConnectionString));

		public static HibernateDataContext DataContext
		{
			get { return _dataContext; }
		}
	}

	public class HibernateDataContext : HDataContext
	{
		public HibernateDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig)
			: base(dbConfig)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			List<Type> l = new List<Type>();
			l.Add(typeof(SPM_SubjectMap));
			l.Add(typeof(EmployeeMap));
			l.Add(typeof(V_OrgUnitMap));
			l.Add(typeof(AppendixMap));

			return l;
		}

		public HTable<SPM_Subject> SPM_Subject
		{
			get
			{
				return new HTable<SPM_Subject>(this, Session.Query<SPM_Subject>());
			}
		}
		public HTable<Employee> Employee
		{
			get
			{
				return new HTable<Employee>(this, Session.Query<Employee>());
			}
		}
		public HTable<V_OrgUnit> V_OrgUnit
		{
			get
			{
				return new HTable<V_OrgUnit>(this, Session.Query<V_OrgUnit>());
			}
		}
		public HTable<Appendix> Appendix
		{
			get
			{
				return new HTable<Appendix>(this, Session.Query<Appendix>());
			}
		}


		public override IDataContext NewDataContext()
		{
			return new HibernateDataContext(DBConfig(ConnectionManager.ConnectionString));
		}

		public override IDataContext NewDataContext(string connectionString)
		{
			return new HibernateDataContext(DBConfig(connectionString));
		}
	}


}
