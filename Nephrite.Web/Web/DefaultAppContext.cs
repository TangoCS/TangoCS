using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Web;
using Nephrite.Http;

namespace Nephrite.Web
{
	public class DefaultAppContext : IHttpContext
	{
		IHttpRequest _request;
		IHttpResponse _response;

		public DefaultAppContext()
		{
			_request = new DefaultAppRequest();
			_response = new DefaultAppResponse();
		}

		public IHttpRequest Request
		{
			get { return _request; }
		}

		public IHttpResponse Response
		{
			get { return _response; }
		}

		public IDictionary Items
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

		public IPrincipal User
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

		public IServiceProvider ApplicationServices { get; set; }
		public IServiceProvider RequestServices { get; set; }
	}

	public class DefaultAppRequest : IHttpRequest
	{
		public string QueryString
		{
			get { return HttpContext.Current.Request.Url.Query; }
		}

		public NameValueCollection Query
		{
			get { return HttpContext.Current.Request.QueryString; }
		}

		Dictionary<string, string> _cookies;
		public IReadOnlyDictionary<string, string> Cookies
		{
			get
			{
				if (_cookies == null)
				{
					_cookies = new Dictionary<string, string>();
					foreach (var cookie in HttpContext.Current.Request.Cookies.AllKeys)
						_cookies.Add(cookie, HttpContext.Current.Request.Cookies.Get(cookie).Value);
				}
				return _cookies;
			}
		}

		//public Uri Url
		//{
		//	get { return HttpContext.Current.Request.Url; }
		//}

		//public NameValueCollection Headers
		//{
		//	get { return HttpContext.Current.Request.Headers; }
		//}

		//public NameValueCollection Form
		//{
		//	get { return HttpContext.Current.Request.Form; }
		//}

		//public Uri UrlReferrer
		//{
		//	get { return HttpContext.Current.Request.UrlReferrer; }
		//}

		//public string UserName
		//{
		//	get { return HttpContext.Current.User.Identity.Name; }
		//}

		//public string UserAgent
		//{
		//	get { return HttpContext.Current.Request.UserAgent; }
		//}

		//public string UserHostAddress
		//{
		//	get { return HttpContext.Current.Request.UserHostAddress; }
		//}

		public string ContentType
		{
			get
			{
				return HttpContext.Current.Request.ContentType;
			}
		}

		public string Method
		{
			get
			{
				return HttpContext.Current.Request.HttpMethod;
			}
		}

		public Stream Body
		{
			get
			{
				return HttpContext.Current.Request.InputStream;
			}
		}
	}

	public class DefaultAppResponse : IHttpResponse
	{
		public Stream Body
		{
			get
			{
				return HttpContext.Current.Response.OutputStream;
			}
		}

		public string ContentType
		{
			get
			{
				return HttpContext.Current.Response.ContentType;
			}

			set
			{
				HttpContext.Current.Response.ContentType = value;
			}
		}

		public void Redirect(string url)
		{
			HttpContext.Current.Response.Redirect(url);
		}
	}
}