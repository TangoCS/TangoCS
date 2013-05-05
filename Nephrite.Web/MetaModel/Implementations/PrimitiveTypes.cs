using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta
{
	public interface IMetaIdentifierType
	{
	}

	public class MetaPrimitiveType : MetaClassifier
	{
		public bool NotNullable { get; set; }
	}

	public class MetaDecimalType : MetaPrimitiveType
	{
		public int Precision { get; set; }
		public int Scale { get; set; }
	}

	public class MetaStringType : MetaClassifier
	{
		public int Length { get; set; }
	}

	public class MetaDateTimeType : MetaPrimitiveType
	{

	}

	public class MetaDateType : MetaPrimitiveType
	{

	}

	public class MetaIntType : MetaPrimitiveType, IMetaIdentifierType
	{

	}

	public class MetaLongType : MetaPrimitiveType
	{

	}

	public class MetaByteArrayType : MetaClassifier
	{

	}

	public class MetaBooleanType : MetaPrimitiveType
	{

	}

	public class MetaGuidType : MetaPrimitiveType, IMetaIdentifierType
	{

	}

	public class MetaFileType : MetaPrimitiveType
	{
		public IMetaIdentifierType IdentifierType { get; set; }
	}

	public static class TypeFactory
	{
		static MetaByteArrayType _byteArray = new MetaByteArrayType { Name = "Data" };

		static MetaDateType _date = new MetaDateType { Name = "Date", NotNullable = true };
		static MetaDateTimeType _dateTime = new MetaDateTimeType { Name = "DateTime", NotNullable = true };
		static MetaIntType _int = new MetaIntType { Name = "Int", NotNullable = true };
		static MetaLongType _long = new MetaLongType { Name = "Long", NotNullable = true };
		static MetaBooleanType _boolean = new MetaBooleanType { Name = "Boolean", NotNullable = true };
		static MetaGuidType _guid = new MetaGuidType { Name = "Guid", NotNullable = true };
		static MetaFileType _fileIDKey = new MetaFileType { Name = "FileID", IdentifierType = TypeFactory.Int(true), NotNullable = true };
		static MetaFileType _fileGUIDKey = new MetaFileType { Name = "FileGUID", IdentifierType = TypeFactory.Guid(true), NotNullable = true };

		static MetaDateType _date_n = new MetaDateType { Name = "Date", NotNullable = false };
		static MetaDateTimeType _dateTime_n = new MetaDateTimeType { Name = "DateTime", NotNullable = false };
		static MetaIntType _int_n = new MetaIntType { Name = "Int", NotNullable = false };
		static MetaLongType _long_n = new MetaLongType { Name = "Long", NotNullable = false };
		static MetaBooleanType _boolean_n = new MetaBooleanType { Name = "Boolean", NotNullable = false };
		static MetaGuidType _guid_n = new MetaGuidType { Name = "Guid", NotNullable = false };
		static MetaFileType _fileIDKey_n = new MetaFileType { Name = "FileID", IdentifierType = TypeFactory.Int(false), NotNullable = false };
		static MetaFileType _fileGUIDKey_n = new MetaFileType { Name = "FileGUID", IdentifierType = TypeFactory.Guid(false), NotNullable = false };

		public static MetaStringType String(int length) { return new MetaStringType { Length = length, Name = "String" }; }
		public static MetaStringType Char(int length) { return new MetaStringType { Length = length, Name = "Char" }; }
		public static MetaByteArrayType ByteArray { get { return _byteArray; } }

		public static MetaDateType Date(bool notNull) { return notNull ? _date : _date_n; }
		public static MetaDateTimeType DateTime(bool notNull) { return notNull ? _dateTime : _dateTime_n; }
		public static MetaIntType Int(bool notNull) { return notNull ? _int : _int_n; }
		public static MetaLongType Long(bool notNull) { return notNull ? _long : _long_n; }
		public static MetaBooleanType Boolean(bool notNull) { return notNull ? _boolean : _boolean_n; }
		public static MetaGuidType Guid(bool notNull) { return notNull ? _guid : _guid_n; }
		public static MetaDecimalType Decimal(int precision, int scale, bool notNull) { return new MetaDecimalType { Precision = precision, Scale = scale, Name = "Decimal", NotNullable = notNull }; }
		public static MetaFileType FileID(bool notNull) { return notNull ? _fileIDKey : _fileIDKey_n; }
		public static MetaFileType FileGUID(bool notNull) { return notNull ? _fileGUIDKey : _fileGUIDKey_n; }		
	}
}