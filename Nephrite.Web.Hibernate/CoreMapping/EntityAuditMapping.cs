using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.EntityAudit;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
	public class IN_ObjectChangeMap : ClassMapping<IN_ObjectChange>
	{
		public IN_ObjectChangeMap()
		{
			Table("N_ObjectChange");
			Lazy(true);
			Id(x => x.ObjectChangeID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.LastModifiedDate, map => map.NotNullable(true));
			Property(x => x.Title);
			Property(x => x.ObjectKey, map => map.NotNullable(true));
			Property(x => x.ObjectTypeSysName);
			Property(x => x.ObjectTitle, map => map.NotNullable(true));
			Property(x => x.UserTitle, map => map.NotNullable(true));
			Property(x => x.UserLogin, map => map.NotNullable(true));
			Property(x => x.ObjectTypeTitle, map => map.NotNullable(true));
			Property(x => x.IP, map => map.NotNullable(true));
			Property(x => x.SubjectID, map => map.NotNullable(true));
			Property(x => x.Details, map => map.NotNullable(true));
		}
	}

	public class IN_ObjectPropertyChangeMap : ClassMapping<IN_ObjectPropertyChange>
	{
		public IN_ObjectPropertyChangeMap()
		{
			Table("N_ObjectPropertyChange");
			Lazy(true);
			Id(x => x.ObjectPropertyChangeID, map => { map.Generator(Generators.Identity); });
			Discriminator(x => x.Formula("0"));
			Property(x => x.Title);
			Property(x => x.PropertySysName);
			Property(x => x.OldValue);
			Property(x => x.NewValue);
			Property(x => x.OldValueTitle);
			Property(x => x.NewValueTitle);
			Property(x => x.ObjectChangeID, map => { map.Formula("ObjectChangeID"); });
			ManyToOne(x => x.ObjectChange, map =>
			{
				map.Column("ObjectChangeID");
				map.Cascade(Cascade.None);
			});
		}
	}
}