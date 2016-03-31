using System;
using System.Xml.Linq;

namespace Nephrite.Meta
{
	public abstract partial class MetaPrimitiveType : IMetaPrimitiveType
	{
		//public bool NotNullable { get; set; }
		//public abstract Func<T, string, IFormatProvider, string> GetStringValue { get; }

		//public IMetaPrimitiveType Clone(bool notNullable)
		//{
		//	var copy = MemberwiseClone() as MetaPrimitiveType;
		//	copy.NotNullable = notNullable;
		//	return copy;
		//}
		public abstract string ToCSharpType(bool nullable);
	}

	public partial class MetaEnumType : MetaPrimitiveType
	{
		public string Name { get; set; }
		public override string ToCSharpType(bool nullable) => "string";
	}
	public partial class MetaZoneDateTimeType : MetaPrimitiveType 
	{
		public override string ToCSharpType(bool nullable) => nullable ? "DateTime?" : "DateTime";
	}
	public partial class MetaDecimalType : MetaPrimitiveType, IMetaNumericType
	{
		public int Precision { get; set; }
		public int Scale { get; set; }
		public override string ToCSharpType(bool nullable) => nullable ? "decimal?" : "decimal";
	}
	public partial class MetaStringType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		public int Length { get; set; }
		public string ColumnSuffix => "";
		public override string ToCSharpType(bool nullable) => "string";
	}
	public partial class MetaDateTimeType : MetaPrimitiveType, IMetaParameterType 
	{
		public override string ToCSharpType(bool nullable) => nullable ? "DateTime?" : "DateTime";
	}
	public partial class MetaDateType : MetaPrimitiveType 
	{
		public override string ToCSharpType(bool nullable) => nullable ? "DateTime?" : "DateTime";
	}
    public partial class MetaXmlType : MetaPrimitiveType 
	{
		public override string ToCSharpType(bool nullable) => "XDocument";
	}
	public partial class MetaIntType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType, IMetaParameterType
	{
		public string ColumnSuffix => "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "int?" : "int";
	}
	public partial class MetaLongType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType 
	{
		public string ColumnSuffix => "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "long?" : "long";
	}
	public partial class MetaByteArrayType : MetaPrimitiveType
	{
		public int Length { get; set; }
		public override string ToCSharpType(bool nullable) => "byte[]";
	}
	public partial class MetaBooleanType : MetaPrimitiveType 
	{
		public override string ToCSharpType(bool nullable) => nullable ? "bool?" : "bool";
	}
	public partial class MetaGuidType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		public string ColumnSuffix => "GUID";
		public override string ToCSharpType(bool nullable) => nullable ? "Guid?" : "Guid";
	}
	public class MetaFileType : MetaPrimitiveType
	{
		public MetaFileType(IMetaIdentifierType identifierType)
		{
			IdentifierType = identifierType;
		}

		public IMetaIdentifierType IdentifierType { get; private set; }
		public override string ToCSharpType(bool nullable) => IdentifierType.ToCSharpType(nullable);
	}

	public static class TypeFactory
	{
		static MetaByteArrayType _byteArray = new MetaByteArrayType();
		static MetaStringType _string = new MetaStringType();
		static MetaDateType _date = new MetaDateType();
		static MetaDateTimeType _dateTime = new MetaDateTimeType();
		static MetaZoneDateTimeType _zoneDateTime = new MetaZoneDateTimeType();
		static MetaIntType _int = new MetaIntType();
		static MetaLongType _long = new MetaLongType();
		static MetaBooleanType _boolean = new MetaBooleanType();
		static MetaGuidType _guid = new MetaGuidType();
		static MetaFileType _fileIntKey = new MetaFileType(_int);
		static MetaFileType _fileGuidKey = new MetaFileType(_guid);
		static MetaDecimalType _decimal = new MetaDecimalType { Precision = 18, Scale = 5 };
		static MetaXmlType _xml = new MetaXmlType();

		public static MetaStringType String => _string;
		public static MetaByteArrayType ByteArray => _byteArray;
		public static MetaDateType Date => _date;
		public static MetaDateTimeType DateTime => _dateTime;
		public static MetaZoneDateTimeType ZoneDateTime => _zoneDateTime;
		public static MetaIntType Int => _int;
		public static MetaLongType Long => _long;
		public static MetaBooleanType Boolean => _boolean;
		public static MetaGuidType Guid => _guid;
		public static MetaDecimalType Decimal => _decimal;
		public static MetaFileType FileIntKey => _fileIntKey;
		public static MetaFileType FileGuidKey => _fileGuidKey;
		public static MetaXmlType Xml => _xml;

		public static MetaEnumType Enum<T>() where T : IEnum { return new MetaEnumType { Name = typeof(T).Name }; }
		public static MetaStringType CustomString(int length) { return new MetaStringType { Length = length }; }
		public static MetaDecimalType CustomDecimal(int precision, int scale) { return new MetaDecimalType { Precision = precision, Scale = scale }; }
		public static MetaByteArrayType CustomByteArray(int length) { return new MetaByteArrayType() { Length = length }; }

		public static IMetaPrimitiveType FromCSharpType(Type type)
		{
			if (type == typeof(string)) return _string;
			if (type == typeof(int) || type == typeof(int?)) return _int;
			if (type == typeof(bool) || type == typeof(bool?)) return _boolean;
			if (type == typeof(Guid) || type == typeof(Guid?)) return _guid;
			if (type == typeof(decimal) || type == typeof(decimal?)) return _decimal;
			if (type == typeof(DateTime) || type == typeof(DateTime?)) return _dateTime;
			//if (typeof(IEnum).IsAssignableFrom(type)) return Enum(type.Name);
			if (type == typeof(long) || type == typeof(long?)) return _long;
			if (type == typeof(byte[])) return _byteArray;
			if (type == typeof(XDocument)) return _xml;
			throw new NotSupportedException("Type " + type.Name + " is not supported");
		}
	}
}