using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Controls;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IN_FilterMap : ClassMapping<IN_Filter>
	{
		public IN_FilterMap()
		{
			Table("N_Filter");
			Lazy(true);
			Id(x => x.FilterID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.ListName);
			Property(x => x.FilterValue, map => map.Type<XDocType>());
			Property(x => x.FilterName);
			Property(x => x.IsDefault, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
			Property(x => x.Group1Column);
			Property(x => x.Group1Sort);
			Property(x => x.Group2Column);
			Property(x => x.Group2Sort);
			Property(x => x.ListParms);
			Property(x => x.Columns);
			Property(x => x.Sort);
			Property(x => x.ItemsOnPage, map => map.NotNullable(true));
			Property(x => x.SubjectID, map =>
			{
				map.Column("SubjectID");
			});
		}
	}

}