 
using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Tango;
using Tango.Meta;
using Abc.Model;

namespace Tango.FileStorage.Std.Model
{
	public partial class MetaN_Folder : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_Folder, int>> _FolderID;
		Lazy<MetaAttribute<N_Folder, DateTime>> _LastModifiedDate;
		Lazy<MetaReference<N_Folder, SPM_Subject, int>> _LastModifiedUser;
		Lazy<MetaAttribute<N_Folder, string>> _Title;
		Lazy<MetaAttribute<N_Folder, bool>> _IsDeleted;
		Lazy<MetaAttribute<N_Folder, string>> _Path;
		Lazy<MetaAttribute<N_Folder, Guid>> _Guid;
		Lazy<MetaAttribute<N_Folder, string>> _GuidPath;
		Lazy<MetaAttribute<N_Folder, Guid>> _SPMActionItemGUID;
		Lazy<MetaReference<N_Folder, N_FileLibrary, int>> _FileLibrary;
		Lazy<MetaReference<N_Folder, N_Folder, int>> _Parent;
		Lazy<MetaReference<N_Folder, N_Folder>> _Folders;
		Lazy<MetaReference<N_Folder, N_File>> _Files;
		Lazy<IMetaOperation> _CreateNew;
		Lazy<IMetaOperation> _ViewList;
		Lazy<IMetaOperation> _Upload;
		Lazy<IMetaOperation> _Edit;
		Lazy<IMetaOperation> _View;
		Lazy<IMetaOperation> _Delete;
		Lazy<IMetaOperation> _PackAndDownload;

		public MetaAttribute<N_Folder, int> FolderID => _FolderID.Value;
		public MetaAttribute<N_Folder, DateTime> LastModifiedDate => _LastModifiedDate.Value;
		public MetaReference<N_Folder, SPM_Subject, int> LastModifiedUser => _LastModifiedUser.Value;
		public MetaAttribute<N_Folder, string> Title => _Title.Value;
		public MetaAttribute<N_Folder, bool> IsDeleted => _IsDeleted.Value;
		public MetaAttribute<N_Folder, string> Path => _Path.Value;
		public MetaAttribute<N_Folder, Guid> Guid => _Guid.Value;
		public MetaAttribute<N_Folder, string> GuidPath => _GuidPath.Value;
		public MetaAttribute<N_Folder, Guid> SPMActionItemGUID => _SPMActionItemGUID.Value;
		public MetaReference<N_Folder, N_FileLibrary, int> FileLibrary => _FileLibrary.Value;
		public MetaReference<N_Folder, N_Folder, int> Parent => _Parent.Value;
		public MetaReference<N_Folder, N_Folder> Folders => _Folders.Value;
		public MetaReference<N_Folder, N_File> Files => _Files.Value;
		public IMetaOperation CreateNew => _CreateNew.Value;
		public IMetaOperation ViewList => _ViewList.Value;
		public IMetaOperation Upload => _Upload.Value;
		public IMetaOperation Edit => _Edit.Value;
		public IMetaOperation View => _View.Value;
		public IMetaOperation Delete => _Delete.Value;
		public IMetaOperation PackAndDownload => _PackAndDownload.Value;

		public MetaN_Folder()		{
			_metaClass = new MetaClass(typeof(N_Folder));
			_builder.N_Folder(_metaClass);
			_FolderID = new Lazy<MetaAttribute<N_Folder, int>>(() => _metaClass.GetProperty("FolderID") as MetaAttribute<N_Folder, int>);
			_LastModifiedDate = new Lazy<MetaAttribute<N_Folder, DateTime>>(() => _metaClass.GetProperty("LastModifiedDate") as MetaAttribute<N_Folder, DateTime>);
			_LastModifiedUser = new Lazy<MetaReference<N_Folder, SPM_Subject, int>>(() => _metaClass.GetProperty("LastModifiedUser") as MetaReference<N_Folder, SPM_Subject, int>);
			_Title = new Lazy<MetaAttribute<N_Folder, string>>(() => _metaClass.GetProperty("Title") as MetaAttribute<N_Folder, string>);
			_IsDeleted = new Lazy<MetaAttribute<N_Folder, bool>>(() => _metaClass.GetProperty("IsDeleted") as MetaAttribute<N_Folder, bool>);
			_Path = new Lazy<MetaAttribute<N_Folder, string>>(() => _metaClass.GetProperty("Path") as MetaAttribute<N_Folder, string>);
			_Guid = new Lazy<MetaAttribute<N_Folder, Guid>>(() => _metaClass.GetProperty("Guid") as MetaAttribute<N_Folder, Guid>);
			_GuidPath = new Lazy<MetaAttribute<N_Folder, string>>(() => _metaClass.GetProperty("GuidPath") as MetaAttribute<N_Folder, string>);
			_SPMActionItemGUID = new Lazy<MetaAttribute<N_Folder, Guid>>(() => _metaClass.GetProperty("SPMActionItemGUID") as MetaAttribute<N_Folder, Guid>);
			_FileLibrary = new Lazy<MetaReference<N_Folder, N_FileLibrary, int>>(() => _metaClass.GetProperty("FileLibrary") as MetaReference<N_Folder, N_FileLibrary, int>);
			_Parent = new Lazy<MetaReference<N_Folder, N_Folder, int>>(() => _metaClass.GetProperty("Parent") as MetaReference<N_Folder, N_Folder, int>);
			_Folders = new Lazy<MetaReference<N_Folder, N_Folder>>(() => _metaClass.GetProperty("Folders") as MetaReference<N_Folder, N_Folder>);
			_Files = new Lazy<MetaReference<N_Folder, N_File>>(() => _metaClass.GetProperty("Files") as MetaReference<N_Folder, N_File>);
			_CreateNew = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("CreateNew"));
			_ViewList = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("ViewList"));
			_Upload = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Upload"));
			_Edit = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Edit"));
			_View = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("View"));
			_Delete = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Delete"));
			_PackAndDownload = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("PackAndDownload"));

			FolderID.GetValue = o => o.FolderID;
			FolderID.GetValueExpression = o => o.FolderID;
			FolderID.SetValue = (o, v) => o.FolderID = v;
			LastModifiedDate.GetValue = o => o.LastModifiedDate;
			LastModifiedDate.GetValueExpression = o => o.LastModifiedDate;
			LastModifiedDate.SetValue = (o, v) => o.LastModifiedDate = v;
			LastModifiedUser.GetValue = o => o.LastModifiedUser;
			LastModifiedUser.GetValueExpression = o => o.LastModifiedUser;
			LastModifiedUser.SetValue = (o, v) => o.LastModifiedUser = v;
			LastModifiedUser.GetValueID = o => o.LastModifiedUserID;
			LastModifiedUser.SetValueID = (o, v) => o.LastModifiedUserID = v;
			Title.GetValue = o => o.Title;
			Title.GetValueExpression = o => o.Title;
			Title.SetValue = (o, v) => o.Title = v;
			IsDeleted.GetValue = o => o.IsDeleted;
			IsDeleted.GetValueExpression = o => o.IsDeleted;
			IsDeleted.SetValue = (o, v) => o.IsDeleted = v;
			Path.GetValue = o => o.Path;
			Path.GetValueExpression = o => o.Path;
			Path.SetValue = (o, v) => o.Path = v;
			Guid.GetValue = o => o.Guid;
			Guid.GetValueExpression = o => o.Guid;
			Guid.SetValue = (o, v) => o.Guid = v;
			GuidPath.GetValue = o => o.GuidPath;
			GuidPath.GetValueExpression = o => o.GuidPath;
			GuidPath.SetValue = (o, v) => o.GuidPath = v;
			SPMActionItemGUID.GetValue = o => o.SPMActionItemGUID;
			SPMActionItemGUID.GetValueExpression = o => o.SPMActionItemGUID;
			SPMActionItemGUID.SetValue = (o, v) => o.SPMActionItemGUID = v;
			FileLibrary.GetValue = o => o.FileLibrary;
			FileLibrary.GetValueExpression = o => o.FileLibrary;
			FileLibrary.SetValue = (o, v) => o.FileLibrary = v;
			FileLibrary.GetValueID = o => o.FileLibraryID;
			FileLibrary.SetValueID = (o, v) => o.FileLibraryID = v;
			Parent.GetValue = o => o.Parent;
			Parent.GetValueExpression = o => o.Parent;
			Parent.SetValue = (o, v) => o.Parent = v;
			Parent.GetValueID = o => o.ParentID;
			Parent.SetValueID = (o, v) => o.ParentID = v;
		}

	}
	public partial class MetaN_File : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_File, int>> _FileID;
		Lazy<MetaAttribute<N_File, DateTime>> _LastModifiedDate;
		Lazy<MetaReference<N_File, SPM_Subject, int>> _LastModifiedUser;
		Lazy<MetaAttribute<N_File, string>> _Title;
		Lazy<MetaAttribute<N_File, string>> _Password;
		Lazy<MetaAttribute<N_File, DateTime>> _BeginDate;
		Lazy<MetaAttribute<N_File, DateTime>> _EndDate;
		Lazy<MetaAttribute<N_File, string>> _Extension;
		Lazy<MetaAttribute<N_File, Guid>> _Guid;
		Lazy<MetaAttribute<N_File, string>> _GuidPath;
		Lazy<MetaAttribute<N_File, string>> _StorageType;
		Lazy<MetaAttribute<N_File, string>> _Tag;
		Lazy<MetaAttribute<N_File, int>> _VersionNumber;
		Lazy<MetaAttribute<N_File, Nullable<DateTime>>> _PublishDate;
		Lazy<MetaAttribute<N_File, long>> _Length;
		Lazy<MetaAttribute<N_File, Nullable<Guid>>> _MainGUID;
		Lazy<MetaAttribute<N_File, string>> _Path;
		Lazy<MetaAttribute<N_File, string>> _StorageParameter;
		Lazy<MetaPersistentComputedAttribute<N_File, bool>> _IsDeleted;
		Lazy<MetaReference<N_File, N_Folder, int>> _Folder;
		Lazy<MetaReference<N_File, SPM_Subject, Nullable<int>>> _CheckedOutBy;
		Lazy<MetaReference<N_File, SPM_Subject, int>> _Creator;
		Lazy<IMetaOperation> _Delete;
		Lazy<IMetaOperation> _CreateNew;
		Lazy<IMetaOperation> _Edit;

		public MetaAttribute<N_File, int> FileID => _FileID.Value;
		public MetaAttribute<N_File, DateTime> LastModifiedDate => _LastModifiedDate.Value;
		public MetaReference<N_File, SPM_Subject, int> LastModifiedUser => _LastModifiedUser.Value;
		public MetaAttribute<N_File, string> Title => _Title.Value;
		public MetaAttribute<N_File, string> Password => _Password.Value;
		public MetaAttribute<N_File, DateTime> BeginDate => _BeginDate.Value;
		public MetaAttribute<N_File, DateTime> EndDate => _EndDate.Value;
		public MetaAttribute<N_File, string> Extension => _Extension.Value;
		public MetaAttribute<N_File, Guid> Guid => _Guid.Value;
		public MetaAttribute<N_File, string> GuidPath => _GuidPath.Value;
		public MetaAttribute<N_File, string> StorageType => _StorageType.Value;
		public MetaAttribute<N_File, string> Tag => _Tag.Value;
		public MetaAttribute<N_File, int> VersionNumber => _VersionNumber.Value;
		public MetaAttribute<N_File, Nullable<DateTime>> PublishDate => _PublishDate.Value;
		public MetaAttribute<N_File, long> Length => _Length.Value;
		public MetaAttribute<N_File, Nullable<Guid>> MainGUID => _MainGUID.Value;
		public MetaAttribute<N_File, string> Path => _Path.Value;
		public MetaAttribute<N_File, string> StorageParameter => _StorageParameter.Value;
		public MetaPersistentComputedAttribute<N_File, bool> IsDeleted => _IsDeleted.Value;
		public MetaReference<N_File, N_Folder, int> Folder => _Folder.Value;
		public MetaReference<N_File, SPM_Subject, Nullable<int>> CheckedOutBy => _CheckedOutBy.Value;
		public MetaReference<N_File, SPM_Subject, int> Creator => _Creator.Value;
		public IMetaOperation Delete => _Delete.Value;
		public IMetaOperation CreateNew => _CreateNew.Value;
		public IMetaOperation Edit => _Edit.Value;

		public MetaN_File()		{
			_metaClass = new MetaClass(typeof(N_File));
			_builder.N_File(_metaClass);
			_FileID = new Lazy<MetaAttribute<N_File, int>>(() => _metaClass.GetProperty("FileID") as MetaAttribute<N_File, int>);
			_LastModifiedDate = new Lazy<MetaAttribute<N_File, DateTime>>(() => _metaClass.GetProperty("LastModifiedDate") as MetaAttribute<N_File, DateTime>);
			_LastModifiedUser = new Lazy<MetaReference<N_File, SPM_Subject, int>>(() => _metaClass.GetProperty("LastModifiedUser") as MetaReference<N_File, SPM_Subject, int>);
			_Title = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("Title") as MetaAttribute<N_File, string>);
			_Password = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("Password") as MetaAttribute<N_File, string>);
			_BeginDate = new Lazy<MetaAttribute<N_File, DateTime>>(() => _metaClass.GetProperty("BeginDate") as MetaAttribute<N_File, DateTime>);
			_EndDate = new Lazy<MetaAttribute<N_File, DateTime>>(() => _metaClass.GetProperty("EndDate") as MetaAttribute<N_File, DateTime>);
			_Extension = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("Extension") as MetaAttribute<N_File, string>);
			_Guid = new Lazy<MetaAttribute<N_File, Guid>>(() => _metaClass.GetProperty("Guid") as MetaAttribute<N_File, Guid>);
			_GuidPath = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("GuidPath") as MetaAttribute<N_File, string>);
			_StorageType = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("StorageType") as MetaAttribute<N_File, string>);
			_Tag = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("Tag") as MetaAttribute<N_File, string>);
			_VersionNumber = new Lazy<MetaAttribute<N_File, int>>(() => _metaClass.GetProperty("VersionNumber") as MetaAttribute<N_File, int>);
			_PublishDate = new Lazy<MetaAttribute<N_File, Nullable<DateTime>>>(() => _metaClass.GetProperty("PublishDate") as MetaAttribute<N_File, Nullable<DateTime>>);
			_Length = new Lazy<MetaAttribute<N_File, long>>(() => _metaClass.GetProperty("Length") as MetaAttribute<N_File, long>);
			_MainGUID = new Lazy<MetaAttribute<N_File, Nullable<Guid>>>(() => _metaClass.GetProperty("MainGUID") as MetaAttribute<N_File, Nullable<Guid>>);
			_Path = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("Path") as MetaAttribute<N_File, string>);
			_StorageParameter = new Lazy<MetaAttribute<N_File, string>>(() => _metaClass.GetProperty("StorageParameter") as MetaAttribute<N_File, string>);
			_IsDeleted = new Lazy<MetaPersistentComputedAttribute<N_File, bool>>(() => _metaClass.GetProperty("IsDeleted") as MetaPersistentComputedAttribute<N_File, bool>);
			_Folder = new Lazy<MetaReference<N_File, N_Folder, int>>(() => _metaClass.GetProperty("Folder") as MetaReference<N_File, N_Folder, int>);
			_CheckedOutBy = new Lazy<MetaReference<N_File, SPM_Subject, Nullable<int>>>(() => _metaClass.GetProperty("CheckedOutBy") as MetaReference<N_File, SPM_Subject, Nullable<int>>);
			_Creator = new Lazy<MetaReference<N_File, SPM_Subject, int>>(() => _metaClass.GetProperty("Creator") as MetaReference<N_File, SPM_Subject, int>);
			_Delete = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Delete"));
			_CreateNew = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("CreateNew"));
			_Edit = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Edit"));

			FileID.GetValue = o => o.FileID;
			FileID.GetValueExpression = o => o.FileID;
			FileID.SetValue = (o, v) => o.FileID = v;
			LastModifiedDate.GetValue = o => o.LastModifiedDate;
			LastModifiedDate.GetValueExpression = o => o.LastModifiedDate;
			LastModifiedDate.SetValue = (o, v) => o.LastModifiedDate = v;
			LastModifiedUser.GetValue = o => o.LastModifiedUser;
			LastModifiedUser.GetValueExpression = o => o.LastModifiedUser;
			LastModifiedUser.SetValue = (o, v) => o.LastModifiedUser = v;
			LastModifiedUser.GetValueID = o => o.LastModifiedUserID;
			LastModifiedUser.SetValueID = (o, v) => o.LastModifiedUserID = v;
			Title.GetValue = o => o.Title;
			Title.GetValueExpression = o => o.Title;
			Title.SetValue = (o, v) => o.Title = v;
			Password.GetValue = o => o.Password;
			Password.GetValueExpression = o => o.Password;
			Password.SetValue = (o, v) => o.Password = v;
			BeginDate.GetValue = o => o.BeginDate;
			BeginDate.GetValueExpression = o => o.BeginDate;
			BeginDate.SetValue = (o, v) => o.BeginDate = v;
			EndDate.GetValue = o => o.EndDate;
			EndDate.GetValueExpression = o => o.EndDate;
			EndDate.SetValue = (o, v) => o.EndDate = v;
			Extension.GetValue = o => o.Extension;
			Extension.GetValueExpression = o => o.Extension;
			Extension.SetValue = (o, v) => o.Extension = v;
			Guid.GetValue = o => o.Guid;
			Guid.GetValueExpression = o => o.Guid;
			Guid.SetValue = (o, v) => o.Guid = v;
			GuidPath.GetValue = o => o.GuidPath;
			GuidPath.GetValueExpression = o => o.GuidPath;
			GuidPath.SetValue = (o, v) => o.GuidPath = v;
			StorageType.GetValue = o => o.StorageType;
			StorageType.GetValueExpression = o => o.StorageType;
			StorageType.SetValue = (o, v) => o.StorageType = v;
			Tag.GetValue = o => o.Tag;
			Tag.GetValueExpression = o => o.Tag;
			Tag.SetValue = (o, v) => o.Tag = v;
			VersionNumber.GetValue = o => o.VersionNumber;
			VersionNumber.GetValueExpression = o => o.VersionNumber;
			VersionNumber.SetValue = (o, v) => o.VersionNumber = v;
			PublishDate.GetValue = o => o.PublishDate;
			PublishDate.GetValueExpression = o => o.PublishDate;
			PublishDate.SetValue = (o, v) => o.PublishDate = v;
			Length.GetValue = o => o.Length;
			Length.GetValueExpression = o => o.Length;
			Length.SetValue = (o, v) => o.Length = v;
			MainGUID.GetValue = o => o.MainGUID;
			MainGUID.GetValueExpression = o => o.MainGUID;
			MainGUID.SetValue = (o, v) => o.MainGUID = v;
			Path.GetValue = o => o.Path;
			Path.GetValueExpression = o => o.Path;
			Path.SetValue = (o, v) => o.Path = v;
			StorageParameter.GetValue = o => o.StorageParameter;
			StorageParameter.GetValueExpression = o => o.StorageParameter;
			StorageParameter.SetValue = (o, v) => o.StorageParameter = v;
			IsDeleted.GetValue = o => o.IsDeleted;
			IsDeleted.GetValueExpression = o => o.IsDeleted;
			Folder.GetValue = o => o.Folder;
			Folder.GetValueExpression = o => o.Folder;
			Folder.SetValue = (o, v) => o.Folder = v;
			Folder.GetValueID = o => o.FolderID;
			Folder.SetValueID = (o, v) => o.FolderID = v;
			CheckedOutBy.GetValue = o => o.CheckedOutBy;
			CheckedOutBy.GetValueExpression = o => o.CheckedOutBy;
			CheckedOutBy.SetValue = (o, v) => o.CheckedOutBy = v;
			CheckedOutBy.GetValueID = o => o.CheckedOutByID;
			CheckedOutBy.SetValueID = (o, v) => o.CheckedOutByID = v;
			Creator.GetValue = o => o.Creator;
			Creator.GetValueExpression = o => o.Creator;
			Creator.SetValue = (o, v) => o.Creator = v;
			Creator.GetValueID = o => o.CreatorID;
			Creator.SetValueID = (o, v) => o.CreatorID = v;
		}

	}
	public partial class MetaN_FileData : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_FileData, Guid>> _FileGUID;
		Lazy<MetaAttribute<N_FileData, string>> _Extension;
		Lazy<MetaAttribute<N_FileData, Byte[]>> _Data;
		Lazy<MetaAttribute<N_FileData, string>> _Title;
		Lazy<MetaAttribute<N_FileData, long>> _Size;
		Lazy<MetaAttribute<N_FileData, DateTime>> _LastModifiedDate;
		Lazy<MetaAttribute<N_FileData, Nullable<Guid>>> _Owner;
		Lazy<IMetaOperation> _CreateNew;
		Lazy<IMetaOperation> _Delete;
		Lazy<IMetaOperation> _ViewList;
		Lazy<IMetaOperation> _Edit;
		Lazy<IMetaOperation> _Upload;

		public MetaAttribute<N_FileData, Guid> FileGUID => _FileGUID.Value;
		public MetaAttribute<N_FileData, string> Extension => _Extension.Value;
		public MetaAttribute<N_FileData, Byte[]> Data => _Data.Value;
		public MetaAttribute<N_FileData, string> Title => _Title.Value;
		public MetaAttribute<N_FileData, long> Size => _Size.Value;
		public MetaAttribute<N_FileData, DateTime> LastModifiedDate => _LastModifiedDate.Value;
		public MetaAttribute<N_FileData, Nullable<Guid>> Owner => _Owner.Value;
		public IMetaOperation CreateNew => _CreateNew.Value;
		public IMetaOperation Delete => _Delete.Value;
		public IMetaOperation ViewList => _ViewList.Value;
		public IMetaOperation Edit => _Edit.Value;
		public IMetaOperation Upload => _Upload.Value;

		public MetaN_FileData()		{
			_metaClass = new MetaClass(typeof(N_FileData));
			_builder.N_FileData(_metaClass);
			_FileGUID = new Lazy<MetaAttribute<N_FileData, Guid>>(() => _metaClass.GetProperty("FileGUID") as MetaAttribute<N_FileData, Guid>);
			_Extension = new Lazy<MetaAttribute<N_FileData, string>>(() => _metaClass.GetProperty("Extension") as MetaAttribute<N_FileData, string>);
			_Data = new Lazy<MetaAttribute<N_FileData, Byte[]>>(() => _metaClass.GetProperty("Data") as MetaAttribute<N_FileData, Byte[]>);
			_Title = new Lazy<MetaAttribute<N_FileData, string>>(() => _metaClass.GetProperty("Title") as MetaAttribute<N_FileData, string>);
			_Size = new Lazy<MetaAttribute<N_FileData, long>>(() => _metaClass.GetProperty("Size") as MetaAttribute<N_FileData, long>);
			_LastModifiedDate = new Lazy<MetaAttribute<N_FileData, DateTime>>(() => _metaClass.GetProperty("LastModifiedDate") as MetaAttribute<N_FileData, DateTime>);
			_Owner = new Lazy<MetaAttribute<N_FileData, Nullable<Guid>>>(() => _metaClass.GetProperty("Owner") as MetaAttribute<N_FileData, Nullable<Guid>>);
			_CreateNew = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("CreateNew"));
			_Delete = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Delete"));
			_ViewList = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("ViewList"));
			_Edit = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Edit"));
			_Upload = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Upload"));

			FileGUID.GetValue = o => o.FileGUID;
			FileGUID.GetValueExpression = o => o.FileGUID;
			FileGUID.SetValue = (o, v) => o.FileGUID = v;
			Extension.GetValue = o => o.Extension;
			Extension.GetValueExpression = o => o.Extension;
			Extension.SetValue = (o, v) => o.Extension = v;
			Data.GetValue = o => o.Data;
			Data.GetValueExpression = o => o.Data;
			Data.SetValue = (o, v) => o.Data = v;
			Title.GetValue = o => o.Title;
			Title.GetValueExpression = o => o.Title;
			Title.SetValue = (o, v) => o.Title = v;
			Size.GetValue = o => o.Size;
			Size.GetValueExpression = o => o.Size;
			Size.SetValue = (o, v) => o.Size = v;
			LastModifiedDate.GetValue = o => o.LastModifiedDate;
			LastModifiedDate.GetValueExpression = o => o.LastModifiedDate;
			LastModifiedDate.SetValue = (o, v) => o.LastModifiedDate = v;
			Owner.GetValue = o => o.Owner;
			Owner.GetValueExpression = o => o.Owner;
			Owner.SetValue = (o, v) => o.Owner = v;
		}

	}
	public partial class MetaN_FileLibraryType : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_FileLibraryType, int>> _FileLibraryTypeID;
		Lazy<MetaAttribute<N_FileLibraryType, DateTime>> _LastModifiedDate;
		Lazy<MetaReference<N_FileLibraryType, SPM_Subject, int>> _LastModifiedUser;
		Lazy<MetaAttribute<N_FileLibraryType, bool>> _IsDeleted;
		Lazy<MetaAttribute<N_FileLibraryType, string>> _Title;
		Lazy<MetaAttribute<N_FileLibraryType, string>> _Extensions;
		Lazy<MetaAttribute<N_FileLibraryType, string>> _ClassName;
		Lazy<IMetaOperation> _CreateNew;
		Lazy<IMetaOperation> _Edit;
		Lazy<IMetaOperation> _Delete;
		Lazy<IMetaOperation> _ViewList;
		Lazy<IMetaOperation> _UnDelete;
		Lazy<IMetaOperation> _View;

		public MetaAttribute<N_FileLibraryType, int> FileLibraryTypeID => _FileLibraryTypeID.Value;
		public MetaAttribute<N_FileLibraryType, DateTime> LastModifiedDate => _LastModifiedDate.Value;
		public MetaReference<N_FileLibraryType, SPM_Subject, int> LastModifiedUser => _LastModifiedUser.Value;
		public MetaAttribute<N_FileLibraryType, bool> IsDeleted => _IsDeleted.Value;
		public MetaAttribute<N_FileLibraryType, string> Title => _Title.Value;
		public MetaAttribute<N_FileLibraryType, string> Extensions => _Extensions.Value;
		public MetaAttribute<N_FileLibraryType, string> ClassName => _ClassName.Value;
		public IMetaOperation CreateNew => _CreateNew.Value;
		public IMetaOperation Edit => _Edit.Value;
		public IMetaOperation Delete => _Delete.Value;
		public IMetaOperation ViewList => _ViewList.Value;
		public IMetaOperation UnDelete => _UnDelete.Value;
		public IMetaOperation View => _View.Value;

		public MetaN_FileLibraryType()		{
			_metaClass = new MetaClass(typeof(N_FileLibraryType));
			_builder.N_FileLibraryType(_metaClass);
			_FileLibraryTypeID = new Lazy<MetaAttribute<N_FileLibraryType, int>>(() => _metaClass.GetProperty("FileLibraryTypeID") as MetaAttribute<N_FileLibraryType, int>);
			_LastModifiedDate = new Lazy<MetaAttribute<N_FileLibraryType, DateTime>>(() => _metaClass.GetProperty("LastModifiedDate") as MetaAttribute<N_FileLibraryType, DateTime>);
			_LastModifiedUser = new Lazy<MetaReference<N_FileLibraryType, SPM_Subject, int>>(() => _metaClass.GetProperty("LastModifiedUser") as MetaReference<N_FileLibraryType, SPM_Subject, int>);
			_IsDeleted = new Lazy<MetaAttribute<N_FileLibraryType, bool>>(() => _metaClass.GetProperty("IsDeleted") as MetaAttribute<N_FileLibraryType, bool>);
			_Title = new Lazy<MetaAttribute<N_FileLibraryType, string>>(() => _metaClass.GetProperty("Title") as MetaAttribute<N_FileLibraryType, string>);
			_Extensions = new Lazy<MetaAttribute<N_FileLibraryType, string>>(() => _metaClass.GetProperty("Extensions") as MetaAttribute<N_FileLibraryType, string>);
			_ClassName = new Lazy<MetaAttribute<N_FileLibraryType, string>>(() => _metaClass.GetProperty("ClassName") as MetaAttribute<N_FileLibraryType, string>);
			_CreateNew = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("CreateNew"));
			_Edit = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Edit"));
			_Delete = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("Delete"));
			_ViewList = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("ViewList"));
			_UnDelete = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("UnDelete"));
			_View = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("View"));

			FileLibraryTypeID.GetValue = o => o.FileLibraryTypeID;
			FileLibraryTypeID.GetValueExpression = o => o.FileLibraryTypeID;
			FileLibraryTypeID.SetValue = (o, v) => o.FileLibraryTypeID = v;
			LastModifiedDate.GetValue = o => o.LastModifiedDate;
			LastModifiedDate.GetValueExpression = o => o.LastModifiedDate;
			LastModifiedDate.SetValue = (o, v) => o.LastModifiedDate = v;
			LastModifiedUser.GetValue = o => o.LastModifiedUser;
			LastModifiedUser.GetValueExpression = o => o.LastModifiedUser;
			LastModifiedUser.SetValue = (o, v) => o.LastModifiedUser = v;
			LastModifiedUser.GetValueID = o => o.LastModifiedUserID;
			LastModifiedUser.SetValueID = (o, v) => o.LastModifiedUserID = v;
			IsDeleted.GetValue = o => o.IsDeleted;
			IsDeleted.GetValueExpression = o => o.IsDeleted;
			IsDeleted.SetValue = (o, v) => o.IsDeleted = v;
			Title.GetValue = o => o.Title;
			Title.GetValueExpression = o => o.Title;
			Title.SetValue = (o, v) => o.Title = v;
			Extensions.GetValue = o => o.Extensions;
			Extensions.GetValueExpression = o => o.Extensions;
			Extensions.SetValue = (o, v) => o.Extensions = v;
			ClassName.GetValue = o => o.ClassName;
			ClassName.GetValueExpression = o => o.ClassName;
			ClassName.SetValue = (o, v) => o.ClassName = v;
		}

	}
	public partial class MetaN_FileLibrary : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_FileLibrary, int>> _FileLibraryID;
		Lazy<MetaAttribute<N_FileLibrary, int>> _MaxFileSize;
		Lazy<MetaAttribute<N_FileLibrary, string>> _StorageType;
		Lazy<MetaAttribute<N_FileLibrary, string>> _StorageParameter;
		Lazy<MetaReference<N_FileLibrary, N_FileLibraryType, int>> _FileLibraryType;

		public MetaAttribute<N_FileLibrary, int> FileLibraryID => _FileLibraryID.Value;
		public MetaAttribute<N_FileLibrary, int> MaxFileSize => _MaxFileSize.Value;
		public MetaAttribute<N_FileLibrary, string> StorageType => _StorageType.Value;
		public MetaAttribute<N_FileLibrary, string> StorageParameter => _StorageParameter.Value;
		public MetaReference<N_FileLibrary, N_FileLibraryType, int> FileLibraryType => _FileLibraryType.Value;

		public MetaN_FileLibrary()		{
			_metaClass = new MetaClass(typeof(N_FileLibrary));
			_builder.N_FileLibrary(_metaClass);
			_FileLibraryID = new Lazy<MetaAttribute<N_FileLibrary, int>>(() => _metaClass.GetProperty("FileLibraryID") as MetaAttribute<N_FileLibrary, int>);
			_MaxFileSize = new Lazy<MetaAttribute<N_FileLibrary, int>>(() => _metaClass.GetProperty("MaxFileSize") as MetaAttribute<N_FileLibrary, int>);
			_StorageType = new Lazy<MetaAttribute<N_FileLibrary, string>>(() => _metaClass.GetProperty("StorageType") as MetaAttribute<N_FileLibrary, string>);
			_StorageParameter = new Lazy<MetaAttribute<N_FileLibrary, string>>(() => _metaClass.GetProperty("StorageParameter") as MetaAttribute<N_FileLibrary, string>);
			_FileLibraryType = new Lazy<MetaReference<N_FileLibrary, N_FileLibraryType, int>>(() => _metaClass.GetProperty("FileLibraryType") as MetaReference<N_FileLibrary, N_FileLibraryType, int>);

			FileLibraryID.GetValue = o => o.FileLibraryID;
			FileLibraryID.GetValueExpression = o => o.FileLibraryID;
			FileLibraryID.SetValue = (o, v) => o.FileLibraryID = v;
			MaxFileSize.GetValue = o => o.MaxFileSize;
			MaxFileSize.GetValueExpression = o => o.MaxFileSize;
			MaxFileSize.SetValue = (o, v) => o.MaxFileSize = v;
			StorageType.GetValue = o => o.StorageType;
			StorageType.GetValueExpression = o => o.StorageType;
			StorageType.SetValue = (o, v) => o.StorageType = v;
			StorageParameter.GetValue = o => o.StorageParameter;
			StorageParameter.GetValueExpression = o => o.StorageParameter;
			StorageParameter.SetValue = (o, v) => o.StorageParameter = v;
			FileLibraryType.GetValue = o => o.FileLibraryType;
			FileLibraryType.GetValueExpression = o => o.FileLibraryType;
			FileLibraryType.SetValue = (o, v) => o.FileLibraryType = v;
			FileLibraryType.GetValueID = o => o.FileLibraryTypeID;
			FileLibraryType.SetValueID = (o, v) => o.FileLibraryTypeID = v;
		}

	}
	public partial class MetaN_DownloadLog : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<N_DownloadLog, int>> _DownloadLogID;
		Lazy<MetaAttribute<N_DownloadLog, DateTime>> _LastModifiedDate;
		Lazy<MetaReference<N_DownloadLog, SPM_Subject, int>> _LastModifiedUser;
		Lazy<MetaAttribute<N_DownloadLog, bool>> _IsDeleted;
		Lazy<MetaAttribute<N_DownloadLog, Guid>> _FileGUID;
		Lazy<MetaAttribute<N_DownloadLog, string>> _IP;
		Lazy<MetaReference<N_DownloadLog, N_File, int>> _File;
		Lazy<IMetaOperation> _ViewList;

		public MetaAttribute<N_DownloadLog, int> DownloadLogID => _DownloadLogID.Value;
		public MetaAttribute<N_DownloadLog, DateTime> LastModifiedDate => _LastModifiedDate.Value;
		public MetaReference<N_DownloadLog, SPM_Subject, int> LastModifiedUser => _LastModifiedUser.Value;
		public MetaAttribute<N_DownloadLog, bool> IsDeleted => _IsDeleted.Value;
		public MetaAttribute<N_DownloadLog, Guid> FileGUID => _FileGUID.Value;
		public MetaAttribute<N_DownloadLog, string> IP => _IP.Value;
		public MetaReference<N_DownloadLog, N_File, int> File => _File.Value;
		public IMetaOperation ViewList => _ViewList.Value;

		public MetaN_DownloadLog()		{
			_metaClass = new MetaClass(typeof(N_DownloadLog));
			_builder.N_DownloadLog(_metaClass);
			_DownloadLogID = new Lazy<MetaAttribute<N_DownloadLog, int>>(() => _metaClass.GetProperty("DownloadLogID") as MetaAttribute<N_DownloadLog, int>);
			_LastModifiedDate = new Lazy<MetaAttribute<N_DownloadLog, DateTime>>(() => _metaClass.GetProperty("LastModifiedDate") as MetaAttribute<N_DownloadLog, DateTime>);
			_LastModifiedUser = new Lazy<MetaReference<N_DownloadLog, SPM_Subject, int>>(() => _metaClass.GetProperty("LastModifiedUser") as MetaReference<N_DownloadLog, SPM_Subject, int>);
			_IsDeleted = new Lazy<MetaAttribute<N_DownloadLog, bool>>(() => _metaClass.GetProperty("IsDeleted") as MetaAttribute<N_DownloadLog, bool>);
			_FileGUID = new Lazy<MetaAttribute<N_DownloadLog, Guid>>(() => _metaClass.GetProperty("FileGUID") as MetaAttribute<N_DownloadLog, Guid>);
			_IP = new Lazy<MetaAttribute<N_DownloadLog, string>>(() => _metaClass.GetProperty("IP") as MetaAttribute<N_DownloadLog, string>);
			_File = new Lazy<MetaReference<N_DownloadLog, N_File, int>>(() => _metaClass.GetProperty("File") as MetaReference<N_DownloadLog, N_File, int>);
			_ViewList = new Lazy<IMetaOperation>(() => _metaClass.GetOperation("ViewList"));

			DownloadLogID.GetValue = o => o.DownloadLogID;
			DownloadLogID.GetValueExpression = o => o.DownloadLogID;
			DownloadLogID.SetValue = (o, v) => o.DownloadLogID = v;
			LastModifiedDate.GetValue = o => o.LastModifiedDate;
			LastModifiedDate.GetValueExpression = o => o.LastModifiedDate;
			LastModifiedDate.SetValue = (o, v) => o.LastModifiedDate = v;
			LastModifiedUser.GetValue = o => o.LastModifiedUser;
			LastModifiedUser.GetValueExpression = o => o.LastModifiedUser;
			LastModifiedUser.SetValue = (o, v) => o.LastModifiedUser = v;
			LastModifiedUser.GetValueID = o => o.LastModifiedUserID;
			LastModifiedUser.SetValueID = (o, v) => o.LastModifiedUserID = v;
			IsDeleted.GetValue = o => o.IsDeleted;
			IsDeleted.GetValueExpression = o => o.IsDeleted;
			IsDeleted.SetValue = (o, v) => o.IsDeleted = v;
			FileGUID.GetValue = o => o.FileGUID;
			FileGUID.GetValueExpression = o => o.FileGUID;
			FileGUID.SetValue = (o, v) => o.FileGUID = v;
			IP.GetValue = o => o.IP;
			IP.GetValueExpression = o => o.IP;
			IP.SetValue = (o, v) => o.IP = v;
			File.GetValue = o => o.File;
			File.GetValueExpression = o => o.File;
			File.SetValue = (o, v) => o.File = v;
			File.GetValueID = o => o.FileID;
			File.SetValueID = (o, v) => o.FileID = v;
		}

	}
	public partial class MetaV_N_FullFolder : IMetaClassDescription 
	{		
		protected IMetaClass _metaClass;
		public IMetaClass GetInfo() => _metaClass;
		protected Builder _builder = new Builder();

		Lazy<MetaAttribute<V_N_FullFolder, int>> _FolderID;
		Lazy<MetaAttribute<V_N_FullFolder, int>> _ParentID;
		Lazy<MetaAttribute<V_N_FullFolder, int>> _ArcLen;

		public MetaAttribute<V_N_FullFolder, int> FolderID => _FolderID.Value;
		public MetaAttribute<V_N_FullFolder, int> ParentID => _ParentID.Value;
		public MetaAttribute<V_N_FullFolder, int> ArcLen => _ArcLen.Value;

		public MetaV_N_FullFolder()		{
			_metaClass = new MetaClass(typeof(V_N_FullFolder));
			_builder.V_N_FullFolder(_metaClass);
			_FolderID = new Lazy<MetaAttribute<V_N_FullFolder, int>>(() => _metaClass.GetProperty("FolderID") as MetaAttribute<V_N_FullFolder, int>);
			_ParentID = new Lazy<MetaAttribute<V_N_FullFolder, int>>(() => _metaClass.GetProperty("ParentID") as MetaAttribute<V_N_FullFolder, int>);
			_ArcLen = new Lazy<MetaAttribute<V_N_FullFolder, int>>(() => _metaClass.GetProperty("ArcLen") as MetaAttribute<V_N_FullFolder, int>);

			FolderID.GetValue = o => o.FolderID;
			FolderID.GetValueExpression = o => o.FolderID;
			FolderID.SetValue = (o, v) => o.FolderID = v;
			ParentID.GetValue = o => o.ParentID;
			ParentID.GetValueExpression = o => o.ParentID;
			ParentID.SetValue = (o, v) => o.ParentID = v;
			ArcLen.GetValue = o => o.ArcLen;
			ArcLen.GetValueExpression = o => o.ArcLen;
			ArcLen.SetValue = (o, v) => o.ArcLen = v;
		}

	}
	
}
