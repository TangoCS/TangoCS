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
			//var l = App.DataContext.ExecuteQuery<SPM_Subject>("select Title from spm_subject");
			//var r = App.DataContext.ExecuteCommand("delete from spm_subject where systemname = ?", "ttt");
			//var l = App.DataContext.GetCommand(App.DataContext.GetTable<SPM_Subject>());
			//var l2 = App.DataContext.SPM_Subject.ToList();
			//var vd = new ViewData { UserName = "admin" };
			//var s = App.DataContext.SPM_Subject.Where(o => o.SystemName == vd.UserName).FirstOrDefault();
			//var s = Find(App.DataContext.SPM_Subject, vd.UserName);
			//Console.Write(s.Title);
			var s = App.DataContext.SPM_Subject.FirstOrDefault(o => o.SystemName.ToLower() == "admin");
			s.Title = "Администратор 2";
			App.DataContext.SubmitChanges();
			Console.Write(App.DataContext.Log.ToString());
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

	public class SPM_Subject : IWithTitle
	{
		public virtual int SubjectID { get; protected set; }
		public virtual string SystemName { get; set; }
		public virtual string Title { get; set; }
	}

	public class SPM_SubjectMap : ClassMapping<SPM_Subject>
	{
		public SPM_SubjectMap()
		{
			Table("SPM_Subject");
			Lazy(true);

			Id(x => x.SubjectID, map => map.Generator(Generators.Identity));
			Property(x => x.SystemName);
			Property(x => x.Title);
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
			throw new NotImplementedException();
		}

		public override IDataContext NewDataContext(string connectionString)
		{
			throw new NotImplementedException();
		}
	}


	



}
