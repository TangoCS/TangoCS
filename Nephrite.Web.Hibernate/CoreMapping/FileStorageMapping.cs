using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Web.FileStorage;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IN_DownloadLogMap : ClassMapping<IN_DownloadLog>
	{
		public IN_DownloadLogMap()
		{
			Table("N_DownloadLog");
			Lazy(true);
			Id(x => x.DownloadLogID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.IP);
			Property(x => x.FileGUID, map => { map.NotNullable(true); MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}

	public class IDbFileMap : ClassMapping<IDbFile>
	{
		public IDbFileMap()
		{
			Schema("dbo");
			Table("V_DbFile");
			//Table("\"V_DbFile\"");
			Lazy(true);
			Id(x => x.ID, map => { map.Generator(Generators.Assigned);/* map.Column("\"ID\"");*/ MappingConfig.GuidIDPropertyConfig(map); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title/*, map => { map.Column("\"Title\""); }*/);
			Property(x => x.Extension/*, map => { map.Column("\"Extension\""); }*/);
			Property(x => x.LastModifiedUserName/*, map => { map.Column("\"LastModifiedUserName\""); }*/);
			Property(x => x.IsDeleted, map => { map.NotNullable(false); MappingConfig.BoolPropertyConfig(map);/* map.Column("\"IsDeleted\"");*/ });
			Property(x => x.LastModifiedDate/*, map => { map.Column("\"LastModifiedDate\""); }*/);
			Property(x => x.LastModifiedUserID/*, map => { map.Column("\"LastModifiedUserID\""); }*/);
			Property(x => x.Size/*, map => { map.Column("\"Size\""); }*/);
			Property(x => x.Path/*, map => { map.Column("\"Path\""); }*/);
			Property(x => x.ParentFolderID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"ParentFolderID\"");*/ });
			Property(x => x.Tag/*, map => { map.Column("\"Tag\""); }*/);
			Property(x => x.VersionNumber/*, map => { map.Column("\"VersionNumber\""); }*/);
			Property(x => x.MainID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"MainID\"");*/ });
			Property(x => x.CheckedOutByID/*, map => { map.Column("\"CheckedOutByID\""); }*/);
			Property(x => x.CheckedOutBy/*, map => { map.Column("\"CheckedOutBy\""); }*/);
			Property(x => x.FeatureGUID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"FeatureGUID\"");*/ });
			Property(x => x.PublishDate/*, map => { map.Column("\"PublishDate\""); }*/);
			Property(x => x.CreatorID/*, map => { map.Column("\"CreatorID\""); }*/);
			Property(x => x.Creator/*, map => { map.Column("\"Creator\""); }*/);
			Property(x => x.ParentFolderID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"ParentID\"");*/ });
			//Property(x => x.IsValid, map => { map.Column("\"IsValid\""); });
			Property(x => x.SPMActionItemGUID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"SPMActionItemGUID\"");*/ });
			Property(x => x.FullPath/*, map => { map.Column("\"FullPath\""); }*/);
		}
	}

	public class IDbFolderMap : ClassMapping<IDbFolder>
	{
		public IDbFolderMap()
		{
			Schema("dbo");
			Table("V_DbFolder");
			//Table("\"V_DbFolder\"");
			Lazy(true);
			Id(x => x.ID, map => { map.Generator(Generators.Assigned); /*map.Column("\"ID\"");*/ MappingConfig.GuidIDPropertyConfig(map); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title/*, map => { map.Column("\"Title\""); }*/);
			Property(x => x.LastModifiedDate/*, map => { map.Column("\"LastModifiedDate\""); }*/);
			Property(x => x.LastModifiedUserID/*, map => { map.Column("\"LastModifiedUserID\""); }*/);
			Property(x => x.IsDeleted, map => { map.NotNullable(false); MappingConfig.BoolPropertyConfig(map); /*map.Column("\"IsDeleted\"");*/ });
			Property(x => x.Size/*, map => { map.Column("\"Size\""); }*/);
			Property(x => x.LastModifiedUserName/*, map => { map.Column("\"LastModifiedUserName\""); }*/);
			Property(x => x.Path/*, map => { map.Column("\"Path\""); }*/);
			Property(x => x.ParentFolderID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map); /*map.Column("\"ParentFolderID\"");*/ });
			Property(x => x.StorageParameter/*, map => { map.Column("\"StorageParameter\""); }*/);
			Property(x => x.SPMActionItemGUID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map); /*map.Column("\"SPMActionItemGUID\"");*/ });
			Property(x => x.EnableVersioning, map => { map.NotNullable(false); MappingConfig.BoolPropertyConfig(map); /*map.Column("\"EnableVersioning\"");*/ });
			//Property(x => x.FileCount, map => { map.Column("\"FileCount\""); });
			Property(x => x.Tag/*, map => { map.Column("\"Tag\""); }*/);
			Property(x => x.PublishDate/*, map => { map.Column("\"PublishDate\""); }*/);
			Property(x => x.CreatorID/*, map => { map.Column("\"CreatorID\""); }*/);
			Property(x => x.Creator/*, map => { map.Column("\"Creator\""); }*/);
			Property(x => x.FullPath/*, map => { map.Column("\"FullPath\""); }*/);
			//Property(x => x.IsValid, map => { map.Column("\"IsValid\""); });
			Property(x => x.StorageType/*, map => { map.Column("\"StorageType\""); }*/);
		}
	}

	public class IDbItemMap : ClassMapping<IDbItem>
	{
		public IDbItemMap()
		{
			Table("V_DbItem");
			//Table("\"V_DbItem\"");
			Lazy(true);

			Id(x => x.ID, map => { map.Generator(Generators.Assigned); /*map.Column("\"ID\"");*/ MappingConfig.GuidIDPropertyConfig(map); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title/*, map => { map.Column("\"Title\""); }*/);
			Property(x => x.LastModifiedDate/*, map => { map.Column("\"LastModifiedDate\""); }*/);
			Property(x => x.LastModifiedUserID/*, map => { map.Column("\"LastModifiedUserID\""); }*/);
			Property(x => x.IsDeleted, map => { map.NotNullable(false); MappingConfig.BoolPropertyConfig(map); /*map.Column("\"IsDeleted\"");*/ });
			Property(x => x.Size/*, map => { map.Column("\"Size\""); }*/);
			Property(x => x.LastModifiedUserName/*, map => { map.Column("\"LastModifiedUserName\""); }*/);
			Property(x => x.Type/*, map => { map.Column("\"Type\""); }*/);
			Property(x => x.Path/*, map => { map.Column("\"Path\""); }*/);
			Property(x => x.ParentID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map); /*map.Column("\"ParentID\"");*/ });
			Property(x => x.Extension/*, map => { map.Column("\"Extension\""); }*/);
			Property(x => x.SPMActionItemGUID, map => { map.NotNullable(false); MappingConfig.GuidPropertyConfig(map);/* map.Column("\"SPMActionItemGUID\"");*/ });
			Property(x => x.EnableVersioning, map => { map.NotNullable(false); MappingConfig.BoolPropertyConfig(map);/* map.Column("\"EnableVersioning\"");*/ });
			Property(x => x.FullPath/*, map => { map.Column("\"FullPath\""); }*/);
			Property(x => x.CheckedOutByID/*, map => { map.Column("\"CheckedOutByID\""); }*/);
			Property(x => x.CheckedOutBy/*, map => { map.Column("\"CheckedOutBy\""); }*/);
			Property(x => x.Tag/*, map => { map.Column("\"Tag\""); }*/);
			Property(x => x.PublishDate/*, map => { map.Column("\"PublishDate\""); }*/);
			Property(x => x.CreatorID/*, map => { map.Column("\"CreatorID\""); }*/);
			Property(x => x.Creator/*, map => { map.Column("\"Creator\""); }*/);
		}
	}

	public class IDbFileDataMap : ClassMapping<IDbFileData>
	{
		public IDbFileDataMap()
		{
			Table("N_FileData");
			Lazy(true);
			Id(x => x.FileGUID, map => { map.Generator(Generators.Assigned); MappingConfig.GuidIDPropertyConfig(map); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Data);
			Property(x => x.Extension);
		}
	}

	public class IN_VirusScanLogMap : ClassMapping<IN_VirusScanLog>
	{
		public IN_VirusScanLogMap()
		{
			Table("N_VirusScanLog");

			Lazy(true);
			Id(x => x.VirusScanLogID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.ResultCode, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}
}