using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SettingsManager;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IN_SettingsMap : ClassMapping<IN_Settings>
	{
		public IN_SettingsMap()
		{
			Table("N_Settings");
			Lazy(true);
			Id(x => x.SettingsGUID, map => { map.Generator(Generators.Assigned); MappingConfig.GuidIDPropertyConfig(map); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.SystemName, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Value, map => map.NotNullable(true));
			Property(x => x.IsSystem, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.AcceptableValues);
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
		}
	}
}