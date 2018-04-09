using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq.SqlClient;
using System.Linq;

namespace Tango.Data.Linq.Test
{
    class Program
    {
        static void Main(string[] args)
        {
			var dc = new Solution.Model.modelDataContext("Database=licbank;UserId=postgres;Password=123*(iop;Server=149.202.216.11;Port=6543");
			dc.CommandTimeout = 300;

			//var data = dc.C_FGA.Where(oc => SqlMethods.Like(oc.Title, oc.ShortTitle + "%"));
			var data = dc.C_CNCategory
			.Where(oc =>
			oc.C_CNCategoryCN.Any(c => SqlMethods.Like("111", c.CN.Code + "%"))
			&& oc.C_CNCategoryOperationType.Any(o => o.OperationTypeID == 1)
			&& oc.Items.Any(o => o.LicenseTypeID == 1)).Distinct().OrderBy(c => c.CategoryCode);

			Console.WriteLine(data);
        }
    }
}
