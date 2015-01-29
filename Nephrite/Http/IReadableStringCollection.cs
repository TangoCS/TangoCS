using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Http
{
	public interface IReadableStringCollection : IEnumerable<KeyValuePair<string, string[]>>
	{
		/// <summary>
		/// Get the associated value from the collection.  Multiple values will be merged.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string this[string key] { get; }

		/// <summary>
		/// Gets the number of elements contained in the collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets a collection containing the keys.
		/// </summary>
		ICollection<string> Keys { get; }

		/// <summary>
		/// Determines whether the collection contains an element with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool ContainsKey(string key);

		/// <summary>
		/// Get the associated value from the collection.  Multiple values will be merged.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string Get(string key);

		/// <summary>
		/// Get the associated values from the collection in their original format.
		/// Returns null if the key is not present.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IList<string> GetValues(string key);
	}
}
