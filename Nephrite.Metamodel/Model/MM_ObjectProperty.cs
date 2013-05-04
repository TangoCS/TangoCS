using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Smo;

namespace Nephrite.Metamodel.Model
{
	public partial class MM_ObjectProperty
	{
		public string ElementName
		{
			get
			{
				string eName = SysName;
				if (UpperBound > 1)
				{
					eName = SysName.EndsWith("y") ? SysName.Substring(0, SysName.Length - 2) + "ies" : SysName + "s";
				}
				return eName;
			}
		}

        public DataType DataType
        {
            get
            {
                switch (TypeCode)
                {
                    case ObjectPropertyType.Boolean:
                        return DataType.Bit;
                    case ObjectPropertyType.Date:
                        return DataType.DateTime;
                    case ObjectPropertyType.DateTime:
                        return DataType.DateTime;
                    case ObjectPropertyType.ZoneDateTime:
                        return DataType.DateTime;
                    case ObjectPropertyType.Number:
                        return DataType.Int;
					case ObjectPropertyType.BigNumber:
						return DataType.BigInt;
					case ObjectPropertyType.String:
                        if (Length.HasValue)
                            return DataType.NVarChar(Length.Value);
                        else
                            return DataType.NVarCharMax;
                    case ObjectPropertyType.Guid:
                        return DataType.UniqueIdentifier;
                    case ObjectPropertyType.Code:
                        return DataType.NVarChar(1);
                    case ObjectPropertyType.Object:
						return RefObjectTypeID.HasValue ? RefObjectType.PrimaryKey.First().DataType : DataType.Int;
                    case ObjectPropertyType.File:
                        return DataType.Int;
					case ObjectPropertyType.FileEx:
						return DataType.UniqueIdentifier;
					case ObjectPropertyType.Data:
                        return DataType.VarBinaryMax;
                    case ObjectPropertyType.Decimal:
                        if (Precision.HasValue && Scale.HasValue)
                            return DataType.Decimal(Scale.Value, Precision.Value);
                        else
                            return DataType.Decimal(5, 18);
                    default:
                        return DataType.Int;
                }
            }
        }

        public string ClrType
        {
            get
            {
                switch (TypeCode)
                {
                    case ObjectPropertyType.Boolean:
                        return "bool";
                    case ObjectPropertyType.Date:
                        return "DateTime";
                    case ObjectPropertyType.DateTime:
                        return "DateTime";
                    case ObjectPropertyType.ZoneDateTime:
                        return "DateTime";
                    case ObjectPropertyType.Number:
                        return "int";
					case ObjectPropertyType.BigNumber:
						return "long";
					case ObjectPropertyType.String:
                        return "string";
                    case ObjectPropertyType.Guid:
                        return "Guid";
                    case ObjectPropertyType.Code:
                        return "string";
                    case ObjectPropertyType.Object:
                        return (RefObjectType == null ? "int" : RefObjectType.PrimaryKey.First().ClrType) + (LowerBound == 0 ? "?" : "");
                    case ObjectPropertyType.File:
                        return LowerBound == 0 ? "int?" : "int";
					case ObjectPropertyType.FileEx:
						return "Guid";
					case ObjectPropertyType.Data:
						return "global::System.Data.Linq.Binary";
                    case ObjectPropertyType.Decimal:
                        return "decimal";
                    default:
                        return "int";
                }
            }
        }

        public string Bounds
        {
            get
            {
                return (LowerBound < 0 ? "*" : LowerBound.ToString()) + ".." + (UpperBound < 0 ? "*" : UpperBound.ToString());
            }
        }

        public string FullSysName
        {
            get { return SysName + " (" + Title + ")"; }
        }

        /// <summary>
        /// Имя столбца в базе данных
        /// </summary>
        public string ColumnName
        {
            get
            {
				if (TypeCode == ObjectPropertyType.Object)
				{
					if (!RefObjectTypeID.HasValue)
						return SysName + "ID";
					if (RefObjectType.PrimaryKey.First().DataType.SqlDataType == SqlDataType.Int)
						return SysName + "ID";
					if (RefObjectType.PrimaryKey.First().DataType.SqlDataType == SqlDataType.UniqueIdentifier)
						return SysName + "GUID";
				}
                if (TypeCode == ObjectPropertyType.File)
                {
                    if (UpperBound == 1)
                        return SysName + "FileID";
                    else
                        return SysName + "FileListID";
                }
				if (TypeCode == ObjectPropertyType.FileEx)
				{
					return SysName + "GUID";
				}
				/*if (TypeCode == ObjectPropertyType.Image && UpperBound == 1)
				{
					return SysName + "ImageID";
				}*/
                if (TypeCode == ObjectPropertyType.String || UpperBound == 1)
                    return SysName;
                return SysName;
            }
        }

        /// <summary>
        /// Имя таблицы-ассоциатора
        /// </summary>
        public string AssoTableName
        {
            get { return UpperBound == -1 && TypeCode == ObjectPropertyType.Object ? MM_ObjectType.SysName + SysName : String.Empty; }
        }
    }
}
