﻿using System;
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
			TValue res = default(TValue);
			dict.TryGetValue(key, out res);
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
	}
}