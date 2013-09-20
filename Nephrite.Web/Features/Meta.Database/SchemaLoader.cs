using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Meta.Database
{
	public interface IDatabaseMetadataReader
	{
		Schema ReadSchema(string name);
	}

	public class SqlServerMetadataReader : IDatabaseMetadataReader
	{
		public Schema ReadSchema(string name)
		{
			throw new NotImplementedException();
		}
	}

	public class DB2MetadataReader : IDatabaseMetadataReader
	{
		public Schema ReadSchema(string name)
		{
			throw new NotImplementedException();
		}
	}
}