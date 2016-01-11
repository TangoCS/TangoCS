using System;

namespace Nephrite.Templating
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
