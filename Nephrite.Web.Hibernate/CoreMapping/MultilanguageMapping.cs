using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.Multilanguage;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IC_LanguageMap : ClassMapping<IC_Language>
	{
		public IC_LanguageMap()
		{
			Schema("DBO");
			Table("C_Language");
			Lazy(true);
			Id(x => x.Code, map => { map.Generator(Generators.Assigned); map.Length(2); map.Column("LanguageCode"); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title, map => map.NotNullable(true));
			Property(x => x.IsDefault, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
		}
	}
}