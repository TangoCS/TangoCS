using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta
{
	public interface IMetaPrimitiveType : IMetaClassifier
	{
		bool NotNullable { get; }
		/// <summary>
		/// Func &lt;TValue, string, IFormatProvider, string&gt;
		/// </summary>
		object GetStringValue { get; }
	}

	public interface IMetaIdentifierType : IMetaPrimitiveType
	{
		string ColumnSuffix { get; }
	}

	public interface IMetaParameterType : IMetaPrimitiveType
	{
	}

	public interface IMetaNumericType : IMetaPrimitiveType { }

	public abstract class MetaPrimitiveType : MetaClassifier, IMetaPrimitiveType
	{
		public bool NotNullable { get; set; }
		/// <summary>
		/// Func &lt;TValue, string, IFormatProvider, string&gt;
		/// </summary>
		public object GetStringValue { get; set; }
	}

	public class MetaEnum : MetaPrimitiveType
	{
		public override string CLRType
		{
			get
			{
				return "string";
			}
		}
	}

	public partial class MetaZoneDateTimeType : MetaPrimitiveType
	{
		public override string CLRType
		{
			get { return ""; }
		}
	}

	public partial class MetaDecimalType : MetaPrimitiveType, IMetaNumericType
	{
		static MetaDecimalType _t = new MetaDecimalType { Precision = 18, Scale = 5, NotNullable = true, GetStringValue = ToStringConverter.Decimal };
		static MetaDecimalType _t_n = new MetaDecimalType { Precision = 18, Scale = 5, NotNullable = false, GetStringValue = ToStringConverter.NullableDecimal };
		public static MetaDecimalType NotNull() { return _t; }
		public static MetaDecimalType Null() { return _t_n; }

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

	public partial class MetaStringType : MetaPrimitiveType, IMetaParameterType
	{
		static MetaStringType _t = new MetaStringType { NotNullable = true, GetStringValue = ToStringConverter.String };
		static MetaStringType _t_n = new MetaStringType { NotNullable = false, GetStringValue = ToStringConverter.String };
		public static MetaStringType NotNull() { return _t; }
		public static MetaStringType Null() { return _t_n; }


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
		static MetaDateType _t = new MetaDateType { NotNullable = true, GetStringValue = ToStringConverter.DateTime };
		static MetaDateType _t_n = new MetaDateType { NotNullable = false, GetStringValue = ToStringConverter.NullableDateTime };
		public static MetaDateType NotNull() { return _t; }
		public static MetaDateType Null() { return _t_n; }

		public override string CLRType
		{
			get
			{
				return NotNullable ? "DateTime" : "DateTime?";
			}
		}
	}

	public partial class MetaDateType : MetaPrimitiveType
	{
		static MetaDateType _t = new MetaDateType { NotNullable = true, GetStringValue = ToStringConverter.Date };
		static MetaDateType _t_n = new MetaDateType { NotNullable = false, GetStringValue = ToStringConverter.NullableDate };
		public static MetaDateType NotNull() { return _t; }
		public static MetaDateType Null() { return _t_n; }

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
		static MetaXmlType _t = new MetaXmlType { NotNullable = true, GetStringValue = ToStringConverter.Xml };
		static MetaXmlType _t_n = new MetaXmlType { NotNullable = false, GetStringValue = ToStringConverter.Xml };
		public static MetaXmlType NotNull() { return _t; }
		public static MetaXmlType Null() { return _t_n; }

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



	public partial class MetaIntType : MetaPrimitiveType, IMetaIdentifierType, IMetaNumericType, IMetaParameterType
	{
		static MetaIntType _t = new MetaIntType { NotNullable = true, GetStringValue = ToStringConverter.Int };
		static MetaIntType _t_n = new MetaIntType { NotNullable = false, GetStringValue = ToStringConverter.NullableInt };
		public static MetaIntType NotNull() { return _t; }
		public static MetaIntType Null() { return _t_n; }

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
		static MetaLongType _t = new MetaLongType { NotNullable = true, GetStringValue = ToStringConverter.Long };
		static MetaLongType _t_n = new MetaLongType { NotNullable = false, GetStringValue = ToStringConverter.NullableLong };
		public static MetaLongType NotNull() { return _t; }
		public static MetaLongType Null() { return _t_n; }

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
		static MetaByteArrayType _t = new MetaByteArrayType { NotNullable = true };
		static MetaByteArrayType _t_n = new MetaByteArrayType { NotNullable = false };
		public static MetaByteArrayType NotNull() { return _t; }
		public static MetaByteArrayType Null() { return _t_n; }

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
		static MetaBooleanType _t = new MetaBooleanType { NotNullable = true, GetStringValue = ToStringConverter.Bool };
		static MetaBooleanType _t_n = new MetaBooleanType { NotNullable = false, GetStringValue = ToStringConverter.NullableBool };
		public static MetaBooleanType NotNull() { return _t; }
		public static MetaBooleanType Null() { return _t_n; }

		public override string CLRType
		{
			get
			{
				return NotNullable ? "bool" : "bool?";
			}
		}
	}



	public partial class MetaGuidType : MetaPrimitiveType, IMetaIdentifierType, IMetaParameterType
	{
		static MetaGuidType _t = new MetaGuidType { NotNullable = true, GetStringValue = ToStringConverter.Guid };
		static MetaGuidType _t_n = new MetaGuidType { NotNullable = false, GetStringValue = ToStringConverter.NullableGuid };
		public static MetaGuidType NotNull() { return _t; }
		public static MetaGuidType Null() { return _t_n; }

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
		static MetaFileType _ti = new MetaFileType { IdentifierType = MetaIntType.NotNull(), NotNullable = true };
		static MetaFileType _tg = new MetaFileType { IdentifierType = MetaGuidType.NotNull(), NotNullable = true };
		static MetaFileType _ti_n = new MetaFileType { IdentifierType = MetaIntType.Null(), NotNullable = false };
		static MetaFileType _tg_n = new MetaFileType { IdentifierType = MetaGuidType.Null(), NotNullable = false };

		public static MetaFileType IntKeyNotNull() { return _ti; }
		public static MetaFileType IntKeyNull() { return _ti_n; }
		public static MetaFileType GuidKeyNotNull() { return _tg; }
		public static MetaFileType GuidKeyNull() { return _tg_n; }

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