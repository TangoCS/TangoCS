using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.FileStorage;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Nephrite.FileStorage.Std;

namespace Nephrite.Hibernate.CoreMapping
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
			Property(x => x.LastModifiedDate);
			Property(x => x.Size);
			Property(x => x.Owner);
			Property(x => x.Title);
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