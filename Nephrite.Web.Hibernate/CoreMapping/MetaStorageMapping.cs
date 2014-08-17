using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.MetaStorage;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IMM_PackageMap : ClassMapping<IMM_Package>
	{
		public IMM_PackageMap()
		{
			Schema("dbo");
			Table("MM_Package");
			Lazy(true);
			Id(x => x.PackageID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.Version);
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.ParentPackageID);
		}
	}

	public class IMM_ObjectTypeMap : ClassMapping<IMM_ObjectType>
	{
		public IMM_ObjectTypeMap()
		{
			Schema("dbo");
			Table("MM_ObjectType");
			Lazy(true);
			Id(x => x.ObjectTypeID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsEnableSPM, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsSeparateTable, map => map.NotNullable(true));
			Property(x => x.IsTemplate, map => map.NotNullable(true));
			Property(x => x.TitlePlural);
			Property(x => x.DefaultOrderBy);
			Property(x => x.LogicalDelete);
			Property(x => x.IsReplicate, map => map.NotNullable(true));
			Property(x => x.IsEnableUserViews, map => map.NotNullable(true));
			Property(x => x.SecurityPackageSystemName);
			Property(x => x.IsEnableObjectHistory, map => map.NotNullable(true));
			Property(x => x.Interface);
			Property(x => x.HistoryTypeCode, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.Description);

			Property(x => x.PackageID);
			Property(x => x.BaseObjectTypeID);
		}
	}

	public class IMM_FormViewMap : ClassMapping<IMM_FormView>
	{
		public IMM_FormViewMap()
		{
			Schema("dbo");
			Table("MM_FormView");
			Lazy(true);
			Id(x => x.FormViewID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.ViewTemplate);
			Property(x => x.TemplateTypeCode);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Guid, map => { map.NotNullable(true); MappingConfig.GuidPropertyConfig(map); });
			Property(x => x.IsCustom, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsCaching, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.CacheKeyParams);
			Property(x => x.CacheTimeout, map => map.NotNullable(true));
			Property(x => x.BaseClass, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID);
			Property(x => x.PackageID);
			//Property(x => x.PackageID, map => map.Formula("PackageID"));

			//ManyToOne(x => x.Package, map =>
			//{
			//	map.Column("PackageID");
			//	map.Cascade(Cascade.None);
			//});
		}
	}

	public class IN_CacheMap : ClassMapping<IN_Cache>
	{
		public IN_CacheMap()
		{
			Schema("dbo");
			Table("N_Cache");
			Lazy(true);
			Discriminator(x => x.Formula("0"));
			Property(x => x.TimeStamp, map => map.NotNullable(true));
		}
	}
}