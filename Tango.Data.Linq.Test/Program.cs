using Nephrite.Metamodel;
using Nephrite.Web;
using Nephrite.Web.FileStorage;
using Nephrite.Web.SPM;
using Solution;
using Solution.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Linq;

namespace Tango.Data.Linq.Test
{
    class Program
    {
        static void Main(string[] args)
        {
			DataContext.ProviderFactory = () => new PgProvider();
			var dc = new Solution.Model.modelDataContext("Database=licweb;UserId=postgres;Password=1;Server=localhost;Port=5433");
			var mmdc = new Nephrite.Metamodel.Model.modelDataContext("Database=licweb;UserId=postgres;Password=1;Server=localhost;Port=5433");
			var webdc = new Nephrite.Web.Model.modelDataContext("Database=licweb;UserId=postgres;Password=1;Server=localhost;Port=5433");

			dc.CommandTimeout = 300;
			App.DataContext = dc;
			Base.Model = dc;
			AppWeb.DataContext = webdc;

			SPMContextBase.PredicateLoader = new SPM2PredicateLoader();

			//CreateLicense.CheckDifference(

			//System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();

			//builder.ConnectionString = dc.Connection.ConnectionString;

			//string server = builder["Data Source"] as string;
			//string database = builder["Initial Catalog"] as string;

			//var res = dc.MM_FormView_CreateHistoryVersion(1091);
			//var res = dc.V_News.Where(o => o.IsPublished).Select(o => o.NewsDate.Year).Distinct().OrderBy(o => o).ToList();

			//FileStorageManager.FileRepository = new Solution.Features.FileStorage.FileRepository();
			//FileStorageManager.FileRepository.DbFiles
			//	.Cast<DbFile>()
			//	.Where(o => o.ID == new Guid("b0e5c545-6ef8-487f-aed4-4a7378fdf0ef"))
			//	.Select(o => o.DataHash)
			//	.FirstOrDefault();

			//FileStorageManager.GetFile(new Guid("b0e5c545-6ef8-487f-aed4-4a7378fdf0ef"));
			//byte[] data;
			//string fileName;
			//string contentType;
			//bool flag = FileProvider.GetFile("/?oid=1052", "0:0:0:0", true, out data, out fileName, out contentType);

			//var f = FileStorageManager.GetFile(new Guid("b0e5c545-6ef8-487f-aed4-4a7378fdf0ef"));

			//var obj = new N_VirusScanLog();
			//dc.N_VirusScanLog.InsertOnSubmit(obj);
			//obj.Title = "test";
			//obj.ResultCode = 1;
			//obj.LastModifiedUserID = 2;
			//obj.LastModifiedDate = DateTime.Now;

			//dc.SubmitChanges();

			//var data = dc.C_FGA.Where(oc => SqlMethods.Like(oc.Title, oc.ShortTitle + "%"));
			//var data = (from i in mmdc.N_MenuItems
			//			select new MenuItem {
			//				EnableSPM = i.IsDeleted ? i.IsVisible : false
			//			}).ToList();

			//Console.WriteLine(data);
		}
    }

	public class SPM2PredicateLoader : Nephrite.Web.SPM.IPredicateLoader
	{
		public void Load(Dictionary<string, Func<PredicateEvaluationContext, bool>> list)
		{
			var t = new SPM2Predicates();
			t.Init(list);
		}
	}

	public class SPM2Predicates
	{
		Dictionary<Guid, Func<PredicateEvaluationContext, bool>> predicates;

		public void Init(Dictionary<string, Func<PredicateEvaluationContext, bool>> list)
		{
			predicates = new Dictionary<Guid, Func<PredicateEvaluationContext, bool>>();
		}
	}

	class MenuItem
	{
		public bool EnableSPM { get; set; }
	}
}
