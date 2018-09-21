using System;
using System.IO;

namespace Tango
{
	public interface IHasEmbeddedResources { }

	public static class EmbeddedResourceManager
	{
		public static string GetString(this IHasEmbeddedResources obj, string name)
		{
			return GetString(obj.GetType(), name);
		}

		public static string GetString(Type t, string name)
		{
			var assembly = t.Assembly;
			var resourceName = assembly.GetName().Name + "." + name;

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
