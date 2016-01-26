using System;

namespace Nephrite.UI
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class NonActionAttribute : Attribute
	{
	}
}
