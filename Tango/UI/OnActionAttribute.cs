using System;
using System.Collections.Generic;

namespace Tango.UI
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class OnActionAttribute : Attribute
	{
		public string Service { get; }
		public string Action { get; }

		public int Func { get; set; }

		public static Dictionary<int, Func<IServiceProvider, IEnumerable<ActionInfo>>> Funcs { get; set; } =
			new Dictionary<int, Func<IServiceProvider, IEnumerable<ActionInfo>>>();

		public OnActionAttribute() { }

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
