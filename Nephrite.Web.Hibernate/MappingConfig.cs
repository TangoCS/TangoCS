using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;

namespace Nephrite.Web.Hibernate
{
	public static class MappingConfig
	{
		public static void BoolPropertyConfig(IPropertyMapper map)
		{
			if (HDataContext.DBType == DBType.DB2) map.Type<IntBackedBoolUserType>();
		}

		public static void GuidPropertyConfig(IPropertyMapper map)
		{
			if (HDataContext.DBType == DBType.DB2) map.Type<StringBackedGuidUserType>();
		}
	}
}