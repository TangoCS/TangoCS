using System;
using System.Linq;
using System.Reflection;

namespace Tango
{
	public static class DI
	{
		//public static IServiceProvider RequestServices { get; set; }
		//public static T GetService<T>()
		//{
		//	return (T)RequestServices.GetService(typeof(T));
  //      }

		public static T InjectProperties<T>(this T obj, IServiceProvider provider)
			where T : IWithPropertyInjection
		{
			var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(InjectAttribute)));
			foreach (var prop in props)
				prop.SetValue(obj, provider.GetService(prop.PropertyType));
			return obj;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{
	}

	public interface IWithPropertyInjection
	{
	} 
}
