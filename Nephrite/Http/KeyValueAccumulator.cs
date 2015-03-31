using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public class KeyValueAccumulator<TKey, TValue>
	{
		private Dictionary<TKey, List<TValue>> _accumulator;
		IEqualityComparer<TKey> _comparer;

		public KeyValueAccumulator([NotNull] IEqualityComparer<TKey> comparer)
		{
			_comparer = comparer;
			_accumulator = new Dictionary<TKey, List<TValue>>(comparer);
		}

		public void Append(TKey key, TValue value)
		{
			List<TValue> values;
			if (_accumulator.TryGetValue(key, out values))
			{
				values.Add(value);
			}
			else
			{
				_accumulator[key] = new List<TValue>(1) { value };
			}
		}

		public IDictionary<TKey, TValue[]> GetResults()
		{
			var results = new Dictionary<TKey, TValue[]>(_comparer);
			foreach (var kv in _accumulator)
			{
				results.Add(kv.Key, kv.Value.ToArray());
			}
			return results;
		}
	}
}
