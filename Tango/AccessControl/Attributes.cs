using System;

namespace Tango.AccessControl
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class SecurableObjectAttribute : Attribute
	{
		public string Name { get; }

		public SecurableObjectAttribute(Type service, string action)
		{
			Name = $"{service.Name}.{action}";
		}
		public SecurableObjectAttribute(string service, string action)
		{
			Name = $"{service}.{action}";
		}
	}
}
