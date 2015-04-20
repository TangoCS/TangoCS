using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Framework.Internal
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class NotNullAttribute : Attribute
	{
	} 
}
