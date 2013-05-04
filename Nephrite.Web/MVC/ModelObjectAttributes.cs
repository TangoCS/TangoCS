using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web
{
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class BusinessPackageAttribute : Attribute
	{
		readonly string _name;
		

		public BusinessPackageAttribute(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}
}
