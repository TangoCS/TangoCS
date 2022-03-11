using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Meta.Database
{
	public class STable : MetaStereotype, IOnTableGenerateLogic
	{
		public void Generate(Table table)
		{
			if (!Name.IsEmpty())
			{
				table.Name = Name;
				table.AssignStereotype(this);
			}
		}
	}

	public class SCustomSql : MetaStereotype, IOnTableGenerateLogic
	{
		public void Generate(Table table)
		{
			if (!Name.IsEmpty())
				table.AssignStereotype(this);
		}
	}
}
