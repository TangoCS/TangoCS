using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Nephrite.Templating
{
	public interface ITypeActivatorCache
	{
		void Init(List<Assembly> assemblies, List<InvokeableTypeInfo> invokeableTypeInfos);
		Tuple<Type, IActionInvoker> Get(string key);
    }

	public class DefaultTypeActivatorCache : ITypeActivatorCache
	{
		static readonly Dictionary<string, Tuple<Type, IActionInvoker>> _typeActivatorCache 
			= new Dictionary<string, Tuple<Type, IActionInvoker>>(StringComparer.OrdinalIgnoreCase);

		public void Init(List<Assembly> assemblies, List<InvokeableTypeInfo> invokeableTypeInfos)
		{
			foreach (var assembly in assemblies)
			{
				foreach (var t in assembly.GetTypes())
				{
					if (t.IsAbstract || t.IsInterface) continue;
					foreach (var info in invokeableTypeInfos)
					{
						if (info.Filter(t))
						{
							var keys = info.Keys(t);
							if (keys == null) continue;
							foreach (var key in info.Keys(t))
							{
								if (!_typeActivatorCache.ContainsKey(key))
									_typeActivatorCache.Add(key, new Tuple<Type, IActionInvoker>(t, info.Invoker));
							}
							break;
						}
					}
				}
            }
		}

		public Tuple<Type, IActionInvoker> Get(string key)
		{
			Tuple<Type, IActionInvoker> ret = null;
			_typeActivatorCache.TryGetValue(key.ToLower(), out ret);
			return ret;
        }
	}

	public class InvokeableTypeInfo
	{
		public Func<Type, bool> Filter { get; set; }
		public Func<Type, List<string>> Keys { get; set; }
		public IActionInvoker Invoker { get; set; }
	}
}
