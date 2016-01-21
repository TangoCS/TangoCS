using System;

namespace Nephrite.Templating
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class OnActionAttribute : Attribute
	{
		public string Service { get; }
		public string Action { get; }

		public OnActionAttribute(string service, string action)
		{
			Service = service;
			Action = action;
		}

		public OnActionAttribute(Type service, string action)
		{
			Service = service.Name;
			Action = action;
		}
	}
}
