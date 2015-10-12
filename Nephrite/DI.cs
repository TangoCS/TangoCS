using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		public static T InjectProperties<T>(this T obj)
		{
			var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute)));
			foreach (var prop in props)
				prop.SetValue(obj, DI.RequestServices.GetService(prop.PropertyType));
			return obj;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{
	} 
}
