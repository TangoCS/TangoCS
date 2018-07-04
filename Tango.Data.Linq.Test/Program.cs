using Nephrite.Web;
using Nephrite.Web.FileStorage;
using Solution;
using Solution.Model;
using System;
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
			dc.CommandTimeout = 300;
			App.DataContext = dc;
			Base.Model = dc;

			//System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();

			//builder.ConnectionString = dc.Connection.ConnectionString;

			//string server = builder["Data Source"] as string;
			//string database = builder["Initial Catalog"] as string;

			dc.MM_FormView_CreateHistoryVersion(1091);

			//FileStorageManager.FileRepository = new Solution.Features.FileStorage.FileRepository();
			//FileStorageManager.GetFile(new Guid("b0e5c545-6ef8-487f-aed4-4a7378fdf0ef"));


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

	class MenuItem
	{
		public bool EnableSPM { get; set; }
	}
}
