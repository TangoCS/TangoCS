using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.ErrorLog;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nephrite.Hibernate.CoreMapping
{
	public class IErrorLogMap : ClassMapping<IErrorLog>
	{
		public IErrorLogMap()
		{
			Table("ErrorLog");
			Lazy(true);
			Id(x => x.ErrorLogID, map => map.Generator(Generators.Identity));
			Discriminator(x => x.Formula("0"));
			Property(x => x.ErrorDate, map => map.NotNullable(true));
			Property(x => x.ErrorText, map => map.NotNullable(true));
			Property(x => x.Url);
			Property(x => x.UrlReferrer);
			Property(x => x.UserHostName);
			Property(x => x.UserHostAddress);
			Property(x => x.UserAgent);
			Property(x => x.RequestType);
			Property(x => x.Headers);
			Property(x => x.SqlLog);
			Property(x => x.UserName);
			Property(x => x.Hash);
			Property(x => x.SimilarErrorID);
		}
	}
}