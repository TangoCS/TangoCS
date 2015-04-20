using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite
{
	public static class DI
	{
		public static IServiceProvider ApplicationServices { get; set; }
		public static IServiceProvider RequestServices { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{
	} 
}
