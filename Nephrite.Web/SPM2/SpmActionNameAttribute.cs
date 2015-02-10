using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.SPM
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class SpmActionNameAttribute : Attribute
	{
		readonly string _name;


		public SpmActionNameAttribute(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}
}