using System;

namespace Nephrite.Templating
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class NonActionAttribute : Attribute
	{
	}
}
