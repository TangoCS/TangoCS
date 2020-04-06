using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Tango
{
	public static class Json
	{
		public static JsonSerializerSettings StdSettings
		{
			get
			{
				return new JsonSerializerSettings {
					ContractResolver = new CamelCaseExceptDictionaryResolver(),
					Converters = new List<JsonConverter> { new KeyValueListConverter(), new StringEnumConverter(true) }
				};
			}
		}
	}

	public class CamelCaseExceptDictionaryResolver : CamelCasePropertyNamesContractResolver
	{
		#region Overrides of DefaultContractResolver

		protected override string ResolveDictionaryKey(string dictionaryKey)
		{
			return dictionaryKey;
		}

		#endregion
	}

	public class KeyValue<TKey, TValue>
	{
		public TKey Key { get; set; }
		public TValue Value { get; set; }

		public KeyValue() { }
		public KeyValue(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}

	public class KeyValueListConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			List<KeyValue<string, object>> list = value as List<KeyValue<string, object>>;
			writer.WriteStartObject();
			foreach (var item in list)
			{
				writer.WritePropertyName(item.Key);
				serializer.Serialize(writer, item.Value);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(List<KeyValue<string, object>>);
		}
	}

	public static class KeyValueListExtensions
	{
		public static void Add<T1, T2>(this List<KeyValue<T1, T2>> l, T1 key, T2 value)
		{
			l.Add(new KeyValue<T1, T2>(key, value));
		}

		public static void Insert<T1, T2>(this List<KeyValue<T1, T2>> l, int index, T1 key, T2 value)
		{
			l.Insert(index, new KeyValue<T1, T2>(key, value));
		}
	}
}
