using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Meta
{
	public class SMvcAction : MetaStereotype
	{
		public string ViewEngine { get; set; }
		public string StdControllerClass { get; set; }
		public string StdControllerAction { get; set; }
	}
}
