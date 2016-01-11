using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.AccessControl
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SecurableObjectAttribute : Attribute
	{
		readonly string _name;

		public SecurableObjectAttribute(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}
}
