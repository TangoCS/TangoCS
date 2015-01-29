using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite;

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
		public Nephrite.Http.IReadableStringCollection Query
		{
			get { throw new NotImplementedException(); }
		}

		public Nephrite.Http.RequestCookiesCollection Cookies
		{
			get { throw new NotImplementedException(); }
		}

		public Uri Url
		{
			get { return HttpContext.Current.Request.Url; }
		}

		public System.Collections.Specialized.NameValueCollection Headers
		{
			get { return HttpContext.Current.Request.Headers; }
		}

		public System.Collections.Specialized.NameValueCollection Form
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