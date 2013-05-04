using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta
{
	public class MetaPrimitiveType : MetaClassifier
	{

	}

	public class MetaDecimalType : MetaClassifier
	{
		public int Precision { get; set; }
		public int Scale { get; set; }
	}

	public class MetaStringType : MetaClassifier
	{
		public int Length { get; set; }
	}



	public static class TypeFactory
	{
		static MetaPrimitiveType _date = new MetaPrimitiveType { Name = "Date", CLRType = "DateTime" };
		static MetaPrimitiveType _dateTime = new MetaPrimitiveType { Name = "DateTime", CLRType = "DateTime" };
		static MetaPrimitiveType _int = new MetaPrimitiveType { Name = "Int", CLRType = "int" };
		static MetaPrimitiveType _long = new MetaPrimitiveType { Name = "Long", CLRType = "long" };
		static MetaPrimitiveType _data = new MetaPrimitiveType { Name = "Data", CLRType = "byte[]" };
		static MetaPrimitiveType _boolean = new MetaPrimitiveType { Name = "Boolean", CLRType = "bool" };
		static MetaPrimitiveType _guid = new MetaPrimitiveType { Name = "Guid", CLRType = "Guid" };
		static MetaPrimitiveType _fileID = new MetaPrimitiveType { Name = "FileID", CLRType = "int" };
		static MetaPrimitiveType _fileGUID = new MetaPrimitiveType { Name = "FileGUID", CLRType = "Guid" };
		static MetaPrimitiveType _zoneDateTime = new MetaPrimitiveType { Name = "ZoneDateTime" };

		public static MetaStringType String(int length) { return new MetaStringType { Length = length, Name = "String" }; }
		public static MetaPrimitiveType Date { get { return _date; } }
		public static MetaPrimitiveType DateTime { get { return _dateTime; } }
		public static MetaPrimitiveType Int { get { return _int; } }
		public static MetaPrimitiveType Long { get { return _long; } }
		public static MetaPrimitiveType Data { get { return _data; } }
		public static MetaPrimitiveType Boolean { get { return _boolean; } }
		public static MetaPrimitiveType Guid { get { return _guid; } }
		public static MetaDecimalType Decimal(int precision, int scale) { return new MetaDecimalType { Precision = precision, Scale = scale, Name = "Decimal" }; }
		public static MetaPrimitiveType FileID { get { return _fileID; } }
		public static MetaPrimitiveType FileGUID { get { return _fileGUID; } }
		public static MetaStringType Char(int length) { return new MetaStringType { Length = length, Name = "Char" }; }
		public static MetaPrimitiveType ZoneDateTime { get { return _zoneDateTime; } }
	}
}