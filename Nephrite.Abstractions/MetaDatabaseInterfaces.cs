namespace Nephrite.Meta
{
	public partial interface IMetaPrimitiveType
	{
		string GetDBType(IDBTypeMapper mapper);
	}

	public interface IDBTypeMapper
	{
		string GetIntType();
		string GetGuidType();
		string GetStringType(int length);
		string GetDecimalType(int precision, int scale);
		string GetDateTimeType();
		string GetDateType();
		string GetZoneDateTimeType();
		string GetLongType();
		string GetByteArrayType(int length);
		string GetBooleanType();
		string GetXmlType();
		IMetaPrimitiveType GetType(string dataType, bool notNull);
	}
}