 
using Tango;
using Tango.Hibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;


namespace Tango.FileStorage.Std.Model
{
	public partial class N_FolderMap : ClassMapping<N_Folder>
	{
		public N_FolderMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FolderID, map => { map.Generator(Generators.Identity); });
			Property(x => x.LastModifiedDate);
			Property(x => x.Title);
			Property(x => x.IsDeleted, map => { MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.Path);
			Property(x => x.Guid, map => { MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.GuidPath);
			Property(x => x.SPMActionItemGUID, map => { MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.LastModifiedUserID);
			Property(x => x.FileLibraryID, map => { map.Formula("FileLibraryID"); });
			ManyToOne(x => x.FileLibrary, map => 
			{
				map.Column("FileLibraryID");
				map.Cascade(Cascade.None);
			});
			Property(x => x.ParentID, map => { map.Formula("ParentID"); });
			ManyToOne(x => x.Parent, map => 
			{
				map.Column("ParentID");
				map.Cascade(Cascade.None);
			});
			Extension();
		}

		partial void Extension();
	}
	public partial class N_FileMap : ClassMapping<N_File>
	{
		public N_FileMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FileID, map => { map.Generator(Generators.Identity); });
			Property(x => x.LastModifiedDate);
			Property(x => x.Title);
			Property(x => x.Password);
			Property(x => x.BeginDate);
			Property(x => x.EndDate);
			Property(x => x.Extension);
			Property(x => x.Guid, map => { MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.GuidPath);
			Property(x => x.StorageType);
			Property(x => x.Tag);
			Property(x => x.VersionNumber);
			Property(x => x.PublishDate);
			Property(x => x.Length);
			Property(x => x.MainGUID, map => { MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.Path);
			Property(x => x.StorageParameter);
			Property(x => x.IsDeleted, map => { MappingConfig.BoolPropertyConfig(map); map.Formula("IsDeleted"); });
			Property(x => x.LastModifiedUserID);
			Property(x => x.FolderID, map => { map.Formula("FolderID"); });
			ManyToOne(x => x.Folder, map => 
			{
				map.Column("FolderID");
				map.Cascade(Cascade.None);
			});
			Property(x => x.CheckedOutByID);
			Property(x => x.CreatorID);
			Extension();
		}

		partial void Extension();
	}
	public partial class N_FileDataMap : ClassMapping<N_FileData>
	{
		public N_FileDataMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FileGUID, map => { map.Generator(Generators.Assigned); MappingConfig.GuidIDPropertyConfig(map); });
			Property(x => x.Extension);
			Property(x => x.Data, map => { map.Length(int.MaxValue); });
			Property(x => x.Title);
			Property(x => x.Size);
			Property(x => x.LastModifiedDate);
			Property(x => x.Owner, map => { MappingConfig.GuidPropertyConfig(map); });
			Extension();
		}

		partial void Extension();
	}
	public partial class N_FileLibraryTypeMap : ClassMapping<N_FileLibraryType>
	{
		public N_FileLibraryTypeMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FileLibraryTypeID, map => { map.Generator(Generators.Identity); });
			Property(x => x.LastModifiedDate);
			Property(x => x.IsDeleted, map => { MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.Title);
			Property(x => x.Extensions);
			Property(x => x.ClassName);
			Property(x => x.LastModifiedUserID);
			Extension();
		}

		partial void Extension();
	}
	public partial class N_FileLibraryMap : ClassMapping<N_FileLibrary>
	{
		public N_FileLibraryMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FileLibraryID, map => { map.Generator(Generators.Identity); });
			Property(x => x.MaxFileSize);
			Property(x => x.StorageType);
			Property(x => x.StorageParameter);
			Property(x => x.FileLibraryTypeID, map => { map.Formula("FileLibraryTypeID"); });
			ManyToOne(x => x.FileLibraryType, map => 
			{
				map.Column("FileLibraryTypeID");
				map.Cascade(Cascade.None);
			});
			Extension();
		}

		partial void Extension();
	}
	public partial class N_DownloadLogMap : ClassMapping<N_DownloadLog>
	{
		public N_DownloadLogMap() 
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.DownloadLogID, map => { map.Generator(Generators.Identity); });
			Property(x => x.LastModifiedDate);
			Property(x => x.IsDeleted, map => { MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.FileGUID, map => { MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.IP);
			Property(x => x.LastModifiedUserID);
			Property(x => x.FileID, map => { MappingConfig.GuidPropertyConfig(map);});
			Extension();
		}

		partial void Extension();
	}
	public class V_N_FullFolderMap : ClassMapping<V_N_FullFolder>
	{
		public V_N_FullFolderMap() 
		{
			Schema("dbo");
			Table("V_N_FullFolder");
			Lazy(true);
			Id(x => x.FolderID);
			Property(x => x.ParentID);
			Property(x => x.ArcLen);
		}
	}
}
