using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	public interface IDC_FileStorage : IDataContext
	{
		ITable<IN_DownloadLog> IN_DownloadLog { get; }
		ITable<IN_VirusScanLog> IN_VirusScanLog { get; }

		IN_DownloadLog NewIN_DownloadLog();
		IN_VirusScanLog NewIN_VirusScanLog();

		IDbFolder NewIDbFolder(Guid ID);
		IDbFile NewIDbFile(Guid ID);
		IDbFileData NewIDbFileData(Guid ID);

		ITable<IDbFile> IDbFile { get; }
		ITable<IDbFolder> IDbFolder { get; }
		ITable<IDbItem> IDbItem { get; }
		ITable<IDbFileData> IDbFileData { get; }
	}

	public interface IN_DownloadLog : IEntity
	{
		int DownloadLogID { get; set; }
		int LastModifiedUserID { get; set; }
		Guid FileGUID { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string IP { get; set; }
	}

	public interface IDbFileData : IEntity
	{
		byte[] Data { get; set; }
		string Extension { get; set; }
		Guid FileGUID { get; set; }
	}

	public interface IN_VirusScanLog : IEntity
	{
		int VirusScanLogID { get; set; }
		int LastModifiedUserID { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string Title { get; set; }
		int ResultCode { get; set; }
	}
}