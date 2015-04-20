using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nephrite.Http
{
	public interface IHttpContext : IDisposable
	{
		IHttpRequest Request { get; }
		IHttpResponse Response { get; }
		IDictionary Items { get; }
		//IPersistentSettings Settings { get; } 
		IPrincipal User { get; }

		//object GetFeature(Type type);
		//void SetFeature(Type type, object instance);
		IServiceProvider ApplicationServices { get; set; }
		IServiceProvider RequestServices { get; set; }

		T GetFeature<T>();
		void SetFeature<T>(T instance);
	}

	public interface IHttpRequest
	{
		NameValueCollection Query { get; }
		RequestCookiesCollection Cookies { get; }
		Uri Url { get; }
		NameValueCollection Headers { get; }
		NameValueCollection Form { get; }

		//Uri UrlReferrer { get; }
		//string UserName { get; }
		//string UserAgent { get; }
		//string UserHostAddress { get; }
	}

	public interface IHttpResponse
	{
		void Redirect(string url);
	}
}
