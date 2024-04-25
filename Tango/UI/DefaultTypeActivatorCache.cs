using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tango.UI
{
	public class DefaultTypeActivatorCache : ITypeActivatorCache, ITypeObserver
	{
		static readonly Dictionary<string, TypeActivatorInfo> _typeActivatorCache
			= new Dictionary<string, TypeActivatorInfo>(StringComparer.OrdinalIgnoreCase);

		protected List<InvokeableTypeInfo> typeInfos = new List<InvokeableTypeInfo>();

		public DefaultTypeActivatorCache()
		{
			typeInfos.Add(new InvokeableTypeInfo {
				Filter = t => t.Name.EndsWith("Controller"),
				Keys = (p, t) => {
					var s = t.Name.Replace("Controller", "");
					var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
						.Where(m => m.ReturnType == typeof(ActionResult) && !m.IsSpecialName && !Attribute.IsDefined(m, typeof(NonActionAttribute)))
						.Select(m => m).ToList();
					var res = new List<ActionInfo>();
					foreach (var m in methods)
					{
						var attrs = m.GetCustomAttributes<OnActionAttribute>();
						if (attrs.Count() == 0)
							res.Add(new ActionInfo { Service = s, Action = m.Name });
						else
							res.AddRange(attrs.Select(a => new ActionInfo { Service = a.Service, Action = a.Action }));
					}
					return res;
				},
				Invoker = new ControllerActionInvoker()
			});
			typeInfos.Add(new InvokeableTypeInfo {
				Filter = t => t.IsSubclassOf(typeof(ViewElement)) && !t.IsAbstract,
				Keys = (p, t) => {
					var parts = t.Name.Split('_');
					var defService = parts.Length > 0 ? parts[0] : "";
					var defAction = parts.Length > 1 ? parts[1] : "";
					var attrs = t.GetCustomAttributes<OnActionAttribute>();
					if (attrs == null) return null;

					var res = new List<ActionInfo>();
					foreach (var attr in attrs)
					{
						if (attr.Func > 0)
							res.AddRange(OnActionAttribute.Funcs[attr.Func](p));
						else
							res.Add(new ActionInfo { Service = attr.Service ?? defService, Action = attr.Action ?? defAction });
					}
					return res;
				},
				Invoker = new CsFormInvoker()
			});
		}

		public void LookOver(IServiceProvider provider, Type t)
		{
			foreach (var info in typeInfos)
			{
				if (info.Filter(t))
				{
					var keys = info.Keys(provider, t);
					if (keys == null || keys.Count == 0) continue;
					foreach (var key in keys)
					{
						var k = key.Service + "." + key.Action;
						_typeActivatorCache[k] = new TypeActivatorInfo { Type = t, Invoker = info.Invoker, Args = key.Args };
					}
					break;
				}
			}
		}

		public TypeActivatorInfo Get(string key)
		{
			if (_typeActivatorCache.TryGetValue(key.ToLower(), out TypeActivatorInfo ret))
				return ret;
			else
				return null;
		}

        public Dictionary<string, TypeActivatorInfo> GetAll()
        {
            return _typeActivatorCache;
        }
    }

	public class InvokeableTypeInfo
	{
		public Func<Type, bool> Filter { get; set; }
		public Func<IServiceProvider, Type, List<ActionInfo>> Keys { get; set; }
		public IActionInvoker Invoker { get; set; }
	}

	public class ActionInfo
	{
		public string Service { get; set; }
		public string Action { get; set; }
		public Dictionary<string, string> Args { get; set; }
	}
}
