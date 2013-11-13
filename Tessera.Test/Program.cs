using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Metamodel;
using Nephrite.Web;
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
			//var fv = AppMM.DataContext.MM_FormViews.Count();
			//var fv1 = App.DataContext.MM_FormView.Where(o => o.FormViewID == 21).FirstOrDefault();
			//var ot = fv1.MM_ObjectType.SysName;
			//var fvs = AppMM.DataContext.MM_FormViews.Where(o => o.MM_ObjectType == null && o.SysName == "gpo_ReportRecipients").Select(o => o.MM_Package.SysName).FirstOrDefault() ?? "";
			//var ot = fv1.MM_ObjectType.SysName;
			//A.Model = new Nephrite.Web.CoreDataContext.HCoreDataContext(Nephrite.Web.Hibernate.HDataContext.DBConfig(ConnectionManager.ConnectionString));//Solution.App.DataContext;
			var r = App.DataContext.ExecuteQuery<int>("select RoleID from V_SPM_AllSubjectRole where SubjectID = ?", 2).ToList();
			//var a = A.Model.ExecuteQuery<SPM_Subject>("select SubjectID, SystemName, Title from SPM_Subject where lower(SystemName) = ?", "Admin").SingleOrDefault();
			//var vd = new ViewData { UserName = "admin" };
			//var s = App.DataContext.SPM_Subject.Where(o => o.SystemName == vd.UserName).FirstOrDefault();
			//var c = App.DataContext.SPM_Subject.Count(); 
			//var s = Find(App.DataContext.SPM_Subject, vd.UserName);
			//var c = App.DataContext.SPM_Subject.Count();
			//var s = App.DataContext.SPM_Subject.Where(o => o.SystemName == "Admin").FirstOrDefault();
			//Console.WriteLine(s.Title);
			//s.LastModifiedUserID = 2;
			//s.LastModifiedUser = new SPM_Subject { SubjectID = 45 };
			//App.DataContext.SubmitChanges();
			Console.WriteLine(App.DataContext.Log.ToString());
			Console.ReadKey();
		}

		public static T Find<T>(IQueryable<T> collection, string name)
			where T : IWithTitle
		{
			return collection.FirstOrDefault(o => o.SystemName == name);
		}
	}

	public class ViewData
	{
		public string UserName { get; set; }
	}

	public interface IWithTitle
	{
		string SystemName { get; set; }
		string Title { get; set; }
	}

	public class SPM_Subject : IEntity, IWithTitle, IWithKey<SPM_Subject, int>
	{
		public virtual int SubjectID { get; set; }
		public virtual string SystemName { get; set; }
		public virtual string Title { get; set; }
		//public virtual int LastModifiedUserID { get; set; }

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
	}

	public partial class MM_FormView
	{
		public MM_FormView()
		{
			//MM_Methods = new List<MM_Method>();
		}
		public virtual int FormViewID { get; set; }
		
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual string ViewTemplate { get; set; }
		public virtual string TemplateTypeCode { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsCustom { get; set; }
		public virtual bool IsDeleted { get; set; }
		
		public virtual int LastModifiedUserID
		{
			get { return LastModifiedUser.SubjectID; }
			set { LastModifiedUser = new SPM_Subject { SubjectID = value }; }
		}
		public virtual SPM_Subject LastModifiedUser { get; set; }

		public virtual bool IsCaching { get; set; }
		public virtual string CacheKeyParams { get; set; }
		public virtual int CacheTimeout { get; set; }
		public virtual string BaseClass { get; set; }
		//public virtual IList<MM_Method> MM_Methods { get; set; }

		//
		
		public virtual Nullable<int> ObjectTypeID
		{
			get
			{
				return MM_ObjectType.ObjectTypeID;
			}
			set
			{
				if (value == null) return;
				MM_ObjectType = new MM_ObjectType { ObjectTypeID = value.Value };
			}
		}
		public virtual MM_ObjectType MM_ObjectType 
		{ 
			get; 
			set;
		}
		


	}

	public partial class MM_ObjectType
	{
		public virtual int ObjectTypeID { get; set; }
		public virtual string Title { get; set; }
		public virtual string SysName { get; set; }
		public virtual bool IsEnableSPM { get; set; }
		public virtual System.Guid Guid { get; set; }
		public virtual bool IsSeparateTable { get; set; }
		public virtual bool IsTemplate { get; set; }
		public virtual string TitlePlural { get; set; }
		public virtual string DefaultOrderBy { get; set; }
		public virtual string LogicalDelete { get; set; }
		public virtual bool IsReplicate { get; set; }
		public virtual bool IsEnableUserViews { get; set; }
		public virtual string SecurityPackageSystemName { get; set; }
		public virtual bool IsEnableObjectHistory { get; set; }
		public virtual string Interface { get; set; }
		public virtual string HistoryTypeCode { get; set; }
		public virtual bool IsDataReplicated { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual System.DateTime LastModifiedDate { get; set; }
		public virtual int LastModifiedUserID { get; set; }
		public virtual int SeqNo { get; set; }
		public virtual string Description { get; set; }
	}

	public class SPM_SubjectMap : ClassMapping<SPM_Subject>
	{
		public SPM_SubjectMap()
		{
			Id(x => x.SubjectID, map => map.Generator(Generators.Identity));
			Property(x => x.SystemName);
			Property(x => x.Title);

			Property(x => x.LastModifiedUserID, map => map.Formula("LastModifiedUserID"));
			ManyToOne(x => x.LastModifiedUser, map => { map.Column("LastModifiedUserID"); map.Cascade(Cascade.None); });
		}
	}

	public class MM_ObjectTypeMap : ClassMapping<MM_ObjectType>
	{
		public MM_ObjectTypeMap()
		{
			//Schema("dbo");
			//Lazy(true);
			Id(x => x.ObjectTypeID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsEnableSPM, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsSeparateTable, map => map.NotNullable(true));
			Property(x => x.IsTemplate, map => map.NotNullable(true));
			Property(x => x.TitlePlural);
			Property(x => x.DefaultOrderBy);
			Property(x => x.LogicalDelete);
			Property(x => x.IsReplicate, map => map.NotNullable(true));
			Property(x => x.IsEnableUserViews, map => map.NotNullable(true));
			Property(x => x.SecurityPackageSystemName);
			Property(x => x.IsEnableObjectHistory, map => map.NotNullable(true));
			Property(x => x.Interface);
			Property(x => x.HistoryTypeCode, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.Description);
		}
	}

	public class MM_FormViewMap : ClassMapping<MM_FormView>
	{
		public MM_FormViewMap()
		{
			//Schema("dbo");
			//Lazy(true);
			Id(x => x.FormViewID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.ViewTemplate);
			Property(x => x.TemplateTypeCode);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsCustom, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));

			Property(x => x.LastModifiedUserID, map => { map.Formula("LastModifiedUserID");  });
			ManyToOne(x => x.LastModifiedUser, map => { map.Column("LastModifiedUserID"); map.Cascade(Cascade.None); });

			Property(x => x.IsCaching, map => map.NotNullable(true));
			Property(x => x.CacheKeyParams);
			Property(x => x.CacheTimeout, map => map.NotNullable(true));
			Property(x => x.BaseClass, map => map.NotNullable(true));

			ManyToOne(x => x.MM_ObjectType, map =>
			{
				map.Column("ObjectTypeID");
				map.Cascade(Cascade.None);
			});
			Property(x => x.ObjectTypeID, map => { map.Formula("ObjectTypeID"); } );

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
			l.Add(typeof(MM_ObjectTypeMap));
			l.Add(typeof(MM_FormViewMap));
			return l;
		}

		public HTable<SPM_Subject> SPM_Subject
		{
			get
			{
				return new HTable<SPM_Subject>(this, Session.Query<SPM_Subject>());
			}
		}

		public HTable<MM_ObjectType> MM_ObjectType
		{
			get
			{
				return new HTable<MM_ObjectType>(this, Session.Query<MM_ObjectType>());
			}
		}

		public HTable<MM_FormView> MM_FormView
		{
			get
			{
				return new HTable<MM_FormView>(this, Session.Query<MM_FormView>());
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
