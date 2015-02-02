using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Meta.Database
{
	public interface IDatabaseMetadataReader
	{
		Schema ReadSchema(string name);
		List<ProcedureDetails> ReadProceduresDetails();
	}
}
