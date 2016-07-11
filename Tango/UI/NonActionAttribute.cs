using System;

namespace Tango.UI
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class NonActionAttribute : Attribute
	{
	}
}
