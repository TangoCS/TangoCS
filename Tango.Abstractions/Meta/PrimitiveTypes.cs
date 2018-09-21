using System;
using System.Xml.Linq;

namespace Tango.Meta
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
	public partial class MetaMoneyType : MetaPrimitiveType, IMetaNumericType
	{
		public int Precision { get; set; }
		public int Scale { get; set; }
		public override string ToCSharpType(bool nullable) => nullable ? "decimal?" : "decimal";
	}
	public partial class MetaStringType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		public int Length { get; set; }
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => "string";
	}
	public partial class MetaDateTimeType : MetaPrimitiveType, IMetaParameterType, IMetaIdentifierType
	{
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "DateTime?" : "DateTime";
	}
	public partial class MetaDateType : MetaPrimitiveType, IMetaIdentifierType
	{
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "DateTime?" : "DateTime";
	}
    public partial class MetaXmlType : MetaPrimitiveType 
	{
		public override string ToCSharpType(bool nullable) => "XDocument";
	}
	public partial class MetaIntType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType, IMetaParameterType
	{
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "int?" : "int";
	}
	public partial class MetaLongType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType 
	{
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "long?" : "long";
	}
	public partial class MetaShortType : MetaPrimitiveType, IMetaNumericType
	{
		public string ColumnSuffix { get; set; } = "ID";
		public override string ToCSharpType(bool nullable) => nullable ? "short?" : "short";
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
		public string ColumnSuffix { get; set; } = "GUID";
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
		public static MetaStringType String = new MetaStringType();
		public static MetaByteArrayType ByteArray = new MetaByteArrayType();
		public static MetaDateType Date = new MetaDateType();
		public static MetaDateTimeType DateTime = new MetaDateTimeType();
		public static MetaZoneDateTimeType ZoneDateTime = new MetaZoneDateTimeType();
		public static MetaIntType Int = new MetaIntType();
		public static MetaLongType Long = new MetaLongType();
		public static MetaShortType Short = new MetaShortType();
		public static MetaBooleanType Boolean = new MetaBooleanType();
		public static MetaGuidType Guid = new MetaGuidType();
		public static MetaDecimalType Decimal = new MetaDecimalType { Precision = 18, Scale = 5 };
		public static MetaMoneyType Money = new MetaMoneyType { Precision = 18, Scale = 2 };
		public static MetaFileType FileIntKey = new MetaFileType(Int);
		public static MetaFileType FileGuidKey = new MetaFileType(Guid);
		public static MetaXmlType Xml = new MetaXmlType();

		public static MetaEnumType Enum<T>() where T : IEnum { return new MetaEnumType { Name = typeof(T).Name }; }
		public static MetaStringType CustomString(int length) { return new MetaStringType { Length = length }; }
		public static MetaDecimalType CustomDecimal(int precision, int scale) { return new MetaDecimalType { Precision = precision, Scale = scale }; }
		public static MetaByteArrayType CustomByteArray(int length) { return new MetaByteArrayType() { Length = length }; }

		public static IMetaPrimitiveType FromCSharpType(Type type)
		{
			if (type == typeof(string)) return String;
			if (type == typeof(int) || type == typeof(int?)) return Int;
			if (type == typeof(bool) || type == typeof(bool?)) return Boolean;
			if (type == typeof(Guid) || type == typeof(Guid?)) return Guid;
			if (type == typeof(decimal) || type == typeof(decimal?)) return Decimal;
			if (type == typeof(DateTime) || type == typeof(DateTime?)) return DateTime;
			//if (typeof(IEnum).IsAssignableFrom(type)) return Enum(type.Name);
			if (type == typeof(long) || type == typeof(long?)) return Long;
			if (type == typeof(short) || type == typeof(short?)) return Short;
			if (type == typeof(byte[])) return ByteArray;
			if (type == typeof(XDocument)) return Xml;
			throw new NotSupportedException("Type " + type.Name + " is not supported");
		}
	}
}