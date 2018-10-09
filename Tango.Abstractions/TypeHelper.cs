using System;
using System.Linq;
using System.Reflection;

namespace Tango
{
	public static class TypeHelper
	{
		public static Type GetDeclaredType<TSelf>(TSelf self)
		{
			return self != null ? self.GetType() : typeof(TSelf);
		}

		public static void CopyProperties(object source, object destination)
		{
			// If any this null throw an exception
			if (source == null || destination == null)
				throw new Exception("Source or/and Destination Objects are null");
			// Getting the Types of the objects
			Type typeDest = destination.GetType();
			Type typeSrc = source.GetType();
			// Collect all the valid properties to map
			var results = from srcProp in typeSrc.GetProperties()
						  let targetProperty = typeDest.GetProperty(srcProp.Name)
						  where srcProp.CanRead
						  && targetProperty != null
						  && (targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate)
						  && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0
						  && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
						  select new { sourceProperty = srcProp, targetProperty = targetProperty };
			//map the properties
			foreach (var props in results)
			{
				props.targetProperty.SetValue(destination, props.sourceProperty.GetValue(source, null), null);
			}
		}
	}
}
