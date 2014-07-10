using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Nephrite.Meta;
using Nephrite.Meta.Database;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using Nephrite.Web;
using Nephrite.Web.Controls;
using Nephrite.Web.CoreDataContext;
using Nephrite.Web.FileStorage;
using Nephrite.Web.Hibernate;
using Nephrite.Web.MetaStorage;
using Nephrite.Web.SPM;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;


namespace Tessera.Test
{
    class Program
    {

		public static Expression<Func<dynamic, bool>> FindByProperty(Type t, string propertyName, object propertyValue)
		{
			PropertyInfo pi = t.GetProperty(propertyName);

			if (pi != null)
			{
				ParameterExpression pe_c = Expression.Parameter(t, "c");
				UnaryExpression ue_c = Expression.Convert(pe_c, t);
				MemberExpression me_id = Expression.Property(ue_c, pi.Name);
				if (propertyValue != null && propertyValue.GetType().IsArray)
				{
					Type arrayElementType = propertyValue.GetType().GetElementType();
					MethodInfo method = typeof(Enumerable).GetMethods()
					.Where(m => m.Name == "Contains" && m.GetParameters().Length == 2)
					.Single().MakeGenericMethod(arrayElementType);

					var callContains = Expression.Call(
						method,
						Expression.Convert(Expression.Constant(propertyValue, propertyValue.GetType()),
						typeof(IEnumerable<>).MakeGenericType(arrayElementType)),
						Expression.Convert(me_id, arrayElementType));
					return Expression.Lambda<Func<object, bool>>(callContains, pe_c);
				}
				else
				{
					BinaryExpression be_eq = Expression.Equal(me_id, Expression.Constant(propertyValue, pi.PropertyType));
					return Expression.Lambda<Func<object, bool>>(be_eq, pe_c);
				}
			}

			throw new Exception("В классе " + t.FullName + " не найдено свойство " + propertyName);
		}

        private static void Main(string[] args)
        {
			//A.DBScript = new DBScriptMSSQL("DBO");
			//ConnectionManager.SetConnectionString("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Servants;Data Source=(local)");
			//HDataContext.DBType = DBType.MSSQL;

            ConnectionManager.SetConnectionString("Database=SRVNTS;UserID=dbo;Password=123*(iop;Server=176.227.213.5:50000");
            HDataContext.DBType = DBType.DB2;
			A.DBScript = new DBScriptDB2("DBO");

			


	
			Listeners l = new Listeners();
			var ael = new AuditEventListener();
			l.PreDeleteEventListeners.Add(ael);
			//l.PreInsertEventListeners.Add(ael);
			l.PreUpdateEventListeners.Add(ael);
			//l.PostDeleteEventListeners.Add(ael);
			//l.PostInsertEventListeners.Add(ael);
			//l.PostUpdateEventListeners.Add(ael);

			A.Model = new HCoreDataContext(HCoreDataContext.DefaultDBConfig(ConnectionManager.ConnectionString), l);
			A.Model.ExecuteCommand("SET SCHEMA = 'DBO';");

			var schema = new DB2ServerMetadataReader().ReadSchema("DBO");

			var classes = MetaSolution.Load().Classes;

			A.Items["CurrentSubject2"] = Subject.FromLogin("Admin");
			var b = ActionSPMContext.Current.Check("ДОКУМЕНТЫ.VIEW", 1);
			//(App.DataContext as HDataContext).Session.EnableFilter("EMP").SetParameter("EmployeeID", "53216139-9773-4811-8181-1b56034fe90d");
			//var q = App.DataContext.F_DocTask.Where(o => o.DocTaskID == 379).ToList();

			//var f = Nephrite.Web.FileStorage.FileStorageManager.CreateFile("", "");
			//var f = FileStorageManager.DbFiles.First(o => o.ID == Guid.Parse("53216139-9773-4811-8181-1b56034fe90d"));
			//f.Tag = "1";
			//A.Model.SubmitChanges();
			//var r = A.Model.ExecuteQuery<int>("select ? from SPM_Subject where SubjectID = 2", 111);
			//var f = FileStorageManager.CreateFile("text.txt", "");
			//A.Model.SubmitChanges();


			//c = c.Where(obj.FindByProperty<dynamic>("ParentFolderID", null));
			//int i = c.Count();
 

			//var sqlServerMetadataReader = new DB2ServerMetadataReader();
			//var schema = sqlServerMetadataReader.ReadSchema("dbo");

          

            //Func<string, Expression<Func<SPM_Subject, bool>>> SearchExpression = s => (o => SqlMethods.Like(o.SystemName, "%" + s + "%"));

            //bool val = false;
            //Expression<Func<SPM_Subject, bool>> column = o => o.IsActive;
            //var expr = Expression.Lambda<Func<SPM_Subject, bool>>(Expression.Equal(column.Body, Expression.Constant(val)), column.Parameters);

            //IMM_FormView r = dc.IMM_FormView.First();
            //r = ApplyFilter(r, SearchExpression, "anonymous");
            //var r2 = r.First();
            //var r = App.DataContext.V_OrgUnit.Where(o => (o.ParentOrgUnitGUID ?? Guid.Empty) == new Guid("00000000-0000-0000-0000-000000000000")).ToList();
            //r.LastModifiedDate = DateTime.Now;
            //dc.SubmitChanges();



            //var r = dc.IMailMessage.Where(o => o.LastSendAttemptDate.HasValue && (o.LastSendAttemptDate - DateTime.Today) > new TimeSpan(o.AttemptsToSendCount, 0, 0, 0)).Select(o => o.MailMessageID).ToList();

            //Console.WriteLine(App.DataContext.Log.ToString());
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
