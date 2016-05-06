using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nephrite
{
	public abstract class AbstractQueryString
	{
		public abstract string Controller { get; }
		public abstract string Action { get; }
		public abstract string ReturnUrl { get; }

		public abstract int GetInt(string parametername, int defaultValue);
		public abstract int? GetInt(string parametername);
		public abstract Guid GetGuid(string parametername);
		public abstract string GetString(string parametername);

		public abstract AbstractQueryString RemoveParameter(params string[] parametername);
		public abstract AbstractQueryString SetParameter(string parm, string value);
		public abstract AbstractQueryString AddParameter(string param);
		public abstract AbstractQueryString AddParameter(string param, string value);
		public abstract AbstractQueryString AddParameter(string param, int value);
		public abstract AbstractQueryString AddParameter(string param, Guid value);

		public abstract string CreateReturnUrl();

		public static implicit operator string(AbstractQueryString m)
		{
			if (m == null) return null;
			return m.ToString();
		}
	}

	public class Url : AbstractQueryString
	{
		string _q = "";
		IDictionary<string, string[]> _parsedQuery;
		IDictionary<string, object> _routeValues;

		IDictionary<string, string[]> ParsedQuery
		{
			get
			{
				if (_parsedQuery == null)
				{
					int i = _q.IndexOf("?");
					if (i >= 0)
						_parsedQuery = ParseQuery(_q.Substring(i));
					else
						_parsedQuery = new Dictionary<string, string[]>();
				}
				return _parsedQuery;
			}
		}

		public Url(string pathAndQuery, IDictionary<string, object> routeValues = null)
		{
			_q = pathAndQuery;
			_routeValues = routeValues ?? new Dictionary<string, object>();
		}

		public static AbstractQueryString From(string query, IDictionary<string, object> routeValues = null)
		{
			return new Url(query, routeValues);
		}

		public override string Controller { get { return GetString(Constants.ServiceName); } }
		public override string Action { get { return GetString(Constants.ActionName); } }
		public override string ReturnUrl { get { return GetString(Constants.ReturnUrl); } }

		public override int GetInt(string parametername, int defaultValue)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToString(_routeValues[parametername]).ToInt32(defaultValue);
			else
				return ParsedQuery.Get(parametername).ToInt32(defaultValue);
		}

		public override int? GetInt(string parametername)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToInt32(_routeValues[parametername]);
			else
				return ParsedQuery.Get(parametername).ToInt32();
		}

		public override Guid GetGuid(string parametername)
		{
			if (_routeValues.ContainsKey(parametername))
			{
				object o = _routeValues[parametername];
				if (o == null)
					return Guid.Empty;
				else
					return o.ToString().ToGuid();
			}
			else
				return ParsedQuery.Get(parametername).ToGuid();
		}

		public override string GetString(string parametername)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToString(_routeValues[parametername]);
			else
				return ParsedQuery.Get(parametername) ?? "";
		}

		public override AbstractQueryString RemoveParameter(params string[] parametername)
		{
			var newQueryString = new Dictionary<string, string[]>(ParsedQuery);
			if (newQueryString.Count == 0) return this;

			foreach (string key in parametername)
				newQueryString.Remove(key);

			string path = _q.Substring(0, _q.IndexOf("?"));
			string pathAndQuery = newQueryString.Count > 0
				? String.Format("{0}?{1}", path, newQueryString.Select(o => o.Key + "=" + o.Value.Join(",")).Join("&"))
				: path;

			return new Url(pathAndQuery);
		}

		public override AbstractQueryString AddParameter(string param)
		{
			return new Url(_q + (_q.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param));
		}

		public override AbstractQueryString AddParameter(string param, string value)
		{
			return new Url(_q + (_q.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value));
		}
		public override AbstractQueryString AddParameter(string param, int value)
		{
			return new Url(_q + (_q.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value.ToString()));
		}
		public override AbstractQueryString AddParameter(string param, Guid value)
		{
			return new Url(_q + (_q.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value.ToString()));
		}

		public override AbstractQueryString SetParameter(string parm, string value)
		{
			var res = RemoveParameter(parm);
			if (!value.IsEmpty())
			{
				res = res.AddParameter(parm, value);
			}
			return res;
		}

		string _returnUrl = null;
		public override string CreateReturnUrl()
		{
			if (_returnUrl != null) return _returnUrl;

			AbstractQueryString url = this;
			var returnurl = WebUtility.UrlEncode(url);
			if (returnurl.Length > MaxReturnUrlLength)
			{
				Stack<string> urlStack = new Stack<string>();

				while (url.GetString(Constants.ReturnUrl) != "")
				{
					urlStack.Push(url.RemoveParameter(Constants.ReturnUrl));
					url = new Url(WebUtility.UrlDecode(url.GetString(Constants.ReturnUrl)));
				}
				url = new Url(urlStack.Pop());
				while (urlStack.Count > 0)
					url = new Url(urlStack.Pop()).AddParameter(Constants.ReturnUrl, WebUtility.UrlEncode(url));
				returnurl = WebUtility.UrlEncode(url);
			}
			_returnUrl = returnurl;
			return _returnUrl;
		}

		public static int MaxReturnUrlLength = 1800;

		public override string ToString()
		{
			return _q;
		}

		/// <summary>
		/// Parse a query string into its component key and value parts.
		/// </summary>
		/// <param name="text">The raw query string value, with or without the leading '?'.</param>
		/// <returns>A collection of parsed keys and values.</returns>
		static IDictionary<string, string[]> ParseQuery(string queryString)
		{
			if (!string.IsNullOrEmpty(queryString) && queryString[0] == '?')
			{
				queryString = queryString.Substring(1);
			}
			var accumulator = new KeyValueAccumulator<string, string>(StringComparer.OrdinalIgnoreCase);

			int textLength = queryString.Length;
			int equalIndex = queryString.IndexOf('=');
			if (equalIndex == -1)
			{
				equalIndex = textLength;
			}
			int scanIndex = 0;
			while (scanIndex < textLength)
			{
				int delimiterIndex = queryString.IndexOf('&', scanIndex);
				if (delimiterIndex == -1)
				{
					delimiterIndex = textLength;
				}
				if (equalIndex < delimiterIndex)
				{
					while (scanIndex != equalIndex && char.IsWhiteSpace(queryString[scanIndex]))
					{
						++scanIndex;
					}
					string name = queryString.Substring(scanIndex, equalIndex - scanIndex);
					string value = queryString.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
					accumulator.Append(
						Uri.UnescapeDataString(name.Replace('+', ' ')),
						Uri.UnescapeDataString(value.Replace('+', ' ')));
					equalIndex = queryString.IndexOf('=', delimiterIndex);
					if (equalIndex == -1)
					{
						equalIndex = textLength;
					}
				}
				scanIndex = delimiterIndex + 1;
			}

			return accumulator.GetResults();
		}
	}

	public class KeyValueAccumulator<TKey, TValue>
	{
		private Dictionary<TKey, List<TValue>> _accumulator;
		IEqualityComparer<TKey> _comparer;

		public KeyValueAccumulator(IEqualityComparer<TKey> comparer)
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
