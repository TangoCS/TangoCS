
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Xml.Linq;
using Tango;
using Tango.Data;
using Tango.Localization;

namespace Tango.FileStorage.Std.Model
{

	[Table("N_DownloadLog")]
	public partial class N_DownloadLog : IEntity, IWithKey<N_DownloadLog, int>, IWithTimeStamp, IWithLogicalDelete
	{
		public N_DownloadLog()
		{

		}


		public virtual Expression<Func<N_DownloadLog, bool>> KeySelector(int id)
		{
			return o => o.DownloadLogID == id;
		}
		public virtual int ID { get { return DownloadLogID; } }
		[Key]
		[Column]
		public virtual int DownloadLogID { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual bool IsDeleted { get; set; }
		[Column]
		public virtual Guid FileGUID { get; set; }
		[Column]
		public virtual string IP { get; set; }

		[Column]
		public virtual int LastModifiedUserID { get; set; }

		[Column]
		public virtual int FileID
		{
			get
			{

				if (File == null) return 0;
				return File.FileID;
			}
			set
			{

				File = new N_File { FileID = value };
			}
		}
		public virtual N_File File { get; set; }
	}

	[Table("N_File")]
	public partial class N_File : IEntity, IWithKey<N_File, int>, IWithTimeStamp, IWithTitle, IWithLogicalDelete
	{
		public N_File()
		{

		}


		public virtual Expression<Func<N_File, bool>> KeySelector(int id)
		{
			return o => o.FileID == id;
		}
		public virtual int ID { get { return FileID; } }
		[Key]
		[Column]
		public virtual int FileID { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual string Title { get; set; }
		[Column]
		public virtual string Password { get; set; }
		[Column]
		public virtual DateTime BeginDate { get; set; }
		[Column]
		public virtual DateTime EndDate { get; set; }
		[Column]
		public virtual string Extension { get; set; }
		[Column]
		public virtual Guid Guid { get; set; }
		[Column]
		public virtual string GuidPath { get; set; }
		[Column]
		public virtual string StorageType { get; set; }
		[Column]
		public virtual string Tag { get; set; }
		[Column]
		public virtual int VersionNumber { get; set; }
		[Column]
		public virtual DateTime? PublishDate { get; set; }
		[Column]
		public virtual long Length { get; set; }
		[Column]
		public virtual Guid? MainGUID { get; set; }
		[Column]
		public virtual string Path { get; set; }
		[Column]
		public virtual string StorageParameter { get; set; }
		[Column]
		public virtual bool IsDeleted { get; set; }

		[Column]
		public virtual int LastModifiedUserID { get; set; }

		[Column]
		public virtual int FolderID
		{
			get
			{

				if (Folder == null) return 0;
				return Folder.FolderID;
			}
			set
			{

				Folder = new N_Folder { FolderID = value };
			}
		}
		public virtual N_Folder Folder { get; set; }

		[Column]
		public virtual int? CheckedOutByID { get; set; }

		[Column]
		public virtual int CreatorID { get; set; }
	}

	[Table("N_FileData")]
	public partial class N_FileData : IEntity, IWithKey<N_FileData, Guid>, IWithTitle
	{
		public N_FileData()
		{
			FileGUID = Guid.NewGuid();

		}


		public virtual Expression<Func<N_FileData, bool>> KeySelector(Guid id)
		{
			return o => o.FileGUID == id;
		}
		public virtual Guid ID { get { return FileGUID; } }
		[Key]
		[Column]
		public virtual Guid FileGUID { get; set; }
		[Column]
		public virtual string Extension { get; set; }
		[Column]
		public virtual byte[] Data { get; set; }
		[Column]
		public virtual string Title { get; set; }
		[Column]
		public virtual long Size { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual Guid? Owner { get; set; }
	}

	[Table("N_FileLibrary")]
	public partial class N_FileLibrary : IEntity, IWithKey<N_FileLibrary, int>
	{
		public N_FileLibrary()
		{

		}


		public virtual Expression<Func<N_FileLibrary, bool>> KeySelector(int id)
		{
			return o => o.FileLibraryID == id;
		}
		public virtual int ID { get { return FileLibraryID; } }
		[Key]
		[Column]
		public virtual int FileLibraryID { get; set; }
		[Column]
		public virtual int MaxFileSize { get; set; }
		[Column]
		public virtual string StorageType { get; set; }
		[Column]
		public virtual string StorageParameter { get; set; }

		[Column]
		public virtual int FileLibraryTypeID
		{
			get
			{

				if (FileLibraryType == null) return 0;
				return FileLibraryType.FileLibraryTypeID;
			}
			set
			{

				FileLibraryType = new N_FileLibraryType { FileLibraryTypeID = value };
			}
		}
		public virtual N_FileLibraryType FileLibraryType { get; set; }
	}

	[Table("N_FileLibraryType")]
	public partial class N_FileLibraryType : IEntity, IWithKey<N_FileLibraryType, int>, IWithTimeStamp, IWithLogicalDelete, IWithTitle
	{
		public N_FileLibraryType()
		{

		}


		public virtual Expression<Func<N_FileLibraryType, bool>> KeySelector(int id)
		{
			return o => o.FileLibraryTypeID == id;
		}
		public virtual int ID { get { return FileLibraryTypeID; } }
		[Key]
		[Column]
		public virtual int FileLibraryTypeID { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual bool IsDeleted { get; set; }
		[Column]
		public virtual string Title { get; set; }
		[Column]
		public virtual string Extensions { get; set; }
		[Column]
		public virtual string ClassName { get; set; }

		[Column]
		public virtual int LastModifiedUserID { get; set; }
	}

	[Table("N_Folder")]
	public partial class N_Folder : IEntity, IWithKey<N_Folder, int>, IWithTimeStamp, IWithTitle
	{
		public N_Folder()
		{

		}


		public virtual Expression<Func<N_Folder, bool>> KeySelector(int id)
		{
			return o => o.FolderID == id;
		}
		public virtual int ID { get { return FolderID; } }
		[Key]
		[Column]
		public virtual int FolderID { get; set; }
		[Column]
		public virtual DateTime LastModifiedDate { get; set; }
		[Column]
		public virtual string Title { get; set; }
		[Column]
		public virtual bool IsDeleted { get; set; }
		[Column]
		public virtual string Path { get; set; }
		[Column]
		public virtual Guid Guid { get; set; }
		[Column]
		public virtual string GuidPath { get; set; }
		[Column]
		public virtual Guid SPMActionItemGUID { get; set; }

		[Column]
		public virtual int LastModifiedUserID { get; set; }

		[Column]
		public virtual int FileLibraryID
		{
			get
			{

				if (FileLibrary == null) return 0;
				return FileLibrary.FileLibraryID;
			}
			set
			{

				FileLibrary = new N_FileLibrary { FileLibraryID = value };
			}
		}
		public virtual N_FileLibrary FileLibrary { get; set; }

		[Column]
		public virtual int ParentID
		{
			get
			{

				if (Parent == null) return 0;
				return Parent.FolderID;
			}
			set
			{

				Parent = new N_Folder { FolderID = value };
			}
		}
		public virtual N_Folder Parent { get; set; }
	}
	public partial class V_N_FullFolder : IEntity, IWithKey<V_N_FullFolder, int>
	{
		public V_N_FullFolder() { }
		public virtual Expression<Func<V_N_FullFolder, bool>> KeySelector(int id)
		{
			return o => o.FolderID == id;
		}
		public virtual int ID { get { return FolderID; } }
		public virtual int FolderID { get; set; }
		public virtual int ParentID { get; set; }
		public virtual int ArcLen { get; set; }
	}
}
