using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Meta.Database
{
	public class PostgreSQLMetadataReader : IDatabaseMetadataReader
    {
		public Schema ReadSchema(string name)
		{
			throw new NotImplementedException();
		}

		public List<ProcedureDetails> ReadProceduresDetails()
		{
			throw new NotImplementedException();
		}
	}
}
