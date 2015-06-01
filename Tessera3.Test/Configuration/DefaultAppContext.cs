using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Nephrite;
using Nephrite.Http;
using Microsoft.AspNet.Http.Internal;


namespace Solution.Configuration
{
	public class DefaultHttpContext : IHttpContext
	{
		IHttpRequest _request;

		public DefaultHttpContext()
		{
			_request = new DefaultAppRequest();
		}

		public IHttpRequest Request
		{
			get { return _request; }
		}

		public System.Collections.IDictionary Items
		{
			get
			{
				if (HttpContext.Current != null) return HttpContext.Current.Items;

				Hashtable ht = AppDomain.CurrentDomain.GetData("ContextItems") as Hashtable;
				if (ht == null)
				{
					ht = new Hashtable();
					AppDomain.CurrentDomain.SetData("ContextItems", ht);
				}
				return ht;
			}
		}

		public IPersistentSettings Settings
		{
			get { throw new NotImplementedException(); }
		}

		public System.Security.Principal.IPrincipal User
		{
			get { return HttpContext.Current.User; }
		}

		public void Dispose()
		{

		}


		public T GetFeature<T>()
		{
			throw new NotImplementedException();
		}

		public void SetFeature<T>(T instance)
		{
			throw new NotImplementedException();
		}


		public IHttpResponse Response
		{
			get { throw new NotImplementedException(); }
		}

		public IServiceProvider ApplicationServices
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IServiceProvider RequestServices
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}

	public class DefaultAppRequest : IHttpRequest
	{
		public NameValueCollection Query
		{
			get { return HttpContext.Current.Request.QueryString; }
		}

		Microsoft.AspNet.Http.IReadableStringCollection _cookies;
		public Microsoft.AspNet.Http.IReadableStringCollection Cookies
		{
			get 
			{ 
				if (_cookies == null)
				{
					StringCollection d = new StringCollection();
					HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;

					foreach (string c in cookies.AllKeys)
						d.Add(c, cookies[c].Value);

					_cookies = d;
				}
				return _cookies; 
			}
		}

		public Uri Url
		{
			get { return HttpContext.Current.Request.Url; }
		}

		public NameValueCollection Headers
		{
			get { return HttpContext.Current.Request.Headers; }
		}

		public NameValueCollection Form
		{
			get { return HttpContext.Current.Request.Form; }
		}

		public Uri UrlReferrer
		{
			get { return HttpContext.Current.Request.UrlReferrer; }
		}

		public string UserName
		{
			get { return HttpContext.Current.User.Identity.Name; }
		}

		public string UserAgent
		{
			get { return HttpContext.Current.Request.UserAgent; }
		}

		public string UserHostAddress
		{
			get { return HttpContext.Current.Request.UserHostAddress; }
		}
	}

	public class StringCollection : Dictionary<string, string>, Microsoft.AspNet.Http.IReadableStringCollection
	{
		ICollection<string> Microsoft.AspNet.Http.IReadableStringCollection.Keys
		{
			get
			{
				return Keys;
			}
		}

		public string Get(string key)
		{
			return Get(key);
		}

		public IList<string> GetValues(string key)
		{
			string value;
			return TryGetValue(key, out value) ? new[] { value } : null;
		}

		IEnumerator<KeyValuePair<string, string[]>> IEnumerable<KeyValuePair<string, string[]>>.GetEnumerator()
		{
			foreach (var pair in this)
			{
				yield return new KeyValuePair<string, string[]>(pair.Key, new[] { pair.Value });
			}
		}
	}
}