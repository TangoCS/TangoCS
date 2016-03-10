using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nephrite.Data;

namespace Nephrite.FileStorage.Std
{
	public interface IDC_DownloadLog : IDataContext
	{
		IQueryable<IN_DownloadLog> IN_DownloadLog { get; }
		IN_DownloadLog NewIN_DownloadLog();
	}

	public interface IN_DownloadLog : IEntity
	{
		int DownloadLogID { get; set; }
		int LastModifiedUserID { get; set; }
		Guid FileGUID { get; set; }
		bool IsDeleted { get; set; }
		DateTime LastModifiedDate { get; set; }
		string IP { get; set; }
	}
}
