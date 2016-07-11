using System;

namespace Tango.UI
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HttpPostAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class HttpGetAttribute : Attribute
	{
	}
}
