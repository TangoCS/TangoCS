using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class HasStandardMvcActionAttribute : Attribute
	{
		public string Name { get; set; }
		public Type StdControllerType { get; set; }
		public string StdControllerAction { get; set; }
		public Type ViewEngineType { get; set; }

		public HasStandardMvcActionAttribute(string name, Type stdControllerType)
		{
			Name = name;
			StdControllerType = stdControllerType;
		}

		public HasStandardMvcActionAttribute(string name, Type stdControllerType, Type engine)
		{
			Name = name;
			StdControllerType = stdControllerType;
			ViewEngineType = engine;
		}

		public HasStandardMvcActionAttribute(string name, Type stdControllerType, string stdControllerAction, Type engine)
		{
			Name = name;
			StdControllerType = stdControllerType;
			StdControllerAction = stdControllerAction;
			ViewEngineType = engine;
        }
	}

	
}
