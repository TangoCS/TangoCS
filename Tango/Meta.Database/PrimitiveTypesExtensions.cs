using Tango.Meta.Database;

namespace Tango.Meta
{
	public abstract partial class MetaPrimitiveType
	{
		public virtual string GetDBType(IDBTypeMapper mapper)
		{
			return "";
		}
	}
	public partial class MetaDecimalType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetDecimalType(Precision, Scale);
		}
	}

	public partial class MetaBooleanType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetBooleanType();
		}

	}
    public partial class MetaXmlType
    {
        public override string GetDBType(IDBTypeMapper mapper)
        {
            return mapper.GetXmlType();
        }

    }
	public partial class MetaStringType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetStringType(Length);
		}
	}

	public partial class MetaDateTimeType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetDateTimeType();
		}
	}
	public partial class MetaZoneDateTimeType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetZoneDateTimeType();
		}
	}

	public partial class MetaDateType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetDateType();
		}
	}

	public partial class MetaIntType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetIntType();
		}
	}

	public partial class MetaLongType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetLongType();
		}
	}
	public partial class MetaGuidType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetGuidType();
		}
	}
	public partial class MetaByteArrayType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetByteArrayType(Length);
		}
	}
	public partial class MetaEnumType
	{
		public override string GetDBType(IDBTypeMapper mapper)
		{
			return mapper.GetStringType(1);
		}
	}
}