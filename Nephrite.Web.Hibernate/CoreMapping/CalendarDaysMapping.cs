using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Web.CalendarDays;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class ICalendarDayMap : ClassMapping<ICalendarDay>
	{
		public ICalendarDayMap()
		{
			Table("CalendarDay");
			Lazy(true);
			Id(x => x.CalendarDayID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.Date, map => map.NotNullable(true));
			Property(x => x.IsWorkingDay, map => { map.NotNullable(true); MappingConfig.BoolPropertyConfig(map); });
		}
	}
}