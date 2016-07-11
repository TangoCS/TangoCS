using System;

namespace Tango.UI
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class HasStandardActionAttribute : Attribute
	{
		public string Name { get; set; }
		public Type StdControllerType { get; set; }
		public string StdControllerAction { get; set; }
		public Type ViewEngineType { get; set; }

		public HasStandardActionAttribute(string name, Type stdControllerType)
		{
			Name = name;
			StdControllerType = stdControllerType;
		}

		public HasStandardActionAttribute(string name, Type stdControllerType, Type engine)
		{
			Name = name;
			StdControllerType = stdControllerType;
			ViewEngineType = engine;
		}

		public HasStandardActionAttribute(string name, Type stdControllerType, string stdControllerAction, Type engine)
		{
			Name = name;
			StdControllerType = stdControllerType;
			StdControllerAction = stdControllerAction;
			ViewEngineType = engine;
        }
	}
}
