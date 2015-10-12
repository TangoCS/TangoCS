using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
							var key = t.Name.ToLower();
							if (!_typeActivatorCache.ContainsKey(key))
								_typeActivatorCache.Add(key, t);
							break;
						}
					}
				}
            }
		}

		public Type Get(string typeName)
		{
			Type ret = null;
			_typeActivatorCache.TryGetValue(typeName.ToLower(), out ret);
			return ret;
        }
	}
}
