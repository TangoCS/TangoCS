using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Meta
{
	public abstract class MetaPrimitiveType : MetaClassifier, IMetaPrimitiveType
	{
		public bool NotNullable { get; set; }
		/// <summary>
		/// Func &lt;TValue, string, IFormatProvider, string&gt;
		/// </summary>
		public object GetStringValue { get; set; }


		public IMetaPrimitiveType Clone(bool notNullable)
		{
			var copy = MemberwiseClone() as MetaPrimitiveType;
			copy.NotNullable = notNullable;
			return copy;
		}
	}

	public partial class MetaEnumType : MetaPrimitiveType
	{
		public static MetaEnumType NotNull(string enumName) { return new MetaEnumType { NotNullable = true, Name = enumName }; }
		public static MetaEnumType Null(string enumName) { return new MetaEnumType { NotNullable = false, Name = enumName }; }

		public override string CLRType => "string";
	}

	public partial class MetaZoneDateTimeType : MetaPrimitiveType
	{
		public override string CLRType => "";
	}

	public partial class MetaDecimalType : MetaPrimitiveType, IMetaNumericType
	{
		public static MetaDecimalType NotNull() { return new MetaDecimalType { Precision = 18, Scale = 5, NotNullable = true, GetStringValue = ToStringConverter.Decimal }; }
		public static MetaDecimalType Null() { return new MetaDecimalType { Precision = 18, Scale = 5, NotNullable = false, GetStringValue = ToStringConverter.NullableDecimal }; }

		public int Precision { get; set; }
		public int Scale { get; set; }

		public override string CLRType => NotNullable ? "decimal" : "decimal?";
	}

	public partial class MetaStringType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		public static MetaStringType NotNull() { return new MetaStringType { Length = -1, NotNullable = true, GetStringValue = ToStringConverter.String }; }
		public static MetaStringType Null() { return new MetaStringType { Length = -1, NotNullable = false, GetStringValue = ToStringConverter.String }; }

		public int Length { get; set; }

		public string ColumnSuffix => "";
		public override string CLRType => "string";
	}

	public partial class MetaDateTimeType : MetaPrimitiveType, IMetaParameterType
	{
		public static MetaDateTimeType NotNull() { return new MetaDateTimeType { NotNullable = true, GetStringValue = ToStringConverter.DateTime }; }
		public static MetaDateTimeType Null() { return new MetaDateTimeType { NotNullable = false, GetStringValue = ToStringConverter.NullableDateTime }; }

		public override string CLRType => NotNullable ? "DateTime" : "DateTime?";
	}

	public partial class MetaDateType : MetaPrimitiveType
	{
		public static MetaDateType NotNull() { return new MetaDateType { NotNullable = true, GetStringValue = ToStringConverter.Date }; }
		public static MetaDateType Null() { return new MetaDateType { NotNullable = false, GetStringValue = ToStringConverter.NullableDate }; }

		public override string CLRType => NotNullable ? "DateTime" : "DateTime?";
	}

    public partial class MetaXmlType : MetaPrimitiveType
    {
		public static MetaXmlType NotNull() { return new MetaXmlType { NotNullable = true, GetStringValue = ToStringConverter.Xml }; }
		public static MetaXmlType Null() { return new MetaXmlType { NotNullable = false, GetStringValue = ToStringConverter.Xml }; }

        public override string CLRType => "XDocument";
    }

	public partial class MetaZoneDateTimeType : MetaPrimitiveType
	{

	}



	public partial class MetaIntType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType, IMetaParameterType
	{
		public static MetaIntType NotNull() { return new MetaIntType { NotNullable = true, GetStringValue = ToStringConverter.Int }; }
		public static MetaIntType Null() { return new MetaIntType { NotNullable = false, GetStringValue = ToStringConverter.NullableInt }; }

		public string ColumnSuffix => "ID";
		public override string CLRType => NotNullable ? "int" : "int?";
	}

	public partial class MetaLongType : MetaPrimitiveType, IMetaNumericType
	{
		public static MetaLongType NotNull() { return new MetaLongType { NotNullable = true, GetStringValue = ToStringConverter.Long }; }
		public static MetaLongType Null() { return new MetaLongType { NotNullable = false, GetStringValue = ToStringConverter.NullableLong }; }

		public override string CLRType => NotNullable ? "long" : "long?";
	}

	public partial class MetaByteArrayType : MetaPrimitiveType
	{
		public static MetaByteArrayType NotNull() { return new MetaByteArrayType { NotNullable = true }; }
		public static MetaByteArrayType Null() { return new MetaByteArrayType { NotNullable = false }; }

		public int Length { get; set; }
		public override string CLRType => "byte[]";
	}

	public partial class MetaBooleanType : MetaPrimitiveType
	{
		public static MetaBooleanType NotNull() { return new MetaBooleanType { NotNullable = true, GetStringValue = ToStringConverter.Bool }; }
		public static MetaBooleanType Null() { return new MetaBooleanType { NotNullable = false, GetStringValue = ToStringConverter.NullableBool }; }

		public override string CLRType => NotNullable ? "bool" : "bool?";
	}



	public partial class MetaGuidType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		public static MetaGuidType NotNull() { return new MetaGuidType { NotNullable = true, GetStringValue = ToStringConverter.Guid }; }
		public static MetaGuidType Null() { return new MetaGuidType { NotNullable = false, GetStringValue = ToStringConverter.NullableGuid }; }

		public string ColumnSuffix => "GUID";
		public override string CLRType => NotNullable ? "Guid" : "Guid?";
	}

	public class MetaFileType : MetaPrimitiveType
	{
		public static MetaFileType IntKeyNotNull() { return new MetaFileType { IdentifierType = MetaIntType.NotNull(), NotNullable = true }; }
		public static MetaFileType IntKeyNull() { return new MetaFileType { IdentifierType = MetaIntType.Null(), NotNullable = false }; }
		public static MetaFileType GuidKeyNotNull() { return new MetaFileType { IdentifierType = MetaGuidType.NotNull(), NotNullable = true }; }
		public static MetaFileType GuidKeyNull() { return new MetaFileType { IdentifierType = MetaGuidType.Null(), NotNullable = false }; }
		public static MetaFileType NotNull() { return GuidKeyNotNull(); }
		public static MetaFileType Null() { return GuidKeyNull(); }


		public IMetaIdentifierType IdentifierType { get; set; }

		public override string ColumnName(string propName)
		{
			return propName + IdentifierType.ColumnSuffix;
		}

		public override string CLRType
		{
			get
			{
				return (IdentifierType as MetaPrimitiveType).CLRType;
			}
		}
	}

	public static class TypeFactory
	{
		static MetaByteArrayType _byteArray = new MetaByteArrayType { Name = "Data", NotNullable = true };
		static MetaStringType _string = new MetaStringType { Name = "String", NotNullable = true };
		static MetaDateType _date = new MetaDateType { Name = "Date", NotNullable = true };
		static MetaDateTimeType _dateTime = new MetaDateTimeType { Name = "DateTime", NotNullable = true };
		static MetaZoneDateTimeType _zoneDateTime = new MetaZoneDateTimeType { Name = "ZoneDateTime", NotNullable = true };
		static MetaIntType _int = new MetaIntType { Name = "Int", NotNullable = true };
		static MetaLongType _long = new MetaLongType { Name = "Long", NotNullable = true };
		static MetaBooleanType _boolean = new MetaBooleanType { Name = "Boolean", NotNullable = true };
		static MetaGuidType _guid = new MetaGuidType { Name = "Guid", NotNullable = true };
		static MetaFileType _fileIntKey = new MetaFileType { Name = "FileID", IdentifierType = _int, NotNullable = true };
		static MetaFileType _fileGuidKey = new MetaFileType { Name = "FileGUID", IdentifierType = _guid, NotNullable = true };
		static MetaDecimalType _decimal = new MetaDecimalType { Precision = 18, Scale = 5, Name = "Decimal", NotNullable = true };
		//static MetaEnumType _enum = new MetaEnumType { Name = "Enum", NotNullable = true };

		static MetaByteArrayType _byteArray_n = new MetaByteArrayType { Name = "Data", NotNullable = false };
		static MetaStringType _string_n = new MetaStringType { Name = "String", NotNullable = false };
		static MetaDateType _date_n = new MetaDateType { Name = "Date", NotNullable = false };
		static MetaDateTimeType _dateTime_n = new MetaDateTimeType { Name = "DateTime", NotNullable = false };
		static MetaZoneDateTimeType _zoneDateTime_n = new MetaZoneDateTimeType { Name = "ZoneDateTime", NotNullable = false };
		static MetaIntType _int_n = new MetaIntType { Name = "Int", NotNullable = false };
		static MetaLongType _long_n = new MetaLongType { Name = "Long", NotNullable = false };
		static MetaBooleanType _boolean_n = new MetaBooleanType { Name = "Boolean", NotNullable = false };
		static MetaGuidType _guid_n = new MetaGuidType { Name = "Guid", NotNullable = false };
		static MetaFileType _fileIntKey_n = new MetaFileType { Name = "FileID", IdentifierType = _int_n, NotNullable = false };
		static MetaFileType _fileGuidKey_n = new MetaFileType { Name = "FileGUID", IdentifierType = _guid_n, NotNullable = false };
		static MetaDecimalType _decimal_n = new MetaDecimalType { Precision = 18, Scale = 5, Name = "Decimal", NotNullable = false };
		//static MetaEnumType _enum_n = new MetaEnumType { Name = "Enum", NotNullable = false };

		public static MetaStringType String(int length, bool notNull) { return new MetaStringType { Length = length, Name = "String", NotNullable = notNull }; }
		public static MetaStringType String(bool notNull) { return notNull ? _string : _string_n; }
		//public static MetaEnumType Enum(bool notNull) { return notNull ? _enum : _enum_n; }
		public static MetaByteArrayType ByteArray(bool notNull) { return notNull ? _byteArray : _byteArray_n; }
		public static MetaByteArrayType ByteArray(int length, bool notNull) { return new MetaByteArrayType() { Name = "ByteArray", Length = length, NotNullable = notNull }; }

		public static MetaDateType Date(bool notNull) { return notNull ? _date : _date_n; }
		public static MetaDateTimeType DateTime(bool notNull) { return notNull ? _dateTime : _dateTime_n; }
		public static MetaZoneDateTimeType ZoneDateTime(bool notNull) { return notNull ? _zoneDateTime : _zoneDateTime_n; }
		public static MetaIntType Int(bool notNull) { return notNull ? _int : _int_n; }
		public static MetaLongType Long(bool notNull) { return notNull ? _long : _long_n; }
		public static MetaBooleanType Boolean(bool notNull) { return notNull ? _boolean : _boolean_n; }
		public static MetaGuidType Guid(bool notNull) { return notNull ? _guid : _guid_n; }
		public static MetaDecimalType Decimal(bool notNull) { return notNull ? _decimal : _decimal_n; }
		public static MetaDecimalType Decimal(int precision, int scale, bool notNull) { return new MetaDecimalType { Precision = precision, Scale = scale, Name = "Decimal", NotNullable = notNull }; }
		public static MetaFileType FileIntKey(bool notNull) { return notNull ? _fileIntKey : _fileIntKey_n; }
		public static MetaFileType FileGuidKey(bool notNull) { return notNull ? _fileGuidKey : _fileGuidKey_n; }

	}
}