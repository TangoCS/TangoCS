using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nephrite;
using Nephrite.Data;
using Nephrite.EntityAudit;
using Nephrite.Web;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.Hibernate;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Tessera.Test
{
	public class ViewData
	{
		public string UserName { get; set; }
	}


	public class SPM_Subject : IEntity, IWithTitle, IWithKey<SPM_Subject, int>, IWithTimeStamp, IWithPropertyAudit
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

		public int ID
		{
			get
			{
				return SubjectID;
			}
		}

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
			ManyToOne(x => x.LastModifiedUser, map => { map.Column("LastModifiedUserID"); });
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
		static HibernateDataContext _dataContext = new HibernateDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);

		public static HibernateDataContext DataContext
		{
			get { return _dataContext; }
		}
	}

	public class HibernateDataContext : HDataContext
	{
		public HibernateDataContext(Action<IDbIntegrationConfigurationProperties> dbConfig, Listeners l)
			: base(dbConfig, l)
		{
		}

		public override IEnumerable<Type> GetEntitiesTypes()
		{
			var emp = new FilterDefinition(
				"EMP",
				null, // or your default condition
				new Dictionary<string, IType> { { "EmployeeID", NHibernateUtil.String } },
				false);
			Configuration.AddFilterDefinition(emp);


			List<Type> l = new List<Type>();
			l.Add(typeof(SPM_SubjectMap));
			l.Add(typeof(EmployeeMap));
			l.Add(typeof(V_OrgUnitMap));
			l.Add(typeof(AppendixMap));
			l.Add(typeof(F_DocTaskMap));

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

		public HTable<F_DocTask> F_DocTask
		{
			get
			{
				return new HTable<F_DocTask>(this, Session.Query<F_DocTask>());
			}
		}


		public override IDataContext NewDataContext()
		{
			return new HibernateDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), null);
		}

		public override IDataContext NewDataContext(string connectionString)
		{
			return new HibernateDataContext(HCoreDataContext.DefaultDBConfig(connectionString), null);
		}
	}

	public partial class F_DocTask
	{
		public virtual Nullable<int> DocTaskID { get; set; }
		public virtual Nullable<bool> IsCheckDeadline { get; set; }
		public virtual string FullTitle { get; set; }
		public virtual string TypeTitle { get; set; }
		public virtual string PensionNo { get; set; }
		public virtual string RFSubjectTitle { get; set; }
		public virtual Nullable<DateTime> DecisionDocDate { get; set; }
		public virtual string RegNo { get; set; }
		public virtual Nullable<DateTime> RegDate { get; set; }
		public virtual string OrgUnitTitle { get; set; }
		public virtual Nullable<DateTime> PlanCompleteDate { get; set; }
		public virtual string PerformerTitle { get; set; }
		public virtual Nullable<bool> CitizenVIP { get; set; }
		public virtual Nullable<int> CompleteWarning { get; set; }
		//public virtual string Status { get; set; }
		public virtual Nullable<int> DocTaskResultID { get; set; }
		public virtual string DocTypeTitle { get; set; }
		public virtual Nullable<bool> IsDeleted { get; set; }
		public virtual Nullable<DateTime> CreateDate { get; set; }
		public virtual string CitizenTitle { get; set; }
		//public virtual Nullable<int> TypeID { get; set; }
	}

	public partial class F_DocTaskMap : ClassMapping<F_DocTask>
	{
		public F_DocTaskMap()
		{
			//Schema("dbo");
			Lazy(true);

			Table("TABLE(DBO.F_DocTask(:EMP.EmployeeID))");
			Id(x => x.DocTaskID);
			Property(x => x.IsCheckDeadline);
			Property(x => x.FullTitle);
			Property(x => x.TypeTitle);
			Property(x => x.PensionNo);
			Property(x => x.RFSubjectTitle);
			Property(x => x.DecisionDocDate);
			Property(x => x.RegNo);
			Property(x => x.RegDate);
			Property(x => x.OrgUnitTitle);
			Property(x => x.PlanCompleteDate);
			Property(x => x.PerformerTitle);
			Property(x => x.CitizenVIP);
			Property(x => x.CompleteWarning);
			//Property(x => x.Status);
			Property(x => x.DocTaskResultID);
			Property(x => x.DocTypeTitle);
			Property(x => x.IsDeleted);
			Property(x => x.CreateDate);
			Property(x => x.CitizenTitle);
			//Property(x => x.TypeID);
		}
	}
}
