using System;

namespace Tango
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class AllowAnonymousAttribute : Attribute
	{
	}
}
