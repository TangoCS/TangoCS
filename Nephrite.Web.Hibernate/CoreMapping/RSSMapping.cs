using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.RSS;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Hibernate.CoreMapping
{
	public class IN_RssFeedMap : ClassMapping<IN_RssFeed>
	{
		public IN_RssFeedMap()
		{
			Table("N_RssFeed");
			Lazy(true);
			Id(x => x.RssFeedID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.Copyright, map => map.NotNullable(true));
			Property(x => x.Description, map => map.NotNullable(true));
			Property(x => x.IsDeleted, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.LastModifiedUserID, map => map.NotNullable(true));
			Property(x => x.ObjectTypeSysName, map => map.NotNullable(true));
			Property(x => x.Predicate, map => map.NotNullable(true));
			Property(x => x.PubDate, map => map.NotNullable(true));
			Property(x => x.SysName, map => map.NotNullable(true));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.Ttl, map => map.NotNullable(true));
			Property(x => x.ViewFormSysName, map => map.NotNullable(true));
			Property(x => x.Author, map => map.NotNullable(true));
			Property(x => x.WebMaster, map => map.NotNullable(true));
			Property(x => x.LinkParams);
		}
	}
}