using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.TextResources;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Web.Hibernate.CoreMapping
{
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