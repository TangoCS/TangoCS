using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.MVC
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
