using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	public interface IDC_FileStorage : IDataContext
	{
		IQueryable<IN_DownloadLog> N_DownloadLog { get; }
		IQueryable<IN_VirusScanLog> N_VirusScanLog { get; }

		IN_DownloadLog NewN_DownloadLog();
		IN_VirusScanLog NewN_VirusScanLog();

		IDbFolder NewDbFolder();
		IDbFile NewDbFile();
		IDbFileData NewDbFileData();

		IQueryable<IDbFile> DbFile { get; }
		IQueryable<IDbFolder> DbFolder { get; }
		IQueryable<IDbItem> DbItem { get; }
		IQueryable<IDbFileData> DbFileData { get; }
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

	/*public interface IN_File : IEntity
	{
		int FileID { get; set; }
		string Title { get; set; }
		int FolderID { get; set; }
		System.Guid Guid { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		long Length { get; set; }
		int? LastModifiedUserID { get; set; }
		int? CheckedOutByID { get; set; }
		int CreatorID { get; set; }
		bool IsDiskStorage { get; set; }
		string Extension { get; set; }
		string Path { get; set; }
		string StorageType { get; set; }
		string StorageParameter { get; set; }
		string Password { get; set; }
		string GuidPath { get; set; }
		System.Nullable<System.Guid> FeatureGUID { get; set; }
		System.DateTime BeginDate { get; set; }
		System.DateTime EndDate { get; set; }
		int VersionNumber { get; set; }
		System.Nullable<System.Guid> MainGUID { get; set; }
		string Tag { get; set; }
		System.Nullable<System.DateTime> PublishDate { get; set; }
	}
	public interface IN_Folder : IEntity
	{
		int FolderID { get; set; }
		int LastModifiedUserID { get; set; }
		int CreatorID { get; set; }
		string Title { get; set; }
		bool IsDeleted { get; set; }
		System.DateTime LastModifiedDate { get; set; }
		string FullPath { get; set; }
		System.Guid Guid { get; set; }
		string StorageType { get; set; }
		string StorageParameter { get; set; }
		string GuidPath { get; set; }
		bool IsReplicable { get; set; }
		System.Guid SPMActionItemGUID { get; set; }
		bool EnableVersioning { get; set; }
		string Tag { get; set; }
		System.Nullable<System.DateTime> PublishDate { get; set; }
	}*/

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