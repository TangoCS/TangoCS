using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.UI
{
	public class DefaultTypeActivatorCache : ITypeActivatorCache, ITypeObserver
	{
		static readonly Dictionary<string, Tuple<Type, IActionInvoker>> _typeActivatorCache
			= new Dictionary<string, Tuple<Type, IActionInvoker>>(StringComparer.OrdinalIgnoreCase);

		List<InvokeableTypeInfo> typeInfos = new List<InvokeableTypeInfo>();

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
						if (attrs == null)
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
					var attrs = t.GetCustomAttributes<OnActionAttribute>();
					if (attrs == null) return null;
					return attrs.Select(a => a.Service + "." + a.Action).ToList();
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
