using System.Linq;
using Tango.Data;
using Abc.Model;
using Tango.FileStorage.Std.Model;

namespace Abc
{
	public static class FileStorageModule
	{
		public static void Config()
		{
			//MetaN_File.IsDeleted.Expression = "isnull(CONVERT([bit],case when [EndDate]>getdate() and [BeginDate]<getdate() then (0) else (1) end,(0)),(1))";

			DefaultTableFilters.AddFor<N_VirusScanLog>("sort", x => x.OrderByDescending(o => o.LastModifiedDate));
			DefaultTableFilters.AddFor<N_DownloadLog>("sort", x => x.OrderByDescending(o => o.LastModifiedDate));
        }
	}
}

