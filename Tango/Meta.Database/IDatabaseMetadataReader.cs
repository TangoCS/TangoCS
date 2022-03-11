using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tango.Meta.Database
{
	public interface IDatabaseMetadataReader
	{
		Schema ReadSchema(string name);
		Schema ReadSchema(string name, XDocument doc);
		//List<ProcedureDetails> ReadProceduresDetails();
	}
}
