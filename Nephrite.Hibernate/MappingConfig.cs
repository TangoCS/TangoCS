using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Data;
using NHibernate.Mapping.ByCode;

namespace Nephrite.Hibernate
{
	public static class MappingConfig
	{
		public static void BoolPropertyConfig(IPropertyMapper map)
		{
			if (ConnectionManager.DBType == DBType.DB2) map.Type<IntBackedBoolUserType>();
		}

		public static void GuidPropertyConfig(IPropertyMapper map)
		{
			if (ConnectionManager.DBType == DBType.DB2) map.Type<StringBackedGuidUserType>();
		}

		public static void GuidIDPropertyConfig(IIdMapper map)
		{
			if (ConnectionManager.DBType == DBType.DB2) map.Type(new StringBackedGuidUserType());
		}
	}
}