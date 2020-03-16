using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tango.UI
{
	public class DefaultTypeActivatorCache : ITypeActivatorCache, ITypeObserver
	{
		static readonly Dictionary<string, (Type Type, IActionInvoker Invoker)> _typeActivatorCache
			= new Dictionary<string, (Type Type, IActionInvoker Invoker)>(StringComparer.OrdinalIgnoreCase);

		protected List<InvokeableTypeInfo> typeInfos = new List<InvokeableTypeInfo>();

		public DefaultTypeActivatorCache()
		{
			typeInfos.Add(new InvokeableTypeInfo {
				Filter = t => t.Name.EndsWith("Controller"),
				Keys = t => {
					var s = t.Name.Replace("Controller", "");
					var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
						.Where(m => m.ReturnType == typeof(ActionResult) && !m.IsSpecialName && !Attribute.IsDefined(m, typeof(NonActionAttribute)))
						.Select(m => m).ToList();
					var res = new List<string>();
					foreach (var m in methods)
					{
						var attrs = m.GetCustomAttributes<OnActionAttribute>();
						if (attrs.Count() == 0)
							res.Add(s + "." + m.Name);
						else
							res.AddRange(attrs.Select(a => a.Service + "." + a.Action).ToList());
					}
					return res;
				},
				Invoker = new ControllerActionInvoker()
			});
			typeInfos.Add(new InvokeableTypeInfo {
				Filter = t => t.IsSubclassOf(typeof(ViewElement)) && !t.IsAbstract,
				Keys = t => {
					var parts = t.Name.Split('_');
					var defService = parts.Length > 0 ? parts[0] : "";
					var defAction = parts.Length > 1 ? parts[1] : "";
					var attrs = t.GetCustomAttributes<OnActionAttribute>();
					if (attrs == null) return null;
					return attrs.Select(a => (a.Service ?? defService) + "." + (a.Action ?? defAction)).ToList();
				},
				Invoker = new CsFormInvoker()
			});
		}

		public void LookOver(Type t)
		{
			foreach (var info in typeInfos)
			{
				if (info.Filter(t))
				{
					var keys = info.Keys(t);
					if (keys == null || keys.Count == 0) continue;
					foreach (var key in keys)
					{
						_typeActivatorCache[key] = (t, info.Invoker);
					}
					break;
				}
			}
		}

		public (Type Type, IActionInvoker Invoker)? Get(string key)
		{
			if (_typeActivatorCache.TryGetValue(key.ToLower(), out (Type Type, IActionInvoker Invoker) ret))
				return ret;
			else
				return null;
		}

        public Dictionary<string, (Type Type, IActionInvoker Invoker)> GetAll()
        {
            return _typeActivatorCache;
        }
    }

	public class InvokeableTypeInfo
	{
		public Func<Type, bool> Filter { get; set; }
		public Func<Type, List<string>> Keys { get; set; }
		public IActionInvoker Invoker { get; set; }
	}
}
