using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNet.WebUtilities;

namespace Nephrite.MVC
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
						_parsedQuery = QueryHelpers.ParseQuery(_q.Substring(i));
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

		public override string Controller { get { return GetString(MvcOptions.ControllerName); } }
		public override string Action { get { return GetString(MvcOptions.ActionName); } }
		public override string ReturnUrl { get { return GetString(MvcOptions.ReturnUrl); } }

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

				while (url.GetString(MvcOptions.ReturnUrl) != "")
				{
					urlStack.Push(url.RemoveParameter(MvcOptions.ReturnUrl));
					url = new Url(WebUtility.UrlDecode(url.GetString(MvcOptions.ReturnUrl)));
				}
				url = new Url(urlStack.Pop());
				while (urlStack.Count > 0)
					url = new Url(urlStack.Pop()).AddParameter(MvcOptions.ReturnUrl, WebUtility.UrlEncode(url));
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
	}
}
