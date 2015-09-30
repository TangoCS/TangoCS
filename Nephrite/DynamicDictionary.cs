using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite
{
	public class DynamicDictionary : DynamicObject, IDictionary<string, object>
	{
		protected IDictionary<string, object> internalDictionary;

		public DynamicDictionary(StringComparer comparer = null)
		{
			if (comparer != null)
				internalDictionary = new Dictionary<string, object>(comparer);
			else
				internalDictionary = new Dictionary<string, object>();
		}

		// Implementing this function improves the debugging experience as it provides the debugger with the list of all
		// the properties currently defined on the object
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return Keys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = this[binder.Name];   // Never throws; null if not present
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			this[binder.Name] = value;
			return true;
		}

		public object this[string key]
		{
			get
			{
				// Always return something without throwing. Return null if no value.
				object val = null;
				internalDictionary.TryGetValue(key, out val);
				return val;
			}
			set
			{
				internalDictionary[key] = value;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#region internalDictionary pass-through
		public void Add(string key, object value)
		{
			internalDictionary.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return internalDictionary.ContainsKey(key);
		}

		public ICollection<string> Keys
		{
			get { return internalDictionary.Keys; }
		}

		public bool Remove(string key)
		{
			return internalDictionary.Remove(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return internalDictionary.TryGetValue(key, out value);
		}

		public ICollection<object> Values
		{
			get { return internalDictionary.Values; }
		}

		public void Add(KeyValuePair<string, object> item)
		{
			internalDictionary.Add(item);
		}

		public void Clear()
		{
			internalDictionary.Clear();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			return internalDictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			internalDictionary.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return internalDictionary.Count; }
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			return internalDictionary.Remove(item);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return internalDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return internalDictionary.GetEnumerator();
		}
		#endregion

	}


}
