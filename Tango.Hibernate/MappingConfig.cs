using Tango.Data;
using NHibernate.Mapping.ByCode;

namespace Tango.Hibernate
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

		public static void SequenceGenerator(this IIdMapper map, string seqName)
		{
			map.Generator(Generators.Sequence, g => g.Params(new { sequence = seqName }));
		}
	}
}