using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.UI
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class UniqueIDAttribute : Attribute
	{
		public Guid Guid { get; }

		public UniqueIDAttribute(Guid guid)
		{
			this.Guid = guid;
		}
	}
}
