﻿using Nephrite.Meta.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Meta
{
	public partial interface IMetaClassifier
	{
		string GetDBType(IDBScript script);
	}

	public abstract partial class MetaClassifier : MetaElement, IMetaClassifier
	{

		public virtual string GetDBType(IDBScript script)
		{
			return "";
		}
		//public bool NotNullable { get; set; }
	}
	public partial class MetaDecimalType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetDecimalType(Precision, Scale);
		}
	}

	public partial class MetaBooleanType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetBooleanType();
		}

	}
    public partial class MetaXmlType
    {
        public override string GetDBType(IDBScript script)
        {
            return script.GetXmlType();
        }

    }
	public partial class MetaStringType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetStringType(Length);
		}
	}

	public partial class MetaDateTimeType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetDateTimeType();
		}
	}
	public partial class MetaZoneDateTimeType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetZoneDateTimeType();
		}
	}

	public partial class MetaDateType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetDateType();
		}
	}

	public partial class MetaIntType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetIntType();
		}
	}

	public partial class MetaLongType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetLongType();
		}
	}
	public partial class MetaGuidType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetGuidType();
		}
	}
	public partial class MetaByteArrayType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetByteArrayType(Length);
		}
	}
	public partial class MetaEnumType
	{
		public override string GetDBType(IDBScript script)
		{
			return script.GetStringType(1);
		}
	}
}