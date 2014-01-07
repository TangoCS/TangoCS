using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using NHibernate;
using Nephrite.Web.MetaStorage;

namespace Nephrite.Web.CoreDataContext
{
	public class IMM_FormViewMap : ClassMapping<IMM_FormView> //IPage could inherit: IMultiTaggedPage, ISearchablePage
	{
		public IMM_FormViewMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.FormViewID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.ViewTemplate);
			Property(x => x.TemplateTypeCode);
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsCustom, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.IsCaching, map => map.NotNullable(true));
			Property(x => x.CacheKeyParams);
			Property(x => x.CacheTimeout, map => map.NotNullable(true));
			Property(x => x.BaseClass, map => map.NotNullable(true));

			Property(x => x.ObjectTypeID);
			Property(x => x.PackageID);
		}
	}
	public class IMM_PackageMap : ClassMapping<IMM_Package> //IPage could inherit: IMultiTaggedPage, ISearchablePage
	{
		public IMM_PackageMap()
		{
			Schema("dbo");
			Lazy(true);
			Id(x => x.PackageID, map => map.Generator(Generators.Identity));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => map.NotNullable(true));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.Guid, map => map.NotNullable(true));
			Property(x => x.IsDataReplicated, map => map.NotNullable(true));
			Property(x => x.Version);
			Property(x => x.SeqNo, map => map.NotNullable(true));
			Property(x => x.ParentPackageID);
		}
	}
}