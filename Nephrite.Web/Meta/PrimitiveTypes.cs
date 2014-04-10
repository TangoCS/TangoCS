using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta
{
	public interface IMetaIdentifierType
	{
		string ColumnSuffix { get; }
	}

	public interface IMetaNumericType { }

	public class MetaPrimitiveType : MetaClassifier
	{
		public bool NotNullable { get; set; }
		public override string CLRType
		{
			get
			{
				return "";
			}
		}
	}

	public partial class MetaEnum : MetaPrimitiveType
	{
		public override string CLRType
		{
			get
			{
				return "string";
			}
		}
	}

	public partial class MetaDecimalType : MetaPrimitiveType, IMetaNumericType
	{
		public int Precision { get; set; }
		public int Scale { get; set; }

		public override string CLRType
		{
			get
			{
				return NotNullable ? "decimal" : "decimal?";
			}
		}
	}

	public partial class MetaStringType : MetaPrimitiveType
	{
		public int Length { get; set; }
		public override string CLRType
		{
			get
			{
				return "string";
			}
		}
	}

	public partial class MetaDateTimeType : MetaPrimitiveType
	{
		public override string CLRType
		{
			get
			{
				return NotNullable ? "DateTime" : "DateTime?";
			}
		}
	}
    public partial class MetaXmlType : MetaPrimitiveType
    {
        public override string CLRType
        {
            get
            {
                return "XDocument";
            }
        }
    }
	public partial class MetaZoneDateTimeType : MetaPrimitiveType
	{

	}

	public partial class MetaDateType : MetaPrimitiveType
	{
		public override string CLRType
		{
			get
			{
				return NotNullable ? "DateTime" : "DateTime?";
			}
		}
	}

	public partial class MetaIntType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType
	{

		public string ColumnSuffix
		{
			get { return "ID"; }
		}

		public override string CLRType
		{
			get
			{
				return NotNullable ? "int" : "int?";
			}
		}
	}

	public partial class MetaLongType : MetaPrimitiveType, IMetaNumericType
	{
		public override string CLRType
		{
			get
			{
				return NotNullable ? "long" : "long?";
			}
		}
	}

	public partial class MetaByteArrayType : MetaPrimitiveType
	{
		public int Length { get; set; }
		public override string CLRType
		{
			get
			{
				return "byte[]";
			}
		}
	}

	public partial class MetaBooleanType : MetaPrimitiveType
	{
		public override string CLRType
		{
			get
			{
				return NotNullable ? "bool" : "bool?";
			}
		}
	}

	

	public partial class MetaGuidType : MetaPrimitiveType, IMetaIdentifierType
	{
		public string ColumnSuffix
		{
			get { return "GUID"; }
		}
		public override string CLRType
		{
			get
			{
				return NotNullable ? "Guid" : "Guid?";
			}
		}
	}

	public class MetaFileType : MetaPrimitiveType
	{
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