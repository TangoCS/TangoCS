using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	public interface IDC_FileStorage : IDataContext
	{
		IQueryable<IN_DownloadLog> IN_DownloadLog { get; }
		IQueryable<IN_VirusScanLog> IN_VirusScanLog { get; }

		IN_DownloadLog NewIN_DownloadLog();
		IN_VirusScanLog NewIN_VirusScanLog();

		IDbFolder NewIDbFolder();
		IDbFile NewIDbFile();
		IDbFileData NewIDbFileData();

		IQueryable<IDbFile> IDbFile { get; }
		IQueryable<IDbFolder> IDbFolder { get; }
		IQueryable<IDbItem> IDbItem { get; }
		IQueryable<IDbFileData> IDbFileData { get; }
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