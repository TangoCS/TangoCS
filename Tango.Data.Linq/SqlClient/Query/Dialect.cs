using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Linq.SqlClient
{
    public abstract class Dialect
    {
		public abstract string CharToInt { get; }

		public abstract string DatePartName(string partName);

		public abstract string Identity(MetaDataMember id, bool canUseScopeIdentity);
	}

	public class DialectSqlServer : Dialect
	{
		public override string CharToInt => "UNICODE";

		public override string DatePartName(string partName) => partName;

		public override string Identity(MetaDataMember id, bool canUseScopeIdentity)
		{
			return canUseScopeIdentity ? "SCOPE_IDENTITY()" : "@@IDENTITY";
		}
	}

	public class DialectPg : Dialect
	{
		public override string CharToInt => "ASCII";
		//public override SqlExpression DatePart => "DATE_PART";

		public override string DatePartName(string partName) => "'" + partName + "'";

		public override string Identity(MetaDataMember id, bool canUseScopeIdentity)
		{
			return $"currval('{id.DeclaringType.Name.ToLower()}_{id.Name.ToLower()}_seq')";
		}
	}
}
