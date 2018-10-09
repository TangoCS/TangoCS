using System;
using System.IO;
using System.Linq;
using System.Text;

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
			StringBuilder sb = new StringBuilder();
			string line;

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				if (!name.ToLower().EndsWith(".sql"))
					return reader.ReadToEnd();

				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("--#include"))
					{
						sb.AppendLine(GetString(t, line.Substring(10).Trim()));
						continue;
					}
					sb.AppendLine(line);
				}
			}

			return sb.ToString();
		}
	}
}
