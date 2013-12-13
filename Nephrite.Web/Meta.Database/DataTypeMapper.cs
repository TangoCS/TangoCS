using System;
using System.Xml.Linq;


namespace Nephrite.Meta.Database
{
    public class DataTypeMapper
    {
        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static  bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }
        
        //http://msdn.microsoft.com/en-us/library/cc716729.aspx
        public Type MapFromSqlServerDBType(string dataType, int? dataLength, int? dataPrecision, int? dataScale)
        {
            return MapFromDBType(dataType, dataLength, dataPrecision, dataScale);
        }



        private Type MapFromDBType(string dataType, int? dataLength, int? dataPrecision, int? dataScale)
        {
			dataType = dataType.Contains("(") ? dataType.Substring(0, dataType.IndexOf("(", System.StringComparison.Ordinal)) : dataType;
            
            switch (dataType.ToUpperInvariant())
            {
				case "XML": 
					return typeof(XDocument);
                case "DATE":
                case "DATETIME":
                case "DATETIME2":
                case "TIMESTAMP":
                case "TIMESTAMP WITH TIME ZONE":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                case "SMALLDATETIME":
                case "TIME":
                    return typeof(DateTime);

                case "NUMBER":
                case "LONG":
                case "BIGINT":
                    return typeof(long);

                case "SMALLINT":
                    return typeof(Int16);

                case "TINYINT":
                    return typeof(Byte);

                case "INT":
                case "INTERVAL YEAR TO MONTH":
                case "BINARY_INTEGER":
                case "INTEGER":
                    return typeof(int);

                case "BINARY_DOUBLE":
                case "NUMERIC":
                    return typeof(double);

                case "FLOAT":
                case "BINARY_FLOAT":
                    return typeof(float);

                case "BLOB":
                case "BFILE *":
                case "LONG RAW":
                case "BINARY":
                case "IMAGE":
                case "VARBINARY":
                    return typeof(byte[]);

                case "INTERVAL DAY TO SECOND":
                    return typeof(TimeSpan);

                case "BIT":
                case "BOOLEAN":
                    return typeof(Boolean);

                case "DECIMAL":
                case "MONEY":
                case "SMALLMONEY":
                    return typeof(decimal);

                case "REAL":
                    return typeof(Single);

                case "UNIQUEIDENTIFIER":
                    return typeof(Guid);

                default:
                    return dataType.Contains("int")
                               ? typeof(int)
                               : // CHAR, CLOB, NCLOB, NCHAR, XMLTYPE, VARCHAR2, NCHAR, NTEXT
                           typeof(string);
            }
        }

        private Type MapFromIngresDbType(string dataType, int? dataLength, int? dataPrecision, int? dataScale)
        {
            if (string.Equals(dataType, "INGRESDATE", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(System.DateTime);
            }

            if (string.Equals(dataType, "INTEGER", StringComparison.OrdinalIgnoreCase))
            {
                if (dataPrecision.HasValue)
                {
                    switch (dataPrecision.Value)
                    {
                        case 1:
                        case 2:
                            return typeof(System.Int16);
                        case 4:
                            return typeof(System.Int32);
                        case 8:
                            return typeof(System.Int64);
                    }
                }
            }

            if (string.Equals(dataType, "DECIMAL", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(dataType, "MONEY", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(System.Decimal);
            }

            if (string.Equals(dataType, "FLOAT", StringComparison.OrdinalIgnoreCase))
            {
                if (dataPrecision.HasValue)
                {
                    switch (dataPrecision.Value)
                    {
                        case 4:
                            return typeof(System.Single);
                        case 8:
                            return typeof(System.Double);
                    }
                }
            }

            if (string.Equals(dataType, "BYTE", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(byte[]);
            }

            return typeof(System.String);
        }

    }
}