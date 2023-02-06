using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Tango
{
	public class DynamicDictionary : DynamicObject, IDictionary<string, object>, IReadOnlyDictionary<string, object>
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

		IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => internalDictionary.Keys;
		IEnumerable<object> IReadOnlyDictionary<string, object>.Values => internalDictionary.Values;

		public bool Remove(KeyValuePair<string, object> item)
		{
			return internalDictionary.Remove(item);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return internalDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return internalDictionary.GetEnumerator();
		}
		#endregion
	}

	public static class DynamicDictionaryExtensions
	{
		public static T Parse<T>(this IReadOnlyDictionary<string, object> dd, string name, T defaultValue = default(T))
		{
			if (!dd.TryGetValue(name, out var d)) return defaultValue;
			if (d == null) return defaultValue;
			string format = null;
			if (dd.TryGetValue("__format_" + name, out var f))
				format = f.ToString();
			return Parse(d, format, defaultValue);
		}

		public static DateTime? ParseDateTime(this IReadOnlyDictionary<string, object> dd, string name, string format)
		{
			if (!dd.TryGetValue(name, out var d)) return null;
			if (d == null) return null;
			var ds = d.ToString();
			if (ds.IsEmpty()) return null;

			if (DateTime.TryParseExact(ds, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
				return dt;
			else
				return null;
		}

		public static DateTime ParseDateTime(this IReadOnlyDictionary<string, object> dd, string name, string format, DateTime defaultValue)
		{
			if (!dd.TryGetValue(name, out var d)) return defaultValue;
			if (d == null) return defaultValue;
			var ds = d.ToString();
			if (ds.IsEmpty()) return defaultValue;

			if (DateTime.TryParseExact(ds, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
				return dt;
			else
				return defaultValue;
		}

		public static decimal ParseDecimal(this IReadOnlyDictionary<string, object> dd, string name, decimal defaultValue = 0)
		{
			if (!dd.TryGetValue(name, out var d)) return defaultValue;
			return d.ToString().ToDecimal(defaultValue);
		}

		public static T ConvertTo<T>(this string s, string format = null, T defaultValue = default)
		{
			return Parse(s, format, defaultValue);
		}

		static T Parse<T>(object d, string format, T defaultValue = default)
		{
			var t = typeof(T);
			if (t == d.GetType())
				return (T)d;
			else if (t == typeof(DateTime?))
			{
                var ds = d.ToString();
                if (ds.IsEmpty()) return defaultValue;
                return (T)(object)ds.ToDate(format, (DateTime?)(object)defaultValue);
            }
			else if(t == typeof(DateTime))
			{
                var ds = d.ToString();
                if (ds.IsEmpty()) return defaultValue;
                return (T)(object)ds.ToDate(format, (DateTime)(object)defaultValue);
            }
			else if (t == typeof(decimal?))
			{
				var ds = d.ToString();
				if (ds.IsEmpty()) return defaultValue;
				return (T)(object)ds.ToDecimal();
			}
			else if (t == typeof(decimal))
			{
				var ds = d.ToString();
				if (ds.IsEmpty()) return defaultValue;
				var dec = ds.ToDecimal();
				if (dec == null)
					return defaultValue;
				else
					return (T)(object)dec;

			}
			else if (t == typeof(int?)) {
				var ds = d.ToString();
				if (ds.IsEmpty()) return defaultValue;
				var dec = ds.ToInt32();
				if (dec == null)
					return defaultValue;
				else
					return (T)(object)dec;

			}
			else if (t.IsEnum)
			{
				var d2 = Convert.ChangeType(d, Enum.GetUnderlyingType(t));
				if (Enum.IsDefined(t, d2))
					return (T)d2;
			}
			#if NET
			else if (typeof(ITuple).IsAssignableFrom(t))
			{
				var ds = d.ToString();
				if (ds.StartsWith('(')) ds = ds.TrimStart('(');
				if (ds.EndsWith(')')) ds = ds.TrimEnd(')');

				var values = ds.Split(',').Select(x => x.Trim()).ToArray();
				var fields = t.GetFields();

				if (fields.Length == values.Length)
				{
					var parms = values.Select((x, i) => Convert.ChangeType(x, fields[i].FieldType)).ToArray();
					var obj = (T)Activator.CreateInstance(t, parms);
					return obj;
				}
				else
					return defaultValue;
			}
			#endif
			var typeConverter = TypeDescriptor.GetConverter(typeof(T));
			if (typeConverter != null && typeConverter.CanConvertFrom(d.GetType()) && typeConverter.IsValid(d))
			{
				var obj = typeConverter.ConvertFrom(d);
				return obj == null ? defaultValue : (T)obj;
			}
			return defaultValue;
		}

		public static List<T> ParseList<T>(this IReadOnlyDictionary<string, object> dd, string name)
		{
			if (!dd.TryGetValue(name, out var d)) return new List<T>();
			if (d == null) return new List<T>();
			string format = dd["__format_" + name + "[]"]?.ToString();
			if (!(d is IList))
			{
				if (d is string)
				{
					if (string.IsNullOrEmpty(d as string)) return new List<T>();
					return (d as string).Split(new char[] { ',' }).Select(o => Parse<T>(o, format)).ToList();
				}
				else
				{
					return new List<T> { Parse<T>(d, format) };
				}
			}
			else
			{
				return (d as List<object>).Select(o => Parse<T>(o, format)).ToList();
			}
		}
	}

	public class DynamicDictionaryConverter : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			// can write is set to false
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return ReadValue(reader);
		}

		private object ReadValue(JsonReader reader)
		{
			while (reader.TokenType == JsonToken.Comment)
			{
				if (!reader.Read())
					throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
			}

			switch (reader.TokenType)
			{
				case JsonToken.StartObject:
					return ReadObject(reader);
				case JsonToken.StartArray:
					return ReadList(reader);
				default:
					switch (reader.TokenType)
					{
						case JsonToken.Integer:
							return Convert.ToInt32(reader.Value);
						case JsonToken.Float:
						case JsonToken.String:
						case JsonToken.Boolean:
						case JsonToken.Undefined:
						case JsonToken.Null:
						case JsonToken.Date:
						case JsonToken.Bytes:
							return reader.Value;
					}

					throw new JsonSerializationException(String.Format(CultureInfo.InvariantCulture, "Unexpected token when converting ExpandoObject: {0}", reader.TokenType));
			}
		}

		private object ReadList(JsonReader reader)
		{
			IList<object> list = new List<object>();

			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonToken.Comment:
						break;
					default:
						object v = ReadValue(reader);

						list.Add(v);
						break;
					case JsonToken.EndArray:
						return list;
				}
			}

			throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
		}

		private object ReadObject(JsonReader reader)
		{
			IDictionary<string, object> expandoObject = new DynamicDictionary(StringComparer.OrdinalIgnoreCase);

			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						string propertyName = reader.Value.ToString();

						if (!reader.Read())
							throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");

						object v = ReadValue(reader);

						expandoObject[propertyName] = v;
						break;
					case JsonToken.Comment:
						break;
					case JsonToken.EndObject:
						return expandoObject;
				}
			}

			throw new JsonSerializationException("Unexpected end when reading ExpandoObject.");
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(DynamicDictionary));
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return false; }
		}
	}
}
