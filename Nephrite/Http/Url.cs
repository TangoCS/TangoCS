using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Nephrite.Http
{
	public class Url
	{
		string _q = "";
		NameValueCollection _c = null;
		IDictionary<string, object> _routeValues = null;

		public Url(string pathAndQuery, NameValueCollection query, IDictionary<string, object> routeValues)
		{
			_q = pathAndQuery;
			_c = query;
			_routeValues = routeValues;
		}
		public Url(string pathAndQuery)
		{
			_q = pathAndQuery;
			_c = QueryHelpers.ParseQuery(_q);
			_routeValues = null;
		}

		public string Mode { get { return GetString("mode"); } }
		public string Action { get { return GetString("action"); } }

		public int GetInt(string parametername, int defaultValue)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToString(_routeValues[parametername]).ToInt32(defaultValue);
			else
				return _c.Get(parametername).ToInt32(defaultValue);
		}

		public int? GetInt(string parametername)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToInt32(_routeValues[parametername]);
			else
				return _c.Get(parametername).ToInt32();
		}

		public Guid GetGuid(string parametername)
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
				return _c.Get(parametername).ToGuid();
		}

		public string GetString(string parametername)
		{
			if (_routeValues.ContainsKey(parametername))
				return Convert.ToString(_routeValues[parametername]);
			else
				return _c.Get(parametername);
		}

		public Url RemoveParameter(params string[] parametername)
		{
			if (_q.IndexOf('?') > 0)
			{
				foreach (string s in parametername)
				{
					_q = Regex.Replace(_q, "[?]" + s + "=(?<1>[^&]*)", "?", RegexOptions.IgnoreCase);
					_q = Regex.Replace(_q, "[&]" + s + "=(?<1>[^&]*)", "", RegexOptions.IgnoreCase);
					_c.Remove(s);
				}
			}

			if (String.IsNullOrEmpty(_q)) _q = "/";
			return this;
		}

		public Url SetParameter(string parm, string value)
		{
			RemoveParameter(parm);
			if (!value.IsEmpty())
			{
				_q = _q.AddQueryParameter(parm, value);
				_c.Add(parm, value);
			}
			return this;
		}

		public string CreateReturnUrl()
		{
			var url = _q;
			var returnurl = WebUtility.UrlEncode(url);
			if (returnurl.Length > MaxReturnUrlLength)
			{
				Stack<string> urlStack = new Stack<string>();

				while (url.GetQueryParameter("returnurl") != "")
				{
					urlStack.Push(url.RemoveQueryParameter("returnurl"));
					
					url = WebUtility.UrlDecode(url.GetQueryParameter("returnurl"));
				}
				url = urlStack.Pop();
				while (urlStack.Count > 0)
					url = urlStack.Pop().AddQueryParameter("returnurl", WebUtility.UrlEncode(url));
				returnurl = WebUtility.UrlEncode(url);
			}
			return returnurl;
		}

		public Url ReturnUrl
		{
			get
			{
				string r = _c["returnurl"];
				if (r.IsEmpty())
					return null;

				//Uri u = new Uri(HttpContext.Current.Request.Url, r);
				return new Url(r);

			}
			set
			{
				SetParameter("returnurl", WebUtility.UrlEncode(value._q));
			}
		}

		public static int MaxReturnUrlLength = 1800;

		public override string ToString()
		{
			return _q;
		}

		public static implicit operator string(Url m)
		{
			if (m == null)
				return null;
			return m.ToString();
		}
	}

	public static class UrlExtensions
	{
		public static string GetQueryParameter(this string querystring, string parameterName)
		{
			if (querystring.IndexOf('?') < 0)
				return String.Empty;

			querystring = querystring.Substring(querystring.IndexOf('?'));

			Match m = Regex.Match(querystring, "[?&]" + parameterName + "=(?<1>[^&]*)", RegexOptions.IgnoreCase);
			if (querystring[0] != '?')
				return "?" + querystring.Substring(1);
			return m.Groups[1].Value;
		}

		public static string RemoveQueryParameter(this string str, params string[] parametername)
		{
			string query = str;
			if (query.IndexOf('?') < 0)
				return query;
			string path = query.Substring(0, query.IndexOf('?'));
			query = query.Substring(query.IndexOf('?'));

			for (int i = 0; i < parametername.Length; i++)
				query = Regex.Replace(query, "[?&]" + parametername[i] + "=(?<1>[^&]*)", "", RegexOptions.IgnoreCase);
			if (query.Length == 0)
				return path;
			if (query[0] != '?')
				return path + "?" + query.Substring(1);
			return path + query;
		}

		public static string AddQueryParameter(this string str, string param)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param);
		}

		public static string AddQueryParameter(this string str, string param, string value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value);
		}
		public static string AddQueryParameter(this string str, string param, int value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value.ToString());
		}
		public static string AddQueryParameter(this string str, string param, Guid value)
		{
			return str + (str.IndexOf('?') >= 0 ? "&" : "?") + Uri.EscapeDataString(param) + "=" + Uri.EscapeDataString(value.ToString());
		}

		public static string SetQueryParameter(this string str, string parm, string value)
		{
			str = str.RemoveQueryParameter(parm);
			if (!value.IsEmpty())
			{
				str = str.AddQueryParameter(parm, value);
			}
			return str;
		}
	}
}
