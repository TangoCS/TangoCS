using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nephrite.Meta.Database;

namespace Nephrite.Meta
{
	public partial interface IMetaClassifier
	{
		string GetDBType(IDBTypeMapper mapper);
	}
}

namespace Nephrite.Meta.Database
{
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
