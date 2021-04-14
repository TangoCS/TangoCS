using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tango
{
	public interface ITypeObserver
	{
		void LookOver(Type t);
	}

	public class TypeCacheAttribute : Attribute
	{
		public string Key { get; }

		public TypeCacheAttribute(string key)
		{
			Key = key;
		}
	}

	public class TypeCache : ITypeObserver
	{
		static readonly Dictionary<string, List<Type>> _typeCache = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);

		public List<Type> Get(string key)
		{
			if (_typeCache.TryGetValue(key, out var list))
				return list;
			else
				return null;
		}

		void ITypeObserver.LookOver(Type t)
		{
			var attr = t.GetCustomAttribute<TypeCacheAttribute>();
			if (attr == null) return;

			if (_typeCache.TryGetValue(attr.Key, out var list))
				list.Add(t);
			else
				_typeCache.Add(attr.Key, new List<Type> { t });
		}
	}
}
