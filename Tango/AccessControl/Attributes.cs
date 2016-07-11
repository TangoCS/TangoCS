using System;

namespace Tango.AccessControl
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
