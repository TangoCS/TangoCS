using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Multilanguage;
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

	public class IN_TextResourceMap : ClassMapping<IN_TextResource>
	{
		public IN_TextResourceMap()
		{

			Table("\"V_N_TextResource\"");

			Lazy(true);
			Id(x => x.TextResourceID, map =>
			{
				map.Generator(Generators.Identity);
				map.Column("\"TextResourceID\"");
			});
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title, map =>
			{
				map.NotNullable(true);
				map.Column("\"Title\"");
			});
			Property(x => x.SysName, map =>
			{
				map.NotNullable(true); ;
				map.Column("\"SysName\"");
			});
			Property(x => x.Text, map => map.Column("\"Text\""));
			Property(x => x.LanguageCode, map =>
			{
				map.NotNullable(true); ;
				map.Column("\"LanguageCode\"");
			});
			Property(x => x.TextResourceID, map =>
			{
				map.NotNullable(true); ;
				map.Column("\"TextResourceID\"");
			});
		}
	}
}