using Nephrite.Meta.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Meta
{

	public abstract partial class MetaClassifier : MetaElement
	{

		public virtual string GetDBType(IDBScript script)
		{
			return "";
		}
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
}