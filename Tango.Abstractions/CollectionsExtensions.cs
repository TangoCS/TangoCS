using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tango
{
	public static class CollectionsExtensions
	{
		public static string Join(this string[] str, string separator, bool excludeEmpties = true)
		{
			return String.Join(separator, str.Where(s => !excludeEmpties || !String.IsNullOrEmpty(s)).ToArray());
		}

		public static string Join(this IEnumerable<string> str, string separator, bool excludeEmpties = true)
		{
			return String.Join(separator, str.Where(s => !excludeEmpties || !String.IsNullOrEmpty(s)).ToArray());
		}

		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			dict.TryGetValue(key, out TValue res);
			return res;
		}

		public static string Get(this IDictionary<string, string[]> dict, string key)
		{
			string res = "";
			if (dict.ContainsKey(key)) res = dict[key].Join(",");
			return res;
		}

		public static void AddSorted<T>(this List<T> list, T value)
		{
			int x = list.BinarySearch(value);
			list.Insert((x >= 0) ? x : ~x, value);
		}

		public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (!dict.ContainsKey(key))
				dict.Add(key, value);
		}

		public static IEnumerable<(TSource prev, TSource cur, bool isLast)> PairwiseWithPrev<TSource>(this IEnumerable<TSource> source)
		{
			var queue = new Queue<TSource>(source);
			if (queue.Count > 0)
				yield return (default(TSource), queue.Peek(), queue.Count == 1);
			while (queue.Count > 2)
				yield return (queue.Dequeue(), queue.Peek(), false);
			if (queue.Count == 2)
				yield return (queue.Dequeue(), queue.Peek(), true);
		}

		public static IEnumerable<(TSource cur, TSource next)> PairwiseWithNext<TSource>(this IEnumerable<TSource> source)
		{
			var queue = new Queue<TSource>(source);
			while (queue.Count > 1)
				yield return (queue.Dequeue(), queue.Peek());
			if (queue.Count == 1)
				yield return (queue.Dequeue(), default(TSource));
		}
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }      
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize = 2000)
        {
			int itemsReturned = 0;
			var list = source.ToList(); // Prevent multiple execution of IEnumerable.
			int count = list.Count;
			while (itemsReturned < count)
			{
				int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
				yield return list.GetRange(itemsReturned, currentChunkSize);
				itemsReturned += currentChunkSize;
			}
        }

		public static IEnumerable<bool> AsEnumerableBool(this BitArray ba)
		{
			for (int i = 0; i < ba.Length; i++)
				yield return ba[i];
		}

		public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
		{
			for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
			{
				yield return array[row, i];
			}
		}

		public static IEnumerable<T> SliceColumn<T>(this T[,] array, int column)
		{
			for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
			{
				yield return array[i, column];
			}
		}
	}
}
