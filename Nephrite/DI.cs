using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite
{
	public static class DI
	{
		public static IServiceProvider ApplicationServices { get; set; }
		public static IServiceProvider RequestServices { get; set; }
		public static T GetService<T>()
		{
			return RequestServices.GetService<T>();
        }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{
	} 
}
