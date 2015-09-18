using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Nephrite.MVC
{
	public interface ITypeActivatorCache
	{
		void Init();
		Type Get(string typeName);
    }

	public class TypeActivatorCacheOptions
	{
		public string[] AssemblySearchPatterns { get; set; }
		public Func<Type, bool>[] TypeFilters { get; set; }
	}

	public class DefaultTypeActivatorCache : ITypeActivatorCache
	{
		static readonly Dictionary<string, Type> _typeActivatorCache = new Dictionary<string, Type>();
		TypeActivatorCacheOptions _options;

		public DefaultTypeActivatorCache(TypeActivatorCacheOptions options)
		{
			_options = options;
        }

		public void Init()
		{
			var files = new List<string>();
			foreach (var s in _options.AssemblySearchPatterns)
			{
				files.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin", s));
			}

			foreach (var f in files)
			{
				var assembly = Assembly.LoadFrom(f);
				foreach (var t in assembly.GetTypes())
				{
					if (t.IsAbstract || t.IsInterface) continue;
					foreach (var filter in _options.TypeFilters)
					{
						if (filter(t))
						{
							if (!_typeActivatorCache.ContainsKey(t.Name))
								_typeActivatorCache.Add(t.Name, t);
							break;
						}
					}
				}
            }
		}

		public Type Get(string typeName)
		{
			Type ret = null;
			_typeActivatorCache.TryGetValue(typeName, out ret);
			return ret;
        }
	}
}
