using Microsoft.AspNetCore.Builder;
using System.Linq;
using System.Reflection;

namespace Tango.AspNetCore
{
	public static class ConfigHelpers
    {
		public static void ObserveTypes(this IApplicationBuilder app, string[] modules, ITypeObserver[] observers)
		{
			var assemblies = modules.Select(o => Assembly.Load(new AssemblyName(o))).ToArray();
			
			foreach (var assembly in assemblies)
			{
				foreach (var t in assembly.ExportedTypes)
				{
					if (t.IsAbstract || t.IsInterface) continue;
					foreach (var o in observers)
						o.LookOver(app.ApplicationServices, t);
				}
			}
		}
	}
}
