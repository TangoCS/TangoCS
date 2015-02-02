using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Nephrite;
using Nephrite.Http;

namespace Tessera3Sample
{
	public class DefaultAppContext : IAppContext
	{
		IRequest _request;

		public DefaultAppContext()
		{
			_request = new DefaultAppRequest();
		}

		public IRequest Request
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
	}

	public class DefaultAppRequest : IRequest
	{
		public NameValueCollection Query
		{
			get { return HttpContext.Current.Request.QueryString; }
		}

		RequestCookiesCollection _cookies;
		public RequestCookiesCollection Cookies
		{
			get 
			{ 
				if (_cookies == null)
				{
					IDictionary<string, string> d = new Dictionary<string, string>();
					var cookies = HttpContext.Current.Request.Cookies;

					foreach (string c in cookies.AllKeys)
						d.Add(c, cookies[c].Value);

					_cookies = new RequestCookiesCollection(d);
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
}