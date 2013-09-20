using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var vd = new ViewData { UserName = "admin" };
			//var s = App.DataContext.SPM_Subject.Where(o => o.SystemName == vd.UserName).FirstOrDefault();
			//var c = App.DataContext.SPM_Subject.Count();
			//var s = Find(App.DataContext.SPM_Subject, vd.UserName);
			var s = App.DataContext.SPM_Subject.Where(o => o.LastModifiedUserID == 45 && o.SystemName == "admin").FirstOrDefault();
			Console.WriteLine(s.LastModifiedUser.Title);
			s.LastModifiedUserID = 2;
			//s.LastModifiedUser = new SPM_Subject { SubjectID = 45 };
			App.DataContext.SubmitChanges();
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

		int _LastModifiedUserID = 0;
		public virtual int LastModifiedUserID
		{
			get { return _LastModifiedUserID; }
			set { LastModifiedUser = new SPM_Subject { SubjectID = value }; }
		}
		public virtual SPM_Subject LastModifiedUser { get; set; }

		public System.Linq.Expressions.Expression<Func<SPM_Subject, bool>> KeySelector(int id)
		{
			return o => o.SubjectID == id;
		}
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
			return l;
		}

		public HTable<SPM_Subject> SPM_Subject
		{
			get
			{
				return new HTable<SPM_Subject>(this, Session.Query<SPM_Subject>());
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
