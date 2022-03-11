using System;
using System.Collections.Concurrent;

namespace Tango.Cache
{
	public interface ICache
	{
		T Get<T>(string name);
		T GetOrAdd<T>(string name, Func<T> factory);
		void Insert<T>(string name, T item);

		void Reset(string name);
		void ResetAll();
	}

	public interface ICacheable
	{
		void ResetCache();
	}

	public class DefaultCache : ICache
	{
		ConcurrentDictionary<string, object> items = new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		static object _lock = new object();

		public void Insert<T>(string name, T item)
		{
			items[name] = item;
		}

		public T Get<T>(string name)
		{
			object res;
			if (items.TryGetValue(name, out res))
				return (T)res;
			else
				return default(T);
		}

		public void Reset(string name)
		{
			object obj;
			items.TryRemove(name, out obj);
		}

		public void ResetAll()
		{
			items.Clear();
		}

		public T GetOrAdd<T>(string name, Func<T> factory)
		{
			return (T)items.GetOrAdd(name, s => factory());
		}
	}
}
